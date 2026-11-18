using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using SpaceRockIT.Reviews.Api.Models;

namespace SpaceRockIT.Reviews.Api.Tests;

public class ApiTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private HttpClient Client => factory.CreateClient();

    [Fact]
    public async Task Health_responds()
    {
        var response = await Client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Health_leaks_nothing_internal()
    {
        var body = await Client.GetStringAsync("/health");

        foreach (var leak in new[] { "ConnectionString", "Data Source", "MachineName", "Path" })
            Assert.DoesNotContain(leak, body, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task Rating_outside_range_returns_bad_request(int rating)
    {
        var response = await Client.PostAsJsonAsync("/reviews", new
        {
            workshopId = "ws-range",
            attendeeId = "attendee-1",
            rating,
            comment = "test"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"workshopId\":\"ws-1\",\"rating\":5}")]
    [InlineData("{\"attendeeId\":\"att-1\",\"rating\":5}")]
    [InlineData("{\"workshopId\":\"ws-1\",\"attendeeId\":\"att-1\"}")]
    [InlineData("{\"workshopId\":\"   \",\"attendeeId\":\"att-1\",\"rating\":5}")]
    [InlineData("{\"workshopId\":\"ws-1\",\"attendeeId\":\"   \",\"rating\":5}")]
    public async Task Required_fields_must_be_present_and_non_blank(string json)
    {
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await Client.PostAsync("/reviews", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Accepted_reviews_update_workshop_metrics()
    {
        var first = await Client.PostAsJsonAsync("/reviews", new
        {
            workshopId = "ws-metrics",
            attendeeId = "attendee-1",
            rating = 5,
            comment = "Excellent"
        });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await Client.PostAsJsonAsync("/reviews", new
        {
            workshopId = "ws-metrics",
            attendeeId = "attendee-2",
            rating = 3
        });
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);

        var metrics = await Client.GetFromJsonAsync<ReviewMetrics>("/reviews/ws-metrics");
        Assert.NotNull(metrics);
        Assert.Equal(4.0, metrics.AverageRating);
        Assert.Equal(2, metrics.Count);
        Assert.Equal(2, metrics.TotalSubmissionCount);
    }

    [Fact]
    public async Task Accepted_review_is_logged_at_information_with_workshop_and_comment()
    {
        var sink = new TestLogSink();
        using var loggedFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(new TestLoggerProvider(sink));
            });
        });

        var client = loggedFactory.CreateClient();
        var response = await client.PostAsJsonAsync("/reviews", new
        {
            workshopId = "ws-ai",
            attendeeId = "attendee-log",
            rating = 5,
            comment = "Great workshop"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains(sink.Entries,
            entry => entry.Level == LogLevel.Information
                     && entry.Message == "Received review for ws-ai: Great workshop");
    }

    [Fact]
    public async Task Decimal_rating_returns_bad_request()
    {
        using var content = new StringContent(
            "{\"workshopId\":\"ws-decimal\",\"attendeeId\":\"att-1\",\"rating\":2.5}",
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await Client.PostAsync("/reviews", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed class TestLogSink
    {
        public List<TestLogEntry> Entries { get; } = [];
    }

    private sealed record TestLogEntry(LogLevel Level, string Message);

    private sealed class TestLoggerProvider(TestLogSink sink) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new TestLogger(sink);
        public void Dispose() { }
    }

    private sealed class TestLogger(TestLogSink sink) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            sink.Entries.Add(new TestLogEntry(logLevel, formatter(state, exception)));
        }
    }
}
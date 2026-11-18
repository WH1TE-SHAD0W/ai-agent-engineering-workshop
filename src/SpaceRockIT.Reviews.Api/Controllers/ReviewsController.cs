using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using SpaceRockIT.Reviews.Api.Models;
using SpaceRockIT.Reviews.Api.Services;

namespace SpaceRockIT.Reviews.Api.Controllers;

[ApiController]
[Route("reviews")]
public sealed partial class ReviewsController(InMemoryReviewStore store, ILogger<ReviewsController> logger) : ControllerBase
{
    // Attendee comments are free text and frequently contain email addresses.
    // GDPR requires these never be logged in plain text.
    [GeneratedRegex(@"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+")]
    private static partial Regex EmailPattern();

    [HttpPost]
    [ProducesResponseType(typeof(ReviewMetrics), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Submit([FromBody] ReviewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WorkshopId)
            || string.IsNullOrWhiteSpace(request.AttendeeId)
            || request.Rating is < 1 or > 5)
        {
            return BadRequest();
        }

        var attendeeId = request.AttendeeId;
        var workshopId = request.WorkshopId;
        store.Add(workshopId, request.Rating);

        var sanitizedComment = EmailPattern().Replace(request.Comment ?? string.Empty, "[redacted-email]");
        logger.LogInformation("Received review for {WorkshopId}: {Comment}", workshopId, sanitizedComment);

        var metrics = store.GetMetrics(workshopId);
        return CreatedAtAction(nameof(GetMetrics), new { workshopId }, metrics);
    }

    [HttpGet("{workshopId}")]
    [ProducesResponseType(typeof(ReviewMetrics), StatusCodes.Status200OK)]
    public ActionResult<ReviewMetrics> GetMetrics(string workshopId)
    {
        return Ok(store.GetMetrics(workshopId));
    }
}

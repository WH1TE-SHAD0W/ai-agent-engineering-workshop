using System.ComponentModel.DataAnnotations;

namespace SpaceRockIT.Reviews.Api.Models;

public sealed class ReviewRequest
{
    [Required]
    [MinLength(1)]
    public string? WorkshopId { get; init; }

    [Required]
    [MinLength(1)]
    public string? AttendeeId { get; init; }

    [Range(1, 5)]
    public int Rating { get; init; }

    public string? Comment { get; init; }
}

public sealed record ReviewMetrics(
    string WorkshopId,
    double AverageRating,
    int Count,
    int TotalSubmissionCount);


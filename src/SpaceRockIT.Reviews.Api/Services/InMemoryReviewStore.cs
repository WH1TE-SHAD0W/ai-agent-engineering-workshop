namespace SpaceRockIT.Reviews.Api.Services;

using SpaceRockIT.Reviews.Api.Models;

public sealed class InMemoryReviewStore
{
    private readonly object gate = new();
    private readonly Dictionary<string, List<int>> ratingsByWorkshop = new(StringComparer.Ordinal);

    public void Add(string workshopId, int rating)
    {
        lock (gate)
        {
            if (!ratingsByWorkshop.TryGetValue(workshopId, out var ratings))
            {
                ratings = [];
                ratingsByWorkshop.Add(workshopId, ratings);
            }

            ratings.Add(rating);
        }
    }

    public ReviewMetrics GetMetrics(string workshopId)
    {
        lock (gate)
        {
            if (!ratingsByWorkshop.TryGetValue(workshopId, out var ratings) || ratings.Count == 0)
                return new ReviewMetrics(workshopId, 0, 0, 0);

            var average = ratings.Sum() / (double)ratings.Count;
            return new ReviewMetrics(workshopId, average, ratings.Count, ratings.Count);
        }
    }
}


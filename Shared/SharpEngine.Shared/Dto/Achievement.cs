namespace SharpEngine.Shared.Dto;

public class Achievement
{
    public string Name { get; init; }
    public string Description { get; init; }
    public DateTime? DateAchieved { get; init; }

    public bool IsAchieved => DateAchieved.HasValue;
}

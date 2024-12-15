namespace AdventOfCode.models;

public class Robot
{
    public int SpaceFromLeft { get; set; }
    public int SpaceFromTop { get; set; }
    
    public int SpeedHorizontal { get; set; }
    public int SpeedVertical { get; set; }

    public string DisplayPosition()
    {
        return $"(left: {SpaceFromLeft}, top: {SpaceFromTop})";
    }
}
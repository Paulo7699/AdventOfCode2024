namespace AdventOfCode.models;

public class Floor
{
    // Si une seule, c'est un étage unique
    public Box? LeftBox { get; set; }
    public Box? RightBox { get; set; }
    
    public int FloorStep { get; set; }

    public override string ToString()
    {
        return $"Step: {FloorStep}, LeftBox: {LeftBox}, RightBox: {RightBox}";
    }
}
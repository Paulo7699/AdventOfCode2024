namespace AdventOfCode.models;

public class Cells
{
    public int Row { get; set; } = 0;
    public int Column { get; set; } = 0;
    public string? Value { get; set; } = null;
    
    // Left, Top, Right, Bottom
    public bool[] MarkedSides { get; set; } = [false, false, false, false];

    public override string ToString()
    {
        return $"{Value}({Row},{Column})";
    }
}
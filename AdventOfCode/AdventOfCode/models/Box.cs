namespace AdventOfCode.models;

public class Box
{
    public Cells? LeftCell { get; set; }
    public Cells? RightCell { get; set; }

    public override string ToString()
    {
        return $"LeftCells: {LeftCell}, RightCells: {RightCell}";
    }
}
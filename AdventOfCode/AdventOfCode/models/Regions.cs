namespace AdventOfCode.models;

public class Regions
{
    public List<Cells> Cells { get; } = new();

    public override string ToString()
    {
        return $"Region {Cells.First().Value}, area : {Cells.Count}";
    }
}
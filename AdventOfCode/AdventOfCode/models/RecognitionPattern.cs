namespace AdventOfCode.models;

public class RecognitionPattern
{
    public string? WordFound { get; set; } = null;
    public int? DestRow { get; set; } = null;
    public int? DestColumn { get; set; } = null;

    public override string ToString()
    {
        return $"{WordFound}, ({DestRow+1},{DestColumn+1})";
    }
}
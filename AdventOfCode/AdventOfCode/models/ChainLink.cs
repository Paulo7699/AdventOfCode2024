namespace AdventOfCode.models;

public class ChainLink
{
    public string Pattern { get; set; } = "";
    public int Occurrency { get; set; } = 0;

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < Occurrency; i++)
        {
            s += Pattern + "/";
        }

        return s;
    }
}
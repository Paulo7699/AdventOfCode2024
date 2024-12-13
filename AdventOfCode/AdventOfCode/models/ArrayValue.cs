namespace AdventOfCode.models;

public class ArrayValue
{
    public List<Column> Columns { get; set; } = new();

    public override string ToString()
    {
        List<string> s = [];
        foreach (var col in Columns)
        {
            s.Add(col.ToString());
        }

        s = RotateMatrix90Degrees(s.ToArray()).ToList();
        s = s.Select(row => new string(row.Reverse().ToArray())).ToList();
        return string.Join("\n", s);
    }
    
    private static string[] RotateMatrix90Degrees(string[] matrix)
    {
        int rows = matrix.Length;
        int cols = matrix[0].Length;
    
        string[] rotated = new string[cols];
    
        for (int col = 0; col < cols; col++)
        {
            char[] newRow = new char[rows];
            for (int row = rows - 1; row >= 0; row--)
            {
                newRow[rows - 1 - row] = matrix[row][col];
            }
        
            rotated[col] = new string(newRow);
        }
    
        return rotated;
    }
    
    public ArrayValue DeepCopy()
    {
        return new ArrayValue
        {
            Columns = Columns.Select(col => new Column
            {
                Index = col.Index,
                Values = new List<int?>(col.Values),
                ValuesString = new List<string?>(col.ValuesString)
            }).ToList()
        };
    }
}

public class Column
{
    public int Index { get; set; }
    public List<int?> Values { get; set; } = new();
    public List<string?> ValuesString { get; set; } = new();

    public override string ToString()
    {
        return string.Join("", ValuesString);
    }
}
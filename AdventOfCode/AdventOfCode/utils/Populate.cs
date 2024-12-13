using AdventOfCode.models;

namespace AdventOfCode.utils;

public class Populate
{
    public static ArrayValue PopulateArrayValue(string filePath, bool shouldSplit = true)
    {
        string content = ReadFile.ReadFileInput(filePath);
        string[] contentSplitted = content.Split("\r\n");
        contentSplitted = RemoveEmptyValuesFromArray(contentSplitted);
        int maxLength = GetMaxLength(contentSplitted, shouldSplit);
        ArrayValue arrayValue = new();
        for (int i = 0; i < contentSplitted.Length; i++)
        {
            string[] contentSplittedColumns;
            if (shouldSplit)
            {
                contentSplittedColumns = contentSplitted[i].Split([' ']);
                contentSplittedColumns = RemoveEmptyValuesFromArray(contentSplittedColumns);
            }
            else
            {
                contentSplittedColumns = contentSplitted[i].ToCharArray().Select(c => c.ToString()).ToArray();
            }
            for (int k = 0; k < maxLength; k++)
            {
                try
                {
                    Column? column = arrayValue.Columns.FirstOrDefault(c => c.Index == k);
                    if (shouldSplit)
                    {
                        int? vj = k < contentSplittedColumns.Length ? int.Parse(contentSplittedColumns[k]) : null;

                        if (column == null)
                        {
                            column = new()
                            {
                                Index = k
                            };
                            arrayValue.Columns.Add(column);
                        }
                
                        column.Values.Add(vj);
                    }
                    else
                    {
                        string? vj = k < contentSplittedColumns.Length ? contentSplittedColumns[k] : null;

                        if (column == null)
                        {
                            column = new()
                            {
                                Index = k
                            };
                            arrayValue.Columns.Add(column);
                        }
                
                        column.ValuesString.Add(vj);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
        return arrayValue;
    }

    private static string[] RemoveEmptyValuesFromArray(string[] array)
    {
        List<string> newArray = new();
        foreach (var value in array)
        {
            if(!string.IsNullOrWhiteSpace(value)) newArray.Add(value);
        }

        return newArray.ToArray();
    }

    private static int GetMaxLength(string[] array, bool shouldSplit)
    {
        if(shouldSplit)return array.Select(a => a.Split([' ']).Length).Max();
        return array.Length;
    }
    
    public static (Dictionary<int, List<int>> instructions, List<string> printings) PopulateRules(string filePath)
    {
        Dictionary<int, List<int>> instructions = new();
        List<string> printings = new();
        string content = ReadFile.ReadFileInput(filePath);
        string[] contentSplitted = content.Split("\r\n\r\n");

        string[] instructionsSplitted = contentSplitted[0].Split("\r\n");

        foreach (var instruction in instructionsSplitted)
        {
            var instructionSplitted = instruction.Split("|");
            instructions.TryGetValue(int.Parse(instructionSplitted[0]), out var pagesBefore);
            if (pagesBefore == null)
            {
                pagesBefore = new() { int.Parse(instructionSplitted[1]) };
                instructions.Add(int.Parse(instructionSplitted[0]), pagesBefore);
            }
            else
            {
                pagesBefore.Add(int.Parse(instructionSplitted[1]));
            }
        }
        
        printings = contentSplitted[1].Split("\r\n").ToList();

        return (instructions, printings);
    }
    
    public static List<EquationLine> PopulateEquations(string filePath)
    {
        List<EquationLine> equationLines = new();
        string content = ReadFile.ReadFileInput(filePath);
        string[] contentSplitted = content.Split("\r\n");

        foreach (var line in contentSplitted)
        {
            string[] splittedLine = line.Split(":");
            long firstOperand = Int64.Parse(splittedLine[0]);
            string[] operationsString = splittedLine[1].Split(" ").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            List<int> operations = operationsString.Select(int.Parse).ToList();
            equationLines.Add(new()
            {
                ExpectedResult = firstOperand,
                Operands = operations
            });
        }

        return equationLines;
    }
}
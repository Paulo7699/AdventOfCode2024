using AdventOfCode.models;

namespace AdventOfCode.utils;

public class Populate
{
    public static List<Robot> PopulateRobots(string filePath)
    {
        List<Robot> robots = new();
        string content = ReadFile.ReadFileInput(filePath);
        string[] contentSplitted = content.Split("\r\n");

        foreach (var robotLines in contentSplitted)
        {
            string[] splittedSpaces = robotLines.Split(" ");
            
            string[] positions = splittedSpaces[0].Replace("p=","").Split(",");
            string[] speeds = splittedSpaces[1].Replace("v=","").Split(",");
            
            robots.Add(new()
            {
                SpaceFromLeft = int.Parse(positions[0]),
                SpaceFromTop = int.Parse(positions[1]),
                SpeedHorizontal = int.Parse(speeds[0]),
                SpeedVertical = int.Parse(speeds[1]),
            });
        }

        return robots;
    }

    public static List<ClawMachine> PopulateClawMachines(string filePath)
    {
        List<ClawMachine> clawMachines = new();
        string content = ReadFile.ReadFileInput(filePath);
        string[] contentSplitted = content.Split("\r\n\r\n");

        foreach (var clawMachineStringGroup in contentSplitted)
        {
            string[] clawMachineLines = clawMachineStringGroup.Split("\r\n");

            string lineButtonA = clawMachineLines[0].Split(":")[1];
            string buttonXa = lineButtonA.Split(",")[0];
            string buttonYa = lineButtonA.Split(",")[1];
            
            string lineButtonB = clawMachineLines[1].Split(":")[1];
            string buttonXb = lineButtonB.Split(",")[0];
            string buttonYb = lineButtonB.Split(",")[1];
            
            string linePrize = clawMachineLines[2].Split(":")[1];
            string prizeX = linePrize.Split(",")[0];
            string prizeY = linePrize.Split(",")[1];

            ClawMachine clawMachine = new()
            {
                ButtonXa = int.Parse(buttonXa.Replace("X+", "")),
                ButtonYa = int.Parse(buttonYa.Replace("Y+", "")),
                ButtonXb = int.Parse(buttonXb.Replace("X+", "")),
                ButtonYb = int.Parse(buttonYb.Replace("Y+", "")),
                PrizeX = int.Parse(prizeX.Replace("X=", "")),
                PrizeY = int.Parse(prizeY.Replace("Y=", "")),
            };
            clawMachines.Add(clawMachine);
        }

        return clawMachines;
    }
    
    public static (ArrayValue arrayValue, string instructions) PopulateLanternfish(string filePath)
    {
        string content = ReadFile.ReadFileInput(filePath);
        string[] contentSplitted = content.Split("\r\n\r\n");
        contentSplitted = RemoveEmptyValuesFromArray(contentSplitted);
        
        ArrayValue arrayValue = new();
        
        // Populate map
        var mapGroup = contentSplitted[0].Split("\r\n");
        for (var rowMap = 0; rowMap < mapGroup.Length; rowMap++)
        {
            var mapLine = mapGroup[rowMap].ToCharArray().Select(c => c.ToString()).ToArray();
            for (int col = 0; col < mapLine.Length; col++)
            {
                Column? column = arrayValue.Columns.FirstOrDefault(c => c.Index == col);
                if (column == null)
                {
                    column = new()
                    {
                        Index = col
                    };
                    arrayValue.Columns.Add(column);
                }
                
                column.ValuesString.Add(mapLine[col]);
            }
        }

        // Populate instructions
        string instructions = string.Join("", contentSplitted[1].Split("\r\n"));
        
        return (arrayValue, instructions);
    }
    
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
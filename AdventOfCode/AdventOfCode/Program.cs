// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using AdventOfCode.models;
using AdventOfCode.utils;

namespace AdventOfCode;

public abstract class Program
{
    public static void Main()
    {
        Day8_P2();
    }

    private static void Day8_P2()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day8.txt", false);
        int count = CountNotPointValues(arrayValue);
        List<Cells> positionsVisited = BuildAntennas(arrayValue);
        
        Console.WriteLine($"{string.Join(",", positionsVisited)}");
        Console.WriteLine($"8_P2 => {positionsVisited.Count + count}");
    }

    private static int CountNotPointValues(ArrayValue arrayValue)
    {
        int count = 0;
        List<Cells> positionsVisited = new();
        for (int i = 0; i < arrayValue.Columns.Count; i++)
        {
            var column = arrayValue.Columns[i];
            for (int j = 0; j < column.ValuesString.Count; j++)
            {
                var cellValue = column.ValuesString[j];
                if(cellValue == ".")continue;
                count++;
            }
        }

        return count;
    }

    private static void Day8_P1()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day8.txt", false);
        List<Cells> positionsVisited = BuildAntennas(arrayValue);
        
        Console.WriteLine($"{string.Join(",", positionsVisited)}");
        Console.WriteLine($"8_P1 => {positionsVisited.Count}");
    }

    private static List<Cells> BuildAntennas(ArrayValue arrayValue, int counter = 0)
    {
        List<Cells> positionsVisited = new();
        for (int i = 0; i < arrayValue.Columns.Count; i++)
        {
            var column = arrayValue.Columns[i];
            for (int j = 0; j < column.ValuesString.Count; j++)
            {
                var cellValue = column.ValuesString[j];
                if (cellValue == ".") continue;
                PopulateAntinodesWithAntennas(cellValue, arrayValue, i, j, positionsVisited);
            }
        }

        return positionsVisited;
    }

    private static void PopulateAntinodesWithAntennas(string? cellValue, ArrayValue arrayValue, int i, int j,
        List<Cells> positionsVisited)
    {
        List<Cells> cellsDifferentThanCurrent = GetCellsDifferentPositionSamePattern(j, i, arrayValue, cellValue);
        Console.WriteLine($"Pour la cellule {cellValue}({j},{i}), => {string.Join(",",cellsDifferentThanCurrent)}");

        foreach (var cell in cellsDifferentThanCurrent)
        {
            PerformCreationVisitCell(arrayValue, i, j, positionsVisited, cell);
        }
    }

    private static void PerformCreationVisitCell(ArrayValue arrayValue, int i, int j, List<Cells> positionsVisited, Cells cell)
    {
        // Le plus en bas :
        Cells bottomCell = new();
        Cells topCell = new();
        if (cell.Row > j)
        {
            bottomCell.Row = cell.Row;
            bottomCell.Column = cell.Column;

            topCell.Row = j;
            topCell.Column = i;
        }
        else
        {
            topCell.Row = cell.Row;
            topCell.Column = cell.Column;

            bottomCell.Row = j;
            bottomCell.Column = i;
        }

        int colOffset = Math.Abs(bottomCell.Column - topCell.Column);
        int rowOffset = Math.Abs(bottomCell.Row - topCell.Row);
            
        // Bas à gauche du haut
        if (bottomCell.Column < topCell.Column)
        {
            int newColBottom = bottomCell.Column - colOffset;
            int newRowBottom = bottomCell.Row + rowOffset;
            AddNewCell(arrayValue, positionsVisited, newColBottom, newRowBottom, bottomCell);
                
            int newColTop = topCell.Column + colOffset;
            int newRowTop = topCell.Row - rowOffset;
            AddNewCell(arrayValue, positionsVisited, newColTop, newRowTop, topCell);
                
            Console.WriteLine($"\t[0] Pour {cell}, ajout de : ({newRowBottom},{newColBottom}) et " +
                              $"({newRowTop},{newColTop}).");
        }
        else // Bas à droite du haut
        {
            int newColBottom = bottomCell.Column + colOffset;
            int newRowBottom = bottomCell.Row + rowOffset;
            AddNewCell(arrayValue, positionsVisited, newColBottom, newRowBottom, bottomCell);
                
            int newColTop = topCell.Column - colOffset;
            int newRowTop = topCell.Row - rowOffset;
            AddNewCell(arrayValue, positionsVisited, newColTop, newRowTop, topCell);
                
            Console.WriteLine($"\t[1] Pour {cell}, ajout de : ({newRowBottom},{newColBottom}) et " +
                              $"({newRowTop},{newColTop}).");
        }
    }

    private static void AddNewCell(ArrayValue arrayValue, List<Cells> positionsVisited, int col, int row, Cells initCell)
    {
        bool added = false;
        if (col < arrayValue.Columns.Count && row < arrayValue.Columns[0].ValuesString.Count && col >= 0 && row >= 0)
        {
            if (!positionsVisited.Any(p => p.Column == col && p.Row == row))
            {
                Cells newCell = new()
                {
                    Column = col,
                    Row = row
                };
                added = true;
                positionsVisited.Add(newCell);
            }
        }
        if(added)PerformCreationVisitCell(arrayValue, col, row, positionsVisited, initCell);
    }

    private static List<Cells> GetCellsDifferentPositionSamePattern(int row, int col, ArrayValue arrayValue,
        string? cellValue)
    {
        List<Cells> cells = new();
        for (int i = 0; i < arrayValue.Columns.Count; i++)
        {
            var column = arrayValue.Columns[i];
            for (int j = 0; j < column.ValuesString.Count; j++)
            {
                var currentValue = column.ValuesString[j];
                bool isSame = row == j && col == i;
                if ((currentValue == cellValue) && !isSame)
                {
                    cells.Add(new Cells()
                    {
                        Column = i,
                        Row = j
                    });
                }
            }
        }

        return cells;
    }

    private static void Day7_P1AndP2()
    {
        List<EquationLine> equationLines = Populate.PopulateEquations("inputs/day7.txt");
        long result = CalculateTotalCalibrationResult(equationLines);
        Console.WriteLine(result);
    }

    private static List<long> GetResultsPossibleTest(List<int> operands)
    {
        return GenerateAllPossibleResults(operands);
    }

    private static List<long> GenerateAllPossibleResults(List<int> operands)
    {
        List<long> results = new();
        GenerateResultsRecursive(operands, 0, results);
        return results.Distinct().ToList();
    }

    private static void GenerateResultsRecursive(
        List<int> operands,
        int currentIndex,
        List<long> results,
        long currentResult = 0,
        bool isFirstIteration = true)
    {
        if (currentIndex == operands.Count)
        {
            results.Add(currentResult);
            return;
        }

        if (isFirstIteration)
        {
            currentResult = operands[0];
            GenerateResultsRecursive(operands, currentIndex + 1, results, currentResult, false);
        }
        else
        {
            long addResult = currentResult + operands[currentIndex];
            GenerateResultsRecursive(operands, currentIndex + 1, results, addResult, false);

            long multiplyResult = currentResult * operands[currentIndex];
            GenerateResultsRecursive(operands, currentIndex + 1, results, multiplyResult, false);

            long concatResult = Int64.Parse($"{currentResult}{operands[currentIndex]}");
            GenerateResultsRecursive(operands, currentIndex + 1, results, concatResult, false);
        }
    }

    private static long CalculateTotalCalibrationResult(List<EquationLine> equationLines)
    {
        long equationsPossible = 0;
        foreach (var equationLine in equationLines)
        {
            bool isPossible = CalculateIsPossible(equationLine);
            if (isPossible) equationsPossible += equationLine.ExpectedResult;
        }

        return equationsPossible;
    }

    private static bool CalculateIsPossible(EquationLine equationLine)
    {
        List<long> resultPossible = GetResultsPossibleTest(equationLine.Operands);
        return resultPossible.Contains(equationLine.ExpectedResult);
    }


    private static void Day6_P2()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day6.txt", false);
        int calculations = 0;
        int iterationLoops = 0;
        for (int i = 0; i < arrayValue.Columns.Count; i++)
        {
            var column = arrayValue.Columns[i];
            for (int j = 0; j < column.ValuesString.Count; j++)
            {
                var cellValue = column.ValuesString[j];
                ArrayValue newArrayValue = arrayValue.DeepCopy();

                // Modification du champ de l'itération actuelle en obstacle.
                if (cellValue == "#") continue;

                newArrayValue.Columns[i].ValuesString[j] = "#";
                Console.WriteLine($"Calculation => {calculations}");
                (int result, bool infiniteLoop) = CalculateNumberOfVisitedMaps(newArrayValue);
                calculations++;
                if (infiniteLoop) iterationLoops++;
            }
        }

        Console.WriteLine($"6_2 => {iterationLoops}");
    }

    private static void Day6_P1()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day6.txt", false);
        (int result, bool infiniteLoop) = CalculateNumberOfVisitedMaps(arrayValue);
        Console.WriteLine($"6_1 --> {result}");
    }

    private static (int result, bool infiniteLoop) CalculateNumberOfVisitedMaps(ArrayValue arrayValue)
    {
        int iterations = 0;
        List<Cells> positionsVisited = new();

        (int? row, int? column) = FindStartPosition(arrayValue);
        if (row == null || column == null)
        {
            return (0, false);
        }

        bool isExited = false;
        string currentDirection = arrayValue.Columns[column.Value].ValuesString[row.Value]!; //r,b,l,u
        while (!isExited)
        {
            iterations++;
            if (iterations > 10000) return (positionsVisited.Count(), true);
            AddPosition(positionsVisited, row, column);
            int? potentialNewRow = null;
            int? potentialNewColumn = null;
            try
            {
                switch (currentDirection)
                {
                    case "^":
                        potentialNewRow = row.Value - 1;
                        break;
                    case ">":
                        potentialNewColumn = column.Value + 1;
                        break;
                    case "v":
                        potentialNewRow = row.Value + 1;
                        break;
                    case "<":
                        potentialNewColumn = column.Value - 1;
                        break;
                }

                string newValueEncountered = arrayValue.Columns[potentialNewColumn ?? column.Value]
                    .ValuesString[potentialNewRow ?? row.Value]!; // exit if error
                if (newValueEncountered == "#")
                {
                    List<string> characterChars = ["^", ">", "v", "<"];
                    currentDirection =
                        characterChars[(characterChars.IndexOf(currentDirection) + 1) % characterChars.Count];
                }
                else
                {
                    row = potentialNewRow ?? row;
                    column = potentialNewColumn ?? column;
                    arrayValue.Columns[column.Value].ValuesString[row.Value] = "X";
                }
            }
            catch (Exception e)
            {
                isExited = true;
            }
        }

        return (positionsVisited.Count(), false);
    }

    private static void AddPosition(List<Cells> positionsVisited, int? row, int? column)
    {
        if (column != null && row != null && !positionsVisited.Any(p => p.Column == column && p.Row == row))
            positionsVisited.Add(new()
            {
                Column = column.Value,
                Row = row.Value
            });
    }


    private static (int? row, int? column) FindStartPosition(ArrayValue arrayValue)
    {
        string[] characterChars = ["^", ">", "v", "<"];
        for (var column = 0; column < arrayValue.Columns.Count; column++)
        {
            var columnValues = arrayValue.Columns[column];
            for (int row = 0; row < columnValues.ValuesString.Count(); row++)
            {
                var character = columnValues.ValuesString[row];
                if (characterChars.Contains(character)) return (row, column);
            }
        }

        return (null, null);
    }

    private static void Day5_P2()
    {
        (Dictionary<int, List<int>> instructions, List<string> printings) = Populate.PopulateRules("inputs/day5.txt");
        int result = HandlePrintingsP2(instructions, printings);
        Console.WriteLine($"5_2 --> {result}");
    }

    private static void Day5_P1()
    {
        (Dictionary<int, List<int>> instructions, List<string> printings) = Populate.PopulateRules("inputs/day5.txt");
        int result = HandlePrintings(instructions, printings);
        Console.WriteLine($"5_1 --> {result}");
    }

    private static int HandlePrintingsP2(Dictionary<int, List<int>> instructions, List<string> printings)
    {
        int result = 0;
        List<List<int>> intPrintings = TransformPrintings(printings);
        for (var index = 0; index < intPrintings.Count; index++)
        {
            var rowsPrinting = intPrintings[index];
            bool lineIsCorrect = HandleInstructionLine(instructions, rowsPrinting);
            if (!lineIsCorrect)
            {
                List<int> newRow = [];
                while (!lineIsCorrect)
                {
                    newRow = ReorderRows(rowsPrinting, instructions);
                    lineIsCorrect = HandleInstructionLine(instructions, newRow);
                }

                Console.WriteLine($"new row : {String.Join(",", newRow)}");
                double d = newRow.Count / 2;
                result += newRow[(int)Math.Floor(d)];
            }
        }

        return result;
    }

    private static List<int> ReorderRows(List<int> rowsPrinting, Dictionary<int, List<int>> instructions)
    {
        List<int> reorderedRowsPrintings = new();

        foreach (var row in rowsPrinting)
        {
            instructions.TryGetValue(row, out var constraints);
            if (constraints == null)
            {
                reorderedRowsPrintings.Add(row);
                continue;
            }

            // Sinon, il faut déterminer la place sur laquelle on doit insérer le nouveau
            int? minIndex = null;
            foreach (var constraint in constraints)
            {
                int index = reorderedRowsPrintings.IndexOf(constraint);
                if (index != -1 && (index < minIndex || minIndex == null))
                    minIndex = index;
            }

            if (minIndex != null)
            {
                reorderedRowsPrintings.Insert(minIndex.Value, row);
            }
            else
            {
                reorderedRowsPrintings.Add(row);
            }
        }

        return reorderedRowsPrintings;
    }

    private static int HandlePrintings(Dictionary<int, List<int>> instructions, List<string> printings)
    {
        int result = 0;
        List<List<int>> intPrintings = TransformPrintings(printings);
        for (var index = 0; index < intPrintings.Count; index++)
        {
            var rowsPrinting = intPrintings[index];
            if (HandleInstructionLine(instructions, rowsPrinting))
            {
                double d = rowsPrinting.Count / 2;
                result += rowsPrinting[(int)Math.Floor(d)];
            }
        }

        return result;
    }

    private static bool HandleInstructionLine(Dictionary<int, List<int>> instructions, List<int> rowsPrinting)
    {
        for (var index = 0; index < rowsPrinting.Count; index++)
        {
            var cellPrinting = rowsPrinting[index];
            List<int> leftCellsPrinting = rowsPrinting.Where(r => rowsPrinting.IndexOf(r) < index).ToList();
            instructions.TryGetValue(cellPrinting, out var rules);
            if (rules == null)
            {
                continue;
            }

            foreach (var rule in rules)
            {
                if (leftCellsPrinting.Contains(rule))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static List<List<int>> TransformPrintings(List<string> printings)
    {
        List<List<int>> result = new();
        foreach (var printing in printings)
        {
            result.Add(printing.Split(",").Select(int.Parse).ToList());
        }

        return result;
    }

    private static void Day4_P2()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day4.txt", false);
        int result = CalculateNumberOfPatternAppearancesP2(arrayValue);
        Console.WriteLine($"4_2 --> {result}");
    }

    private static void Day4_P1()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day4.txt", false);
        int result = CalculateNumberOfPatternAppearances(arrayValue);
        Console.WriteLine($"4_1 --> {result}");
    }

    private static int CalculateNumberOfPatternAppearancesP2(ArrayValue arrayValue)
    {
        int occurencies = 0;
        for (int currentLine = 0; currentLine < arrayValue.Columns.First().ValuesString.Count; currentLine++)
        {
            foreach (var column in arrayValue.Columns)
            {
                if (column.ValuesString[currentLine] != "A") continue;
                var upLeft = GetInfoUpLeft(arrayValue, column.Index, currentLine, 2);
                var upRight = GetInfoUpRight(arrayValue, column.Index, currentLine, 2);
                var bottomRight = GetInfoBottomRight(arrayValue, column.Index, currentLine, 2);
                var bottomLeft = GetInfoBottomLeft(arrayValue, column.Index, currentLine, 2);

                Console.WriteLine($"\n{currentLine},{column.Index} / {column.ValuesString[currentLine]}");
                if (DiagMatches(upLeft?.WordFound, bottomRight?.WordFound) &&
                    DiagMatches(upRight?.WordFound, bottomLeft?.WordFound))
                {
                    occurencies += 1;
                }
            }
        }

        return occurencies;
    }

    private static bool DiagMatches(string? d1, string? d2)
    {
        if (d1 == null || d2 == null) return false;
        string expected = "MAS";
        try
        {
            bool exp = $"{d2.Replace("A", "")}{d1}" == expected || $"{d2.Replace("A", "")}{d1}" == Reverse(expected);

            Console.WriteLine($"Enters with {d1},{d2} ==> {d2.Replace("A", "")}{d1} // exp => {exp}");
            return exp;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private static int CalculateNumberOfPatternAppearances(ArrayValue arrayValue)
    {
        string searchedWord = "XMAS";
        int occurencies = 0;
        for (int currentLine = 0; currentLine < arrayValue.Columns.First().ValuesString.Count; currentLine++)
        {
            foreach (var column in arrayValue.Columns)
            {
                var upLeft = GetInfoUpLeft(arrayValue, column.Index, currentLine, searchedWord.Length);
                var up = GetInfoUp(arrayValue, column.Index, currentLine, searchedWord.Length);
                var upRight = GetInfoUpRight(arrayValue, column.Index, currentLine, searchedWord.Length);
                var right = GetInfoRight(arrayValue, column.Index, currentLine, searchedWord.Length);
                var bottomRight = GetInfoBottomRight(arrayValue, column.Index, currentLine, searchedWord.Length);
                var bottom = GetInfoBottom(arrayValue, column.Index, currentLine, searchedWord.Length);
                var bottomLeft = GetInfoBottomLeft(arrayValue, column.Index, currentLine, searchedWord.Length);
                var left = GetInfoLeft(arrayValue, column.Index, currentLine, searchedWord.Length);

                int count = MatchExpectedValue(searchedWord, upLeft, up, upRight, right, bottomRight,
                    bottom,
                    bottomLeft,
                    left);

                occurencies += count;
            }
        }

        return occurencies;
    }

    private static int MatchExpectedValue(string expectedValue, params RecognitionPattern?[] values)
    {
        return values.Count(value => value?.WordFound == expectedValue);
    }

    private static string? Reverse(string? s)
    {
        if (s == null) return null;
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    private static RecognitionPattern? GetInfoUpLeft(ArrayValue arrayValue, int currentColumn, int currentRow,
        int searchedWordLength)
    {
        string s = "";
        try
        {
            for (int i = 0; i < searchedWordLength; i++)
            {
                s += arrayValue.Columns[currentColumn - i].ValuesString[currentRow - i];
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return new()
        {
            WordFound = s,
            DestRow = currentRow - (searchedWordLength - 1),
            DestColumn = currentColumn - (searchedWordLength - 1),
        };
    }

    private static RecognitionPattern? GetInfoUp(ArrayValue arrayValue, int currentColumn, int currentRow,
        int searchedWordLength)
    {
        string s = "";
        try
        {
            for (int i = 0; i < searchedWordLength; i++)
            {
                s += arrayValue.Columns[currentColumn].ValuesString[currentRow - i];
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return new()
        {
            WordFound = s,
            DestRow = currentRow - (searchedWordLength - 1),
            DestColumn = currentColumn,
        };
    }

    private static RecognitionPattern? GetInfoUpRight(ArrayValue arrayValue, int currentColumn, int currentRow,
        int searchedWordLength)
    {
        string s = "";
        try
        {
            for (int i = 0; i < searchedWordLength; i++)
            {
                s += arrayValue.Columns[currentColumn + i].ValuesString[currentRow - i];
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return new()
        {
            WordFound = s,
            DestRow = currentRow - (searchedWordLength - 1),
            DestColumn = currentColumn + (searchedWordLength - 1),
        };
    }

    private static RecognitionPattern? GetInfoRight(ArrayValue arrayValue, int currentColumn, int currentRow,
        int searchedWordLength)
    {
        string s = "";
        try
        {
            for (int i = 0; i < searchedWordLength; i++)
            {
                s += arrayValue.Columns[currentColumn + i].ValuesString[currentRow];
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return new()
        {
            WordFound = s,
            DestRow = currentRow,
            DestColumn = currentColumn + (searchedWordLength - 1),
        };
    }

    private static RecognitionPattern? GetInfoBottomRight(ArrayValue arrayValue, int currentColumn, int currentRow,
        int searchedWordLength)
    {
        string s = "";
        try
        {
            for (int i = 0; i < searchedWordLength; i++)
            {
                s += arrayValue.Columns[currentColumn + i].ValuesString[currentRow + i];
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return new()
        {
            WordFound = s,
            DestRow = currentRow + (searchedWordLength - 1),
            DestColumn = currentColumn + (searchedWordLength - 1),
        };
    }

    private static RecognitionPattern? GetInfoBottom(ArrayValue arrayValue, int currentColumn, int currentRow,
        int searchedWordLength)
    {
        string s = "";
        try
        {
            for (int i = 0; i < searchedWordLength; i++)
            {
                s += arrayValue.Columns[currentColumn].ValuesString[currentRow + i];
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return new()
        {
            WordFound = s,
            DestRow = currentRow + (searchedWordLength - 1),
            DestColumn = currentColumn,
        };
    }

    private static RecognitionPattern? GetInfoBottomLeft(ArrayValue arrayValue, int currentColumn, int currentRow,
        int searchedWordLength)
    {
        string s = "";
        try
        {
            for (int i = 0; i < searchedWordLength; i++)
            {
                s += arrayValue.Columns[currentColumn - i].ValuesString[currentRow + i];
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return new()
        {
            WordFound = s,
            DestRow = currentRow + (searchedWordLength - 1),
            DestColumn = currentColumn - (searchedWordLength - 1),
        };
    }

    private static RecognitionPattern? GetInfoLeft(ArrayValue arrayValue, int currentColumn, int currentRow,
        int searchedWordLength)
    {
        string s = "";
        try
        {
            for (int i = 0; i < searchedWordLength; i++)
            {
                s += arrayValue.Columns[currentColumn - i].ValuesString[currentRow];
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return new()
        {
            WordFound = s,
            DestRow = currentRow,
            DestColumn = currentColumn - (searchedWordLength - 1),
        };
    }

    private static void Day3_P2()
    {
        string content = ReadFile.ReadFileInput("inputs/day3.txt");
        var (_, mulPositions) = CalculateMulResult(content);

        List<InstructionPosition> instructionPositions = new();
        PopulateInstructions(instructionPositions, content, @"do\(\)", true);
        PopulateInstructions(instructionPositions, content, @"don't\(\)", false);

        // Enable by default
        instructionPositions.Add(new()
        {
            Position = -1,
            Enable = true
        });
        instructionPositions = instructionPositions.OrderBy(i => i.Position).ToList();
        int result = CalculateMulResultWithInstructions(mulPositions, instructionPositions);
        Console.WriteLine($"Result 3_2 -> {result}");
    }

    private static int CalculateMulResultWithInstructions(List<MulPosition> mulPositions,
        List<InstructionPosition> instructionPositions)
    {
        int result = 0;
        foreach (var instruction in instructionPositions)
        {
            if (HandleInstruction(mulPositions, instructionPositions, instruction, ref result)) return result;
        }

        return result;
    }

    private static bool HandleInstruction(List<MulPosition> mulPositions,
        List<InstructionPosition> instructionPositions, InstructionPosition instruction,
        ref int result)
    {
        if (!instruction.Enable) return false;

        InstructionPosition? nextDisablePosition =
            instructionPositions.FirstOrDefault(i => i.Position > instruction.Position && !i.Enable);

        if (nextDisablePosition == null)
        {
            List<MulPosition> mulPositionsRemaining =
                mulPositions.Where(s => s.Position > instruction.Position).ToList();
            foreach (var mulPosition in mulPositionsRemaining)
            {
                result += mulPosition.M1 * mulPosition.M2;
                Console.WriteLine($"Adding last {mulPosition.M1} * {mulPosition.M2}");
            }

            return true;
        }
        else
        {
            List<MulPosition> mulPositionsRemaining = mulPositions.Where(s =>
                s.Position > instruction.Position && s.Position < nextDisablePosition.Position && !s.Added).ToList();
            foreach (var mulPosition in mulPositionsRemaining)
            {
                mulPosition.Added = true;
                result += mulPosition.M1 * mulPosition.M2;
                Console.WriteLine($"Adding {mulPosition.M1} * {mulPosition.M2}");
            }
        }

        return false;
    }

    private static void PopulateInstructions(List<InstructionPosition> instructionPositions, string content,
        string pattern, bool enable)
    {
        Regex regex = new Regex(pattern);

        MatchCollection matchCollection = regex.Matches(content);
        foreach (Match match in matchCollection)
        {
            try
            {
                if (match.Success)
                {
                    instructionPositions.Add(new()
                    {
                        Enable = enable,
                        Position = match.Index
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

// 161
    private static void Day3_P1()
    {
        string content = ReadFile.ReadFileInput("inputs/day3.txt");
        var results = CalculateMulResult(content);
        Console.WriteLine($"Mul result : {results.result}");
    }

    private static (int result, List<MulPosition> mulPositions) CalculateMulResult(string content)
    {
        string pattern = @"mul\((\d+),(\d+)\)";
        Regex regex = new Regex(pattern);
        List<MulPosition> mulPositions = new();

        MatchCollection matchCollection = regex.Matches(content);
        int totalResult = 0;
        foreach (Match match in matchCollection)
        {
            try
            {
                if (match.Success)
                {
                    int m1 = int.Parse(match.Groups[1].Value);
                    int m2 = int.Parse(match.Groups[2].Value);
                    mulPositions.Add(new()
                    {
                        Position = match.Index,
                        M1 = m1,
                        M2 = m2
                    });
                    totalResult += m1 * m2;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return (totalResult, mulPositions);
    }

    private static void Day2_P2()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day2.txt");
        int nbLinesSafe = CountSafeLines(arrayValue, dampener: true);
        Console.WriteLine($"nb lines safe : {nbLinesSafe}");
    }

// 402
    private static void Day2_P1()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day2.txt");
        int nbLinesSafe = CountSafeLines(arrayValue);
        Console.WriteLine($"nb lines safe : {nbLinesSafe}");
    }

    private static int CountSafeLines(ArrayValue arrayValue, bool dampener = false)
    {
        int nbLines = arrayValue.Columns.First().Values.Count;
        int nbLinesSafe = 0;

        for (int i = 0; i < nbLines; i++)
        {
            if (IsLineSafe(arrayValue, i, dampener))
            {
                nbLinesSafe++;
            }
        }

        return nbLinesSafe;
    }

    private static bool IsLineSafe(ArrayValue arrayValue, int lineIndex, bool dampener)
    {
        List<int?> lineValues = PopulateLineValue(arrayValue, lineIndex);

        if (!dampener)
        {
            return IsConsecutiveValuesValid(lineValues).isValid;
        }

        for (int i = 0; i < lineValues.Count; i++)
        {
            var tempLineValues = new List<int?>(lineValues);
            tempLineValues.RemoveAt(i);

            var (isValid, _) = IsConsecutiveValuesValid(tempLineValues);
            if (isValid)
            {
                return true;
            }
        }

        return IsConsecutiveValuesValid(lineValues).isValid;
    }

    private static (bool isValid, int index) IsConsecutiveValuesValid(List<int?> lineValues)
    {
        char? order = null;

        for (int j = 0; j < lineValues.Count - 1; j++)
        {
            int? currentValue = lineValues[j];
            int? nextValue = lineValues[j + 1];

            if (currentValue == null || nextValue == null)
                continue;

            if (!IsValuePairValid(currentValue.Value, nextValue.Value, ref order))
            {
                return (false, j + 1);
            }
        }

        return (true, 0);
    }

    private static bool IsValuePairValid(int currentValue, int nextValue, ref char? order)
    {
        if (currentValue == nextValue || Math.Abs(currentValue - nextValue) > 3)
        {
            Console.WriteLine("Unsafe 33");
            return false;
        }

        char currentOrder = currentValue > nextValue ? 'd' : 'i';

        if (order == null)
        {
            order = currentOrder;
            return true;
        }

        if (currentOrder != order)
        {
            Console.WriteLine($"Unsafe 48 => {currentOrder} / {order}");
            return false;
        }

        return true;
    }

    private static List<int?> PopulateLineValue(ArrayValue arrayValue, int indexLine)
    {
        List<int?> list = new();
        foreach (var column in arrayValue.Columns)
        {
            try
            {
                list.Add(column.Values[indexLine]);
            }
            catch (Exception e)
            {
                //
            }
        }

        return list;
    }

// 23741109
    private static void Day1_P2()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day1.txt");

        int similarityTotal = 0;
        foreach (var c1Value in arrayValue.Columns.First().Values)
        {
            if (c1Value == null) continue;
            int appearances = arrayValue.Columns[1].Values.FindAll(c2 => c2 == c1Value).Count;
            similarityTotal += appearances * c1Value.Value;
        }

        Console.WriteLine(similarityTotal);
    }

// 2166959
    private static void Day1_P1()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day1.txt");

        if (!Utils.AllColumnsHaveSameCount(arrayValue))
        {
            Console.WriteLine("Impossible !");
            return;
        }

        int distance = 0;
        List<int> toAdd = new();
        while (!Utils.AllColumnsHave0Values(arrayValue))
        {
            List<int> mins = new();
            foreach (var column in arrayValue.Columns)
            {
                mins.Add(Utils.RemoveAndReturnMinimum(column.Values));
            }

            int absValue = 0;
            foreach (var min in mins)
            {
                absValue = Math.Abs(absValue - min);
            }

            toAdd.Add(absValue);
        }

        foreach (var v in toAdd)
        {
            distance += v;
        }

        Console.WriteLine(distance);
    }
}
// See https://aka.ms/new-console-template for more information

using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode.models;
using AdventOfCode.utils;

namespace AdventOfCode;

public abstract class Program
{
    public static void Main()
    {
        Day15_P1();
    }

    private static void Day15_P1()
    {
        (ArrayValue arrayValue, string instructions) = Populate.PopulateLanternfish("inputs/day15.txt");
        Console.WriteLine($"Instructions : {instructions}");
        Console.WriteLine($"Array value :\n{arrayValue}");
        // foreach (var instruction in instructions)
        // {
            PlayInstruction(arrayValue, '>');
            // Console.WriteLine($"Array value :\n{arrayValue}");
        // }
    }

    private static void PlayInstruction(ArrayValue arrayValue, char instruction)
    {
        Cells? robotPosition = RetrieveRobotPosition(arrayValue);
        if (robotPosition == null) return;
        
        Console.WriteLine($"Robot position : {robotPosition}");

        (int nextRow, int nextCol, string? nextVal) = GetNextCell(instruction, arrayValue, robotPosition);
        if(nextVal == null || nextVal == "#") return;

        if (nextVal == ".")
        {
            arrayValue.Columns[nextCol].ValuesString[nextRow] = "@";
            arrayValue.Columns[robotPosition.Column].ValuesString[robotPosition.Row] = ".";
            // return;
        }
        
        // nextVal == "0" => Petit train
        int distanceFromNextWall = GetDistanceFromNextWall(arrayValue, instruction, nextVal, nextRow, nextCol);
        int numberOfNextO = GetNumberOfExpectedCharFromHere(arrayValue, instruction, robotPosition, "O");
        int availableSpace = GetAvailableSpaceFromLastO(arrayValue, instruction, robotPosition, numberOfNextO);
        
        // Console.WriteLine($"distanceFromNextWall : {distanceFromNextWall}");
        // Console.WriteLine($"numberOfNext0 : {numberOfNextO}");
        // Console.WriteLine($"available space : {availableSpace}");
        
    }

    private static int GetAvailableSpaceFromLastO(ArrayValue arrayValue, char instruction, Cells robotPosition, int numberOfNextO)
    {
        int newRow = robotPosition.Row;
        int newCol = robotPosition.Column;
        switch (instruction)
        {
            case 'v':
                newRow += numberOfNextO;
                break;
            case '>':
                newCol += numberOfNextO;
                break;
            case '^':
                newRow -= numberOfNextO;
                break;
            case '<':
                newCol -= numberOfNextO;
                break;
        }
        
        Console.WriteLine($"New cell : {newRow} {newCol}");

        return GetNumberOfExpectedCharFromHere(arrayValue, instruction, new()
        {
            Column = newCol,
            Row = newRow,
            Value = robotPosition.Value
        }, ".");
    }

    private static int GetNumberOfExpectedCharFromHere(ArrayValue arrayValue, char instruction, Cells robotPosition, string expectedChar)
    {
        int numberOfExpectedChar = 0;

        (int nextRow, int nextCol, string? nextVal) = GetNextCell(instruction, arrayValue, robotPosition);
        if (nextVal == expectedChar)
        {
            numberOfExpectedChar++;
            numberOfExpectedChar += GetNumberOfExpectedCharFromHere(arrayValue, instruction, new()
            {
                Row = nextRow,
                Column = nextCol,
                Value = nextVal
            }, expectedChar);
        }
        
        return numberOfExpectedChar;
    }

    private static int GetDistanceFromNextWall(ArrayValue arrayValue, char instruction, string? nextVal, int nextRow, int nextCol)
    {
        int total = 0;

        if (nextVal == "#" || nextVal == null) return total;
        total++;

        (int nextRowAgain, int nextColAgain, string? nextValAgain) = GetNextCell(instruction, arrayValue, new()
        {
            Row = nextRow,
            Column = nextCol,
            Value = nextVal
        });

        total += GetDistanceFromNextWall(arrayValue, instruction, nextValAgain, nextRowAgain, nextColAgain);
        return total;
    }

    private static (int nextRow, int nextCol, string? nextVal) GetNextCell(char instruction, ArrayValue arrayValue, Cells robotPosition)
    {
        try
        {
            switch (instruction)
            {
                case 'v':
                    return (robotPosition.Row + 1, robotPosition.Column, arrayValue.Columns[robotPosition.Column].ValuesString[robotPosition.Row + 1]);
                case '>':
                    return (robotPosition.Row, robotPosition.Column + 1, arrayValue.Columns[robotPosition.Column + 1].ValuesString[robotPosition.Row]);
                case '^':
                    return (robotPosition.Row - 1, robotPosition.Column, arrayValue.Columns[robotPosition.Column].ValuesString[robotPosition.Row - 1]);
                case '<':
                    return (robotPosition.Row, robotPosition.Column - 1, arrayValue.Columns[robotPosition.Column - 1].ValuesString[robotPosition.Row]);
                default:
                    return (0, 0, null);
            }
        }
        catch (Exception e)
        {
            return (0, 0, null);
        }
    }

    private static Cells? RetrieveRobotPosition(ArrayValue arrayValue)
    {
        for (var col = 0; col < arrayValue.Columns.Count; col++)
        {
            var column = arrayValue.Columns[col];
            for (var row = 0; row < column.ValuesString.Count; row++)
            {
                var cell = column.ValuesString[row];
                if (cell == "@")
                {
                    return new()
                    {
                        Row = row,
                        Column = col,
                        Value = cell
                    };
                }
            }
        }

        return null;
    }

    private static void Day14_P2()
    {
        List<Robot> robots = Populate.PopulateRobots("inputs/day14.txt");

        (int wide, int tall)dimensions = (101, 103);
        
        long iterations = 0;
        while (!AllUnique(robots))
        {
            foreach (var robot in robots)
            {
                UpdatePosition(dimensions, robot, 1, onlyOnce:true);
            }
            Console.WriteLine($"Iterations : {iterations}");
            iterations++;
        }
        Console.WriteLine($"14_P2 => {iterations}");
    }

    private static bool AllUnique(List<Robot> robots)
    {
        foreach (var robot in robots)
        {
            if (robots.Count(r => r.SpaceFromLeft == robot.SpaceFromLeft && r.SpaceFromTop == robot.SpaceFromTop) >
                1) return false;
        }

        return true;
    }

    private static void Day14_P1()
    {
        List<Robot> robots = Populate.PopulateRobots("inputs/day14.txt");

        (int wide, int tall)dimensions = (101, 103);
        foreach (var robot in robots)
        {
            UpdatePosition(dimensions, robot, 1);
        }

        int populationQuadrant = CalculatePopulationQuadrants(robots, dimensions);
        Console.WriteLine($"14_P1 => {populationQuadrant}");
    }

    private static int CalculatePopulationQuadrants(List<Robot> robots, (int wide, int tall) dimensions)
    {
        int middleRangeWide = (int)Math.Floor((double)dimensions.wide / 2) - 1;
        int middleRangeTall = (int)Math.Floor((double)dimensions.tall / 2) - 1;
        
        int q1 = CalculatePopulationQuadrant(robots, startWide: 0, endWide: middleRangeWide, startTall: 0, endTall: middleRangeTall);
        int q2 = CalculatePopulationQuadrant(robots, startWide: middleRangeWide +2, endWide: dimensions.wide - 1, startTall: 0, endTall: middleRangeTall);
        int q3 = CalculatePopulationQuadrant(robots, startWide: 0, endWide: middleRangeWide, startTall: middleRangeTall +2, endTall: dimensions.tall - 1);
        int q4 = CalculatePopulationQuadrant(robots, startWide: middleRangeWide +2, endWide: dimensions.wide - 1, startTall: middleRangeTall +2, endTall: dimensions.tall - 1);

        return q1 * q2 * q3 * q4;
    }

    private static int CalculatePopulationQuadrant(List<Robot> robots, int startWide, int endWide, int startTall, int endTall)
    {
        return robots.Count(r => r.SpaceFromLeft >= startWide && r.SpaceFromLeft <= endWide
                                                              && r.SpaceFromTop >= startTall && r.SpaceFromTop <= endTall);
    }

    private static void UpdatePosition((int wide, int tall) dimensions, Robot robot, int elapsedTime, bool onlyOnce = false)
    {
        if (!onlyOnce && elapsedTime > 100) return;

        int tryNewLeft = robot.SpaceFromLeft + robot.SpeedHorizontal;
        int tryNewTop = robot.SpaceFromTop + robot.SpeedVertical;
        

        if (tryNewLeft < 0)
        {
            robot.SpaceFromLeft = dimensions.wide + tryNewLeft;
        } else if (tryNewLeft > dimensions.wide - 1)
        {
            robot.SpaceFromLeft = tryNewLeft % dimensions.wide;
        }
        else
        {
            robot.SpaceFromLeft = tryNewLeft;
        }

        if (tryNewTop < 0)
        {
            robot.SpaceFromTop = dimensions.tall + tryNewTop;
        } else if (tryNewTop > dimensions.tall - 1)
        {
            robot.SpaceFromTop = tryNewTop % dimensions.tall;
        }
        else
        {
            robot.SpaceFromTop = tryNewTop;
        }
        
        if(!onlyOnce)UpdatePosition(dimensions, robot, elapsedTime + 1);
    }

    private static void Day13_P2()
    {
        List<ClawMachine> clawMachines = Populate.PopulateClawMachines("inputs/day13.txt");
        
        long total = 0;
        foreach (var clawMachine in clawMachines)
        {
            long solveComplex = SolveSystem(clawMachine, 10000000000000);
            if (solveComplex > 0)
            {
                total += solveComplex;
            }
        }
        
        Console.WriteLine($"13_P2 => {total}");
    }

    private static long SolveSystem(ClawMachine clawMachine, long needToAdd)
    {
        var prizeX = needToAdd + (double)clawMachine.PrizeX;
        var prizeY = needToAdd + (double)clawMachine.PrizeY;
        var buttonAx = (double)clawMachine.ButtonXa;
        var buttonAy = (double)clawMachine.ButtonYa;
        var buttonBx = (double)clawMachine.ButtonXb;
        var buttonBy = (double)clawMachine.ButtonYb;
        
        // Résoudre le système, enfait c'est beaucoup plus simple..
        long b = (long)Math.Round((prizeY - (prizeX / buttonAx) * buttonAy) / (buttonBy - (buttonBx / buttonAx) * buttonAy));
        long a = (long)Math.Round((prizeX - b * buttonBx) / buttonAx);

        var actualX = a * buttonAx + b * buttonBx;
        var actualY = a * buttonAy + b * buttonBy;
        if (actualX == prizeX && actualY == prizeY && a >= 0 && b >= 0)
        {
            return a * 3 + b;
        }

        return -1;
    }

    private static void Day13_P1()
    {
        List<ClawMachine> clawMachines = Populate.PopulateClawMachines("inputs/day13.txt");
        int total = 0;
        foreach (var clawMachine in clawMachines)
        {
            var presses = Solve(clawMachine);
            if(presses == null)continue;
        
            total += presses.Value.PressesA * 3 + presses.Value.PressesB;
        }
        
        Console.WriteLine($"13_P1 => {total}");
    }

    private static Dictionary<(int, int, int, int), bool> memo = new Dictionary<(int, int, int, int), bool>();

    private static (int PressesA, int PressesB)? Solve(ClawMachine clawMachine)
    {
        try
        {
            memo.Clear();

            var result = FindSolution(clawMachine, 0, 0, 0, 0, clawMachine.PrizeX, clawMachine.PrizeY);

            return result != null
                ? (result.PressesA, result.PressesB)
                : throw new Exception("Aucune solution trouvée");
        }
        catch (Exception e)
        {
            return null;
        }
    }

    private static State? FindSolution(
        ClawMachine clawMachine, 
        int currentX, 
        int currentY, 
        int pressesA, 
        int pressesB, 
        long targetX, 
        long targetY)
    {
        if (pressesA > 100 || pressesB > 100)
            return null;

        if (currentX == targetX && currentY == targetY)
            return new State 
            { 
                X = currentX, 
                Y = currentY, 
                PressesA = pressesA, 
                PressesB = pressesB 
            };

        var memoKey = (currentX, currentY, pressesA, pressesB);
        if (memo.ContainsKey(memoKey))
            return null;

        memo[memoKey] = true;

        var resultA = FindSolution(
            clawMachine, 
            currentX + clawMachine.ButtonXa, 
            currentY + clawMachine.ButtonYa, 
            pressesA + 1, 
            pressesB, 
            targetX, 
            targetY
        );

        if (resultA != null)
            return resultA;

        var resultB = FindSolution(
            clawMachine, 
            currentX + clawMachine.ButtonXb, 
            currentY + clawMachine.ButtonYb, 
            pressesA, 
            pressesB + 1, 
            targetX, 
            targetY
        );

        return resultB;
    }

    private static void Day12_P2()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day12.txt", false);

        List<Regions> regionsList = PopulateRegions(arrayValue);

        int calculatedResult = CalculateAreaTimesNumberOfSides(regionsList, arrayValue);
        Console.WriteLine($"12_P2 => {calculatedResult}");
    }

    private static int CalculateAreaTimesNumberOfSides(List<Regions> regionsList, ArrayValue arrayValue)
    {
        int total = 0;
        foreach (Regions region in regionsList)
        {
            int area = region.Cells.Count;
            int nbSides = CalculateNumberOfSides(region, arrayValue);

            total += area * nbSides;
        }

        return total;
    }

    private static int CalculateNumberOfSides(Regions region, ArrayValue arrayValue)
    {
        MarkAllOpenSides(region, arrayValue);
        AdjustOpenSides(region);
        
        int nbSides = region.Cells.Sum(c => c.MarkedSides.Count(m => m));
        Console.WriteLine($"{region} => nbSides {nbSides}");
        return nbSides;
    }

    private static void AdjustOpenSides(Regions region)
    {
        var sortedCells = region.Cells.OrderBy(c => c.Row).ThenBy(c => c.Column).ToList();
        foreach (var cell in sortedCells)
        {
            Cells? rightValue = region.Cells.FirstOrDefault(c => c.Row == cell.Row && c.Column == cell.Column + 1);
            Cells? leftValue = region.Cells.FirstOrDefault(c => c.Row == cell.Row && c.Column == cell.Column - 1);
            Cells? topValue = region.Cells.FirstOrDefault(c => c.Row == cell.Row - 1 && c.Column == cell.Column);
            Cells? bottomValue = region.Cells.FirstOrDefault(c => c.Row == cell.Row + 1 && c.Column == cell.Column);

            if (rightValue != null && rightValue.Value == cell.Value)
            {
                if (rightValue.MarkedSides[1])cell.MarkedSides[1] = false;
                if(rightValue.MarkedSides[3])cell.MarkedSides[3] = false;
            }
            
            if (leftValue != null && leftValue.Value == cell.Value)
            {
                if(leftValue.MarkedSides[1])cell.MarkedSides[1] = false;
                if(leftValue.MarkedSides[3])cell.MarkedSides[3] = false;
            }
            
            if (topValue != null && topValue.Value == cell.Value)
            {
                if(topValue.MarkedSides[0])cell.MarkedSides[0] = false;
                if(topValue.MarkedSides[2])cell.MarkedSides[2] = false;
            }
            
            if (bottomValue != null && bottomValue.Value == cell.Value)
            {
                if(bottomValue.MarkedSides[0])cell.MarkedSides[0] = false;
                if(bottomValue.MarkedSides[2])cell.MarkedSides[2] = false;
            }
        }
    }

    private static void MarkAllOpenSides(Regions region, ArrayValue arrayValue)
    {
        foreach (var cell in region.Cells)
        {
            Cells? rightValue = GetNextCell(cell.Row, cell.Column + 1, arrayValue);
            Cells? leftValue = GetNextCell(cell.Row, cell.Column - 1, arrayValue);
            Cells? topValue = GetNextCell(cell.Row - 1, cell.Column, arrayValue);
            Cells? bottomValue = GetNextCell(cell.Row + 1, cell.Column, arrayValue);

            cell.MarkedSides[0] = leftValue == null || leftValue.Value != cell.Value;
            cell.MarkedSides[1] = topValue == null || topValue.Value != cell.Value;
            cell.MarkedSides[2] = rightValue == null || rightValue.Value != cell.Value;
            cell.MarkedSides[3] = bottomValue == null || bottomValue.Value != cell.Value;
        }
    }

    private static void Day12_P1()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day12.txt", false);

        List<Regions> regionsList = PopulateRegions(arrayValue);
        int calculatedResult = CalculateAreaTimesPerimeter(regionsList);
        Console.WriteLine($"12_P1 => {calculatedResult}");
    }

    private static int CalculateAreaTimesPerimeter(List<Regions> regionsList)
    {
        int total = 0;
        foreach (Regions region in regionsList)
        {
            int area = region.Cells.Count;
            int perimeter = CalculatePerimeter(region);

            total += area * perimeter;
        }

        return total;
    }

    private static int CalculatePerimeter(Regions region)
    {
        int total = 0;
        int nbSides = 4;
        foreach (var cell in region.Cells)
        {
            int neighborRightCount = region.Cells.Count(r => r.Column == cell.Column +1 && r.Row == cell.Row);
            int neighborLeftCount = region.Cells.Count(r => r.Column == cell.Column -1 && r.Row == cell.Row);
            int neighborTopCount = region.Cells.Count(r => r.Column == cell.Column && r.Row == cell.Row - 1);
            int neighborBottomCount = region.Cells.Count(r => r.Column == cell.Column && r.Row == cell.Row + 1); 
            
            total += nbSides - (neighborRightCount + neighborLeftCount + neighborTopCount + neighborBottomCount);
        }

        return total;
    }

    private static List<Regions> PopulateRegions(ArrayValue arrayValue)
    {
        List<Regions> regionsList = new();
        for (int i = 0; i < arrayValue.Columns.Count; i++)
        {
            var column = arrayValue.Columns[i];
            for (int j = 0; j < column.ValuesString.Count; j++)
            {
                if(IsInRegionList(regionsList, j, i))continue;
                
                Regions region = new();
                Cells currentCell = new()
                {
                    Row = j,
                    Column = i,
                    Value = column.ValuesString[j]
                };
                region.Cells.Add(currentCell);
                AddNeighbors(arrayValue, region, currentCell);
                
                regionsList.Add(region);
            }
        }

        return regionsList;
    }

    private static void AddNeighbors(ArrayValue arrayValue, Regions region, Cells cell)
    {
        Cells? rightValue = GetNextCell(cell.Row, cell.Column + 1, arrayValue);
        Cells? leftValue = GetNextCell(cell.Row, cell.Column - 1, arrayValue);
        Cells? topValue = GetNextCell(cell.Row - 1, cell.Column, arrayValue);
        Cells? bottomValue = GetNextCell(cell.Row + 1, cell.Column, arrayValue);
        
        HandleNextValue(arrayValue, region, rightValue, cell);
        HandleNextValue(arrayValue, region, leftValue, cell);
        HandleNextValue(arrayValue, region, topValue, cell);
        HandleNextValue(arrayValue, region, bottomValue, cell);
    }

    private static void HandleNextValue(ArrayValue arrayValue, Regions region, Cells? nextValue, Cells currentCell)
    {
        if (nextValue != null && nextValue.Value == currentCell.Value)
        {
            if (!IsInRegion(region, nextValue.Row, nextValue.Column))
            {
                region.Cells.Add(nextValue);
                AddNeighbors(arrayValue, region, nextValue);
            }
        }
    }

    private static bool IsInRegion(Regions region, int row, int col)
    {
        return region.Cells.Any(c => c.Row == row && c.Column == col);
    }
    
    private static bool IsInRegionList(List<Regions> regionsList, int row, int col)
    {
        return regionsList.Any(r => r.Cells.Any(c => c.Row == row && c.Column == col));
    }

    private static void Day11_P2()
    {
        
        string content = ReadFile.ReadFileInput("inputs/day11.txt");
        var numbers = content.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Dictionary<string, long> currentState = new();
        
        // On compte les occurences, pas besoin de les stocker plusieurs fois
        numbers.GroupBy(x => x).ToList().ForEach(x => currentState.Add(x.Key, x.Count()));
        
        for (int i = 0; i < 75; i++)
        {
            var newState = new Dictionary<string, long>();
            foreach (var (number, count) in currentState)
            {
                ApplyRulesAndUpdateUnique(number, count, newState);
            }

            currentState = newState;
        }
        
        Console.WriteLine($"Stones : {currentState.Sum(n => n.Value)}");
    }
    
    private static void ApplyRulesAndUpdateUnique(string number, long count, Dictionary<string, long> next)
    {
        void AddOrUpdate(string key, long value)
        {
            if (next.ContainsKey(key))
            {
                next[key] += value;
            }
            else
            {
                next.Add(key, value);
            }
        }

        if (number == "0")
        {
            AddOrUpdate("1", count);
        }
        else if (number.Length % 2 == 0)
        {
            var half = number.Length / 2;
            var left = number.Substring(0, half);
            var right = number.Substring(half).TrimStart('0');
            if (right == "")
            {
                right = "0";
            }

            AddOrUpdate(left, count);
            AddOrUpdate(right, count);
        }
        else
        {
            var bigNumber = BigInteger.Parse(number);
            var multiple = bigNumber * 2024;
            AddOrUpdate(multiple.ToString(), count);
        }
    }
    
    private static void Day11_P1()
    {
        
        string content = ReadFile.ReadFileInput("inputs/day11.txt");
        List<string> stones = content.Split(" ").ToList();
        int numberOfBlinks = 25;
        for (int i = 0; i < numberOfBlinks; i++)
        {
            Blink182(stones);
            Console.WriteLine($"After {i+1} blink:\n\t{string.Join(" ", stones)}");
        }
        
        Console.WriteLine($"Stones : {stones.Count}");
    }

    private static void Blink182(List<string> stones)
    {
        for (int i = 0; i < stones.Count; i++)
        {
            // Rule 1 :
            if (stones[i] == "0")
            {
                stones[i] = "1";
            } else if (stones[i].Length % 2 == 0) // Rule 2
            {
                (string part1, string part2) = DivideStoneIntoTwo(stones[i]);
                stones[i] = part1;
                stones.Insert(i+1, part2);
                i++;
            }
            else
            {
                stones[i] = (int.Parse(stones[i]) * 2024).ToString();
            }
        }
    }

    private static (string part1, string part2) DivideStoneIntoTwo(string stone)
    {
        int middleIndex = stone.Length / 2;
        string part1 = string.Empty;
        string part2 = string.Empty;

        for (int i = 0; i < middleIndex; i++)
        {
            part1 += stone[i];
        }
        
        for (int i = middleIndex; i < stone.Length; i++)
        {
            part2 += stone[i];
        }

        return (RemovedUselessZeros(part1), RemovedUselessZeros(part2));
    }

    private static string RemovedUselessZeros(string partWithZeros)
    {
        if (!partWithZeros.StartsWith("0")) return partWithZeros;
        if (partWithZeros.All(p => p == '0')) return "0";

        char firstNot0 = partWithZeros.FirstOrDefault(p => p != '0');
        int indexOfFirstNot0 = partWithZeros.IndexOf(firstNot0);

        return partWithZeros.Substring(indexOfFirstNot0);
    }

    private static void Day10_P2()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day10.txt", false);
        List<Cells> trailHeads = RetrieveExpectedValuePositions(arrayValue);

        Console.WriteLine($"Trailheads on : {String.Join(", and ", trailHeads.Select(c => $"({c.Row},{c.Column})"))}");
        int total = 0;
        foreach (var trailHead in trailHeads)
        {
            List<List<Cells>> paths = new();
            List<Cells> score = CalculateScore(trailHead, arrayValue, paths);
            total += CountUniquePathes(paths);
        }

        Console.WriteLine($"Total score : {total}");
    }

    private static int CountUniquePathes(List<List<Cells>> paths)
    {
        HashSet<string> uniquePaths = new();
        foreach (var currentPath in paths)
        {
            string pathToString = string.Join(" -> ", currentPath.Select(c => $"({c.Row},{c.Column})"));
            uniquePaths.Add(pathToString);
        }
        
        return uniquePaths.Count;
    }

    private static void Day10_P1()
    {
        ArrayValue arrayValue = Populate.PopulateArrayValue("inputs/day10.txt", false);
        List<Cells> trailHeads = RetrieveExpectedValuePositions(arrayValue);

        Console.WriteLine($"Trailheads on : {String.Join(", and ", trailHeads.Select(c => $"({c.Row},{c.Column})"))}");
        int total = 0;
        foreach (var trailHead in trailHeads)
        {
            List<Cells> score = CalculateScore(trailHead, arrayValue, new());
            List<Cells> scoreWithoutDoublons = RemoveDoublons(score);
            Console.WriteLine(
                $"\tScore of ({trailHead.Row},{trailHead.Column}) => {scoreWithoutDoublons.Count} ({string.Join("", scoreWithoutDoublons)})");
            total += scoreWithoutDoublons.Count;
        }

        Console.WriteLine($"Total score : {total}");
    }

    private static List<Cells> RemoveDoublons(List<Cells> score)
    {
        List<Cells> newCells = new();
        foreach (var s in score)
        {
            if (newCells.Any(e => e.Column == s.Column && e.Row == s.Row)) continue;
            newCells.Add(new Cells()
            {
                Column = s.Column,
                Row = s.Row,
                Value = s.Value
            });
        }

        return newCells;
    }

    private static List<Cells> CalculateScore(Cells trailHead, ArrayValue arrayValue, List<List<Cells>> paths)
    {
        List<Cells> encounteredNines = new();
        List<Cells> path = new List<Cells> { trailHead }; // Commencer le chemin depuis le point de départ

        var (rightValue, leftValue, topValue, bottomValue) = GetNeighboorValues(trailHead, arrayValue);

        HandleNextPositionWithPath(trailHead, arrayValue, rightValue, encounteredNines, path, paths);
        HandleNextPositionWithPath(trailHead, arrayValue, leftValue, encounteredNines, path, paths);
        HandleNextPositionWithPath(trailHead, arrayValue, topValue, encounteredNines, path, paths);
        HandleNextPositionWithPath(trailHead, arrayValue, bottomValue, encounteredNines, path, paths);

        return encounteredNines;
    }

    private static void HandleNextPositionWithPath(Cells trailHead, ArrayValue arrayValue, Cells? nextTrailHead,
        List<Cells> encounteredNines, List<Cells> currentPath, List<List<Cells>> paths)
    {
        if (trailHead.Value == "9")
        {
            AddNewTrail(encounteredNines, trailHead);

            // Afficher le chemin vers ce 9
            paths.Add(currentPath);
        }

        if (nextTrailHead != null && nextTrailHead.Value != null)
        {
            try
            {
                if (trailHead.Value != null && int.Parse(nextTrailHead.Value) == int.Parse(trailHead.Value) + 1)
                {
                    // Créer une nouvelle liste de chemin pour cette branche
                    var newPath = new List<Cells>(currentPath) { nextTrailHead };

                    // Utiliser newPath au lieu de currentPath pour le prochain appel récursif
                    var subPaths = CalculateScoreWithPath(nextTrailHead, arrayValue, newPath,paths);
                    encounteredNines.AddRange(subPaths);
                }
            }
            catch (Exception e)
            {
                // Not a number
            }
        }
    }

    private static List<Cells> CalculateScoreWithPath(Cells trailHead, ArrayValue arrayValue, List<Cells> currentPath, List<List<Cells>> paths)
    {
        List<Cells> encounteredNines = new();

        var (rightValue, leftValue, topValue, bottomValue) = GetNeighboorValues(trailHead, arrayValue);

        HandleNextPositionWithPath(trailHead, arrayValue, rightValue, encounteredNines, currentPath,paths);
        HandleNextPositionWithPath(trailHead, arrayValue, leftValue, encounteredNines, currentPath,paths);
        HandleNextPositionWithPath(trailHead, arrayValue, topValue, encounteredNines, currentPath,paths);
        HandleNextPositionWithPath(trailHead, arrayValue, bottomValue, encounteredNines, currentPath,paths);

        return encounteredNines;
    }

    private static (Cells? rightValue, Cells? leftValue, Cells? topValue, Cells? bottomValue) GetNeighboorValues(
        Cells trailHead, ArrayValue arrayValue)
    {
        Cells? rightValue = GetNextCell(trailHead.Row, trailHead.Column + 1, arrayValue);
        Cells? leftValue = GetNextCell(trailHead.Row, trailHead.Column - 1, arrayValue);
        Cells? topValue = GetNextCell(trailHead.Row - 1, trailHead.Column, arrayValue);
        Cells? bottomValue = GetNextCell(trailHead.Row + 1, trailHead.Column, arrayValue);

        return (rightValue, leftValue, topValue, bottomValue);
    }

    private static Cells? GetNextCell(int newRow, int newColumn, ArrayValue arrayValue)
    {
        try
        {
            string? newValue = arrayValue.Columns[newColumn].ValuesString[newRow];
            if (newValue != null)
            {
                return new Cells()
                {
                    Column = newColumn,
                    Row = newRow,
                    Value = newValue
                };
            }
        }
        catch (Exception e)
        {
            // Do nothing, return null
        }

        return null;
    }

    private static void HandleNextPosition(Cells trailHead, ArrayValue arrayValue, Cells? nextTrailHead,
        List<Cells> encounteredNines)
    {
        if (trailHead.Value == "9")
        {
            AddNewTrail(encounteredNines, trailHead);
        }

        if (nextTrailHead != null && nextTrailHead.Value != null)
        {
            try
            {
                if (trailHead.Value != null && int.Parse(nextTrailHead.Value) == int.Parse(trailHead.Value) + 1)
                {
                    encounteredNines.AddRange(CalculateScore(nextTrailHead, arrayValue, new()));
                }
            }
            catch (Exception e)
            {
                // Not a number
            }
        }
    }

    private static void AddNewTrail(List<Cells> encounteredNines, Cells newTrailHead)
    {
        encounteredNines.Add(newTrailHead);
    }

    private static List<Cells> RetrieveExpectedValuePositions(ArrayValue arrayValue, string pattern = "0")
    {
        List<Cells> cellsList = new();
        for (int i = 0; i < arrayValue.Columns.Count; i++)
        {
            var column = arrayValue.Columns[i];
            for (int j = 0; j < column.ValuesString.Count; j++)
            {
                var cellValue = column.ValuesString[j];
                if (cellValue == pattern)
                {
                    cellsList.Add(new()
                    {
                        Column = i,
                        Row = j,
                        Value = cellValue
                    });
                }
            }
        }

        return cellsList;
    }

    private static void Day9_P2()
    {
        string readLine = ReadFile.ReadFileInput("inputs/day9.txt");
        List<ChainLink> diskMap = BuildDiskMap(readLine);
        string diskMapRearrangedString = RearrangeDiskMapIterative(diskMap);
        Console.WriteLine("Diskmap rearranged!");
        long calculatedResult = CalculateResult(diskMapRearrangedString);

        Console.WriteLine($"9_P2 => {calculatedResult}");
    }

    private static int GetLastIndexOfDigits(List<ChainLink> diskMap)
    {
        for (int i = diskMap.Count - 1; i >= 0; i--)
        {
            var chain = diskMap[i];
            if (chain.Pattern != ".") return i;
        }

        return -1;
    }

    // private static string RearrangeDiskMapP2(List<ChainLink> diskMapChainLinks, int startReverseIndex)
    // {
    //     if (startReverseIndex == -1) return string.Join("", diskMapChainLinks);
    //     var chain = diskMapChainLinks[startReverseIndex];
    //
    //     var indexOfFirstSpaceAvailable = GetIndexOfFirstSpaceAvailable(chain.Occurrency, diskMapChainLinks);
    //
    //     if (indexOfFirstSpaceAvailable != null && indexOfFirstSpaceAvailable < startReverseIndex && chain.Pattern != ".")
    //     {
    //         var spaceFound = diskMapChainLinks[indexOfFirstSpaceAvailable.Value];
    //         if (spaceFound.Occurrency == chain.Occurrency)
    //         {
    //             spaceFound.Pattern = chain.Pattern;
    //             chain.Pattern = ".";
    //         }
    //         else
    //         {
    //             // Need to split the space found into two
    //             spaceFound.Pattern = chain.Pattern;
    //             var occurrencyOffset = spaceFound.Occurrency - chain.Occurrency;
    //             spaceFound.Occurrency = chain.Occurrency;
    //             diskMapChainLinks.Insert(indexOfFirstSpaceAvailable.Value + 1, new()
    //             {
    //                 Occurrency = occurrencyOffset,
    //                 Pattern = "."
    //             });
    //             chain.Pattern = ".";
    //         }
    //     }
    //     
    //
    //     return RearrangeDiskMapP2(diskMapChainLinks, startReverseIndex - 1);
    // }

    private static string RearrangeDiskMapIterative(List<ChainLink> diskMapChainLinks)
    {
        int startReverseIndex = GetLastIndexOfDigits(diskMapChainLinks);

        while (startReverseIndex >= 0)
        {
            var chain = diskMapChainLinks[startReverseIndex];

            var indexOfFirstSpaceAvailable = GetIndexOfFirstSpaceAvailable(chain.Occurrency, diskMapChainLinks);

            if (indexOfFirstSpaceAvailable != null &&
                indexOfFirstSpaceAvailable < startReverseIndex &&
                chain.Pattern != ".")
            {
                var spaceFound = diskMapChainLinks[indexOfFirstSpaceAvailable.Value];

                if (spaceFound.Occurrency == chain.Occurrency)
                {
                    spaceFound.Pattern = chain.Pattern;
                    chain.Pattern = ".";
                }
                else
                {
                    // Need to split the space found into two
                    spaceFound.Pattern = chain.Pattern;
                    var occurrencyOffset = spaceFound.Occurrency - chain.Occurrency;
                    spaceFound.Occurrency = chain.Occurrency;
                    diskMapChainLinks.Insert(indexOfFirstSpaceAvailable.Value + 1, new()
                    {
                        Occurrency = occurrencyOffset,
                        Pattern = "."
                    });
                    chain.Pattern = ".";
                }
            }

            startReverseIndex--;
        }

        return string.Join("", diskMapChainLinks);
    }

    private static int? GetIndexOfFirstSpaceAvailable(int chainOccurrency, List<ChainLink> diskMapChainLinks)
    {
        for (var index = 0; index < diskMapChainLinks.Count; index++)
        {
            var chainLink = diskMapChainLinks[index];
            if (chainLink.Pattern == "." && chainLink.Occurrency >= chainOccurrency)
            {
                return index;
            }
        }

        return null;
    }

    private static void Day9_P1()
    {
        string readLine = ReadFile.ReadFileInput("inputs/day9.txt");
        List<ChainLink> diskMap = BuildDiskMap(readLine);
        Console.WriteLine($"BuildDiskMap {string.Join(",", diskMap)}!");
        string diskMapRearrangedString = RearrangeDiskMap(diskMap);
        Console.WriteLine($"Diskmap rearranged !");
        long calculatedResult = CalculateResult(diskMapRearrangedString);

        Console.WriteLine($"9_P1 => {calculatedResult}");
    }

    private static long CalculateResult(string diskMapRearranged)
    {
        if (diskMapRearranged.Length == 0) return 0;
        int index = 0;
        long result = 0;

        string[] splittedDiskMapRearranged = diskMapRearranged.Split("/");
        foreach (var s in splittedDiskMapRearranged)
        {
            if (string.IsNullOrWhiteSpace(s.Replace(".", "")))
            {
                index++;
                continue;
            }

            ;
            int parsedChar = int.Parse(s);
            result += index * parsedChar;
            index++;
        }

        return result;
    }

    private static string RearrangeDiskMap(List<ChainLink> diskMapChainLinks)
    {
        long index = 0;
        List<string> diskMap = string.Join("", diskMapChainLinks).Split("/").ToList();
        while (true)
        {
            Console.WriteLine($"Index : {index}");

            int positionOfFirstPoint = diskMap.IndexOf(".");
            string lastNumberChar = diskMap.Last(s => s != "." && !string.IsNullOrWhiteSpace(s));
            int positionOfLastNumber = diskMap.LastIndexOf(lastNumberChar);

            if (positionOfFirstPoint > positionOfLastNumber) return string.Join("/", diskMap);

            diskMap[positionOfFirstPoint] = lastNumberChar;
            diskMap[positionOfLastNumber] = ".";

            index++;
        }
    }

    // private static string RearrangeDiskMap(string diskMap)
    // {
    //     int positionOfFirstPoint = diskMap.IndexOf(".", StringComparison.InvariantCulture);
    //     char lastNumberChar = diskMap.LastOrDefault(s => s.ToString() != ".");
    //     int positionOfLastNumber = diskMap.LastIndexOf(lastNumberChar);
    //
    //     if (positionOfFirstPoint > positionOfLastNumber) return diskMap;
    //
    //     var diskMapArray = diskMap.ToCharArray();
    //     diskMapArray[positionOfFirstPoint] = lastNumberChar;
    //     diskMapArray[positionOfLastNumber] = '.';
    //
    //     return RearrangeDiskMap(new string(diskMapArray));
    // }

    private static List<ChainLink> BuildDiskMap(string readLine)
    {
        List<ChainLink> diskMap = [];
        int id = 0;
        bool isFreeSpace = false;

        foreach (var charReadLine in readLine)
        {
            int parsedChar = int.Parse(charReadLine.ToString());
            ChainLink chainLink = new()
            {
                Pattern = isFreeSpace ? "." : id.ToString(),
                Occurrency = parsedChar
            };
            diskMap.Add(chainLink);

            if (isFreeSpace) id++;
            isFreeSpace = !isFreeSpace;
        }

        return diskMap;
    }

    // Pas fini
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
                if (cellValue == ".") continue;
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
        Console.WriteLine($"Pour la cellule {cellValue}({j},{i}), => {string.Join(",", cellsDifferentThanCurrent)}");

        foreach (var cell in cellsDifferentThanCurrent)
        {
            PerformCreationVisitCell(arrayValue, i, j, positionsVisited, cell);
        }
    }

    private static void PerformCreationVisitCell(ArrayValue arrayValue, int i, int j, List<Cells> positionsVisited,
        Cells cell)
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

    private static void AddNewCell(ArrayValue arrayValue, List<Cells> positionsVisited, int col, int row,
        Cells initCell)
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

        if (added) PerformCreationVisitCell(arrayValue, col, row, positionsVisited, initCell);
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
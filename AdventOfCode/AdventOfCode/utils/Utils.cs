using AdventOfCode.models;

namespace AdventOfCode.utils;

public static class Utils
{
    public static int RemoveAndReturnMinimum(List<int?> numbers)
    {
        if (numbers == null || numbers.Count == 0)
        {
            throw new InvalidOperationException("La liste est vide ou null.");
        }

        int minValue = numbers.Where(n => n!=null).Min()!.Value;

        numbers.Remove(minValue);

        return minValue;
    }

    public static bool AllColumnsHaveSameCount(ArrayValue arrayValue)
    {
        int count = 0;
        for (var index = 0; index < arrayValue.Columns.Count; index++)
        {
            var cx = arrayValue.Columns[index];
            if (index == 0)
            {
                count = cx.Values.Count;
                continue;
            }

            if (cx.Values.Count != count) return false;
        }

        return true;
    }

    public static bool AllColumnsHave0Values(ArrayValue arrayValue)
    {
        return arrayValue.Columns.All(c => c.Values.Count == 0);
    }
}
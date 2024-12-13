namespace AdventOfCode.utils;

public static class ReadFile
{
    public static string ReadFileInput(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("File does not exist !");
            return string.Empty;
        }
        
        string readText = File.ReadAllText(path);
        return readText;
    }
}
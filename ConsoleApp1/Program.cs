using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Program
{
    static void Main()
    {
        // Get the parent directory of the base directory to go up one level
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        // Combine the project directory with the input file name
        string inputFilePath = Path.Combine(projectDirectory, "input.txt");
        Console.WriteLine(inputFilePath);
        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("Input file does not exist. Exiting.");
            return;
        }

        List<Point> points = ReadPointsFromFile(inputFilePath);

        if (points.Count == 0)
        {
            Console.WriteLine("No valid points found in the input file.");
            return;
        }

        DrawGraph(points);
        List<Point> furthestPoints = FindFurthestPoints(points);

        PrintFurthestPoints(furthestPoints);

        // Use Path.Combine to create the output file path inside the project directory
        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt");

        WriteFurthestPointsToFile(furthestPoints, outputPath);
    }

    static List<Point> ReadPointsFromFile(string filePath)
    {
        List<Point> points = new List<Point>();

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Point point = ParsePoint(line);
                    if (point != null)
                    {
                        points.Add(point);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        }

        return points;
    }


    static Point ParsePoint(string line)
    {
        try
        {
            string[] parts = line.Split(new[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 3)
            {
                string name = parts[0].Trim();
                int x = int.Parse(parts[1].Trim());
                int y = int.Parse(parts[2].Trim());

                return new Point(name, x, y);
            }
        }
        catch (FormatException)
        {
            Console.WriteLine($"Invalid format for input line: {line}");
        }

        return null;
    }

    static List<Point> FindFurthestPoints(List<Point> points)
    {
        double maxDistance = points.Max(p => CalculateDistance(p.X, p.Y));
        return points.Where(p => CalculateDistance(p.X, p.Y) == maxDistance).ToList();
    }

    static void PrintFurthestPoints(List<Point> furthestPoints)
    {
        Console.WriteLine("Furthest Point(s) from the center (0, 0):");
        foreach (var point in furthestPoints)
        {
            Console.WriteLine($"{point.Name} ({point.X}, {point.Y})");
            Console.WriteLine($"Quadrant: {GetQuadrant(point.X, point.Y)}");
        }
    }

    static void WriteFurthestPointsToFile(List<Point> furthestPoints, string fileName)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("Furthest Point(s) from the center (0, 0):");
                foreach (var point in furthestPoints)
                {
                    sw.WriteLine($"{point.Name} ({point.X}, {point.Y})");
                    sw.WriteLine($"Quadrant: {GetQuadrant(point.X, point.Y)}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
        }
    }

    static double CalculateDistance(int x, int y)
    {
        return Math.Sqrt(x * x + y * y);
    }

    static string GetQuadrant(int x, int y)
    {
        if (x > 0 && y > 0)
            return "Quadrant I";
        else if (x < 0 && y > 0)
            return "Quadrant II";
        else if (x < 0 && y < 0)
            return "Quadrant III";
        else if (x > 0 && y < 0)
            return "Quadrant IV";
        else
            return "On an axis";
    }
    static void DrawGraph(List<Point> points)
    {
        // Determine the dimensions of the graph
        int maxX = points.Max(p => Math.Abs(p.X));
        int maxY = points.Max(p => Math.Abs(p.Y));

        // Define the size of the smaller graph
        int graphWidth = 80;  // Adjust the width as needed
        int graphHeight = 40; // Adjust the height as needed

        // Calculate scaling factors to fit all points within the graph
        double scaleX = graphWidth / (2.0 * (maxX + 1)); // Add 1 to include the outermost point
        double scaleY = graphHeight / (2.0 * (maxY + 1)); // Add 1 to include the outermost point

        // Create an empty grid for the graph
        char[,] graph = new char[graphHeight, graphWidth];

        // Initialize the grid with empty spaces
        for (int row = 0; row < graphHeight; row++)
        {
            for (int col = 0; col < graphWidth; col++)
            {
                graph[row, col] = ' ';
            }
        }

        // Draw X and Y axes
        int xAxis = (int)(graphHeight / 2);
        int yAxis = (int)(graphWidth / 2);

        for (int row = 0; row < graphHeight; row++)
        {
            graph[row, yAxis] = '|'; // Vertical line for Y-axis
        }

        for (int col = 0; col < graphWidth; col++)
        {
            graph[xAxis, col] = '-'; // Horizontal line for X-axis
        }

        // Plot the points on the grid and add labels
        foreach (var point in points)
        {
            int x = (int)(point.X * scaleX) + yAxis;
            int y = (int)(-point.Y * scaleY) + xAxis;

            if (x >= 0 && x < graphWidth && y >= 0 && y < graphHeight)
            {
                if (graph[y, x] == ' ')
                {
                    graph[y, x] = 'o'; // Use 'o' to represent points on the graph
                }
                else
                {
                    // Handle overlapping points by using a different character
                    graph[y, x] = '*';
                }

                // Add the name to the right of the point
                int labelX = x + 1;
                int labelY = y;

                if (labelX + point.Name.Length < graphWidth)
                {
                    for (int i = 0; i < point.Name.Length; i++)
                    {
                        graph[labelY, labelX + i] = point.Name[i];
                    }
                }
            }
        }

        // Display the smaller graph
        for (int row = 0; row < graphHeight; row++)
        {
            for (int col = 0; col < graphWidth; col++)
            {
                Console.Write(graph[row, col]);
            }
            Console.WriteLine();
        }

        // Print axis labels
        Console.WriteLine($"\nX-Axis (0 to {maxX}):");
        Console.WriteLine($"Y-Axis (0 to {maxY}):");
    }

}

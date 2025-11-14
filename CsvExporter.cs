using MarkdownTaskExport.Models;
using System.Text;

namespace MarkdownTaskExport;

/// <summary>
/// Exports tasks to CSV format with proper escaping.
/// </summary>
public class CsvExporter
{
    /// <summary>
    /// Exports tasks to a CSV file with UTF-8 BOM encoding.
    /// </summary>
    public void Export(List<TaskItem> tasks, string outputPath, bool compressLevels = false, bool includeHeader = true)
    {
        if (tasks.Count == 0)
        {
            throw new InvalidOperationException("No tasks to export");
        }

        // Determine the maximum level depth based on compression mode
        int maxLevelDepth = compressLevels 
            ? DetermineMaxCompressedLevelDepth(tasks) 
            : DetermineMaxLevelDepth(tasks);

        var csv = new StringBuilder();
        
        // Build header row based on max level depth
        if (includeHeader)
        {
            csv.Append("CustomerName,ProjectName");
            for (int i = 1; i <= maxLevelDepth; i++)
            {
                csv.Append($",Level{i}");
            }
            csv.AppendLine(",Task");
        }

        // Write data rows
        foreach (var task in tasks)
        {
            csv.Append(EscapeField(task.CustomerName));
            csv.Append(',');
            csv.Append(EscapeField(task.ProjectName));
            
            if (compressLevels)
            {
                // Compressed mode: only output non-empty levels, skipping empty slots
                foreach (var level in task.Levels)
                {
                    if (!string.IsNullOrEmpty(level))
                    {
                        csv.Append(',');
                        csv.Append(EscapeField(level));
                    }
                }
                // Fill remaining columns with empty values to maintain consistent column count
                var nonEmptyCount = task.Levels.Count(l => !string.IsNullOrEmpty(l));
                for (int i = nonEmptyCount; i < maxLevelDepth; i++)
                {
                    csv.Append(',');
                }
            }
            else
            {
                // Non-compressed mode: output all levels including empty slots to preserve hierarchy
                for (int i = 0; i < maxLevelDepth; i++)
                {
                    csv.Append(',');
                    if (i < task.Levels.Count)
                    {
                        csv.Append(EscapeField(task.Levels[i]));
                    }
                }
            }
            
            csv.Append(',');
            csv.Append(EscapeField(task.Task));
            csv.AppendLine();
        }

        // Write with UTF-8 BOM for Excel compatibility
        var encoding = new UTF8Encoding(true);
        File.WriteAllText(outputPath, csv.ToString(), encoding);
    }

    /// <summary>
    /// Determines the maximum level depth across all tasks (includes empty slots).
    /// </summary>
    private int DetermineMaxLevelDepth(List<TaskItem> tasks)
    {
        int maxDepth = 0;
        
        foreach (var task in tasks)
        {
            if (task.Levels.Count > maxDepth)
                maxDepth = task.Levels.Count;
        }
        
        return maxDepth;
    }

    /// <summary>
    /// Determines the maximum number of non-empty levels across all tasks.
    /// </summary>
    private int DetermineMaxCompressedLevelDepth(List<TaskItem> tasks)
    {
        int maxDepth = 0;
        
        foreach (var task in tasks)
        {
            int nonEmptyCount = task.Levels.Count(l => !string.IsNullOrEmpty(l));
            if (nonEmptyCount > maxDepth)
                maxDepth = nonEmptyCount;
        }
        
        return maxDepth;
    }

    /// <summary>
    /// Escapes a CSV field according to RFC 4180.
    /// - Wraps in quotes if contains comma, newline, or quote
    /// - Doubles any quotes inside the field
    /// </summary>
    private string EscapeField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;

        // Check if escaping is needed
        bool needsQuotes = field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r');

        if (needsQuotes)
        {
            // Double any existing quotes
            field = field.Replace("\"", "\"\"");
            // Wrap in quotes
            return $"\"{field}\"";
        }

        return field;
    }
}

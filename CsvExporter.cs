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
    public void Export(List<TaskItem> tasks, string outputPath)
    {
        if (tasks.Count == 0)
        {
            throw new InvalidOperationException("No tasks to export");
        }

        // Determine the maximum header depth needed
        int maxHeaderDepth = DetermineMaxHeaderDepth(tasks);

        var csv = new StringBuilder();
        
        // Build header row based on max header depth
        csv.Append("CustomerName,ProjectName");
        for (int i = 1; i <= maxHeaderDepth; i++)
        {
            csv.Append($",Header{i}");
        }
        csv.AppendLine(",Task");

        // Write data rows
        foreach (var task in tasks)
        {
            csv.Append(EscapeField(task.CustomerName));
            csv.Append(',');
            csv.Append(EscapeField(task.ProjectName));
            
            // Only include headers that have values
            var headers = GetNonEmptyHeaders(task);
            for (int i = 0; i < maxHeaderDepth; i++)
            {
                csv.Append(',');
                if (i < headers.Count)
                {
                    csv.Append(EscapeField(headers[i]));
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
    /// Determines the maximum header depth across all tasks.
    /// </summary>
    private int DetermineMaxHeaderDepth(List<TaskItem> tasks)
    {
        int maxDepth = 0;
        
        foreach (var task in tasks)
        {
            int depth = 0;
            if (!string.IsNullOrEmpty(task.Header1)) depth = 1;
            if (!string.IsNullOrEmpty(task.Header2)) depth = 2;
            if (!string.IsNullOrEmpty(task.Header3)) depth = 3;
            
            if (depth > maxDepth)
                maxDepth = depth;
        }
        
        return maxDepth;
    }

    /// <summary>
    /// Gets non-empty headers from a task item in order.
    /// </summary>
    private List<string> GetNonEmptyHeaders(TaskItem task)
    {
        var headers = new List<string>();
        
        if (!string.IsNullOrEmpty(task.Header1))
            headers.Add(task.Header1);
        
        if (!string.IsNullOrEmpty(task.Header2))
            headers.Add(task.Header2);
        
        if (!string.IsNullOrEmpty(task.Header3))
            headers.Add(task.Header3);
        
        return headers;
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

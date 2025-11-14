using MarkdownTaskExport.Models;
using System.Text.RegularExpressions;

namespace MarkdownTaskExport;

/// <summary>
/// Parses markdown files to extract outstanding tasks with their hierarchical context.
/// </summary>
public class MarkdownParser
{
    private static readonly Regex HeaderRegex = new(@"^(#{2,})\s+(.+)$", RegexOptions.Compiled);
    private static readonly Regex TaskRegex = new(@"^(\s*)-\s+\[([ ])\]\s+(.+)$", RegexOptions.Compiled);
    private static readonly Regex CompletedTaskRegex = new(@"^(\s*)-\s+\[(x|X)\]", RegexOptions.Compiled);
    private static readonly Regex CheckmarkRegex = new(@"✅\s+\d{4}-\d{2}-\d{2}", RegexOptions.Compiled);

    /// <summary>
    /// Parses a markdown file and extracts all outstanding tasks.
    /// </summary>
    public List<TaskItem> ParseFile(string filePath, string customerName, string projectName)
    {
        var tasks = new List<TaskItem>();
        var lines = File.ReadAllLines(filePath);
        
        // Use a list to track headers at any depth (0-indexed, where 0 is ##, 1 is ###, etc.)
        var headers = new List<string>();
        
        // Track parent tasks for nested structure
        var parentTaskStack = new Stack<(int indentLevel, string taskText)>();
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            
            // Check for headers (##, ###, ####, etc.)
            var headerMatch = HeaderRegex.Match(line);
            if (headerMatch.Success)
            {
                var headerLevel = headerMatch.Groups[1].Value.Length - 1; // Subtract 1 because ## is level 1 (index 0)
                var headerText = headerMatch.Groups[2].Value.Trim();
                
                // Ensure the headers list is large enough
                while (headers.Count <= headerLevel)
                {
                    headers.Add(string.Empty);
                }
                
                // Set this header level
                headers[headerLevel] = headerText;
                
                // Clear any deeper levels
                for (int j = headerLevel + 1; j < headers.Count; j++)
                {
                    headers[j] = string.Empty;
                }
                
                // Clear parent task stack when we hit a new header
                parentTaskStack.Clear();
                continue;
            }
            
            // Skip completed tasks
            if (CompletedTaskRegex.IsMatch(line))
                continue;
            
            // Check for unchecked tasks
            var taskMatch = TaskRegex.Match(line);
            if (taskMatch.Success)
            {
                var indent = taskMatch.Groups[1].Value.Length;
                var taskText = taskMatch.Groups[3].Value.Trim();
                
                // Skip tasks with checkmark indicators
                if (CheckmarkRegex.IsMatch(taskText))
                    continue;
                
                // Check if this task has sub-tasks by looking ahead
                bool hasSubTasks = HasSubTasks(lines, i + 1, indent);
                
                if (hasSubTasks)
                {
                    // This is a parent task, push it onto the stack
                    // Remove any parent tasks at the same or deeper level
                    while (parentTaskStack.Count > 0 && parentTaskStack.Peek().indentLevel >= indent)
                    {
                        parentTaskStack.Pop();
                    }
                    parentTaskStack.Push((indent, taskText));
                }
                else
                {
                    // This is a task to export
                    // Remove parent tasks at the same or deeper level
                    while (parentTaskStack.Count > 0 && parentTaskStack.Peek().indentLevel >= indent)
                    {
                        parentTaskStack.Pop();
                    }
                    
                    // Build the task item with proper header hierarchy
                    var task = new TaskItem
                    {
                        CustomerName = customerName,
                        ProjectName = projectName,
                        Task = taskText
                    };
                    
                    // Add document headers - keep all levels including empty ones for proper structure
                    var levels = new List<string>();
                    
                    // Copy all header levels (including empty ones) to preserve hierarchy
                    levels.AddRange(headers);
                    
                    // Add parent task levels
                    if (parentTaskStack.Count > 0)
                    {
                        var parents = parentTaskStack.Reverse().ToList();
                        foreach (var parent in parents)
                        {
                            levels.Add(parent.taskText);
                        }
                    }
                    
                    task.Levels = levels;
                    tasks.Add(task);
                }
            }
        }
        
        return tasks;
    }
    
    /// <summary>
    /// Checks if a task has sub-tasks by looking at following lines.
    /// </summary>
    private bool HasSubTasks(string[] lines, int startIndex, int currentIndent)
    {
        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
            
            // Empty lines don't matter
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            // If we hit a header, no more sub-tasks
            if (HeaderRegex.IsMatch(line))
                return false;
            
            // Check if this is a task
            var taskMatch = TaskRegex.Match(line);
            if (taskMatch.Success)
            {
                var indent = taskMatch.Groups[1].Value.Length;
                
                // If indented more than current task, it's a sub-task
                if (indent > currentIndent)
                    return true;
                
                // If at same or less indent, no sub-tasks
                if (indent <= currentIndent)
                    return false;
            }
        }
        
        return false;
    }
}

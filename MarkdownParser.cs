using MarkdownTaskExport.Models;
using System.Text.RegularExpressions;

namespace MarkdownTaskExport;

/// <summary>
/// Parses markdown files to extract outstanding tasks with their hierarchical context.
/// </summary>
public class MarkdownParser
{
    private static readonly Regex HeaderRegex = new(@"^(#{2,4})\s+(.+)$", RegexOptions.Compiled);
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
        
        string header1 = string.Empty;
        string header2 = string.Empty;
        string header3 = string.Empty;
        
        // Track parent tasks for nested structure
        var parentTaskStack = new Stack<(int indentLevel, string taskText)>();
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            
            // Check for headers (##, ###, ####)
            var headerMatch = HeaderRegex.Match(line);
            if (headerMatch.Success)
            {
                var headerLevel = headerMatch.Groups[1].Value.Length;
                var headerText = headerMatch.Groups[2].Value.Trim();
                
                switch (headerLevel)
                {
                    case 2: // ##
                        header1 = headerText;
                        header2 = string.Empty;
                        header3 = string.Empty;
                        break;
                    case 3: // ###
                        header2 = headerText;
                        header3 = string.Empty;
                        break;
                    case 4: // ####
                        header3 = headerText;
                        break;
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
                    
                    // Apply headers based on nesting level
                    if (parentTaskStack.Count == 0)
                    {
                        // No parent tasks, use document headers
                        task.Header1 = header1;
                        task.Header2 = header2;
                        task.Header3 = header3;
                    }
                    else if (parentTaskStack.Count == 1)
                    {
                        // One parent task
                        task.Header1 = header1;
                        task.Header2 = string.IsNullOrEmpty(header2) ? parentTaskStack.Peek().taskText : header2;
                        task.Header3 = string.IsNullOrEmpty(header2) ? string.Empty : parentTaskStack.Peek().taskText;
                    }
                    else if (parentTaskStack.Count == 2)
                    {
                        // Two parent tasks
                        var parents = parentTaskStack.ToArray();
                        task.Header1 = header1;
                        task.Header2 = string.IsNullOrEmpty(header2) ? parents[1].taskText : header2;
                        task.Header3 = string.IsNullOrEmpty(header2) ? parents[0].taskText : (string.IsNullOrEmpty(header3) ? parents[1].taskText : header3);
                    }
                    else
                    {
                        // More than two parent tasks - take the last ones
                        var parents = parentTaskStack.Take(3).Reverse().ToArray();
                        task.Header1 = header1;
                        task.Header2 = string.IsNullOrEmpty(header2) ? (parents.Length > 1 ? parents[1].taskText : parents[0].taskText) : header2;
                        task.Header3 = string.IsNullOrEmpty(header2) ? parents[0].taskText : (string.IsNullOrEmpty(header3) ? (parents.Length > 0 ? parents[0].taskText : string.Empty) : header3);
                    }
                    
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

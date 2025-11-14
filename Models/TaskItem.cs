namespace MarkdownTaskExport.Models;

/// <summary>
/// Represents a task extracted from a markdown file with its hierarchical context.
/// </summary>
public class TaskItem
{
    public string CustomerName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public List<string> Levels { get; set; } = new List<string>();
    public string Task { get; set; } = string.Empty;
}

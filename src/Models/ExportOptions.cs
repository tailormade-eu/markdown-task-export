namespace MarkdownTaskExport.Models;

/// <summary>
/// Configuration options for task export.
/// </summary>
public class ExportOptions
{
    public string InputPath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = "outstanding_tasks.csv";
    public bool Verbose { get; set; }
    public bool CompressLevels { get; set; } = false;
    public bool IncludeHeader { get; set; } = true;
    public char Delimiter { get; set; } = ',';
}

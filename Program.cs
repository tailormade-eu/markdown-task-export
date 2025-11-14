using MarkdownTaskExport;
using MarkdownTaskExport.Models;
using Serilog;
using Serilog.Events;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var options = ParseArguments(args);
    
    if (options == null)
    {
        ShowHelp();
        return 1;
    }

    // Update log level if verbose
    if (options.Verbose)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .CreateLogger();
    }

    Log.Information("Markdown Task Export Tool");
    Log.Information("Input: {Input}", options.InputPath);
    Log.Information("Output: {Output}", options.OutputPath);
    Log.Information("");

    // Validate input path
    if (!Directory.Exists(options.InputPath))
    {
        Log.Error("Input directory not found: {Path}", options.InputPath);
        return 2;
    }

    // Extract tasks
    var extractor = new TaskExtractor(Log.Logger);
    var tasks = extractor.ExtractTasks(options.InputPath, options.Verbose);

    if (tasks.Count == 0)
    {
        Log.Warning("No outstanding tasks found.");
        return 3;
    }

    Log.Information("Found {Count} outstanding task(s)", tasks.Count);

    // Export to CSV
    try
    {
        var exporter = new CsvExporter();
        exporter.Export(tasks, options.OutputPath);
        
        Log.Information("Successfully exported to: {Output}", options.OutputPath);
        return 0;
    }
    catch (Exception ex)
    {
        Log.Error("Failed to write output file: {Error}", ex.Message);
        return 4;
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unexpected error occurred");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static ExportOptions? ParseArguments(string[] args)
{
    var options = new ExportOptions();
    
    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "-i":
            case "--input":
                if (i + 1 < args.Length)
                {
                    options.InputPath = args[++i];
                }
                break;
            
            case "-o":
            case "--output":
                if (i + 1 < args.Length)
                {
                    options.OutputPath = args[++i];
                }
                break;
            
            case "-v":
            case "--verbose":
                options.Verbose = true;
                break;
            
            case "-h":
            case "--help":
                return null;
            
            case "--version":
                Console.WriteLine("Markdown Task Export v1.0.0");
                Console.WriteLine("Repository: https://github.com/tailormade-eu/markdown-task-export");
                Environment.Exit(0);
                break;
        }
    }
    
    // Validate required arguments
    if (string.IsNullOrEmpty(options.InputPath))
    {
        return null;
    }
    
    return options;
}

static void ShowHelp()
{
    Console.WriteLine("Markdown Task Export Tool");
    Console.WriteLine("Repository: https://github.com/tailormade-eu/markdown-task-export");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  MarkdownTaskExport -i <input-path> [-o <output-path>] [-v]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -i, --input <path>     Path to Customers folder (required)");
    Console.WriteLine("  -o, --output <path>    Output CSV file path (default: outstanding_tasks.csv)");
    Console.WriteLine("  -v, --verbose          Show detailed processing information");
    Console.WriteLine("  -h, --help             Show help information");
    Console.WriteLine("  --version              Show version information");
    Console.WriteLine();
    Console.WriteLine("Example:");
    Console.WriteLine("  MarkdownTaskExport -i ./Customers -o tasks.csv");
    Console.WriteLine("  MarkdownTaskExport -i \"C:\\Projects\\Customers\" -v");
}

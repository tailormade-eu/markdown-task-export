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
    
    // Handle PowerShell's escaped quote issue by re-splitting arguments if needed
    var cleanedArgs = CleanAndSplitArguments(args);
    
    for (int i = 0; i < cleanedArgs.Count; i++)
    {
        switch (cleanedArgs[i])
        {
            case "-i":
            case "--input":
                if (i + 1 < cleanedArgs.Count)
                {
                    options.InputPath = cleanedArgs[++i];
                }
                break;
            
            case "-o":
            case "--output":
                if (i + 1 < cleanedArgs.Count)
                {
                    var outputArg = cleanedArgs[++i];
                    
                    // Check if output is a directory - if so, append default filename
                    if (Directory.Exists(outputArg))
                    {
                        options.OutputPath = Path.Combine(outputArg, "outstanding_tasks.csv");
                    }
                    else if (!outputArg.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        // If it's not a .csv file, treat it as a directory and append default filename
                        options.OutputPath = Path.Combine(outputArg, "outstanding_tasks.csv");
                    }
                    else
                    {
                        options.OutputPath = outputArg;
                    }
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

static List<string> CleanAndSplitArguments(string[] args)
{
    var result = new List<string>();
    
    foreach (var arg in args)
    {
        // Check if this argument contains an escaped quote followed by a flag
        // Pattern: ends with " followed by space and a flag like -o, -i, etc.
        var pattern = @"^(.+?)\""\s+(-[iovh]|--\w+)(.*)$";
        var match = System.Text.RegularExpressions.Regex.Match(arg, pattern);
        
        if (match.Success)
        {
            // Split this malformed argument
            result.Add(match.Groups[1].Value.Trim('"')); // The path without trailing quote
            result.Add(match.Groups[2].Value); // The flag (-o, -i, etc.)
            
            // If there's more after the flag, add it too
            if (!string.IsNullOrWhiteSpace(match.Groups[3].Value))
            {
                result.Add(match.Groups[3].Value.Trim().Trim('"'));
            }
        }
        else
        {
            // Normal argument, just clean quotes
            result.Add(arg.Trim('"'));
        }
    }
    
    return result;
}

static void ShowHelp()
{
    Console.WriteLine("Markdown Task Export Tool");
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

using MarkdownTaskExport.Models;
using Serilog;

namespace MarkdownTaskExport;

/// <summary>
/// Extracts tasks from a directory structure of markdown files.
/// </summary>
public class TaskExtractor
{
    private readonly MarkdownParser _parser;
    private readonly ILogger _logger;

    public TaskExtractor(ILogger logger)
    {
        _parser = new MarkdownParser();
        _logger = logger;
    }

    /// <summary>
    /// Extracts all outstanding tasks from the Customers directory.
    /// </summary>
    public List<TaskItem> ExtractTasks(string customersPath, bool verbose)
    {
        var allTasks = new List<TaskItem>();
        
        if (!Directory.Exists(customersPath))
        {
            _logger.Error("Customers directory not found: {Path}", customersPath);
            return allTasks;
        }

        var customerDirs = Directory.GetDirectories(customersPath);
        
        foreach (var customerDir in customerDirs)
        {
            var customerName = Path.GetFileName(customerDir);
            
            // Skip hidden directories
            if (customerName.StartsWith("."))
                continue;
            
            ProcessCustomerDirectory(customerDir, customerName, allTasks, verbose);
        }
        
        return allTasks;
    }

    /// <summary>
    /// Processes a customer directory recursively to find all markdown files.
    /// </summary>
    private void ProcessCustomerDirectory(string directory, string customerName, List<TaskItem> allTasks, bool verbose)
    {
        try
        {
            // Process all markdown files in this directory
            var markdownFiles = Directory.GetFiles(directory, "*.md");
            
            foreach (var file in markdownFiles)
            {
                var projectName = Path.GetFileNameWithoutExtension(file);
                
                if (verbose)
                    _logger.Information("Processing: {Customer} / {Project}", customerName, projectName);
                
                try
                {
                    var tasks = _parser.ParseFile(file, customerName, projectName);
                    
                    if (tasks.Count > 0)
                    {
                        allTasks.AddRange(tasks);
                        
                        if (verbose)
                            _logger.Information("  Found {Count} task(s)", tasks.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning("Failed to process file {File}: {Error}", file, ex.Message);
                }
            }
            
            // Process subdirectories recursively
            var subdirs = Directory.GetDirectories(directory);
            
            foreach (var subdir in subdirs)
            {
                var subdirName = Path.GetFileName(subdir);
                
                // Skip hidden directories
                if (subdirName.StartsWith("."))
                    continue;
                
                ProcessCustomerDirectory(subdir, customerName, allTasks, verbose);
            }
        }
        catch (Exception ex)
        {
            _logger.Warning("Failed to access directory {Directory}: {Error}", directory, ex.Message);
        }
    }
}

# GitHub Copilot Workspace Instructions

## Development Philosophy

### Keep It Simple, Stupid (KISS)
When creating code:
- Write simple, clear, and maintainable code
- Avoid over-engineering or premature optimization
- Prefer straightforward solutions over clever ones
- Use clear variable and function names
- Keep functions small and focused on a single task
- Write code that is easy to understand and modify
- Add comments only when necessary to explain "why", not "what"

### Iteration-Based Development
- Development follows an iterative approach
- Each iteration builds upon the previous one
- Start with core functionality first
- Add features incrementally
- Test and validate each iteration before moving forward
- Refactor and improve in subsequent iterations
- Keep each iteration deployable and functional

## Standard NuGet Packages

When developing .NET projects, prefer these standard packages:

### Database & Data Access
- **Entity Framework Core** - For database operations and ORM
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.SqlServer` (or appropriate provider)
  - `Microsoft.EntityFrameworkCore.Tools`

### Error Handling & Result Patterns
- **FluentResults** - For functional error handling and result patterns
  - `FluentResults`
  - Use instead of throwing exceptions for expected failures
  - Return `Result<T>` or `Result` from methods

### Logging
- **Serilog** - For structured logging
  - `Serilog`
  - `Serilog.Sinks.Console` - For console applications
  - `Serilog.Sinks.Seq` - For Seq integration
  - `Serilog.Extensions.Logging.File` - For file logging
  - `Serilog.Settings.Configuration` - For configuration integration
- Use **Microsoft.Extensions.Logging.ILogger** interface in classes
- Configure Serilog in Program.cs/Startup for console apps
- Use structured logging with message templates

### Logging Best Practices
```csharp
// In classes, inject ILogger
public class MyService
{
    private readonly ILogger<MyService> _logger;
    
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public void DoWork()
    {
        _logger.LogInformation("Processing {ItemCount} items", count);
    }
}

// In console apps (Program.cs), configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
```

## Project-Specific Guidelines

### Markdown Task Export Tool

This is a simple console application that extracts tasks from Markdown files. Keep it lightweight:
- **No database** - This is a file processing tool only
- **Minimal dependencies** - Use only Serilog for logging, avoid EF Core
- **FluentResults** - Use for error handling instead of exceptions
- **Simple file I/O** - Use standard .NET file operations
- **CSV generation** - Manual CSV writing with proper escaping (no heavy libraries)

### Command-Line Argument Handling

When processing command-line arguments for file paths:
- **Always strip quotes** - Path arguments may have leading/trailing quotes that need to be removed
- **Handle escaped quotes** - PowerShell/Windows escapes trailing backslashes before quotes (e.g., `"C:\Path\"` becomes `C:\Path"`)
- **Clean all path arguments** - Use `Trim('"')` or similar to remove quotes from path values
- **Example**: `"C:\Users\Path\"` may be parsed as `C:\Users\Path" -o` due to escaped quote - clean this before using

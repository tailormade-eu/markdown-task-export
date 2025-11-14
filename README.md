# markdown-task-export

Extract outstanding tasks from Markdown files and export to CSV for time tracking applications like ManicTime.

## Description

A C# console application that scans a directory structure of customer project Markdown files and generates a CSV export of all outstanding tasks with their hierarchical context.

Perfect for consultants and project managers who track work in Markdown and need integration with time tracking software.

## Features

- 📁 **Recursive Folder Scanning** - Traverses customer directory structure
- ✅ **Task Extraction** - Finds all unchecked tasks (`- [ ]`)
- 🏗️ **Hierarchical Context** - Maintains header structure (unlimited depth)
- 📊 **Dynamic Levels** - Supports headers at any depth (Level1, Level2, Level3, ...)
- 🗜️ **Compression Mode** - Optional removal of empty hierarchy columns
- 📋 **Header Control** - Include or exclude CSV header row
- 🔣 **Configurable Delimiter** - Choose comma or semicolon delimiter
- 👥 **Customer/Project Detection** - Extracts names from folder structure
- 📊 **CSV Export** - ManicTime-compatible format
- 🔤 **Smart Escaping** - Handles commas, quotes, special characters
- 🚀 **Fast & Lightweight** - Processes hundreds of files in seconds

## Quick Start

```bash
# Clone repository
git clone https://github.com/tailormade-eu/markdown-task-export.git
cd markdown-task-export

# Build
dotnet build

# Run
dotnet run -- --input "C:\path\to\Customers" --output "tasks.csv"
```

## Installation

### Prerequisites

- .NET 10 SDK
- Windows, Linux, or macOS

### Build from Source

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build -c Release

# Run tests
dotnet test
```

### Create Standalone Executable

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained -o ./publish

# Linux
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish

# macOS
dotnet publish -c Release -r osx-x64 --self-contained -o ./publish
```

## Usage

### Basic Command

```bash
dotnet run -- --input "./Customers" --output "outstanding_tasks.csv"
```

### Command Line Options

```
Options:
  -i, --input <path>       Path to Customers folder (required)
  -o, --output <path>      Output CSV file path (default: outstanding_tasks.csv)
  -d, --delimiter <value>  CSV delimiter: 'comma' or 'semicolon' (default: comma)
  -v, --verbose            Show detailed processing information
  --compress-levels        Compress empty levels (skip empty hierarchy columns)
  --no-header              Exclude CSV header row
  -h, --help               Show help information
  --version                Show version information
```

### Examples

```bash
# Basic usage
dotnet run -- -i "C:\Projects\Customers"

# Custom output location
dotnet run -- -i "./Customers" -o "C:\Reports\tasks_2025.csv"

# Verbose mode for debugging
dotnet run -- -i "./Customers" -v

# Compress empty hierarchy levels
dotnet run -- -i "./Customers" -o "tasks.csv" --compress-levels

# Export without header row
dotnet run -- -i "./Customers" -o "tasks.csv" --no-header

# Use semicolon delimiter for Excel in some regions
dotnet run -- -i "./Customers" -o "tasks.csv" --delimiter semicolon

# Use semicolon with short flag
dotnet run -- -i "./Customers" -d ";" 

# Combine options
dotnet run -- -i "./Customers" -v --compress-levels --no-header

# Using published executable
./TaskExport -i "./Customers" -o "tasks.csv"
```

## Input Format

### Expected Folder Structure

Your Markdown files should be organized as:

```
Customers/
├── Customer A/
│   ├── Project 1.md
│   └── Project 2.md
├── Customer B/
│   ├── Subfolder/
│   │   └── Project X.md
│   └── Project Y.md
```

### Markdown Task Format

Uses standard Markdown task checkboxes:

```markdown
## Main Section

- [ ] Outstanding task to do
- [x] Completed task (ignored)
- [ ] Another task with commas, quotes, and "special" chars

### Subsection

- [ ] Task under subsection
  - [ ] Nested sub-task

#### Deep Section (Header3)

- [ ] Task at third header level
```

## Output Format

### CSV Structure

```csv
CustomerName,ProjectName,Level1,Level2,Task
Customer A,Project 1,,Main Section,Outstanding task to do
Customer A,Project 1,,Main Section,Subsection,Task under subsection
Customer A,Project 1,,Main Section,Subsection,Nested sub-task
Customer A,Project 1,,Main Section,Subsection,Deep Section (Header3),Task at third header level
```

**Note:** Column names changed from `Header1,Header2,Header3` to `Level1,Level2,Level3,...` to support unlimited depth.

### Key Features

- **Dynamic Columns**: Automatically adapts to the maximum header depth in your files
- **Compression Mode**: Use `--compress-levels` to remove empty hierarchy columns
- **Header Control**: Use `--no-header` to exclude the CSV header row
- **Proper CSV Escaping**: Handles commas, quotes, newlines in task descriptions
- **UTF-8 with BOM**: Ensures Excel compatibility
- **Hierarchical Structure**: Preserves unlimited levels of markdown headers

## Use Cases

This tool is designed for:

- **Consultants** tracking multiple client projects
- **Project Managers** organizing team tasks in Markdown
- **Freelancers** integrating with time tracking software
- **Teams** using Obsidian or similar Markdown-based tools
- **Anyone** who needs structured task exports from Markdown

## Technical Details

### Technology Stack

- C# 13 / .NET 10
- Standard libraries (no external dependencies required)

### Project Structure

```
markdown-task-export/
├── src/
│   ├── Program.cs              # Entry point & CLI
│   ├── TaskExtractor.cs        # Core extraction logic
│   ├── MarkdownParser.cs       # Markdown parsing
│   ├── CsvExporter.cs          # CSV generation
│   └── Models/
│       ├── TaskItem.cs         # Task data model
│       └── ExportOptions.cs    # Configuration
├── tests/
│   ├── TaskExtractorTests.cs
│   └── fixtures/               # Test markdown files
├── README.md
└── TaskExport.csproj
```

## Configuration

### Environment Variables (Optional)

```bash
# Default input path
export TASK_EXPORT_INPUT="./Customers"

# Default output path
export TASK_EXPORT_OUTPUT="./tasks.csv"
```

## Troubleshooting

### Common Issues

**"Cannot find path to Customers folder"**
- Verify the path exists and is accessible
- Use absolute paths to avoid confusion
- Check file permissions

**"No tasks found"**
- Ensure files contain unchecked tasks: `- [ ]`
- Verify files are in expected folder structure
- Use verbose mode (`-v`) to see which files are processed

**"CSV encoding issues in Excel"**
- Tool outputs UTF-8 with BOM for Excel
- Try "Import Data" in Excel if double-click doesn't work
- Use a text editor to verify CSV format

## Development

### Running Tests

```bash
dotnet test
```

### Code Style

- Follow C# naming conventions
- Use async/await for file operations
- Add XML documentation comments
- Keep methods focused and testable

### Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Submit a pull request

## Roadmap

- [x] Configuration file support (JSON/YAML)
- [x] Configurable CSV delimiter (comma/semicolon)
- [ ] Filter tasks by date range
- [ ] Support custom task patterns (beyond `- [ ]`)
- [ ] Export statistics and summary
- [ ] Watch mode for continuous export
- [ ] GUI wrapper for non-technical users
- [ ] Docker image

## Changelog

### v1.0.1 (2025-11-14)
- ✨ Added configurable CSV delimiter (comma or semicolon)
- ✨ Added `-d/--delimiter` command-line option
- 🌍 Improved Excel compatibility for European locales
- 📝 Updated documentation with delimiter examples

### v1.0.0 (2025-11-11)
- 🎉 Initial release
- ✅ Task extraction from markdown files
- ✅ Hierarchical header parsing
- ✅ CSV export with compression mode
- ✅ Customer/Project detection
- ✅ Command-line interface

## Related Projects

- **[obsidian-task-export-plugin](https://github.com/tailormade-eu/obsidian-task-export-plugin)** - Obsidian plugin version with auto-export

## License

MIT License - See LICENSE file for details

## Author

Created for personal use in managing customer project tasks and time tracking integration.

## Support

- 🐛 **Issues**: [Report a bug](https://github.com/tailormade-eu/markdown-task-export/issues)
- 💬 **Discussions**: [Ask a question](https://github.com/tailormade-eu/markdown-task-export/discussions)
- ⭐ **Star** the repo if you find it useful!

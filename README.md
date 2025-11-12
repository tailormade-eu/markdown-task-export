# markdown-task-export

Extract outstanding tasks from Markdown files and export to CSV for time tracking applications like ManicTime.

## Description

A C# console application that scans a directory structure of customer project Markdown files and generates a CSV export of all outstanding tasks with their hierarchical context.

Perfect for consultants and project managers who track work in Markdown and need integration with time tracking software.

## Features

- 📁 **Recursive Folder Scanning** - Traverses customer directory structure
- ✅ **Task Extraction** - Finds all unchecked tasks (`- [ ]`)
- 🏗️ **Hierarchical Context** - Maintains header structure (up to 3 levels)
- 👥 **Customer/Project Detection** - Extracts names from folder structure
- 📊 **CSV Export** - ManicTime-compatible format
- 🔤 **Smart Escaping** - Handles commas, quotes, special characters
- 🚀 **Fast & Lightweight** - Processes hundreds of files in seconds

## Quick Start

```bash
# Clone repository
git clone https://github.com/yourusername/markdown-task-export.git
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
  -i, --input <path>     Path to Customers folder (required)
  -o, --output <path>    Output CSV file path (default: outstanding_tasks.csv)
  -v, --verbose          Show detailed processing information
  -h, --help             Show help information
  --version              Show version information
```

### Examples

```bash
# Basic usage
dotnet run -- -i "C:\Projects\Customers"

# Custom output location
dotnet run -- -i "./Customers" -o "C:\Reports\tasks_2025.csv"

# Verbose mode for debugging
dotnet run -- -i "./Customers" -v

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
CustomerName,ProjectName,Header1,Header2,Header3,Task
Customer A,Project 1,Main Section,Outstanding task to do
Customer A,Project 1,Main Section,Subsection,Task under subsection
Customer A,Project 1,Main Section,Subsection,Nested sub-task
Customer A,Project 1,Main Section,Subsection,Deep Section (Header3),Task at third header level
```

### Key Features

- **Dynamic Columns**: Only includes header columns that have values (no empty trailing commas)
- **Proper CSV Escaping**: Handles commas, quotes, newlines in task descriptions
- **UTF-8 with BOM**: Ensures Excel compatibility
- **Hierarchical Structure**: Preserves up to 3 levels of markdown headers

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

- [ ] Configuration file support (JSON/YAML)
- [ ] Filter tasks by date range
- [ ] Support custom task patterns (beyond `- [ ]`)
- [ ] Export statistics and summary
- [ ] Watch mode for continuous export
- [ ] GUI wrapper for non-technical users
- [ ] Docker image

## Related Projects

- **[obsidian-task-export-plugin](https://github.com/yourusername/obsidian-task-export-plugin)** - Obsidian plugin version with auto-export

## License

MIT License - See LICENSE file for details

## Author

Created for personal use in managing customer project tasks and time tracking integration.

## Support

- 🐛 **Issues**: [Report a bug](https://github.com/yourusername/markdown-task-export/issues)
- 💬 **Discussions**: [Ask a question](https://github.com/yourusername/markdown-task-export/discussions)
- ⭐ **Star** the repo if you find it useful!

# Requirements - C# Console Application

Extract outstanding tasks from Markdown files and export them to CSV format for ManicTime time tracking.

## Overview

A standalone console application that scans a directory structure containing customer project Markdown files and generates a CSV export of all outstanding tasks with their hierarchical context.

## Functional Requirements

### 1. Input Processing

**Directory Structure Scanning**
- Recursively scan a root "Customers" folder
- Process all `.md` (Markdown) files found
- Support nested folder structures (Customer → Project → Subfolders)
- Ignore hidden files and folders (starting with `.`)
- Handle large directory trees efficiently

**File Reading**
- Read Markdown files with UTF-8 encoding
- Handle files of various sizes (up to several MB)
- Skip empty files
- Log files that cannot be read

### 2. Task Detection

**Task Identification**
- Find all unchecked tasks marked with `- [ ]`
- Ignore completed tasks marked with `- [x]`
- Ignore tasks with completion indicators (e.g., `✅ 2025-11-11`)
- Support tasks at any indentation level
- Handle multi-line task descriptions

**Task Patterns to Recognize**
```markdown
- [ ] Task description
  - [ ] Nested sub-task (indented with 2+ spaces)
    - [ ] Deeper nested task
```

**Task Patterns to Ignore**
```markdown
- [x] Completed task
- [x] Completed with date ✅ 2025-11-11
- [-] Cancelled task
- [>] Forwarded task
```

### 3. Hierarchical Structure Parsing

**Header Detection**
- Extract Markdown headers: `##` (Header1), `###` (Header2), `####` (Header3)
- Ignore H1 (`#`) as it's typically the document title
- Track the current header context as file is parsed
- Associate each task with its parent headers

**Nested Task Handling**
- When a task has indented sub-tasks, treat the parent task text as an additional header level
- Example:
  ```markdown
  ## MSAccess
  - [ ] Check creating invoice header
    - [ ] Sub-task 1
    - [ ] Sub-task 2
  ```
  Should produce:
  - Header1: MSAccess
  - Header2: Check creating invoice header
  - Task: Sub-task 1

**Edge Cases**
- Tasks without headers (use empty string)
- Multiple levels of nesting
- Headers that contain no tasks

### 4. Customer and Project Name Extraction

**Folder Structure Interpretation**
```
Customers/
├── CustomerName/              ← Customer name
│   └── ProjectName.md         ← Project name (filename without .md)
└── CustomerName/
    └── SubFolder/
        └── ProjectName.md     ← Customer = CustomerName, Project = filename
```

**Rules**
- **CustomerName**: First folder directly under "Customers/"
- **ProjectName**: Markdown filename without `.md` extension
- Handle folder/filenames with special characters: `()`, `-`, spaces
- Examples:
  - `Customers/Belgian Recycling Network (BRN)/Billit implementation.md`
    - Customer: `Belgian Recycling Network (BRN)`
    - Project: `Billit implementation`
  - `Customers/I.Deeds/Cetec/Billit implementation.md`
    - Customer: `I.Deeds`
    - Project: `Billit implementation`

### 5. CSV Output Generation

**CSV Format**
- Header row: `CustomerName,ProjectName,Header1,Header2,Header3,Task`
- **Dynamic columns**: Only include header columns that contain values
- No empty trailing commas

**Examples**

With 1 header:
```csv
CustomerName,ProjectName,Header1,Task
Customer A,Project 1,Section,Task description
```

With 2 headers:
```csv
CustomerName,ProjectName,Header1,Header2,Task
Customer A,Project 1,Section,Subsection,Task description
```

With 3 headers:
```csv
CustomerName,ProjectName,Header1,Header2,Header3,Task
Customer A,Project 1,Section,Subsection,Deep Section,Task description
```

**CSV Escaping Rules**
- Wrap fields containing commas in double quotes
- Escape double quotes by doubling them: `"` becomes `""`
- Handle newlines within task descriptions
- Example: Task with comma and quotes:
  ```
  BTWvoet hoe bepalen, manueel, standaard 21?
  ```
  Becomes:
  ```csv
  Customer,Project,Header,"BTWvoet hoe bepalen, manueel, standaard 21?"
  ```

**File Output**
- UTF-8 encoding with BOM (for Excel compatibility)
- Configurable output path
- Default filename: `outstanding_tasks.csv`
- Overwrite existing file (with optional confirmation)

## Technical Stack

- **Framework**: .NET 10
- **Language**: C# 13
- **Target**: Console Application

## Non-Functional Requirements

### Performance
- Process 100 files in under 5 seconds
- Support directory trees with 1000+ files
- Memory efficient (don't load all files at once)

### Reliability
- Handle file access errors gracefully
- Validate paths before processing
- Provide meaningful error messages
- Exit with appropriate status codes

### Usability
- Clear command-line interface
- Progress indicators for large operations
- Summary statistics (files processed, tasks found)
- Verbose mode for debugging

### Maintainability
- Clean, well-documented code
- Separation of concerns (parsing, extraction, export)
- Unit testable components
- Follow C# coding conventions

## Command Line Interface

### Required Arguments
- `-i, --input <path>`: Path to Customers folder

### Optional Arguments
- `-o, --output <path>`: Output CSV file path (default: `outstanding_tasks.csv`)
- `-v, --verbose`: Show detailed processing information
- `-h, --help`: Display help information
- `--version`: Show version information

### Examples
```bash
# Basic usage
TaskExport -i "./Customers"

# Specify output
TaskExport -i "C:\Projects\Customers" -o "C:\Reports\tasks.csv"

# Verbose mode
TaskExport -i "./Customers" -o "tasks.csv" -v
```

### Exit Codes
- `0`: Success
- `1`: Invalid arguments
- `2`: Input path not found
- `3`: No tasks found
- `4`: Output write error

## Error Handling

### Input Validation
- Verify Customers folder exists and is accessible
- Validate output path is writable
- Check for valid markdown files

### Processing Errors
- Log files that cannot be read (permissions, corruption)
- Skip invalid markdown (continue processing)
- Handle filesystem errors gracefully

### Output Errors
- Detect write failures (disk full, permissions)
- Validate CSV was written successfully
- Provide rollback on critical errors

## Testing Requirements

### Unit Tests
- Markdown parsing logic
- Task detection patterns
- CSV escaping rules
- Customer/Project name extraction
- Header hierarchy tracking

### Integration Tests
- End-to-end processing with sample data
- Various folder structures
- Edge cases (empty files, no tasks, deep nesting)

### Test Fixtures
Provide sample markdown files covering:
- Simple tasks (no headers)
- Tasks with 1, 2, 3 header levels
- Nested/indented tasks
- Special characters in tasks
- Completed tasks (to be ignored)
- Empty files
- Mixed content

## Future Enhancements (Out of Scope for v1)

- Configuration file support
- Filter by date range
- Custom task patterns
- Multiple output formats (JSON, XML)
- Watch mode (continuous monitoring)
- GUI application
- Task statistics and analytics

## Success Criteria

The application is successful if it:
1. Correctly extracts all unchecked tasks from test data
2. Maintains hierarchical structure in output
3. Produces valid CSV that imports into ManicTime
4. Handles errors without crashing
5. Processes typical workload (50-100 files) in under 3 seconds
6. Has 80%+ code coverage in tests

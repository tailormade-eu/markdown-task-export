# Task Export Tool Requirements

Create a C# console application that extracts outstanding tasks from Markdown files and exports them to a CSV file for ManicTime time tracking.

## Requirements

### 1. Input
Scan a directory structure containing customer folders with markdown (.md) files
- Example structure: `Customers/[CustomerName]/[ProjectName].md` or `Customers/[CustomerName]/[SubFolder]/[ProjectName].md`

### 2. Task Detection
Find all unchecked tasks marked with `- [ ]` (checkbox syntax in markdown)
- Ignore completed tasks marked with `- [x]`
- Ignore tasks with checkmarks like `✅ 2025-11-11`

### 3. Hierarchical Structure Parsing
- Extract markdown headers (##, ###, ####) as Header1, Header2, Header3
- Associate each task with its parent headers in the hierarchy
- Handle nested/indented tasks (sub-bullets under parent tasks)
- When a task has sub-tasks (indented bullets), treat the parent task text as an additional header level

### 4. Customer and Project Name Extraction
- **CustomerName** = the folder name directly under `Customers/` (e.g., "Belgian Recycling Network (BRN)", "I.Deeds", "Cetec")
- **ProjectName** = the markdown filename without extension (e.g., "Billit implementation", "Webshop")
- Handle nested customer folders (e.g., `Customers/I.Deeds/Cetec/` → Customer: "I.Deeds", look in subfolder for projects)

### 5. CSV Output Format
- Header row: `CustomerName,ProjectName,Header1,Header2,Header3,Task`
- **Only include header columns that have values** (no empty trailing commas)
- If Header2 doesn't exist, row should be: `CustomerName,ProjectName,Header1,Task` (skip empty Header2 and Header3 columns)
- Handle commas within task text by properly escaping with quotes
- Handle quotes within task text by doubling them

### 6. Output File
Create `outstanding_tasks.csv` in the root directory

## Example Input (Markdown file)

```markdown
## MSAccess
- [ ] blokkeren van de facturen
- [x] duedate niet altijd ingevuld ✅ 2025-11-11

### Check creating invoice header
- [ ] BTWvoet hoe bepalen, manueel, standaard 21?
- [ ] Factuurnummer hoe bepalen?

## Questions
- [ ] check with accountant
  - [ ] Check shipping + product ledger?
```

## Example Output (CSV)

```csv
CustomerName,ProjectName,Header1,Header2,Task
Bellucci,Billit implementation,MSAccess,blokkeren van de facturen
Bellucci,Billit implementation,MSAccess,Check creating invoice header,"BTWvoet hoe bepalen, manueel, standaard 21?"
Bellucci,Billit implementation,MSAccess,Check creating invoice header,Factuurnummer hoe bepalen?
Bellucci,Billit implementation,Questions,check with accountant,Check shipping + product ledger?
```

## Technical Details

- Use C# with .NET 10
- Use proper CSV escaping (quotes around fields containing commas, newlines, or quotes)
- Handle file encoding (UTF-8 with BOM for Excel compatibility)
- Recursively scan all subdirectories under `Customers/`
- Ignore empty markdown files
- Provide console output showing progress (files processed, tasks found)

## Edge Cases to Handle

- Tasks with special characters (commas, quotes, newlines)
- Multiple levels of indentation in task lists
- Empty header sections
- Files with no outstanding tasks
- Folder names with special characters in parentheses like "(BRN)"

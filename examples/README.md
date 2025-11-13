# Examples

This directory contains sample markdown files to demonstrate the Markdown Task Export tool.

## Directory Structure

```
examples/
├── Customers/
│   ├── Acme/
│   │   └── Website Redesign.md
│   └── GlobalTech/
│       └── CRM Integration.md
├── output.csv
└── README.md
```

## Running the Example

From the project root directory, run:

```bash
dotnet run -- -i examples/Customers -o examples/output.csv
```

Or with verbose output:

```bash
dotnet run -- -i examples/Customers -o examples/output.csv -v
```

## Expected Output

The tool will extract **15 outstanding tasks** from the markdown files and generate a CSV file (`output.csv`) that can be imported into ManicTime or other time tracking applications.

### Sample CSV Output

```csv
CustomerName,ProjectName,Header1,Header2,Task
Acme,Website Redesign,Planning Phase,,Create wireframes
Acme,Website Redesign,Planning Phase,,Get stakeholder approval
Acme,Website Redesign,Development,Implement homepage,Add header component
...
GlobalTech,CRM Integration,Analysis,,Review current CRM system
...
```

## Features Demonstrated

1. **Multiple customers**: Acme and GlobalTech
2. **Hierarchical headers**: ##, ### levels preserved
3. **Nested tasks**: Parent tasks become headers (e.g., "Implement homepage")
4. **Completed tasks ignored**: Tasks marked with [x] or ✅ are excluded
5. **Dynamic columns**: CSV includes only the header levels that have values

## Creating Your Own Structure

To use this tool with your own data:

1. Create a `Customers` directory
2. Inside, create subdirectories for each customer
3. Add markdown files for each project
4. Use standard markdown task syntax: `- [ ] Task description`
5. Organize tasks under headers: `## Section`, `### Subsection`
6. Run the tool pointing to your `Customers` directory

## Markdown Format Guidelines

### Task Syntax
- Outstanding: `- [ ] Task description`
- Completed: `- [x] Task description` (will be ignored)
- With date: `- [ ] Task ✅ 2025-11-11` (will be ignored)

### Headers
- Level 1 (##): Main section
- Level 2 (###): Subsection
- Level 3 (####): Deep section

### Nested Tasks
```markdown
## Main Section
- [ ] Parent task with sub-tasks:
  - [ ] Sub-task 1
  - [ ] Sub-task 2
```

The parent task text ("Parent task with sub-tasks") becomes a header level for the sub-tasks.

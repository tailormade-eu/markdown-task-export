# New Features Documentation

## Dynamic Header Depth

The tool now supports unlimited header depth instead of being limited to 3 levels.

### Example Input (Markdown):
```markdown
## Level 1
- [ ] Task at level 1

### Level 2
- [ ] Task at level 2

#### Level 3
- [ ] Task at level 3

##### Level 4
- [ ] Task at level 4

###### Level 5
- [ ] Task at level 5
```

### Output CSV (Default Mode):
```csv
CustomerName,ProjectName,Level1,Level2,Level3,Level4,Level5,Task
Customer,Project,,Level 1,,,,Task at level 1
Customer,Project,,Level 1,Level 2,,,Task at level 2
Customer,Project,,Level 1,Level 2,Level 3,,Task at level 3
Customer,Project,,Level 1,Level 2,Level 3,Level 4,Task at level 4
Customer,Project,,Level 1,Level 2,Level 3,Level 4,Level 5,Task at level 5
```

Notice: Empty columns preserve the hierarchical structure.

## Compress Levels Mode

Use `--compress-levels` to remove empty hierarchy columns.

### Output CSV (Compressed Mode):
```csv
CustomerName,ProjectName,Level1,Level2,Level3,Level4,Level5,Task
Customer,Project,Level 1,,,,Task at level 1
Customer,Project,Level 1,Level 2,,,Task at level 2
Customer,Project,Level 1,Level 2,Level 3,,Task at level 3
Customer,Project,Level 1,Level 2,Level 3,Level 4,Task at level 4
Customer,Project,Level 1,Level 2,Level 3,Level 4,Level 5,Task at level 5
```

Notice: Headers shift left, removing empty slots.

## No Header Mode

Use `--no-header` to exclude the CSV header row.

### Output CSV (No Header):
```csv
Customer,Project,,Level 1,,,,Task at level 1
Customer,Project,,Level 1,Level 2,,,Task at level 2
```

## Usage Examples

```bash
# Default mode - preserves hierarchy with empty columns
dotnet run -- -i ./Customers -o tasks.csv

# Compressed mode - removes empty columns
dotnet run -- -i ./Customers -o tasks.csv --compress-levels

# No header - excludes CSV header row
dotnet run -- -i ./Customers -o tasks.csv --no-header

# Combined - compressed and no header
dotnet run -- -i ./Customers -o tasks.csv --compress-levels --no-header

# With verbose output
dotnet run -- -i ./Customers -o tasks.csv -v --compress-levels
```

## Use Cases

### Default Mode (Non-Compressed)
- When you need to preserve the exact markdown header structure
- For importing into systems that expect fixed column positions
- When working with templates that have predefined column layouts

### Compressed Mode
- When you want a more compact CSV without empty columns
- For better readability when header depths vary significantly
- When importing into systems that can handle variable column layouts
- To reduce file size

### No Header Mode
- When appending to existing CSV files
- For systems that don't expect/handle header rows
- When using CSV import tools that auto-detect columns

# C# Sorter

A simple CLI tool for sorting C# source code files, inspired by nArrange.

## 📖 Overview

The tool analyzes C# files using Roslyn and sorts:

* `using` directives alphabetically
* Type declarations (classes, enums, interfaces) in their original order
* Members within classes (fields, constructors, properties, methods, events)

The sorting follows a straightforward, configurable logic based on nArrange.

## 🚀 Prerequisites

* .NET 8.0 SDK
* NuGet packages:

  * `Microsoft.CodeAnalysis.CSharp` (>= 4.8.0)
  * `System.CommandLine` (2.0.0) _(prerelease)_

Install with:

```bash
dotnet add package Microsoft.CodeAnalysis.CSharp --version 4.8.0
dotnet add package System.CommandLine --version 2.0.0
```

## ⚙️ Usage

Build and run the tool with `dotnet run`. Arguments must follow `--`:

```bash
dotnet run -- --input <path> [--recursive] [--output <file>]
```

* `--input`, `-i`
  Path to a `.cs` file or directory.
* `--recursive`, `-r`
  Recursively search directories for `.cs` files.
* `--output`, `-o`
  Path to the output file. If omitted, the source file is overwritten.

### Examples

Sort a single file and overwrite it:

```bash
dotnet run -- -i Calculator.cs_
```

Sort all `.cs` files in the `src` folder recursively and save to `Sorted.cs`:

```bash
dotnet run -- -i src -r -o Sorted.cs
```

Sort `Calculator.cs_` and write to `Out.cs_`:

```bash
dotnet run -- -i Calculator.cs_ -o Out.cs_
```

## 🔧 Testing

1. Place `Calculator.cs_` (unsorted example) in the project directory.
2. Place `Out.cs_` (expected sorted result) alongside it.
3. Run:

```bash
dotnet run -- -i Calculator.cs_ -o Actual.cs_
```
4. Compare `Calculator.cs_` with `Out.cs_`.
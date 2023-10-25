# GetProcNames

`GetProcNames` is a .NET console application that scans through C# source files to extract SQL stored procedure names and associated metadata, then exports the extracted data into an Excel file. 

The tool specifically searches for stored procedure names prefixed with "{SchemaPrefixHere.}" within string literals, extracts the corresponding class and method names, and writes the results to an Excel file.

## 🌟 Features

- 🔍 Scans C# files for specific string literals containing stored procedure names.
- 📝 Extracts associated class and method names.
- 📊 Exports the results to an Excel file using ClosedXML library.

## 📋 Prerequisites

- .NET Core or .NET Framework
- ClosedXML NuGet package (for Excel export functionality)

## 💽 Installation

1. Clone the repository:
```bash
git clone https://github.com/trueblood/GetProcNames.git

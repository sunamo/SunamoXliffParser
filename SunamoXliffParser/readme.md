### SunamoXliffParser

A .NET library for parsing, modifying, and exporting XLIFF (XML Localization Interchange File Format) documents. Fork of [fmdev.XliffParser](https://www.nuget.org/packages/fmdev.XliffParser) updated for .NET 8+.

## Features

- Read and write XLIFF 1.1, 1.2, and 2.0 documents
- Support for Standard, RCWinTrans11, and Multilingual App Toolkit dialects
- Convert between XLIFF and ResX resource file formats
- Add, update, and remove translation units programmatically
- Update XLIFF documents from ResX source files

## Installation

```bash
dotnet add package SunamoXliffParser
```

## Key Classes

- **XlfDocument** - Main entry point for loading and manipulating XLIFF files
- **XlfFile** - Represents a file element within an XLIFF document
- **XlfTransUnit** - Represents a single translation unit
- **ResXFile** - Utility for reading and writing ResX resource files

## Usage

```csharp
// Load an XLIFF file
var doc = new XlfDocument("translations.xlf");

// Access translation units
foreach (var file in doc.Files)
{
    foreach (var unit in file.TransUnits)
    {
        Console.WriteLine($"{unit.Id}: {unit.Source} -> {unit.Target}");
    }
}

// Save as ResX
doc.SaveAsResX("output.resx");

// Update from source ResX file
var result = doc.UpdateFromSource();
```

## Target Frameworks

`net10.0;net9.0;net8.0`

## Links

- [NuGet](https://www.nuget.org/profiles/sunamo)
- [GitHub](https://github.com/sunamo/PlatformIndependentNuGetPackages)
- [Developer site](https://sunamo.cz)

Request for new features / bug report: [Mail](mailto:radek.jancik@sunamo.cz) or on GitHub

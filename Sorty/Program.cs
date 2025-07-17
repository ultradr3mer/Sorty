using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.CommandLine;

var inputOption = new Option<string>("--input", "-i") { Description = "Pfad zur eingabe Datei, oder Ordner" };
var recursiveOption = new Option<bool>("--recursive", "-r") {  Description = "Dateien rekursiv durchsuchen" };
var outputOption = new Option<string?>("--output", "-o") {  Description = "Ausgabedatei (optional)" };

var rootCommand = new RootCommand("C# Sortierer")
{
    inputOption,
    recursiveOption,
    outputOption
};

rootCommand.SetAction(parseResult =>
{
  if (parseResult.GetValue(inputOption) is string path)
  {
    var output = parseResult.GetValue(outputOption);
    if (File.Exists(path))
    {
      ProcessFile(path, output);
    }
    else if (Directory.Exists(path))
    {
      var recussive = parseResult.GetValue(recursiveOption);
      var files = Directory.GetFiles(path, "*.cs", recussive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
      foreach (var file in files)
      {
        ProcessFile(file, null); // Output wird hier ignoriert, Überschreiben ist Standard
      }
    }
    else
    {
      Console.Error.WriteLine($"Pfad '{path}' existiert nicht.");
    }
  }
});

if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
{
    // Show help menu
    rootCommand.Parse(new[] { "--help" }).Invoke();
    return;
}

rootCommand.Parse(args).Invoke();

void ProcessFile(string filePath, string? outputPath)
{
  var code = File.ReadAllText(filePath);
  var tree = CSharpSyntaxTree.ParseText(code);
  var root = tree.GetCompilationUnitRoot();

  var sorted = SortCompilationUnit(root);
  var formatted = sorted.NormalizeWhitespace().ToFullString();

  var outPath = outputPath ?? filePath;
  File.WriteAllText(outPath, formatted);

  Console.WriteLine($"✔ Sortiert: {filePath}");
}

CompilationUnitSyntax SortCompilationUnit(CompilationUnitSyntax root)
{
  // Usings sortieren
  var usings = SyntaxFactory.List(root.Usings.OrderBy(u => u.ToString()));

  // Typdeklarationen sortieren (z.B. Klassen, Enums, Interfaces)
  var members = SyntaxFactory.List(root.Members.Select(Walk));

  return root.WithUsings(usings).WithMembers(members);
}

MemberDeclarationSyntax Walk(MemberDeclarationSyntax member)
{
  if (member is NamespaceDeclarationSyntax ns)
  {
    var newMembers = SyntaxFactory.List(ns.Members.Select(Walk));
    return ns.WithMembers(newMembers);
  }
 
  if (member is ClassDeclarationSyntax cls)
  {
    var sortedMembers = SyntaxFactory.List(cls.Members.OrderBy(GetMemberOrderKey));
    return cls.WithMembers(sortedMembers);
  }
  return member;
}

int GetMemberOrderKey(MemberDeclarationSyntax member)
{
  return member switch
  {
    FieldDeclarationSyntax => 0,
    ConstructorDeclarationSyntax => 1,
    PropertyDeclarationSyntax => 2,
    MethodDeclarationSyntax => 3,
    EventFieldDeclarationSyntax => 4,
    _ => 10
  };
}

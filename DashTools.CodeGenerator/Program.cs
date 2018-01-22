using Microsoft.Build.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DashTools.CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine();
            Console.WriteLine("Press <Enter> to exit");
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            string filePath = await GenerateCodeForMpdSchema();
            string refactoredFilePath = await RefactorMpdCodeAsync(filePath);

            return;



            //var workspace = MSBuildWorkspace.Create();
            //var solution = await workspace.OpenSolutionAsync("../../../SharpDashTools.sln");
            //var project = solution.Projects.Single(p => p.AssemblyName == typeof(Qoollo.MpegDash.MpdDownloader).Assembly.GetName().Name);
            //var document = project.Documents.Single(d => d.Name == "MpdSchema.cs");
            //var syntaxTree = await document.GetSyntaxRootAsync();

            //var walker = new ClassWalker();
            //walker.Visit(syntaxTree);

            //var rewriter = new MpdCodeRefactorer();
            //var changed = rewriter.Visit(syntaxTree);
            //File.WriteAllText(document.FilePath + "test.cs", changed.ToFullString());

            //var generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);
        }

        private static async Task<string> GenerateCodeForMpdSchema()
        {
            var schemaFiles = await DownloadMpdSchemaAsync();

            string result = GenerateCodeForMpdSchema(schemaFiles);

            foreach (var file in schemaFiles)
                File.Delete(file);

            return result;
        }

        private static async Task<string[]> DownloadMpdSchemaAsync()
        {
            const string xsdUrl = "https://github.com/Dash-Industry-Forum/Conformance-and-reference-source/raw/master/conformance/MPDValidator/schemas/DASH-MPD.xsd";
            const string xsdFile = "DASH-MPD.xsd";

            const string xlinkUrl = "https://github.com/Dash-Industry-Forum/Conformance-and-reference-source/raw/master/conformance/MPDValidator/schemas/xlink.xsd";
            const string xlinkFile = "xlink.xsd";

            var webClient = new WebClient();
            await webClient.DownloadFileTaskAsync(xsdUrl, xsdFile);
            await webClient.DownloadFileTaskAsync(xlinkUrl, xlinkFile);

            return new[] { xsdFile, xlinkFile };
        }

        private static string GenerateCodeForMpdSchema(IEnumerable<string> xsdFiles)
        {
            const string xsdExePath = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\xsd.exe";
            if (!File.Exists(xsdExePath))
                throw new InvalidOperationException("xsd.exe not found at " + xsdExePath);

            const string newFileName = "MpdSchema.cs";
            if (File.Exists(newFileName))
                File.Delete(newFileName);

            var process = Process.Start(xsdExePath, string.Join(" ", xsdFiles) + " /c /n:Qoollo.MpegDash");
            if (!process.WaitForExit(20000))
                throw new TimeoutException("XSD -> C# code generation takes too long. Something's wrong.");

            string fileName = string.Join("_", xsdFiles.Select(e => Path.GetFileNameWithoutExtension(e))) + ".cs";
            if (!File.Exists(fileName))
                throw new Exception("XSD -> C# code generation succeeded, but file " + fileName + " was not found");

            File.Move(fileName, newFileName);

            return newFileName;
        }

        private static async Task<string> RefactorMpdCodeAsync(string filePath)
        {
            var workspace = new AdhocWorkspace();
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, versionStamp, "test", "test", LanguageNames.CSharp);
            projectInfo = projectInfo.WithMetadataReferences(
                new [] 
                {
                    typeof(int),
                    typeof(object),
                    typeof(GeneratedCodeAttribute),
                    typeof(XmlTypeAttribute)
                }
                .Select(e => e.Assembly.Location)
                .Select(e => MetadataReference.CreateFromFile(e))
            );
            var project = workspace.AddProject(projectInfo);

            var sourceText = SourceText.From(File.ReadAllText(filePath));
            var document = workspace.AddDocument(project.Id, "NewFile.cs", sourceText);
            var syntaxRoot = await document.GetSyntaxRootAsync();
            

            var semanticModel = await document.GetSemanticModelAsync();
            var diagnostics = semanticModel.GetDiagnostics();

            var rewriters = new CSharpSyntaxRewriter[]
            {
                new MoveNamespacesToUsingsRewriter(),
                new RemoveAttributeSuffixRewriter()
            };
            foreach (var rewriter in rewriters)
            {
                syntaxRoot = rewriter.Visit(syntaxRoot);

                workspace.TryApplyChanges(workspace.CurrentSolution.WithDocumentSyntaxRoot(document.Id, syntaxRoot));
                document = workspace.CurrentSolution.GetDocument(document.Id);
                syntaxRoot = await document.GetSyntaxRootAsync();
                semanticModel = await document.GetSemanticModelAsync();
            }

            var refactorer = new MpdCodeRefactorer(semanticModel);
            syntaxRoot = await document.GetSyntaxRootAsync();
            syntaxRoot = refactorer.Visit(syntaxRoot);
            while (refactorer.Refactoring != null)
            {
                diagnostics = semanticModel.GetDiagnostics();

                var solution = await Renamer.RenameSymbolAsync(workspace.CurrentSolution, refactorer.Refactoring.Symbol, refactorer.Refactoring.NewName, workspace.Options);
                if (!workspace.TryApplyChanges(solution))
                {
                    throw new InvalidOperationException("Failed to apply changes after rename");
                }
                Console.WriteLine("Applied refactoring:\n\t{0}", refactorer.Refactoring);
                
                document = workspace.CurrentSolution.GetDocument(document.Id);
                syntaxRoot = await document.GetSyntaxRootAsync();
                semanticModel = await document.GetSemanticModelAsync();
                refactorer = new MpdCodeRefactorer(semanticModel);
                refactorer.Visit(syntaxRoot);
            }

            const string newFileName = "NewFileRefactored.cs";
            File.WriteAllText(newFileName, syntaxRoot.ToFullString());

            return newFileName;
        }

    }
}

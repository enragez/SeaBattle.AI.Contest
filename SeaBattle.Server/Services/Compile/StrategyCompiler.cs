namespace SeaBattle.Server.Services.Compile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using Engine;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Models;

    public class StrategyCompiler : IStrategyCompiler
    {
        public async Task<byte[]> Compile(Stream zipStream)
        {
            var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            var csEntries =
                zipArchive.Entries.Where(entry => entry.FullName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase));

            if (!csEntries.Any())
            {
                throw new StrategyCompilationException("В zip-архиве отсутствуют cs-файлы");
            }

            var streams = csEntries.Select(entry => entry.Open());

            var readers = streams.Select(stream => new StreamReader(stream));

            var syntaxTrees = new List<SyntaxTree>();

            try
            {
                foreach (var reader in readers)
                {
                    var text = await reader.ReadToEndAsync();

                    if (text.Contains("System.Reflection"))
                    {
                        throw new StrategyCompilationException("В стратегии присутствует рефлексия");
                    }

                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(text));
                }
            }
            finally
            {
                foreach (var reader in readers)
                {
                    reader.Dispose();
                }
            }

            var compilation = CSharpCompilation.Create("strategy")
                                            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                                            .AddReferences(GetReferences())
                                            .AddSyntaxTrees(syntaxTrees);

            var strategyAssemblyStream = new MemoryStream();

            var result = compilation.Emit(strategyAssemblyStream);
            if (result.Success)
            {
                return strategyAssemblyStream.ToArray();
            }

            var compilationErrors = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)
                                       .Select(d => d.ToString());
                    
            throw new StrategyCompilationException(string.Join(Environment.NewLine,
                                                               compilationErrors));

        }

        private IEnumerable<MetadataReference> GetReferences()
        {
            yield return MetadataReference.CreateFromFile(typeof(Engine).Assembly.Location);
            
            // System.Private.CorLib.dll
            yield return MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            
            //The location of the .NET Framework assemblies
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            
            yield return MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll"));
            yield return MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll"));
            yield return MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll"));
        }
    }
}
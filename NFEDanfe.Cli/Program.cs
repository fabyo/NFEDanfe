using NFEDanfe.Domain.Models;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

namespace NFEDanfe.Cli;

internal static class Program
{
    private const string LogoFileName = "logo.png";
    private const string XmlTestDirectoryName = "xml_testes";

    private static int Main(string[] args)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // Registrar fontes customizadas se a pasta existir
        string fontsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fonts");
        if (!Directory.Exists(fontsPath))
        {
            fontsPath = Path.Combine(Directory.GetCurrentDirectory(), "fonts");
        }

        if (Directory.Exists(fontsPath))
        {
            foreach (string file in Directory.GetFiles(fontsPath, "*.ttf"))
            {
                try
                {
                    using var stream = File.OpenRead(file);
                    FontManager.RegisterFont(stream);
                    Console.WriteLine($"[FONTE] Registrada com sucesso: {Path.GetFileName(file)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AVISO] Falha ao registrar fonte {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }

        Console.WriteLine("NFEDanfe - Gerador de DANFE em PDF");
        Console.WriteLine("-----------------------------------");

        CliOptions options = CliOptions.Parse(args);
        Directory.CreateDirectory(options.OutputDirectory);

        IReadOnlyList<string> xmlPaths = options.XmlPaths.Count > 0
            ? options.XmlPaths
            : LocalizarXmls();

        int generated = 0;

        foreach (string xmlPath in xmlPaths)
        {
            generated += GerarDanfeDoXml(xmlPath, options);
        }

        if (options.GenerateMock || generated == 0)
        {
            generated += GerarDanfeMock(options);
        }

        Console.WriteLine();
        Console.WriteLine($"PDFs gerados: {generated}");
        Console.WriteLine($"Diretório de saída: {Path.GetFullPath(options.OutputDirectory)}");

        return generated > 0 ? 0 : 1;
    }

    private static int GerarDanfeDoXml(string xmlPath, CliOptions options)
    {
        try
        {
            string resolvedXmlPath = ResolveInputPath(xmlPath);
            int count = 0;

            // Gerar PDF padrão (sem sufixo)
            {
                string outputName = $"danfe_{Path.GetFileNameWithoutExtension(resolvedXmlPath)}{(options.Landscape ? "_paisagem" : string.Empty)}{(options.HasLogo ? "_com_logo" : string.Empty)}.pdf";
                string outputPath = Path.Combine(options.OutputDirectory, outputName);

                using (FileStream output = File.Create(outputPath))
                {
                    DanfeGenerator.GenerateFromXml(resolvedXmlPath, output, CreateDanfeOptions(options));
                }

                if (options.GenerateSnapshot)
                {
                    WriteSnapshot(resolvedXmlPath, outputPath, options);
                }

                int pageCount = GetPdfPageCount(outputPath);
                Console.WriteLine($"[OK] {Path.GetFileName(resolvedXmlPath)} -> {outputPath} ({pageCount} página(s))");
                count++;
            }

            // Gerar PDFs de demonstração para cada fonte disponível
            var fontesExibicao = new[]
            {
                (Font: DanfeFont.Arial, Suffix: "_arial"),
                (Font: DanfeFont.Inter, Suffix: "_inter"),
                (Font: DanfeFont.Roboto, Suffix: "_roboto"),
                (Font: DanfeFont.IbmPlexSans, Suffix: "_ibm_plex_sans")
            };

            foreach (var (font, suffix) in fontesExibicao)
            {
                string outputName = $"danfe_{Path.GetFileNameWithoutExtension(resolvedXmlPath)}{(options.Landscape ? "_paisagem" : string.Empty)}{(options.HasLogo ? "_com_logo" : string.Empty)}{suffix}.pdf";
                string outputPath = Path.Combine(options.OutputDirectory, outputName);

                DanfeOptions danfeOptions = CreateDanfeOptions(options) with { Font = font };

                using (FileStream output = File.Create(outputPath))
                {
                    DanfeGenerator.GenerateFromXml(resolvedXmlPath, output, danfeOptions);
                }

                int pageCount = GetPdfPageCount(outputPath);
                Console.WriteLine($"[OK] {Path.GetFileName(resolvedXmlPath)} ({font}) -> {outputPath} ({pageCount} página(s))");
                count++;
            }

            return count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] {xmlPath}: {ex.Message}");
            return 0;
        }
    }

    private static int GerarDanfeMock(CliOptions options)
    {
        try
        {
            byte[]? logoBytes = ObterLogoBytes(options);
            DanfeModel model = MockDanfeFactory.Create(logoBytes);
            string outputPath = Path.Combine(options.OutputDirectory, $"danfe_mock{(options.Landscape ? "_paisagem" : string.Empty)}{(logoBytes != null ? "_com_logo" : string.Empty)}.pdf");

            DanfeGenerator.Generate(model, outputPath, CreateDanfeOptions(options) with { LogoBytes = null });

            if (options.GenerateSnapshot)
            {
                File.WriteAllText(Path.ChangeExtension(outputPath, ".snapshot.txt"), DanfeSnapshot.CreateText(model));
            }

            Console.WriteLine($"[OK] mock -> {outputPath}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] mock: {ex.Message}");
            return 0;
        }
    }

    private static void WriteSnapshot(string xmlPath, string outputPath, CliOptions options)
    {
        DanfeModel model = DanfeGenerator.LoadFromXml(xmlPath, CreateDanfeOptions(options));
        File.WriteAllText(Path.ChangeExtension(outputPath, ".snapshot.txt"), DanfeSnapshot.CreateText(model));
    }

    private static DanfeOptions CreateDanfeOptions(CliOptions options)
    {
        return new DanfeOptions
        {
            LogoBytes = ObterLogoBytes(options),
            ValidateBeforeGenerate = true,
            EmitFooter = true,
            TipoImpressaoOverride = options.Landscape ? 2 : null,
            CanceledOverride = options.Cancelado ? true : null
        };
    }

    private static IReadOnlyList<string> LocalizarXmls()
    {
        return CandidateDirectories()
            .Where(Directory.Exists)
            .SelectMany(dir => Directory.EnumerateFiles(dir, "*.xml", SearchOption.TopDirectoryOnly))
            .Where(path => Path.GetFileName(path).Contains("-proc", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path)
            .ToList();
    }

    private static string ResolveInputPath(string xmlPath)
    {
        if (Path.IsPathFullyQualified(xmlPath) && File.Exists(xmlPath))
        {
            return xmlPath;
        }

        foreach (string dir in CandidateDirectories())
        {
            string candidate = Path.Combine(dir, xmlPath);
            if (File.Exists(candidate))
            {
                return Path.GetFullPath(candidate);
            }
        }

        return xmlPath;
    }

    private static byte[]? ObterLogoBytes(CliOptions options)
    {
        if (!options.HasLogo)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(options.LogoPath))
        {
            string logoPath = ResolveInputPath(options.LogoPath);
            if (!File.Exists(logoPath))
            {
                throw new FileNotFoundException($"Logo não encontrado: {options.LogoPath}", options.LogoPath);
            }

            return File.ReadAllBytes(logoPath);
        }

        foreach (string dir in CandidateDirectories())
        {
            string logoPath = Path.Combine(dir, LogoFileName);
            if (File.Exists(logoPath))
            {
                return File.ReadAllBytes(logoPath);
            }
        }

        return null;
    }

    private static IEnumerable<string> CandidateDirectories()
    {
        HashSet<string> emitted = new(StringComparer.OrdinalIgnoreCase);

        foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            DirectoryInfo? dir = new(start);
            for (int i = 0; i < 8 && dir != null; i++)
            {
                if (emitted.Add(dir.FullName))
                {
                    yield return dir.FullName;
                }

                string xmlTestDir = Path.Combine(dir.FullName, XmlTestDirectoryName);
                if (Directory.Exists(xmlTestDir) && emitted.Add(xmlTestDir))
                {
                    yield return xmlTestDir;
                }

                string samplesDir = Path.Combine(dir.FullName, "samples");
                if (Directory.Exists(samplesDir) && emitted.Add(samplesDir))
                {
                    yield return samplesDir;
                }

                dir = dir.Parent;
            }
        }
    }

    private static int GetPdfPageCount(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        string text = System.Text.Encoding.Latin1.GetString(bytes);
        return System.Text.RegularExpressions.Regex.Matches(text, @"/Type\s*/Page\b").Count;
    }


    private sealed record CliOptions(
        IReadOnlyList<string> XmlPaths,
        string OutputDirectory,
        bool IncludeLogo,
        string? LogoPath,
        bool GenerateMock,
        bool GenerateSnapshot,
        bool Landscape,
        bool Cancelado)
    {
        public bool HasLogo => IncludeLogo || !string.IsNullOrWhiteSpace(LogoPath);

        public static CliOptions Parse(IReadOnlyList<string> args)
        {
            List<string> xmlPaths = [];
            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "out");
            bool includeLogo = false;
            string? logoPath = null;
            bool generateMock = false;
            bool generateSnapshot = false;
            bool landscape = false;
            bool cancelado = false;

            for (int i = 0; i < args.Count; i++)
            {
                string arg = args[i];

                if (arg is "--logo" or "-l")
                {
                    includeLogo = true;
                }
                else if (arg is "--landscape" or "-p" or "--paisagem")
                {
                    landscape = true;
                }
                else if (arg is "--cancelado" or "-c")
                {
                    cancelado = true;
                }
                else if (arg is "--logo-path")
                {
                    if (i + 1 >= args.Count)
                    {
                        throw new ArgumentException("Informe o caminho do logo após --logo-path.");
                    }

                    logoPath = args[++i];
                }
                else if (arg is "--mock" or "-m")
                {
                    generateMock = true;
                }
                else if (arg is "--snapshot")
                {
                    generateSnapshot = true;
                }
                else if (arg is "--output" or "-o")
                {
                    if (i + 1 >= args.Count)
                    {
                        throw new ArgumentException("Informe o diretório após --output.");
                    }

                    outputDirectory = args[++i];
                }
                else if (arg.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    xmlPaths.Add(arg);
                }
            }

            return new CliOptions(xmlPaths, outputDirectory, includeLogo, logoPath, generateMock, generateSnapshot, landscape, cancelado);
        }
    }
}

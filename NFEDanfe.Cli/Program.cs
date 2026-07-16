using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NFEDanfe;
using NFEDanfe.Options;

namespace NFEDanfe.Cli;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("NFEDanfe CLI - Gerador de DANFE");
        Console.WriteLine("--------------------------------");

        if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
        {
            ShowHelp();
            return 0;
        }

        try
        {
            var options = CliOptions.Parse(args);

            // Configure fonts
            var defaultFontConfig = new DanfeFontConfig();
            string regFont = options.FontRegPath ?? defaultFontConfig.BaseFontPath;
            string boldFont = options.FontBoldPath ?? defaultFontConfig.BaseFontBoldPath;

            if (!File.Exists(regFont))
            {
                Console.WriteLine($"[ERRO] Fonte regular não encontrada em '{regFont}'.");
                Console.WriteLine("Por favor, especifique o caminho usando --font-reg <caminho>.");
                return 1;
            }

            if (!File.Exists(boldFont))
            {
                Console.WriteLine($"[ERRO] Fonte negrito não encontrada em '{boldFont}'.");
                Console.WriteLine("Por favor, especifique o caminho usando --font-bold <caminho>.");
                return 1;
            }

            // Prepare DanfeOptions
            var danfeOpts = new DanfeOptions
            {
                LogoPath = options.LogoPath,
                TipoImpressaoOverride = options.Landscape ? 2 : null,
                CanceledOverride = options.Cancelado ? true : null,
                EmitFooter = true,
                FontConfig = new DanfeFontConfig
                {
                    BaseFontPath = regFont,
                    BaseFontBoldPath = boldFont
                }
            };

            Directory.CreateDirectory(options.OutputDirectory);

            int generated = 0;
            foreach (string xmlPath in options.XmlPaths)
            {
                if (!File.Exists(xmlPath))
                {
                    Console.WriteLine($"[AVISO] Arquivo XML não encontrado: {xmlPath}");
                    continue;
                }

                string outputName = $"danfe_{Path.GetFileNameWithoutExtension(xmlPath)}{(options.Landscape ? "_paisagem" : "")}.pdf";
                string outputPath = Path.Combine(options.OutputDirectory, outputName);

                Console.WriteLine($"[PROCESSANDO] {Path.GetFileName(xmlPath)}...");

                using (var outputStream = File.Create(outputPath))
                {
                    DanfeGenerator.GenerateFromXml(xmlPath, outputStream, danfeOpts);
                }

                Console.WriteLine($"[OK] PDF gerado: {outputPath}");
                generated++;
            }

            Console.WriteLine();
            Console.WriteLine($"Concluído! PDFs gerados: {generated}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Ocorreu uma falha: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return 1;
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Uso:");
        Console.WriteLine("  nfedanfe [arquivos.xml...] [opções]");
        Console.WriteLine();
        Console.WriteLine("Opções:");
        Console.WriteLine("  --logo-path <caminho>   Caminho do logotipo (usa o padrão se for inválido).");
        Console.WriteLine("  --landscape, -p         Gera o PDF no modo Paisagem.");
        Console.WriteLine("  --cancelado, -c         Gera com marca d'água de cancelado.");
        Console.WriteLine("  --output, -o <pasta>    Diretório de saída para os PDFs gerados (padrão: ./out).");
        Console.WriteLine("  --font-reg <caminho>    Caminho para o arquivo .ttf de fonte regular.");
        Console.WriteLine("  --font-bold <caminho>   Caminho para o arquivo .ttf de fonte negrito.");
        Console.WriteLine();
        Console.WriteLine("Exemplo:");
        Console.WriteLine("  nfedanfe nfe.xml --logo-path logo.png --output ./pdfs");
    }

    private sealed class CliOptions
    {
        public List<string> XmlPaths { get; } = new();
        public string OutputDirectory { get; set; } = "./out";
        public string? LogoPath { get; set; }
        public bool Landscape { get; set; }
        public bool Cancelado { get; set; }
        public string? FontRegPath { get; set; }
        public string? FontBoldPath { get; set; }

        public static CliOptions Parse(string[] args)
        {
            var options = new CliOptions();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg == "--logo-path")
                {
                    if (i + 1 >= args.Length) throw new ArgumentException("Caminho do logotipo ausente após --logo-path.");
                    options.LogoPath = args[++i];
                }
                else if (arg is "--landscape" or "-p")
                {
                    options.Landscape = true;
                }
                else if (arg is "--cancelado" or "-c")
                {
                    options.Cancelado = true;
                }
                else if (arg is "--output" or "-o")
                {
                    if (i + 1 >= args.Length) throw new ArgumentException("Diretório de saída ausente após --output.");
                    options.OutputDirectory = args[++i];
                }
                else if (arg == "--font-reg")
                {
                    if (i + 1 >= args.Length) throw new ArgumentException("Caminho da fonte regular ausente após --font-reg.");
                    options.FontRegPath = args[++i];
                }
                else if (arg == "--font-bold")
                {
                    if (i + 1 >= args.Length) throw new ArgumentException("Caminho da fonte negrito ausente após --font-bold.");
                    options.FontBoldPath = args[++i];
                }
                else if (arg.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    options.XmlPaths.Add(arg);
                }
            }

            if (options.XmlPaths.Count == 0)
            {
                // Look for XML files in current folder
                var localXmls = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");
                options.XmlPaths.AddRange(localXmls);
            }

            return options;
        }
    }
}

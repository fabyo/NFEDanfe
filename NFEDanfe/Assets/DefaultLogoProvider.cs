using System;
using System.IO;
using System.Threading;

namespace NFEDanfe.Assets;

internal static class DefaultLogoProvider
{
    private const string ResourceName = "NFEDanfe.logo.png";
    private static readonly Lazy<byte[]> LogoBytes = new(LoadLogo, LazyThreadSafetyMode.ExecutionAndPublication);

    internal static byte[] GetBytes() => LogoBytes.Value;

    private static byte[] LoadLogo()
    {
        using Stream stream = typeof(DefaultLogoProvider).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"O recurso de logotipo padrão '{ResourceName}' não foi encontrado.");
        using var output = new MemoryStream();
        stream.CopyTo(output);
        return output.ToArray();
    }
}

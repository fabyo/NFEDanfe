using NFEDanfe;
using NFEDanfe.Options;
using Xunit;

namespace NFEDanfe.Tests;

public sealed class DanfeFontConfigTests
{
    [Fact]
    public void Default_config_uses_distinct_regular_and_bold_files()
    {
        var config = new DanfeFontConfig();

        Assert.NotEqual(config.BaseFontPath, config.BaseFontBoldPath);
        config.Validate();
    }

    [Fact]
    public void Default_font_option_is_Arial()
    {
        Assert.Equal(DanfeFont.Arial, new DanfeOptions().Font);
    }

    [Theory]
    [InlineData("IBMPlexSans")]
    [InlineData("Inter")]
    [InlineData("Roboto")]
    public void Bundled_font_families_have_regular_and_bold_files(string family)
    {
        string fontsDirectory = FindFontsDirectory();
        string regularPath = Path.Combine(fontsDirectory, $"{family}-Regular.ttf");
        string boldPath = Path.Combine(fontsDirectory, $"{family}-Bold.ttf");

        Assert.True(File.Exists(regularPath), $"Fonte regular não encontrada: {regularPath}");
        Assert.True(File.Exists(boldPath), $"Fonte negrito não encontrada: {boldPath}");
        Assert.False(File.ReadAllBytes(regularPath).SequenceEqual(File.ReadAllBytes(boldPath)));
    }

    [Theory]
    [InlineData(DanfeFont.Inter, "Inter")]
    [InlineData(DanfeFont.Roboto, "Roboto")]
    [InlineData(DanfeFont.IbmPlexSans, "IBMPlexSans")]
    public void Font_option_selects_the_matching_regular_and_bold_files(DanfeFont font, string filePrefix)
    {
        var options = new DanfeOptions { Font = font };

        Assert.EndsWith($"{filePrefix}-Regular.ttf", options.FontConfig.BaseFontPath, StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith($"{filePrefix}-Bold.ttf", options.FontConfig.BaseFontBoldPath, StringComparison.OrdinalIgnoreCase);
        Assert.NotEqual(options.FontConfig.BaseFontPath, options.FontConfig.BaseFontBoldPath);
        options.FontConfig.Validate();
    }

    private static string FindFontsDirectory()
    {
        string? current = AppContext.BaseDirectory;
        for (int level = 0; level < 8 && !string.IsNullOrWhiteSpace(current); level++)
        {
            string candidate = Path.Combine(current, "fonts");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new DirectoryNotFoundException("Pasta fonts não encontrada.");
    }
}

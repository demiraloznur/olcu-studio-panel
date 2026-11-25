using System.Text;

namespace MiniCRMProject.Tests;

public class EncodingTests
{
    internal static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "MiniCRMProject.sln")))
        {
            dir = dir.Parent;
        }
        return dir?.FullName ?? throw new InvalidOperationException("MiniCRMProject.sln bulunamadı.");
    }

    [Fact]
    public void Kaynak_dosyalar_gecerli_UTF8()
    {
        var projectDir = Path.Combine(RepoRoot(), "MiniCRMProject");
        var files = Directory.EnumerateFiles(projectDir, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".cs") || f.EndsWith(".cshtml"))
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                     && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"));

        var strictUtf8 = new UTF8Encoding(false, true);
        var broken = new List<string>();
        foreach (var file in files)
        {
            try
            {
                var text = strictUtf8.GetString(File.ReadAllBytes(file));
                if (text.Contains('�')) broken.Add(file);
            }
            catch (DecoderFallbackException)
            {
                broken.Add(file);
            }
        }

        Assert.Empty(broken);
    }
}

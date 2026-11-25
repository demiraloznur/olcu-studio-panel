using MiniCRMProject.Services;

namespace MiniCRMProject.Tests;

public class PasswordServiceTests
{
    private readonly PasswordService service = new();

    [Fact]
    public void Hash_duz_metni_icermez()
    {
        string first = service.Hash("Olcu2026!");
        string second = service.Hash("Olcu2026!");

        Assert.DoesNotContain("Olcu2026!", first);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Verify_dogru_sifre_true()
    {
        string hash = service.Hash("Olcu2026!");
        Assert.True(service.Verify(hash, "Olcu2026!"));
    }

    [Fact]
    public void Verify_yanlis_sifre_false()
    {
        string hash = service.Hash("Olcu2026!");
        Assert.False(service.Verify(hash, "olcu2026!"));
    }

    [Fact]
    public void Verify_bozuk_hash_false()
    {
        Assert.False(service.Verify("abc", "x"));
    }
}

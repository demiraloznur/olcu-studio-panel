using MiniCRMProject.Models;

namespace MiniCRMProject.Tests;

public class DashboardSummaryTests
{
    private static readonly DateTime Today = new(2026, 11, 25);

    private static ProjectModel P(ProjectStage stage, int? daysUntilEnd, string name = "p") => new()
    {
        ProjectName = name,
        Stage = stage,
        EndDate = daysUntilEnd.HasValue ? Today.AddDays(daysUntilEnd.Value).AddHours(15) : null
    };

    [Fact]
    public void Tarihsiz_proje_ne_yaklasan_ne_geciken()
    {
        var summary = DashboardSummary.From(new[] { P(ProjectStage.Konsept, null) }, Today);

        Assert.Empty(summary.Upcoming);
        Assert.Empty(summary.Overdue);
    }

    [Fact]
    public void Teslim_edilmis_eski_proje_geciken_sayilmaz()
    {
        var summary = DashboardSummary.From(new[] { P(ProjectStage.TeslimEdildi, -10) }, Today);

        Assert.Empty(summary.Overdue);
    }

    [Fact]
    public void Tarihi_gecmis_teslim_edilmemis_proje_geciken()
    {
        var summary = DashboardSummary.From(new[] { P(ProjectStage.Uygulama, -1) }, Today);

        Assert.Single(summary.Overdue);
        Assert.Empty(summary.Upcoming);
    }

    [Fact]
    public void Bugun_bitecek_proje_yaklasan()
    {
        var summary = DashboardSummary.From(new[] { P(ProjectStage.Cizim, 0) }, Today);

        Assert.Single(summary.Upcoming);
        Assert.Empty(summary.Overdue);
    }

    [Fact]
    public void Otuz_gun_sinirda_otuz_bir_disarida()
    {
        var summary = DashboardSummary.From(new[] { P(ProjectStage.Cizim, 30, "a"), P(ProjectStage.Cizim, 31, "b") }, Today);

        Assert.Equal("a", Assert.Single(summary.Upcoming).ProjectName);
    }

    [Fact]
    public void Yaklasanlar_tarihe_gore_sirali()
    {
        var summary = DashboardSummary.From(new[] { P(ProjectStage.Konsept, 20, "sonra"), P(ProjectStage.Konsept, 5, "once") }, Today);

        Assert.Equal(new[] { "once", "sonra" }, summary.Upcoming.Select(p => p.ProjectName));
    }

    [Fact]
    public void Asama_dagilimi_bes_anahtar_ve_toplam()
    {
        var summary = DashboardSummary.From(new[] { P(ProjectStage.Konsept, null), P(ProjectStage.Konsept, 3), P(ProjectStage.Cizim, null) }, Today);

        Assert.Equal(3, summary.Total);
        Assert.Equal(5, summary.ByStage.Count);
        Assert.Equal(2, summary.ByStage[ProjectStage.Konsept]);
        Assert.Equal(0, summary.ByStage[ProjectStage.TeslimEdildi]);
    }
}

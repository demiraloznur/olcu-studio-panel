using MiniCRMProject.Models;

namespace MiniCRMProject.Tests;

public class ProjectStageTests
{
    [Theory]
    [InlineData(ProjectStage.OlcuAlindi, "Ölçü alındı", 20)]
    [InlineData(ProjectStage.Konsept, "Konsept", 40)]
    [InlineData(ProjectStage.Cizim, "Çizim", 60)]
    [InlineData(ProjectStage.Uygulama, "Uygulama", 80)]
    [InlineData(ProjectStage.TeslimEdildi, "Teslim edildi", 100)]
    public void Asama_adi_ve_ilerleme(ProjectStage stage, string name, int percent)
    {
        Assert.Equal(name, stage.DisplayName());
        Assert.Equal(percent, stage.ProgressPercent());
    }

    [Theory]
    [InlineData(ProjectCategory.Konut, "Konut")]
    [InlineData(ProjectCategory.Ofis, "Ofis")]
    [InlineData(ProjectCategory.Ticari, "Ticari")]
    public void Kategori_adi(ProjectCategory category, string name)
    {
        Assert.Equal(name, category.DisplayName());
    }

    [Fact]
    public void Asamalar_sirali_ve_bes_tane()
    {
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, Enum.GetValues<ProjectStage>().Select(s => (byte)s));
    }
}

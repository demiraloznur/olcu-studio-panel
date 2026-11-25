using MiniCRMProject.Models;

namespace MiniCRMProject.Tests;

public class ProjectListQueryTests
{
    [Fact]
    public void Arama_metni_sorguya_degil_parametreye_gider()
    {
        var query = new ProjectListQuery { Search = "x'; DROP TABLE Users--" };

        var (sql, parameters) = query.BuildWhereAndOrder();

        Assert.DoesNotContain("DROP TABLE", sql);
        var search = Assert.Single(parameters, p => p.ParameterName == "@search");
        Assert.Equal("%x'; DROP TABLE Users--%", search.Value);
    }

    [Fact]
    public void Bilinmeyen_siralama_yeniden_eskiye_olur()
    {
        var (sql, _) = new ProjectListQuery { Sort = "DROP" }.BuildWhereAndOrder();

        Assert.Contains("ORDER BY Projects.CreatedAt DESC", sql);
        Assert.DoesNotContain("DROP", sql);
    }

    [Fact]
    public void Musteri_filtresi_parametreli()
    {
        var (sql, parameters) = new ProjectListQuery { CustomerId = 3 }.BuildWhereAndOrder();

        Assert.Contains("Projects.CustomerId = @customerId", sql);
        Assert.Equal(3, Assert.Single(parameters, p => p.ParameterName == "@customerId").Value);
    }

    [Fact]
    public void Bitise_gore_siralamada_tarihsizler_sonda()
    {
        var (sql, _) = new ProjectListQuery { Sort = "bitis" }.BuildWhereAndOrder();

        Assert.Contains("ORDER BY CASE WHEN Projects.EndDate IS NULL THEN 1 ELSE 0 END, Projects.EndDate ASC", sql);
    }

    [Fact]
    public void Kategori_ve_asama_filtresi()
    {
        var query = new ProjectListQuery { Category = ProjectCategory.Ofis, Stage = ProjectStage.Cizim };

        var (sql, parameters) = query.BuildWhereAndOrder();

        Assert.Contains("Projects.Category = @category", sql);
        Assert.Contains("Projects.Stage = @stage", sql);
        Assert.Equal((byte)2, parameters.Single(p => p.ParameterName == "@category").Value);
        Assert.Equal((byte)3, parameters.Single(p => p.ParameterName == "@stage").Value);
    }

    [Fact]
    public void Filtresiz_sorguda_WHERE_yok()
    {
        var (sql, parameters) = new ProjectListQuery { Search = "   " }.BuildWhereAndOrder();

        Assert.DoesNotContain("WHERE", sql);
        Assert.Empty(parameters);
    }
}

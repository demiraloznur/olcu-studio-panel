using System.ComponentModel.DataAnnotations;
using MiniCRMProject.Models;

namespace MiniCRMProject.Tests;

public class ValidationTests
{
    internal static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    private static RegisterViewModel ValidRegister() => new()
    {
        FirstName = "Elif",
        LastName = "Koç",
        Email = "elif@olcustudio.test",
        Password = "Musteri2026!"
    };

    [Fact]
    public void Gecerli_kayit_hatasiz()
    {
        Assert.Empty(Validate(ValidRegister()));
    }

    [Fact]
    public void Kisa_sifre_reddedilir()
    {
        var model = ValidRegister();
        model.Password = "1234567";

        var error = Assert.Single(Validate(model));
        Assert.Equal("Şifre en az 8 karakter olmalı.", error.ErrorMessage);
    }

    [Fact]
    public void Gecersiz_eposta_reddedilir()
    {
        var model = ValidRegister();
        model.Email = "elif-olcustudio";

        Assert.Contains(Validate(model), e => e.MemberNames.Contains(nameof(RegisterViewModel.Email)));
    }

    [Fact]
    public void Bos_ad_reddedilir()
    {
        var model = ValidRegister();
        model.FirstName = "";

        Assert.Contains(Validate(model), e => e.MemberNames.Contains(nameof(RegisterViewModel.FirstName)));
    }

    [Fact]
    public void Proje_adi_150_karakteri_gecemez()
    {
        var project = new ProjectModel { ProjectName = new string('a', 151), CustomerId = 2 };

        Assert.Contains(Validate(project), e => e.MemberNames.Contains(nameof(ProjectModel.ProjectName)));
    }

    [Fact]
    public void Musterisiz_proje_reddedilir()
    {
        var project = new ProjectModel { ProjectName = "Nilüfer daire", CustomerId = 0 };

        Assert.Contains(Validate(project), e => e.MemberNames.Contains(nameof(ProjectModel.CustomerId)));
    }

    [Fact]
    public void Gecerli_proje_hatasiz()
    {
        var project = new ProjectModel { ProjectName = "Nilüfer daire", CustomerId = 2 };

        Assert.Empty(Validate(project));
    }
}

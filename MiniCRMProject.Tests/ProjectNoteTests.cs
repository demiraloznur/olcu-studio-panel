using MiniCRMProject.Models;

namespace MiniCRMProject.Tests;

public class ProjectNoteTests
{
    [Fact]
    public void Bos_not_reddedilir()
    {
        var note = new ProjectNote { ProjectId = 1, Body = "" };

        Assert.Contains(ValidationTests.Validate(note), e => e.MemberNames.Contains(nameof(ProjectNote.Body)));
    }

    [Fact]
    public void Bin_karakteri_asan_not_reddedilir()
    {
        var note = new ProjectNote { ProjectId = 1, Body = new string('a', 1001) };

        Assert.Contains(ValidationTests.Validate(note), e => e.MemberNames.Contains(nameof(ProjectNote.Body)));
    }

    [Fact]
    public void Gecerli_not_hatasiz()
    {
        var note = new ProjectNote { ProjectId = 1, Body = "Konsept sunumu yapıldı." };

        Assert.Empty(ValidationTests.Validate(note));
    }
}

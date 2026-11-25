using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using MiniCRMProject.Models;
using MiniCRMProject.Security;

namespace MiniCRMProject.Tests;

public class AuthorizationTests
{
    private static ActionExecutingContext Context(SessionUser? user)
    {
        var http = new DefaultHttpContext { Session = new TestSession() };
        user?.WriteTo(http.Session);
        var action = new ActionContext(http, new RouteData(), new ActionDescriptor());
        return new ActionExecutingContext(action, new List<IFilterMetadata>(), new Dictionary<string, object?>(), controller: new object());
    }

    private static readonly SessionUser Admin = new(1, "Zeynep Arslan", "Admin");
    private static readonly SessionUser Customer = new(2, "Elif Koç", "Customer");

    [Fact]
    public void RequireAdmin_musteriyi_anasayfaya_yonlendirir()
    {
        var context = Context(Customer);
        new RequireAdminAttribute().OnActionExecuting(context);

        var redirect = Assert.IsType<RedirectToActionResult>(context.Result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public void RequireAdmin_oturumsuzu_girise_yonlendirir()
    {
        var context = Context(null);
        new RequireAdminAttribute().OnActionExecuting(context);

        var redirect = Assert.IsType<RedirectToActionResult>(context.Result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
    }

    [Fact]
    public void RequireAdmin_yoneticiyi_gecirir()
    {
        var context = Context(Admin);
        new RequireAdminAttribute().OnActionExecuting(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public void RequireLogin_oturumsuzu_girise_yonlendirir()
    {
        var context = Context(null);
        new RequireLoginAttribute().OnActionExecuting(context);

        var redirect = Assert.IsType<RedirectToActionResult>(context.Result);
        Assert.Equal("Login", redirect.ActionName);
    }

    [Fact]
    public void RequireLogin_musteriyi_gecirir()
    {
        var context = Context(Customer);
        new RequireLoginAttribute().OnActionExecuting(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public void CanView_musteri_baskasinin_projesini_goremez()
    {
        Assert.False(ProjectAccess.CanView(new ProjectModel { CustomerId = 3 }, Customer));
    }

    [Fact]
    public void CanView_musteri_kendi_projesini_gorur()
    {
        Assert.True(ProjectAccess.CanView(new ProjectModel { CustomerId = 2 }, Customer));
    }

    [Fact]
    public void CanView_yonetici_her_projeyi_gorur()
    {
        Assert.True(ProjectAccess.CanView(new ProjectModel { CustomerId = 3 }, Admin));
    }

    [Fact]
    public void SessionUser_oturuma_yazilip_okunur()
    {
        var session = new TestSession();
        Customer.WriteTo(session);

        Assert.Equal(Customer, SessionUser.From(session));
    }
}

internal class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> store = new();
    public bool IsAvailable => true;
    public string Id => "test";
    public IEnumerable<string> Keys => store.Keys;
    public void Clear() => store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => store.Remove(key);
    public void Set(string key, byte[] value) => store[key] = value;
    public bool TryGetValue(string key, out byte[] value) => store.TryGetValue(key, out value!);
}

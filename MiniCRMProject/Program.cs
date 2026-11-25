var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Model bağlama hatalarının Türkçe mesajları (ör. müşteri seçilmeden gönderilen form).
    var messages = options.ModelBindingMessageProvider;
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"{field} için geçerli bir değer girin.");
    messages.SetValueMustNotBeNullAccessor(value => "Bu alan zorunlu.");
    messages.SetValueIsInvalidAccessor(value => "Lütfen geçerli bir değer girin.");
    messages.SetValueMustBeANumberAccessor(field => "Lütfen bir sayı girin.");
    messages.SetMissingBindRequiredValueAccessor(field => "Bu alan zorunlu.");
});
builder.Services.AddSession();

builder.Services.AddSingleton<MiniCRMProject.Data.SqlConnectionFactory>();
builder.Services.AddScoped<MiniCRMProject.Data.UserRepository>();
builder.Services.AddScoped<MiniCRMProject.Data.ProjectRepository>();
builder.Services.AddScoped<MiniCRMProject.Data.NoteRepository>();
builder.Services.AddSingleton<MiniCRMProject.Services.PasswordService>();
builder.Services.AddScoped<MiniCRMProject.Data.DemoSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<MiniCRMProject.Data.DemoSeeder>().SeedIfEmpty();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

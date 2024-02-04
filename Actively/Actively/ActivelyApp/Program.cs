using ActivelyApp.Middleware;
using ActivelyApp.Services.ServiceRegistration;
using ActivelyInfrastructure;
using Microsoft.AspNetCore.Localization;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterServices(builder);
var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<ActivelyDbSeeder>().Seed();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();

var languageSettings = builder.Configuration.GetSection("LanguageSettings:SupportedLanguages").Get<List<string>>();
var supportedCultures = new List<CultureInfo>();
languageSettings.ForEach(x => supportedCultures.Add(new CultureInfo(x)));
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

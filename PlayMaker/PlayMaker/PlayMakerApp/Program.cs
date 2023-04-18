using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "C:\\C_Sources\\PlayMaker2\\PlayMaker\\PlayMaker\\Resources");
builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization(options =>
{
    var type = typeof(Resources.Common);
    var assembly = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
    var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
    var localizer = factory.Create("Common", assembly.Name);
    options.DataAnnotationLocalizerProvider = (t, f) => localizer;  
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("pl")
};
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

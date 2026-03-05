using AAP_Authentication;
using AAP_Inventory_Zone_Tracker.Components;
using Data.INV_DB;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Services.DB_Services;
using VQDotSticker_Blazor8.Services.Authentication;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddLogging();

builder.Services.AddSingleton<DB_Services>();
builder.Services.AddSingleton<Admin_Services>();


//Auth.
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthorization(options =>
{
    AAP_Authorize_Policy.AddCustomPolicies(options); // Policy for Authorization to Site and actions.
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

//DB connections


var connectionString = builder.Configuration.GetConnectionString("MySQLConection")
        ?? throw new NullReferenceException("No connection string in config!");

builder.Services.AddPooledDbContextFactory<InventoryDbContext>((DbContextOptionsBuilder options) => options.UseMySQL(connectionString));
builder.Services.AddPooledDbContextFactory<AuthenticationContext>((DbContextOptionsBuilder options) => options.UseMySQL(connectionString));







var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

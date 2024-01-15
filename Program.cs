using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IDP.Data;
using IdentityServer4.Models;
using IDP;
using DemoIDP;
using Microsoft.Extensions.Hosting;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
var usersConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var identityConnection = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

var migrationAssembly = typeof(Program).Assembly.GetName().Name;
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(usersConnection,
            sql => sql.MigrationsAssembly(migrationAssembly)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser , IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();




//builder.Services.AddIdentityServer()
//    .AddInMemoryClients(new List<Client>())
//    .AddInMemoryIdentityResources(new List<IdentityResource>())
//    .AddInMemoryApiResources(new List<ApiResource>())
//    .AddInMemoryApiScopes(new List<ApiScope>())
//    .AddTestUsers(new List<IdentityServer4.Test.TestUser>())
//    .AddDeveloperSigningCredential();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(option =>
    {
        option.ConfigureDbContext = builder=>builder.UseSqlServer(identityConnection,
            sql=>sql.MigrationsAssembly(migrationAssembly));
    })
    .AddOperationalStore(option =>
    {
        option.ConfigureDbContext = builder=> builder.UseSqlServer(identityConnection,
            sql => sql.MigrationsAssembly(migrationAssembly));
    })
    .AddDeveloperSigningCredential();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//Mapper
builder.Services.AddAutoMapper(typeof(IdentityMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var mapper = services.GetRequiredService<IMapper>();
    try
    {


        SeedData.EnsureSeedData(usersConnection, identityConnection, mapper);
    }
    catch (Exception ex)
    {
        // Log any exceptions that occur during data seeding
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();


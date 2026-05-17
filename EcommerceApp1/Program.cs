using EcommerceApp1.Data;
using EcommerceApp1.Models;
using EcommerceApp1.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from the configuration.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Tell DbContext to use the connection string to connect to the database.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// DependencyInjection, DbContext, Identity,
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IShoppingCartRepository, ShoppingCartRepository>();

var app = builder.Build();

// Seed the database with sample data.
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedDefaultData(scope.ServiceProvider);
}

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

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    "category",
    "{controller}/{action}/{category}/{searchString}/{minPrice}/{maxPrice}/{page}"
    ,new {controller = "Products", action = "Category", page = 1}
    );

app.MapControllerRoute(
    "deleteProduct",
    "{controller=Dashboard}/{action=DeleteProduct}"
);

app.MapRazorPages()
    .WithStaticAssets();

app.Run();
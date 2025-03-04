using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopComputer.Areas.Admin.Repository;
using ShopComputer.Models;
using ShopComputer.Repository;

var builder = WebApplication.CreateBuilder(args);

//connect db
builder.Services.AddDbContext<DataContext>(options =>
{
	options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

// Add Email Sender
 builder.Services.AddTransient<IEmailSender, EmailSender>();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();	
builder.Services.AddSession(opption =>
{
	opption.IdleTimeout = TimeSpan.FromMinutes(30);
	opption.Cookie.IsEssential = true; 
});

builder.Services.AddIdentity<AppUserModel,IdentityRole>()
	.AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
	// Password settings.
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 4;

	options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

app.UseSession();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
	name: "Areas",
	pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "category",
	pattern: "/category/{Slug?}",
	defaults: new { Controller = "Category", Action = "Index" });

app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new { Controller = "Brand", Action = "Index" });

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

// Seeding data
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
Seeddata.SeedingData(context);


app.Run();

using Microsoft.EntityFrameworkCore;
using KurumsalEnvanterMVC.Data;
using KurumsalEnvanterMVC.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Veritabaný baðlantý servisi eklendi (5. Gün)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Generic Repository servisi baðýmlýlýk enjeksiyonuna kaydedildi (6. Gün)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 26. GÜN: Özel Hata Sayfalarý Yönlendirme Middleware'i
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Geliþtirme ortamýnda da özel 404/500 sayfalarýný test edebilmek için:
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
}


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
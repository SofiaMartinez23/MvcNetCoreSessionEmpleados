using Microsoft.EntityFrameworkCore;
using MvcNetCoreSessionEmpleados.Data;
using MvcNetCoreSessionEmpleados.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

string connectionString = builder.Configuration.GetConnectionString("SqlHospital");
builder.Services.AddTransient<RepositoryEmpleados>();
builder.Services.AddDbContext<HospitalContext>
    (options => options.UseSqlServer(connectionString));
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

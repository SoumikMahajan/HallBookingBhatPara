using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Extension;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800; // 50 MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")

));

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret")!;

// Add services to the container.
builder.Services.AddControllersWithViews(u =>
{
    u.Filters.Add<GlobalExceptionRedirection>();
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<GlobalExceptionRedirection>();
builder.Services.AddScoped<ExceptionHandlingHelper>();
builder.Services.AddScoped<LogService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.ConfigureGlobalExceptionHandler();
    app.UseHsts();
}
else
{
    app.ConfigureGlobalExceptionHandler();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();

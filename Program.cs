using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.Utility;
using HallBookingBhatPara.Extension;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;
using HallBookingBhatPara.Infrastructure.Service;
using HallBookingBhatPara.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// Add services to the container.
builder.Services.AddControllersWithViews(u =>
{
    u.Filters.Add<GlobalExceptionRedirection>();
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

builder.Services.AddHttpContextAccessor();

// Register custom services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<GlobalExceptionRedirection>();
builder.Services.AddScoped<ExceptionHandlingHelper>();
builder.Services.AddScoped<LogService>();

builder.Services.AddDistributedMemoryCache();

// Add JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["ApiSettings:Secret"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    // Get token from cookie instead of Authorization header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey(SD.AccessToken))
            {
                context.Token = context.Request.Cookies[SD.AccessToken];
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();

            // If it's an AJAX request, return JSON
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                context.Request.Headers["Content-Type"].ToString().Contains("application/json"))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var response = new
                {
                    isSuccess = false,
                    message = "Authentication required",
                    redirectUrl = "/User/Login"
                };
                return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }

            // For regular requests, redirect to login
            context.Response.Redirect("/User/Login");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            // User is logged in but doesn't have permission → redirect to Home/Index
            context.Response.Redirect("/Home/Index");
            return Task.CompletedTask;
        }
    };
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/User/Logout";
    options.AccessDeniedPath = "/User/Login";
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.ConfigureGlobalExceptionHandler();
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add JWT Middleware
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=CheckTokenAndRedirect}/{id?}");

app.Run();
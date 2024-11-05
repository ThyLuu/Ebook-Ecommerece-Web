using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<VnPaymentService>();

builder.Services.Configure<VnPaymentRequest>(builder.Configuration.GetSection("Payment:VnPayment"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(p => {
        p.LoginPath = "/auth/login";
        p.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

builder.Services.AddDbContext<OrganicContext>(p => 
    p.UseSqlServer(builder.Configuration.GetConnectionString("Organic")));
builder.Services.AddMvc();

var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.Run();
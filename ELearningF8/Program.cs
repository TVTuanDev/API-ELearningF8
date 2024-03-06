using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Prng;
using System.Text;
using System;
using ELearningF8.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        //ValidAudience = builder.Configuration["Jwt:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? ""))
    };
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login/";
    //options.LogoutPath = "/logout/";
    //options.AccessDeniedPath = "/khongduoctruycap.html";
    options.Cookie.HttpOnly = false;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

// AddTransient: Dịch vụ được tạo mới mỗi khi nó được yêu cầu.
// AddScoped: Dịch vụ được tạo một lần cho mỗi yêu cầu HTTP.
// AddSingleton: Dịch vụ được tạo một lần duy nhất.
//builder.Services.AddTransient<RandomGenerator>();
//builder.Services.AddTransient<PasswordManager>();
//builder.Services.AddScoped<MailHandleServices>();
//builder.Services.AddScoped<JwtAuthorizeFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ELearningF8.Data;
using ELearningF8.Models;
using ELearningF8.Services;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using ELearningF8.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ServerDbContext"));
    //options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext"));
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
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var ggConf = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = ggConf["ClientId"];
    options.ClientSecret = ggConf["ClientSecret"];
    options.CallbackPath = "/signin-google";

    //options.Scope.Clear();
    //options.Scope.Add("openid");
    //options.Scope.Add("profile");
    //options.Scope.Add("email");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", 
        builder => builder.WithOrigins(
            "https://localhost:44352/",
            "http://apif8.somee.com/",
            "http://clonef8.somee.com/")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());
});

// Đăng ký Identity
//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();
//builder.Services.AddAuthorization();

// AddTransient: Dịch vụ được tạo mới mỗi khi nó được yêu cầu.
// AddScoped: Dịch vụ được tạo một lần cho mỗi yêu cầu HTTP.
// AddSingleton: Dịch vụ được tạo một lần duy nhất.
builder.Services.AddTransient<RandomGenerator>();
builder.Services.AddTransient<PasswordManager>();
builder.Services.AddTransient<ExpriedToken>();
builder.Services.AddScoped<SendMailServices>();
builder.Services.AddScoped<MailHandleController>();
builder.Services.AddScoped<Cloudinary>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var account = new Account(
        configuration["Cloudinary:CloudName"],
        configuration["Cloudinary:ApiKey"],
        configuration["Cloudinary:ApiSecret"]
    );
    return new Cloudinary(account);
});
//builder.Services.AddScoped<JwtAuthorizeFilter>();
//builder.Services.AddScoped<SignInManager<AppDbContext>, SignInManager<AppDbContext>>();
//builder.Services.AddScoped<UserManager<AppDbContext>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

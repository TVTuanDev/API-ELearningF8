Các lệnh khác
dotnet watch run
dotnet build
dotnet publish
libman restore

Chạy lệnh để tạo Migration, Scaffold và tạo db
Scaffold-DbContext 'Name=ConnectionStrings:AppDbContext' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data -Context AppDbContext -NoOnConfiguring -f
dotnet ef migrations add TenMigration
dotnet ef migrations remove
dotnet ef database update
dotnet ef dbcontext scaffold "ChuoiKetNoi" Microsoft.EntityFrameworkCore.SqlServer -o TenThuMuc

Các câu lệnh thường dùng
#dotnet aspnet-codegenerator area Admin
#dotnet aspnet-codegenerator controller -name Category -namespace TShop.Areas.Products.Controllers -outDir Areas/Products/Controllers -m TShop.Models.Blog.Post -dc TShop.Models.TShopDbContext -udl
#dotnet aspnet-codegenerator view -name Create -outDir Areas/Contact/Views/Contact -m App.Models.Contact.Contact -dc App.Models.AppDbContext
#dotnet aspnet-codegenerator view Index Empty -outDir Areas/Contact/Views/Contact -m planet -l \_Layout -f
#dotnet aspnet-codegenerator razorpage -name YourPageName --relativeFolderPath Pages\YourFolder
// Phát sinh giao diện Identity
#dotnet aspnet-codegenerator identity -dc TShop.Models.TShopDbContext

Đảm bảo cài đặt công cụ lệnh dotnet aspnet-codegenerator
dotnet tool install --global Microsoft.Web.LibraryManager.Cli
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 6
dotnet tool install -g dotnet-aspnet-codegenerator --version 6
dotnet tool update -g dotnet-aspnet-codegenerator --version 6

Thêm package
dotnet add package Microsoft.EntityFrameworkCore --version 6
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 6
dotnet add package Microsoft.AspNetCore.Identity.UI --version 6
dotnet add package Microsoft.AspNetCore.Authentication --version 6
dotnet add package Microsoft.AspNetCore.Authentication.Facebook --version 6
dotnet add package Microsoft.AspNetCore.Authentication.Google --version 6
dotnet add package Microsoft.AspNetCore.Authentication.Twitter --version 6
dotnet add package Microsoft.AspNetCore.Authentication.Cookies --version 6
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 6
dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount --version 6
dotnet add package Microsoft.AspNetCore.Authentication.oAuth --version 6
dotnet add package Microsoft.AspNetCore.Authentication.OpenIDConnect --version 6
dotnet tool install --global Microsoft.Web.LibraryManager.Cli
dotnet add package MailKit
dotnet add package MimeKit

Gõ lệnh để xem hướng dẫn
dotnet aspnet-codegenerator -h

Cấu hình appsettings.json
"ConnectionStrings": {
"MyDbContext": "Server=DESKTOP-PENP0E8\\SQLEXPRESS; Database=Tshop2024; Integrated Security=True"
"MyDbContext": "Server=DESKTOP-PENP0E8\\SQLEXPRESS; Database=Tshop2024; User Id=tvtuan; Password=123456"
},
"MailSettings": {
"Mail": "attuan9@gmail.com",
"DisplayName": "Tuan",
"Password": "mat0khau",
"Host": "smtp.gmail.com",
"Port": 587
},
"Authentication": {
"Google": {
"ClientId": "986193807084-7a951t10di2e3vq2mg5v4rm6loludra7.apps.googleusercontent.com",
"ClientSecret": "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"
},
"Facebook": {
"AppId": "403987211974066",
"AppSecret": "9117811550354b3d87f4bc481816e3c7"
}
},

SignIn
var claims = new List<Claim> { new Claim(ClaimTypes.Name, "username"), };
var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
bool isPersistent = false;
var authenticationProperties = new AuthenticationProperties {IsPersistent = isPersistent};

    HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        claimsPrincipal,
        authenticationProperties ?? new AuthenticationProperties());

SignOut
HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

■■ MyShop – ASP.NET Core MVC E-Commerce Website
MyShop is a full-featured e-commerce web application built using ASP.NET Core MVC, designed to
provide a smooth and responsive online shopping experience. It includes essential e-commerce
features such as product browsing, shopping cart management, user authentication, and an admin
dashboard for managing products and categories.
■ Live Demo: https://myshopcommerce.runasp.net
■ Features
- ■ Customer Side: Browse products, add to cart, manage orders and profile
- ■■ Admin Side: Manage products, categories, users, and orders via dashboard
- ■ UI/UX: Responsive Bootstrap 5 layout, Dark/Light mode toggle, clean and modern design
- ■ Authentication: ASP.NET Core Identity for secure login and registration
■■ Tech Stack
Frontend: HTML5, CSS3, Bootstrap 5, JavaScript
Backend: ASP.NET Core MVC (.NET 8)
Database: Microsoft SQL Server
ORM: Entity Framework Core
Authentication: ASP.NET Core Identity
Hosting: runasp.net (Monsterasp.net free hosting plan)

■ Project Structure
MyShop/
■■■ MyShop/ # Main ASP.NET Core MVC project
■ ■■■ Controllers/ # Controllers (Customer, Admin, Account, etc.)
■ ■■■ Models/ # Entity and View Models
■ ■■■ Views/ # Razor Views (Shared, Home, Admin, etc.)
■ ■■■ wwwroot/ # Static files (CSS, JS, Images)
■ ■■■ Areas/ # Admin area
■ ■■■ appsettings.json # Configuration (DB connection)
■ ■■■ Program.cs # App startup
■■■ MyShop.DataAccess/ # Data access layer (EF DbContext, repositories)
■■■ MyShop.Business/ # Business logic layer
■■■ MyShop.sln # Visual Studio solution file
■ Getting Started
Prerequisites:
- .NET 8 SDK
- SQL Server
- Visual Studio 2022
Setup Instructions:
1. Clone Repository: git clone https://github.com/yourusername/myshop.git
2. Open Solution: MyShop.sln
3. Update Connection String in appsettings.json
4. Apply Migrations: Update-Database
5. Run the Project: dotnet run
6. Access Site: Local - https://localhost:5001, Online - https://myshopcommerce.runasp.net

7. Live Demo : https://myshopcommerce.runasp.net/

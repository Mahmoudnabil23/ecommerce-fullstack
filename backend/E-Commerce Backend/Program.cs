using E_Commerce_Backend.Configurations;
using E_Commerce_Backend.Data.Seeders;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories;
using E_Commerce_Backend.Repositories.Implementations;
using E_Commerce_Backend.Repositories.Interfaces;
using E_Commerce_Backend.Services.AdminServices;
using E_Commerce_Backend.Services.AdminServices.Interfaces;
using E_Commerce_Backend.Services.AuthServices;
using E_Commerce_Backend.Services.AuthServices.Interfaces;
using E_Commerce_Backend.Services.Email;
using E_Commerce_Backend.Services.EmailService;
using E_Commerce_Backend.Services.Products;
using E_Commerce_Backend.Services.Payments;
using E_Commerce_Backend.Services.Payments.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using E_Commerce_Backend.Services.Cart;
using E_Commerce_Backend.Services.Orders;
using E_Commerce_Backend.Services.Orders.Interfaces;

namespace E_Commerce_Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load environment variables from .env file
            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Angular dev server
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            #region Swagger Configuration
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "E-Commerce Backend API",
                    Description = " E-Commerce Project"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme 
                    { Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer" }},
                    new string[] {}
                    }
                    });
            });
            #endregion 


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


            // jwt configuration 
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt")); // bind the JwtSettings class to the "Jwt" section in appsettings.json

            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>(); 

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // instead of cookies, we will use JWT for authentication
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // returns unauthorized if the user is not authenticated instead of redirecting to Account/Login
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

            // bind smtp settings
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            // service registrations
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IdentitySeeder>();
            builder.Services.AddScoped<DomainSeeder>();
            builder.Services.AddScoped<IAppSeeder, AppSeeder>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IAdminProductService, AdminProductService>();
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IEmailService, SmtpEmailService>();
            builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            // CORS policy for Angular frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });



            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                using var scope = app.Services.CreateScope();
                var seeder = scope.ServiceProvider.GetRequiredService<IAppSeeder>();
                await seeder.SeedAsync();
            }

            app.UseCors("AllowAngular");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WorkoutTracker.WebApi.Data.DbContexts;
using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Mappers;
using WorkoutTracker.WebApi.Mappers.Interfaces;
using WorkoutTracker.WebApi.Middlewares;
using WorkoutTracker.WebApi.Models.WTUsers;
using WorkoutTracker.WebApi.Repositories;
using WorkoutTracker.WebApi.Repositories.Interfaces;
using WorkoutTracker.WebApi.Services;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database
            builder.Services.AddDbContext<WTDbContext>(options =>
            {
                options.UseInMemoryDatabase("WTDbContext");
            });

            // Jwt Auth
            ConfigureJwtAuthentication(builder);

            // Swagger UI
            ConfigureSwaggerServices(builder);

            // Misc
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // DI Registration
            DIRegistration(builder);

            // Test data
            CreateInMemoryUserStore(builder);

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<WTDbContext>();
                    dbContext.Database.EnsureCreated();
                }

                app.MapOpenApi();

                // Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void DIRegistration(WebApplicationBuilder builder)
        {
            // JWT
            builder.Services.AddScoped<IJwtService, JwtService>();

            // Mappers
            builder.Services.AddScoped<IWTUsersMapper, WTUsersMapper>();

            // User
            builder.Services.AddScoped<IWTUsersService, WTUsersService>();
            builder.Services.AddScoped<IWTUsersRepository, WTUsersRepository>();

            // WorkoutDay
            builder.Services.AddScoped<IWorkoutDaysService, WorkoutDaysService>();
            builder.Services.AddScoped<IWorkoutDaysRepository, WorkoutDaysRepository>();

            // Misc
            builder.Services.AddScoped<IPasswordHasher<WTUserEntity>, PasswordHasher<WTUserEntity>>();
            builder.Services.AddSingleton<IExceptionHandlingMiddlewareService, ExceptionHandlingMiddlewareService> ();
        }

        private static void ConfigureJwtAuthentication(WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            ArgumentNullException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(issuer);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(audience);

            var keyBytes = Encoding.ASCII.GetBytes(key);

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                    };
                });
        }

        private static void ConfigureSwaggerServices(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter \"Bearer\" [space] and then your token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        private static void CreateInMemoryUserStore(WebApplicationBuilder builder)
        {
            var users = new List<WTUserDto>();

            builder.Services.AddSingleton(users);
        }
    }
}

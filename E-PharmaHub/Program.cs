
using E_PharmaHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using E_PharmaHub.Services;

namespace E_PharmaHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()   
                              .AllowAnyHeader()  
                              .AllowAnyMethod(); 
                    });
            });

            builder.Services.AddControllers()
                         .AddJsonOptions(options =>
                               {
                                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
                                });
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddGoogle(options =>
            {
               options.ClientId = builder.Configuration["AuthenticationGoogle:Google:ClientId"];
               options.ClientSecret = builder.Configuration["AuthenticationGoogle:Google:ClientSecret"];
               options.CallbackPath = "/signin-google";
            })
            .AddFacebook(options =>
            {
               options.AppId = builder.Configuration["AuthenticationFacebook:Facebook:AppId"];
               options.AppSecret = builder.Configuration["AuthenticationFacebook:Facebook:AppSecret"];
               options.CallbackPath = "/signin-facebook";
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<EHealthDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<EHealthDbContext>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

            app.UseCors("AllowAll");


            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                foreach (var roleName in Enum.GetNames(typeof(UserRole)))
                {
                    if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                    {
                        roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                    }
                }
            }


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

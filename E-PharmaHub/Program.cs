
using E_PharmaHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using E_PharmaHub.Services;
using E_PharmaHub.Repositories;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddJwtBearer(options =>
            {
              options.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              ValidIssuer = builder.Configuration["JWT:Issuer"],
              ValidAudience = builder.Configuration["JWT:Audience"],
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            };
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

            builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();

            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            builder.Services.AddScoped<IGenericRepository<Pharmacy>, PharmacyRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IPharmacyService, PharmacyService>();
            builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);
            builder.Services.AddScoped<IGenericRepository<Clinic>, ClinicRepository>();
            builder.Services.AddScoped<IClinicService, ClinicService>();
            builder.Services.AddScoped<IPharmacistRepository, PharmacistRepository>();
            builder.Services.AddScoped<IPharmacistService, PharmacistService>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
            builder.Services.AddHttpContextAccessor();



            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<EHealthDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<EHealthDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
              options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
              options.JsonSerializerOptions.WriteIndented = true;
            });

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

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
using Stripe;
using E_PharmaHub.Hubs;
using Microsoft.Extensions.FileProviders;
using Azure.Storage.Blobs;
using E_PharmaHub.Repositories.AddressRepo;
using E_PharmaHub.Repositories.AppointmentRepo;
using E_PharmaHub.Repositories.CartRepo;
using E_PharmaHub.Repositories.ChatRepo;
using E_PharmaHub.Repositories.ClinicRepo;
using E_PharmaHub.Repositories.DoctorRepo;
using E_PharmaHub.Repositories.FavoriteMedicationRepo;
using E_PharmaHub.Repositories.FavouriteClinicRepo;
using E_PharmaHub.Repositories.FavouriteDoctorRepo;
using E_PharmaHub.Repositories.InventoryItemRepo;
using E_PharmaHub.Repositories.MedicineRepo;
using E_PharmaHub.Repositories.UserRepo;
using E_PharmaHub.Repositories.ReviewRepo;
using E_PharmaHub.Repositories.OrderRepo;
using E_PharmaHub.Repositories.PaymentRepo;
using E_PharmaHub.Repositories.PharmacistRepo;
using E_PharmaHub.Repositories.PharmacyRepo;
using E_PharmaHub.Repositories.MessageThreadRepo;
using E_PharmaHub.Repositories.PrescriptionRepo;
using E_PharmaHub.Services.AddressServ;
using E_PharmaHub.Services.AppointmentServ;
using E_PharmaHub.Services.CartServ;
using E_PharmaHub.Services.ChatServ;
using E_PharmaHub.Services.DoctorFavouriteServ;
using E_PharmaHub.Services.ClinicServ;
using E_PharmaHub.Services.DoctorServ;
using E_PharmaHub.Services.EmailSenderServ;
using E_PharmaHub.Services.FavoriteClinicServ;
using E_PharmaHub.Services.FavoriteMedicationServ;
using E_PharmaHub.Services.FileStorageServ;
using E_PharmaHub.Services.InventoryServ;
using E_PharmaHub.Services.PharmacyServ;
using E_PharmaHub.Services.MedicineServ;
using E_PharmaHub.Services.PaymentServ;
using E_PharmaHub.Services.OrderServ;
using E_PharmaHub.Services.ReviewServ;
using E_PharmaHub.Services.UserServ;
using E_PharmaHub.Services.PharmacistServ;
using E_PharmaHub.Services.StripePaymentServ;
using E_PharmaHub.Services.PrescriptionServ;
using E_PharmaHub.Models.Enums;
using Hangfire;
using E_PharmaHub.Services.NotificationServ;
using E_PharmaHub.Repositories.NotificationRepo;
using E_PharmaHub.Services.AppointmentNotificationScheduleServe;
using E_PharmaHub.Services.UserIdProviderServ;
using E_PharmaHub.Repositories.CartItemRepo;
using System;
using E_PharmaHub.Repositories.PrescriptionItemRepo;

namespace E_PharmaHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials() 
                        .SetIsOriginAllowed(_ => true); 
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
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        Console.WriteLine($"Path: {path}, Token: {accessToken}");

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs/notification"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
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


            builder.Services.AddSingleton(x =>
            {
                var connectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");
                return new BlobServiceClient(connectionString);
            });

            builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();

            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, Services.ReviewServ.ReviewService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            builder.Services.AddScoped<IPharmacyRepository, PharmacyRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();
            builder.Services.AddScoped<IBloodRequestService, BloodRequestService>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IPharmacyService, PharmacyService>();
            builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);
            builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
            builder.Services.AddScoped<IClinicService, ClinicService>();
            builder.Services.AddScoped<IDonorRepository, DonorRepository>();
            builder.Services.AddScoped<IDonorService, DonorService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IPharmacistRepository, PharmacistRepository>();
            builder.Services.AddScoped<IPharmacistService, PharmacistService>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
            builder.Services.AddScoped<IDonorMatchRepository, DonorMatchRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IFavoriteClinicService, FavoriteClinicService>();
            builder.Services.AddScoped<IFavoriteMedicationRepository, FavoriteMedicationRepository>();
            builder.Services.AddScoped<IFavoriteMedicationService, FavoriteMedicationService>();
            builder.Services.AddScoped<IFavouriteClinicRepository, FavouriteClinicRepository>();
            builder.Services.AddScoped<IFavouriteDoctorRepository, FavouriteDoctorRepository>();
            builder.Services.AddScoped<IDoctorFavouriteService, DoctorFavouriteService>();
            builder.Services.AddScoped<IMessageThreadRepository, MessageThreadRepository>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<IPrescriptionItemRepository, PrescriptionItemRepository>();
            builder.Services.AddScoped<IAppointmentNotificationScheduler, AppointmentNotificationScheduler>();
            builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);

            builder.Services.AddHangfireServer();
       

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<EHealthDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<AppUser, IdentityRole>(
                options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<EHealthDbContext>()
                .AddDefaultTokenProviders();




            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
              options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
              options.JsonSerializerOptions.WriteIndented = true;
            });
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();
            app.UseRouting();

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

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            var env = app.Environment;

            string? home = Environment.GetEnvironmentVariable("HOME");

            string rootPath;

            if (!string.IsNullOrEmpty(home))
            {
                rootPath = Path.Combine(home, "site", "wwwroot");
            }
            else
            {
                rootPath = env.WebRootPath;
            }

            var doctorsPath = Path.Combine(rootPath, "doctors");
            var medicinesPath = Path.Combine(rootPath, "medicines");
            var pharmaciesPath = Path.Combine(rootPath, "pharmacies");

            Directory.CreateDirectory(doctorsPath);
            Directory.CreateDirectory(medicinesPath);
            Directory.CreateDirectory(pharmaciesPath);

            app.UseStaticFiles(); 

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(doctorsPath),
                RequestPath = "/doctors"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(medicinesPath),
                RequestPath = "/medicines"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(pharmaciesPath),
                RequestPath = "/pharmacies"
            });


            app.UseWebSockets();
            app.MapHub<ChatHub>("/hubs/chat");
            app.Use(async (context, next) =>
            {
                if (context.User.Identity.IsAuthenticated)
                    Console.WriteLine("User Authenticated: " + context.User.Identity.Name);
                else
                    Console.WriteLine("User NOT Authenticated");
                await next();
            });

            app.MapHub<NotificationHub>("/hubs/notification");
            app.UseHangfireDashboard();
            //5221
            app.Run();

        }
    }
}

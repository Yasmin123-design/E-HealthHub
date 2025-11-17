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
            builder.Services.AddScoped<E_PharmaHub.Services.IReviewService, E_PharmaHub.Services.ReviewService>();
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

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.UseStaticFiles();

            app.UseWebSockets();
            app.MapHub<ChatHub>("/hubs/chat");

            app.Run();
        }
    }
}

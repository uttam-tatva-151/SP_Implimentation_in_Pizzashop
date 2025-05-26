using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSData;
using PMSData.Interfaces;
using PMSData.Reposetories;
using PMSData.Repositories;
using PMSServices.Interfaces;
using PMSServices.Services;

namespace PMSWebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            string? DatabaseConnectionString = configuration.GetConnectionString(Constants.DATABASE_DEFAULT_CONNECTION);
            // Configure file upload size
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB limit
            });

            // Bind JwtConfig and EmailSettings to strongly-typed classes
            services.Configure<JwtConfig>(configuration.GetSection(Constants.JWT_CONFIG));
            services.Configure<EmailSettings>(configuration.GetSection(Constants.EMAIL_CONFIG));

            // Register DbContext with connection string from appsettings.json
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(DatabaseConnectionString));

            NpgsqlDataSourceBuilder dataSourceBuilder = new(DatabaseConnectionString);
            dataSourceBuilder.RegisterAppComposites();
            NpgsqlDataSource dataSource = dataSourceBuilder.Build();

            services.AddSingleton<NpgsqlDataSource>(dataSource); // singleton — one shared instance for the whole app.

            // DAL (Data Access Layer)
            services.AddScoped<ICommonRepo, CommonRepo>();
            services.AddScoped<IAuthRepo, AuthRepo>();
            services.AddScoped<IRoleRepo, RoleRepo>();
            services.AddScoped<IItemRepo, ItemRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IModifierRepo, ModifierRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
            services.AddScoped<IItemModifierRepo, ItemModifierRepo>();
            services.AddScoped<ITableRepo, TableRepo>();
            services.AddScoped<ISectionRepo, SectionRepo>();
            services.AddScoped<ITaxesRepo, Taxesrepo>();
            services.AddScoped<IOrderRepo, OrderRepo>();
            services.AddScoped<IInvoiceRepo, InvoiceRepo>();
            services.AddScoped<IInvoiceItemMappingRepo, InvoiceItemMappingRepo>();
            services.AddScoped<IInvoiceTaxesMappingRepo, InvoiceTaxesMappingRepo>();
            services.AddScoped<ICustomerRepo, CustomerRepo>();
            services.AddScoped<IWaitingRepo, WaitingRepo>();
            services.AddScoped<IPaymentRepo, PaymentRepo>();
            services.AddScoped<IFeedbackRepo, FeedbackRepo>();
            services.AddScoped<IFavoriteItemRepo, FavoriteItemRepo>();


            // BLL (Business Logic Layer)
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICommonServices, CommonServices>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IItemModifierService, ItemModifierService>();
            services.AddScoped<ISectionAndTablesService, SectionAndTablesService>();
            services.AddScoped<ITaxesAndFeesService, TaxesAndFeesService>();
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderAppService, OrderAppService>();
            services.AddScoped<IOrderAppMenuService, OrderAppMenuService>();
            services.AddScoped<IKOTService, KOTService>();
            services.AddScoped<IWaitingListService, WaitingListService>();
            services.AddScoped<IDashboardService, DashboardService>();

            // Add Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }
    }
}

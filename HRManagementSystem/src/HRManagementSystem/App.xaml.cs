// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// App.xaml.cs - ملف التطبيق الرئيسي
// =====================================================

using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Data.Database;
using HRManagementSystem.Data.Repositories;
using HRManagementSystem.Services;
using HRManagementSystem.Services.Interfaces;
using HRManagementSystem.ViewModels;
using HRManagementSystem.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;
using System.Windows;

namespace HRManagementSystem;

/// <summary>
/// التطبيق الرئيسي
/// Main Application
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        // إعداد Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "hr-.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        // إعداد حقن التبعيات
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// تكوين الخدمات
    /// Configure services
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        // إنشاء مجلد البيانات إذا لم يكن موجوداً
        var dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        // قاعدة البيانات
        var dbPath = Path.Combine(dataFolder, "hr_database.db");
        services.AddSingleton<IDatabaseContext>(sp => new DatabaseContext(dbPath));
        services.AddSingleton<DatabaseInitializer>();

        // المستودعات
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // الخدمات
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IBackupService, BackupService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<EmployeesViewModel>();
        services.AddTransient<AttendanceViewModel>();
        services.AddTransient<LeavesViewModel>();
        services.AddTransient<PayrollViewModel>();
        services.AddTransient<DepartmentsViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Views
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
    }

    /// <summary>
    /// عند بدء التطبيق
    /// On application startup
    /// </summary>
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            Log.Information("بدء تشغيل نظام إدارة الموارد البشرية");

            // تطبيق الثيم المحفوظ
            ApplySavedTheme();

            // تهيئة قاعدة البيانات
            var dbInitializer = _serviceProvider.GetRequiredService<DatabaseInitializer>();
            await dbInitializer.InitializeAsync();

            // عرض نافذة تسجيل الدخول
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "خطأ فادح أثناء بدء التطبيق");
            MessageBox.Show($"خطأ أثناء بدء التطبيق: {ex.Message}", "خطأ",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    /// <summary>
    /// تطبيق الثيم المحفوظ
    /// Apply saved theme
    /// </summary>
    private void ApplySavedTheme()
    {
        try
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            var isDarkMode = HRManagementSystem.Properties.Settings.Default.IsDarkMode;
            theme.SetBaseTheme(isDarkMode ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "تعذر تطبيق الثيم المحفوظ");
        }
    }

    /// <summary>
    /// عند إغلاق التطبيق
    /// On application exit
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("إغلاق نظام إدارة الموارد البشرية");
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    /// <summary>
    /// الحصول على خدمة
    /// Get service
    /// </summary>
    public static T GetService<T>() where T : class
    {
        var app = Current as App;
        return app?._serviceProvider.GetRequiredService<T>()!;
    }
}


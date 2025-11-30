// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Program.cs - نقطة الدخول لتطبيق Electron.NET
// =====================================================

using ElectronNET.API;
using ElectronNET.API.Entities;
using HRManagementSystem.Core.Interfaces;
using HRManagementSystem.Data.Database;
using HRManagementSystem.Data.Repositories;
using HRManagementSystem.Services;
using HRManagementSystem.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// إعداد Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "hr-.log"),
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// إضافة Electron.NET
builder.WebHost.UseElectron(args);

// إضافة الخدمات
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// قاعدة البيانات
var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "hr_database.db");
builder.Services.AddSingleton<IDatabaseContext>(sp => new DatabaseContext(dbPath));
builder.Services.AddSingleton<DatabaseInitializer>();

// المستودعات
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// الخدمات
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// تهيئة قاعدة البيانات
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await dbInitializer.InitializeAsync();
}

// إعداد الـ Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// بدء Electron
if (HybridSupport.IsElectronActive)
{
    await CreateElectronWindow();
}

app.Run();

/// <summary>
/// إنشاء نافذة Electron
/// Create Electron window
/// </summary>
async Task CreateElectronWindow()
{
    var browserWindowOptions = new BrowserWindowOptions
    {
        Width = 1280,
        Height = 768,
        Show = false,
        Title = "نظام إدارة الموارد البشرية",
        WebPreferences = new WebPreferences
        {
            NodeIntegration = false,
            ContextIsolation = true
        }
    };

    var window = await Electron.WindowManager.CreateWindowAsync(browserWindowOptions);
    
    await window.WebContents.Session.ClearCacheAsync();
    
    window.OnReadyToShow += () => window.Show();
    window.OnClosed += () => Electron.App.Quit();

    // إعداد القائمة
    var menu = new MenuItem[]
    {
        new MenuItem
        {
            Label = "ملف",
            Submenu = new MenuItem[]
            {
                new MenuItem { Label = "خروج", Role = MenuRole.quit }
            }
        },
        new MenuItem
        {
            Label = "تحرير",
            Submenu = new MenuItem[]
            {
                new MenuItem { Label = "تراجع", Role = MenuRole.undo },
                new MenuItem { Label = "إعادة", Role = MenuRole.redo },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Label = "قص", Role = MenuRole.cut },
                new MenuItem { Label = "نسخ", Role = MenuRole.copy },
                new MenuItem { Label = "لصق", Role = MenuRole.paste }
            }
        },
        new MenuItem
        {
            Label = "عرض",
            Submenu = new MenuItem[]
            {
                new MenuItem { Label = "إعادة تحميل", Role = MenuRole.reload },
                new MenuItem { Label = "ملء الشاشة", Role = MenuRole.togglefullscreen },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Label = "أدوات المطور", Role = MenuRole.toggledevtools }
            }
        },
        new MenuItem
        {
            Label = "مساعدة",
            Submenu = new MenuItem[]
            {
                new MenuItem { Label = "حول البرنامج", Click = async () => 
                {
                    await Electron.Dialog.ShowMessageBoxAsync(window, new MessageBoxOptions("نظام إدارة الموارد البشرية\nالإصدار 1.0.0")
                    {
                        Title = "حول البرنامج",
                        Type = MessageBoxType.info
                    });
                }}
            }
        }
    };

    Electron.Menu.SetApplicationMenu(menu);
}


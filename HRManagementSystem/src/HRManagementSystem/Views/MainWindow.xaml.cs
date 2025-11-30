// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// MainWindow.xaml.cs - النافذة الرئيسية
// =====================================================

using HRManagementSystem.ViewModels;
using System.Windows;

namespace HRManagementSystem.Views;

/// <summary>
/// النافذة الرئيسية
/// Main Window
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // تحميل لوحة التحكم عند البدء
        Loaded += async (s, e) => await _viewModel.NavigateToDashboardCommand.ExecuteAsync(null);
    }
}


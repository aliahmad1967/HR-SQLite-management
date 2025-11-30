// =====================================================
// نظام إدارة الموارد البشرية - HR Management System
// Converters.cs - محولات القيم
// =====================================================

using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace HRManagementSystem.Converters;

/// <summary>
/// محول Boolean إلى Visibility
/// Boolean to Visibility Converter
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}

/// <summary>
/// محول Boolean معكوس إلى Visibility
/// Inverted Boolean to Visibility Converter
/// </summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility != Visibility.Visible;
        }
        return true;
    }
}

/// <summary>
/// محول Null إلى Boolean
/// Null to Boolean Converter
/// </summary>
public class NullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// محول Null إلى Visibility
/// Null to Visibility Converter
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// محول حالة الموظف إلى لون
/// Employee Status to Color Converter
/// </summary>
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status switch
            {
                "نشط" => "#4CAF50",
                "في إجازة" => "#FF9800",
                "منتهي الخدمة" => "#F44336",
                "معلق" => "#9E9E9E",
                _ => "#333333"
            };
        }
        return "#333333";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// محول العملة
/// Currency Converter
/// </summary>
public class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal amount)
        {
            return $"{amount:N0} د.ع";
        }
        return "0 د.ع";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// محول اسم الصورة إلى BitmapImage
/// Image Name to BitmapImage Converter
/// </summary>
public class ImagePathConverter : IValueConverter
{
    private static string ImagesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images");

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string imageName && !string.IsNullOrEmpty(imageName))
        {
            var fullPath = Path.Combine(ImagesFolder, imageName);
            if (File.Exists(fullPath))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                    bitmap.DecodePixelWidth = 50;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


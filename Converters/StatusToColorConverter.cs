using System.Globalization;

namespace IntegraCamp.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == "Свободен")
                return Color.FromArgb("#4CAF50"); // Зеленый
            else if (status == "Закреплен")
                return Color.FromArgb("#F44336"); // Красный
            return Color.FromArgb("#2196F3"); // Синий
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System.Globalization;

namespace PersonalFinanceApp
{
    public class AmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double amount)
            {
                return amount % 1 == 0
                    ? $"₽ {amount:N0}"
                    : $"₽ {amount:N2}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

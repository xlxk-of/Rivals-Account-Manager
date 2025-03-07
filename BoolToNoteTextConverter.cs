using System;
using System.Globalization;
using System.Windows.Data;

namespace RivalsAccountManager
{
    public class BoolToNoteTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool expanded = (bool)value;
            return expanded ? "Hide Note" : "Show Note";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;
using System.Collections;
using Behaviors.DragDrop.DataFormats;

namespace ControlTests.Converters
{
    public class DragEventArgsToPeopleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Behaviors.DragDrop.EventArgs.DragEventArgs args))
            {
                args = new Behaviors.DragDrop.EventArgs.DragEventArgs(value as System.Windows.DragEventArgs);
            }
            if (args.Data.GetDataPresent(LocalDataFormats.PersonViewModel))
            {
                return (IList)args.Data.GetData(LocalDataFormats.PersonViewModel);
            }
            return args.Data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

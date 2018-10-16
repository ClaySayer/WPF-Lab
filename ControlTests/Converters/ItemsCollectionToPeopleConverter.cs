using Behaviors.DragDrop.EventArgs;
using ControlTests.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Behaviors.DragDrop;
using System.Collections;
using Behaviors.DragDrop.DataFormats;

namespace ControlTests.Converters
{
    public class ItemsCollectionToPeopleConverter : IValueConverter
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Controls;assembly=Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class ImageButton : Button
    {
        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButton), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton), new PropertyMetadata(double.NaN));
        public ImageSource Image {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }
        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }
    }

    public class VisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

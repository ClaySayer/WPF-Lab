using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace DragandDrop
{
    public class DragContext: DependencyObject
    {
        public static readonly DependencyProperty DragVisualTemplateProperty = DependencyProperty.Register("DragVisualTemplate", typeof(DataTemplate), typeof(DragContext), new PropertyMetadata(GetDefaultDataTemplate()));
        public DataTemplate DragVisualTemplate
        {
            get { return (DataTemplate)GetValue(DragVisualTemplateProperty); }
            set { SetValue(DragVisualTemplateProperty, value); }
        }

        private DragVisual _dragVisual;
       
        public DragContext(UIElement sender)
        {
            if (!(sender is Panel panel)) return;
            if(panel.Background == null)
            {
                panel.Background = Brushes.Transparent;
            }
            panel.AllowDrop = true;
            panel.DragOver += new DragEventHandler(Context_DragOver);
            panel.PreviewDragEnter += new DragEventHandler(Context_PreviewDragEnter);
            DragManager.RegisterDragContext(panel, Context_DropCompleted);
        }

        private void Context_DropCompleted(object sender, DropCompletedEventArgs e)
        {
            if (_dragVisual != null)
            {
                _dragVisual.Destroy();
                _dragVisual = null;
            }
        }

        private static DataTemplate GetDefaultDataTemplate()
        {
            XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XElement xDataTemplate =
                new XElement(ns + "DataTemplate",
                    new XAttribute(XNamespace.Xmlns + "x", "http://schemas.microsoft.com/winfx/2006/xaml"),
                    new XElement(ns + "Border", new XAttribute("Background", "White"),
                        new XAttribute("BorderBrush", Brushes.DarkGray),
                        new XAttribute("BorderThickness", 1),
                        new XAttribute("Padding", "5,0,5,0"),
                    new XElement(ns + "TextBlock", new XAttribute("Text", "{Binding Path=Content}"),
                        new XAttribute("Foreground", Brushes.DarkGray))));
            StringReader sr = new StringReader(xDataTemplate.ToString());
            XmlReader xr = XmlReader.Create(sr);
            return XamlReader.Load(xr) as DataTemplate;
        }

        private void Context_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (_dragVisual == null)
            {
                Panel source = sender as Panel;
                Point dragStart = DragManager.DragStartPosition;
                Control dragItem = GetDragItemControl(source, dragStart);
                Point itemPos = dragItem.TranslatePoint(new Point(), source);
                UIElement dragVisual = CloneElement(dragItem);
                Point position = e.GetPosition(source);
                Point offset = new Point(position.X - itemPos.X, position.Y - itemPos.Y);
                Size size = new Size(dragItem.ActualWidth, dragItem.ActualHeight);
                _dragVisual = new DragVisual(e.Data.GetData(DragDataManager.DataFormat), source, DragVisualTemplate, offset, size, dragVisual);
            }
        }

        private UIElement CloneElement(UIElement element)
        {
            string shapestring = XamlWriter.Save(element);
            StringReader stringReader = new StringReader(shapestring);
            XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
            UIElement DeepCopyobject = (UIElement)XamlReader.Load(xmlTextReader);
            return DeepCopyobject;
        }

        private Control GetDragItemControl(Panel container, Point point)
        {
            HitTestResult result = VisualTreeHelper.HitTest(container, point);
            if (result != null)
            {
                DependencyObject element = result.VisualHit as DependencyObject;
                while (element != null)
               {
                    if(element is Control)
                    {
                        return element as Control;
                    }
                    element = VisualTreeHelper.GetParent(element);
               }
            }
            return null;
        }

        private void Context_DragOver(object sender, DragEventArgs e)
        {
            Panel source = sender as Panel;
            UIElement DragItem = e.Source as UIElement;
            Point position = e.GetPosition(source);
            if (_dragVisual != null)
            {
                _dragVisual.UpdatePosition(position.X, position.Y);
            }
            e.Handled = true;
        }
    }
}

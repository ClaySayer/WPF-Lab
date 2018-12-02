using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace DragDrop
{
    public class DragContextBehavior : Behavior<Panel>
    {
        #region Dependency Properties

        public static readonly DependencyProperty DragVisualTemplateProperty = DependencyProperty.Register("DragVisualTemplate", typeof(DataTemplate), typeof(DragContextBehavior), null);
        public DataTemplate DragVisualTemplate
        {
            get { return (DataTemplate)GetValue(DragVisualTemplateProperty); }
            set { SetValue(DragVisualTemplateProperty, value); }
        }

        #endregion

        #region Private Properties

        private DragVisual _dragVisual;

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is Panel panel)) return;
            if(panel.Background == null)
            {
                panel.Background = Brushes.Transparent;
            }
            panel.AllowDrop = true;
            panel.DragEnter += new DragEventHandler(AssociateObject_DragEnter);
            panel.DragOver += new DragEventHandler(AssociatedObject_DragOver);
            DragManager.RegisterDragContext(panel, AssociatedObject_DropCompleted);
        }

        private void AssociateObject_DragEnter(object sender, DragEventArgs e)
        {
            if (_dragVisual == null)
            {
                Panel source = sender as Panel;
                //FrameworkElement dragVisual = null;
                Point dragStart = DragManager.DragStartPosition;
                Control dragItem = GetDragItemControl(source, dragStart);
                Point itemPos = dragItem.TranslatePoint(new Point(), source);
                if(DragVisualTemplate == null)
                {
                    DragVisualTemplate = GetDefaultDataTemplate();
                }
                Point position = e.GetPosition(source);
                Point offset = new Point(dragStart.X - itemPos.X, dragStart.Y - itemPos.Y);
                Size size = new Size(dragItem.ActualWidth, dragItem.ActualHeight);
                IList data = DragDataManager.GetData(e.Data);
                _dragVisual = new DragVisual(data, source, DragVisualTemplate, offset, size);
                //if (dragItem.Parent is ItemsControl)
                //{
                //    ItemsControl parent = dragItem.Parent as ItemsControl;
                //    DataTemplate template = parent.ItemTemplate;
                //    if (template == null)
                //    {
                //        DragVisualTemplate = GetDefaultDataTemplate();
                //    }
                //    else
                //    {
                //        DragVisualTemplate = template;
                //    }
                //    Point position = e.GetPosition(source);
                //    Point offset = new Point(position.X - itemPos.X, position.Y - itemPos.Y);
                //    Size size = new Size(dragItem.ActualWidth, dragItem.ActualHeight);
                //    _dragVisual = new DragVisual(e.Data.GetData(DragDataManager.DataFormat) as IList, source, DragVisualTemplate, offset, size);
                //}
                //else
                //{
                //    dragVisual = CloneElement(dragItem);
                //    Point position = e.GetPosition(source);
                //    Point offset = new Point(position.X - itemPos.X, position.Y - itemPos.Y);
                //    Size size = new Size(dragItem.ActualWidth, dragItem.ActualHeight);
                //    _dragVisual = new DragVisual(e.Data.GetData(DragDataManager.DataFormat), source, offset, size, dragVisual);
                //}
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

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            Panel source = sender as Panel;
            FrameworkElement DragItem = e.Source as FrameworkElement;
            Point position = e.GetPosition(source);
            if (_dragVisual != null)
            {
                _dragVisual.UpdatePosition(position.X, position.Y);
            }
            e.Handled = true;
        }

        private void AssociatedObject_DropCompleted(object sender, DropCompletedEventArgs e)
        {
            if (_dragVisual != null)
            {
                _dragVisual.Destroy();
                _dragVisual = null;
            }
        }

        private Control GetDragItemControl(Panel container, Point point)
        {
            HitTestResult result = VisualTreeHelper.HitTest(container, point);
            if (result != null)
            {
                DependencyObject element = result.VisualHit as DependencyObject;
                while (element != null)
                {
                    if (element is Control)
                    {
                        return element as Control;
                    }
                    element = VisualTreeHelper.GetParent(element);
                }
            }
            return null;
        }

        private FrameworkElement CloneElement(UIElement element)
        {
            string shapestring = XamlWriter.Save(element);
            StringReader stringReader = new StringReader(shapestring);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            FrameworkElement DeepCopyobject = (FrameworkElement)XamlReader.Load(xmlReader);
            return DeepCopyobject;
        }
    }
}


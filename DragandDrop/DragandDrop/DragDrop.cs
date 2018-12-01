using System;
using System.Windows;
using System.Windows.Controls;

namespace DragandDrop
{
    public static class DragDrop
    {
        public static readonly RoutedEvent DropCompletedEvent =
            EventManager.RegisterRoutedEvent("DropCompleted",
                RoutingStrategy.Bubble,
                typeof(DropCompletedEventHandler),
                typeof(DragDrop));

        public static void AddDropCompletedHandler(DependencyObject o, DropCompletedEventHandler handler)
        {
            ((UIElement)o).AddHandler(DropCompletedEvent, handler);
        }

        public static void RemoveDropCompletedHandler(DependencyObject o, DropCompletedEventHandler handler)
        {
            ((UIElement)o).RemoveHandler(DropCompletedEvent, handler);
        }

        public static readonly DependencyProperty DragSourceProperty = DependencyProperty.RegisterAttached
            ("DragSource", typeof(Type), typeof(DragDrop), new PropertyMetadata(null, DragSourceChanged, CoerceValue));

        public static Type GetIsDragSource(DependencyObject sender)
        {
            return (Type)sender.GetValue(DragSourceProperty);
        }
        public static void SetDragSource(DependencyObject sender, Type value)
        {
            sender.SetValue(DragSourceProperty, value);
        }


        private static Type CoerceValue( DependencyObject sender, object data)
        {
            if (data == null)
            {
                return typeof(DefaultDragSource);
            }
            return data as Type;
        }

        private static void DragSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement source = sender as UIElement;
            if (e.NewValue == null)
            {
                DefaultDragSource dragSource = new DefaultDragSource(source);
            }
            else
            {
                Type type = e.NewValue as Type;
                Activator.CreateInstance(type, new object[] { source });
            }
        }
        
        public static readonly DependencyProperty IsDragContextProperty = DependencyProperty.RegisterAttached
            ("IsDragContext", typeof(bool), typeof(DragContext), new PropertyMetadata(false, IsDragContextChanged));

        public static bool GetIsDragContext(DependencyObject sender)
        {
            return (bool)sender.GetValue(IsDragContextProperty);
        }

        public static void SetIsDragContext(DependencyObject sender, bool value)
        {
            sender.SetValue(IsDragContextProperty, value);
        }

        public static void IsDragContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Panel panel)) return;
            DragContext context = new DragContext(sender as UIElement);
        }
        
        public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.RegisterAttached
            ("ItemType", typeof(Type), typeof(DragDrop), new PropertyMetadata(typeof(string)));

        public static Type GetItemType(DependencyObject sender)
        {
            return (Type)sender.GetValue(ItemTypeProperty);
        }
        public static void SetItemType(DependencyObject sender, Type value)
        {
            sender.SetValue(ItemTypeProperty, value);
        }
    }
}

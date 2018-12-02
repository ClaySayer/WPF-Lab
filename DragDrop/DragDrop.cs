using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DragDrop
{
    class DragDrop
    {
        #region DragDrop Public Events
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
        #endregion
    }
}

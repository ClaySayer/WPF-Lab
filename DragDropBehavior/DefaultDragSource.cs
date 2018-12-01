using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DragDrop
{
    public class DefaultDragSource : DragSourceBehavior<FrameworkElement>
    {
        protected override void OnChanged()
        {
            DragManager.RegisterDragSource(AssociatedObject, OnDragInitialize, OnDropComplete, null, null, null);
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            Control source = sender as Control;
            Panel context = DragManager.DragContext;
            Point dragStart = DragManager.DragStartPosition;
            e.Effects = DragDropEffects.All;
            Point mousePos = e.GetPosition(context);
            Point itemPos = source.TranslatePoint(new Point(), source);
            Point offset = new Point(mousePos.X - itemPos.X, mousePos.Y - itemPos.Y);
            e.DragVisualSize = new Size(source.ActualWidth, source.ActualHeight);
            e.DragVisualOffset = offset;
            e.Data = DragDataManager.GenerateDataObject(source);
        }

        private void OnDropComplete(object sender, DropCompletedEventArgs e)
        {

        }

        private Control GetDragItemControl(UIElement container, Point point)
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
    }
}

using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Input;

namespace DragandDrop
{
    public class DragSourceBehavior: Behavior<UIElement>
    {
        private static bool _isMouseDown = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Source_PreviewMouseLeftButtonDown);
            AssociatedObject.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Source__PreviewMouseLeftButtonUp);
            AssociatedObject.PreviewMouseMove += new MouseEventHandler(Source_PreviewMouseMove);
            
        }

        private void Source_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement source = sender as UIElement;
            Point p = e.GetPosition(source);
            DragManager.InitializeDrag(p);
            _isMouseDown = true;
        }

        private void OnDropComplete(object sender, DropCompletedEventArgs e)
        {
            _isMouseDown = false;
        }

        private void Source_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                UIElement source = sender as UIElement;
                Point p = e.GetPosition(source);
                DragManager.InitiateDrag(source, p);
            }
        }

        private void Source__PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            DragManager.FinalizeDrag();
        }
    }
}

using System.Windows;
using System.Windows.Input;

namespace DragandDrop
{
    public class DragSource: DependencyObject
    {
        private bool _isMouseDown;
        private readonly UIElement _source;

        protected DragSource(UIElement source)
        {
            _isMouseDown = false;
            _source = source;
            source.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Source_PreviewMouseLeftButtonDown);
            source.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Source__PreviewMouseLeftButtonUp);
            source.PreviewMouseMove += new MouseEventHandler(Source_PreviewMouseMove);
            DragDrop.AddDropCompletedHandler(source, OnDropComplete);
        }

        private void Source_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement source = sender as UIElement;
            Point p = e.GetPosition(source);
            DragManager.InitializeDrag(source, p);
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
                FrameworkElement source = sender as FrameworkElement;
                Point p = e.GetPosition(source);
                DragManager.InitiateDrag(source, p);
            }
        }

        private void Source__PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement source = sender as FrameworkElement;
            _isMouseDown = false;
            DragManager.FinalizeDrag();
        }
    }
}

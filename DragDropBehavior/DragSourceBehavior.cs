using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DragDrop
{
    public class DragSourceBehavior<T>: Behavior<T> where T: FrameworkElement
    {
        #region Private Member Variables
        private bool _isMouseDown;
        private FrameworkElement _associatedObject;
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _isMouseDown = false;
            _associatedObject = sender as FrameworkElement;
            _associatedObject.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(AssociatedObject_PreviewMouseLeftButtonDown);
            _associatedObject.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(AssociatedObject_PreviewMouseLeftButtonUp);
            _associatedObject.PreviewMouseMove += new MouseEventHandler(AssociatedObject_PreviewMouseMove);
            DragDrop.AddDropCompletedHandler(_associatedObject, AssociatedObject_OnDropComplete);
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(_associatedObject);
            DragManager.InitializeDrag(_associatedObject, position);
            _isMouseDown = true;
        }

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                Point position = e.GetPosition(_associatedObject);
                DragManager.InitiateDrag(_associatedObject, position);
            }
        }

        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            DragManager.FinalizeDrag();
        }

        private void AssociatedObject_OnDropComplete(object sender, DropCompletedEventArgs e)
        {
            _isMouseDown = false;
        }
    }
}

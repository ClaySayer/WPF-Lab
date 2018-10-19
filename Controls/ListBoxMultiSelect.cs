using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Controls
{
    public class ListBoxMultiSelect : System.Windows.Controls.ListBox
    {
        public static readonly RoutedEvent DropCompletedEvent = EventManager.RegisterRoutedEvent("Fred", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
        public event RoutedEventHandler DropCompleted
        {
            add { AddHandler(DropCompletedEvent, value); }
            remove { RemoveHandler(DropCompletedEvent, value); }
        }
        public ListBoxMultiSelect()
        {
            SelectionMode = SelectionMode.Extended;
        }
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new ListBoxMultiSelectItem();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin)
            {
                if (this.SelectedItems != null)
                {
                    this.SelectedItems.Clear();
                }
            }
            base.OnLostKeyboardFocus(e);
        }
    }

    public class ListBoxMultiSelectItem : ListBoxItem
    {
        private bool _deferSelection = false;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1 && IsSelected)
            {
                // the user may start a drag by clicking into selected items
                // delay destroying the selection to the Up event
                _deferSelection = true;
                e.Handled = true;
            }
            else
            {
                base.OnMouseLeftButtonDown(e);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_deferSelection)
            {
                try
                {
                    base.OnMouseLeftButtonDown(e);
                }
                finally
                {
                    _deferSelection = false;
                }
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // abort deferred Down
            _deferSelection = false;
            base.OnMouseLeave(e);
        }
    }
}

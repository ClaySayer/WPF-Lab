using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using Behaviors.DragDrop;
using System.Collections;
using System.Collections.ObjectModel;
using Behaviors.DragDrop.DataFormats;

namespace Behaviors.ListBoxDragDropBehavior
{
    public class ListBoxDragDropBehavior: Behavior<ListBox>
    {
        #region Dependency Properties
        public static readonly DependencyProperty DragVisualProperty = DependencyProperty.Register("DragVisual", typeof(DataTemplate), typeof(ListBoxDragDropBehavior), null);
        #endregion

        #region Properties
        public DataTemplate DragVisual
        {
            get { return (DataTemplate)GetValue(DragVisualProperty); }
            set { SetValue(DragVisualProperty, value); }
        }
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            DragDropManager.AddDragSource(AssociatedObject, OnDragInitialize, OnGiveFeedback, null, OnDropCompleted);
            DragDropManager.AddDropTarget(AssociatedObject, null);
        }

        private void OnDragInitialize(object sender, DragDrop.EventArgs.DragEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            e.Effects = DragDropEffects.All;
            var data = GetItems(listBox);
            if (data.Count > 0)
            {
                UIElement item = GetItemContainerFromPoint(listBox, e.StartPoint);
                Point mousePosition = e.GetPosition(listBox);
                Point itemPosition = item.TranslatePoint(new Point(), listBox);
                Point offset = new Point(mousePosition.X - itemPosition.X, mousePosition.Y - itemPosition.Y);
                e.DragOffset = offset;
                string itemType = listBox.SelectedItems[0].GetType().Name;
                e.Data = new DataObject(LocalDataFormats.GetDataFormat(itemType).Name, data);
                e.DragVisual = CreateDragVisual(data);
                e.DragElement = item;
            }
        }

        public static UIElement GetItemContainerFromPoint(ListBox ListBox, Point p)
        {
            UIElement element = ListBox.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == ListBox)
                    return null;

                object data = ListBox.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                {
                    return element;
                }
                else
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }
            }
            return null;
        }

        private DataTemplate CreateDragVisual(IList items)
        {
            DataTemplate dragVisualTemplate = SelectDragVisualTemplate();
            switch (items.Count)
            {
                case 1: return dragVisualTemplate;
                case 2: return dragVisualTemplate;
                default: return dragVisualTemplate;

            }
        }

        private DataTemplate SelectDragVisualTemplate()
        {
            if(DragVisual != null)
            {
                return DragVisual;
            }
            if(AssociatedObject.ItemTemplate != null)
            {
                return AssociatedObject.ItemTemplate;
            }
            return null;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            DragDropManager.RemoveDragSource(AssociatedObject);
        }

        private void OnDropCompleted(object sender, DragDrop.EventArgs.DragEventArgs e)
        {
            UIElement element = sender as UIElement;
            //e.RoutedEvent = Controls.ListBoxMultiSelect.DropCompletedEvent;
            //element.RaiseEvent(e);
        }

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

            if (e.Effects == DragDropEffects.None)
            {
                Mouse.SetCursor(Cursors.No);
            }
            else
            {
                Mouse.SetCursor(Cursors.None);
            }
            e.Handled = true;
        }

        private ListBox GetSourceListBox(UIElement originalSource)
        {
            while (originalSource != null)
            {
                if (originalSource is ListBox)
                    return (ListBox)originalSource;

                originalSource = VisualTreeHelper.GetParent(originalSource) as UIElement;
            }
            return null;
        }

        private DataTemplate SelectDataTemplate()
        {
            if (DragVisual != null)
            {
                return DragVisual;
            }
            if (AssociatedObject.ItemTemplate != null)
            {
                return AssociatedObject.ItemTemplate;
            }
            return null;
        }
        private IList GetItems(ListBox source)
        {
            if (source.SelectionMode != SelectionMode.Single)
            {
                var items = source.SelectedItems;
                return (IList)items;

            }
            ObservableCollection<object> list = new ObservableCollection<object>
            {
                source.SelectedItem
            };
            return (IList)list;
        }
    }
}
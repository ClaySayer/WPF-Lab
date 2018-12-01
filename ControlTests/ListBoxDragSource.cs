using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DragDrop;

namespace ControlTests
{
    public class ListBoxDragSource : DragDrop.DragSourceBehavior<ListBox>
    {
        private DragVisual _dragVisual;
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.SelectionMode = SelectionMode.Extended;
            //AssociatedObject.PreviewDragEnter += new DragEventHandler(AssociateObject_PreviewDragEnter);
            //AssociatedObject.PreviewDragOver += new DragEventHandler(AssociatedObject_PreviewDragOver);
            //AssociatedObject.PreviewDragLeave += new DragEventHandler(AssociatedObject_PreviewDragLeave);
        }

        private void AssociatedObject_PreviewDragLeave(object sender, DragEventArgs e)
        {
            _dragVisual.Destroy();
            _dragVisual = null;
        }

        private void AssociatedObject_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (AssociatedObject.ItemTemplate != null)
            {
                FrameworkElement source = sender as FrameworkElement;
                //Point dragStart = DragManager.DragStartPosition;
                //FrameworkElement dragItem = GetItemContainerFromPoint(AssociatedObject, dragStart) as FrameworkElement;
                Point position = e.GetPosition(source);
                if (_dragVisual != null)
                {
                    _dragVisual.UpdatePosition(position.X, position.Y);
                }
                e.Handled = true;
            }
        }

        private void AssociateObject_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (AssociatedObject.ItemTemplate != null)
            {
                if (_dragVisual == null)
                {
                    FrameworkElement source = sender as FrameworkElement;
                    Point dragStart = DragManager.DragStartPosition;
                    FrameworkElement dragItem = GetItemContainerFromPoint(AssociatedObject, dragStart) as FrameworkElement;
                    Point itemPos = dragItem.TranslatePoint(new Point(), source);
                    Point position = e.GetPosition(source);
                    Point offset = new Point(dragStart.X - itemPos.X, dragStart.Y - itemPos.Y);
                    Size size = new Size(dragItem.ActualWidth, dragItem.ActualHeight);
                    IList data = DragDataManager.GetData(e.Data);
                    _dragVisual = new DragVisual(data, source, AssociatedObject.ItemTemplate, offset, size);
                }
                e.Handled = true;
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

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            DragManager.RegisterDragSource(
                AssociatedObject,
                AssociatedObject_OnDragInitialize,
                AssociatedObject_OnDropCompleted,
                AssociateObject_PreviewDragEnter,
                AssociatedObject_PreviewDragOver,
                AssociatedObject_PreviewDragLeave);
        }

        private void AssociatedObject_OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            IList selectedItems = GetSelectedItemsAsList(AssociatedObject);
            if (selectedItems.Count > 0)
            {
                if (GetItemContainerFromPoint(AssociatedObject, e.DragStartPosition) is FrameworkElement item)
                {
                    Point mousePos = e.GetPosition(AssociatedObject);
                    Point itemPos = item.TranslatePoint(new Point(), AssociatedObject);
                    Point offset = new Point(mousePos.X - itemPos.X, mousePos.Y - itemPos.Y);
                    e.DragVisualSize = new Size(item.ActualWidth, item.ActualHeight);
                    e.DragVisualOffset = offset;
                    e.SourceElement = AssociatedObject;
                    e.Data = DragDataManager.GenerateDataObject(selectedItems);
                }
                else
                {
                    AssociatedObject.SelectedItems.Clear();
                }
            }
        }

        private void AssociatedObject_OnDropCompleted(object sender, DropCompletedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            listBox.SelectedItems.Clear();
        }

        private UIElement GetItemContainerFromPoint(ListBox listBox, Point point)
        {
            UIElement element = listBox.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == listBox)
                    return null;
                object item = listBox.ItemContainerGenerator.ItemFromContainer(element);
                if (item != DependencyProperty.UnsetValue)
                {
                    return element;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        //Convert all Drag Data to a List removes any concerns about selection mode
        private IList GetSelectedItemsAsList(ListBox listBox)
        {
            if (listBox.SelectionMode != SelectionMode.Single)
            {
                var items = listBox.SelectedItems;
                return (IList)items;
            }
            List<object> list = new List<object>
            {
                listBox.SelectedItem
            };
            return list as IList;
        }
    }
}

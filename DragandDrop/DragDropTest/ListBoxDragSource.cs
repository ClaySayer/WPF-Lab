using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DragandDrop;

namespace DragDropTest
{
    public class ListBoxDragSource: DragSource
    {
        public ListBoxDragSource(UIElement source):base(source)
        {
            DragandDrop.DragManager.RegisterDragSource(source, OnDragInitialize, OnDropComplete);
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            e.Effects = DragDropEffects.All;
            IList selectedItems = GetSelectedItemsAsList(listBox);
            if(selectedItems.Count > 0)
            {
                UIElement item = GetItemContainerFromPoint(listBox, e.DragStartPosition);
                if (item != null)
                {
                    Point mousePos = e.GetPosition(listBox);
                    Point itemPos = item.TranslatePoint(new Point(), listBox);
                    Point offset = new Point(mousePos.X - itemPos.X, mousePos.Y - itemPos.Y);
                    e.DragVisualOffset = offset;
                    e.Data = DragDataManager.GenerateDataObject(selectedItems);
                }
                else
                {
                    listBox.SelectedItems.Clear();
                }
            }
        }

        private void OnDropComplete(object sender, DropCompletedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            listBox.SelectedItems.Clear();
        }
        
        private UIElement GetItemContainerFromPoint(ListBox listBox, Point point)
        {
            UIElement element = listBox.InputHitTest(point) as UIElement;
            while(element != null)
            {
                if(element == listBox)
                    return null;
                object item = listBox.ItemContainerGenerator.ItemFromContainer(element);
                if(item != DependencyProperty.UnsetValue)
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
            if(listBox.SelectionMode != SelectionMode.Single)
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

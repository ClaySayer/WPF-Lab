using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;


namespace ListBoxDragDropBehavior.Library
{
    public static class Helper
    {
        public static bool DoesItemExists(ListBox ListBox, object item)
        {
            if (ListBox.Items.Count > 0)
            {
                return ListBox.Items.Contains(item);
            }
            return false;
        }

        public static void AddItem(ListBox ListBox, object item, int insertIndex)
        {
            if (ListBox.ItemsSource != null)
            {
                IList iList = ListBox.ItemsSource as IList;
                if (iList != null)
                {
                    iList.Insert(insertIndex, item);
                }
                else
                {
                    Type type = ListBox.ItemsSource.GetType();
                    Type genericList = type.GetInterface("IList`1");
                    if (genericList != null)
                    {
                        type.GetMethod("Insert").Invoke(ListBox.ItemsSource, new object[] { insertIndex, item });
                    }
                }
            }
            else
            {
                ListBox.Items.Insert(insertIndex, item);
            }
        }

        public static void RemoveItem(ListBox ListBox, object itemToRemove)
        {
            if (itemToRemove != null)
            {
                int index = ListBox.Items.IndexOf(itemToRemove);
                if (index != -1)
                {
                    RemoveItem(ListBox, index);
                }
            }
        }

        public static void RemoveItem(ListBox ListBox, int removeIndex)
        {
            if (removeIndex != -1 && removeIndex < ListBox.Items.Count)
            {
                if (ListBox.ItemsSource != null)
                {
                    IList iList = ListBox.ItemsSource as IList;
                    if (iList != null)
                    {
                        iList.RemoveAt(removeIndex);
                    }
                    else
                    {
                        Type type = ListBox.ItemsSource.GetType();
                        Type genericList = type.GetInterface("IList`1");
                        if (genericList != null)
                        {
                            type.GetMethod("RemoveAt").Invoke(ListBox.ItemsSource, new object[] { removeIndex });
                        }
                    }
                }
                else
                {
                    ListBox.Items.RemoveAt(removeIndex);
                }
            }
        }

        public static object GetDataObjectFromListBox(ListBox ListBox, Point p)
        {
            UIElement element = ListBox.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == ListBox)
                    return null;

                object data = ListBox.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
                else
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }
            }
            return null;
        }

        public static UIElement GetItemContainerFromPoint(ListBox listBox, Point p)
        {
            UIElement element = listBox.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == listBox)
                    return null;

                object data = listBox.ItemContainerGenerator.ItemFromContainer(element);
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

        public static UIElement GetItemContainerFromListBox(ListBox ListBox)
        {
            UIElement container = null;
            if (ListBox != null && ListBox.Items.Count > 0)
            {
                container = ListBox.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
            }
            else
            {
                container = ListBox;
            }
            return container;
        }

        public static bool IsPointInTopHalf(ListBox ListBox, DragEventArgs e)
        {
            UIElement selectedItemContainer = GetItemContainerFromPoint(ListBox, e.GetPosition(ListBox));
            Point relativePosition = e.GetPosition(selectedItemContainer);
            if (IsItemControlOrientationHorizontal(ListBox))
            {
                return relativePosition.Y < ((FrameworkElement)selectedItemContainer).ActualHeight / 2;
            }
            return relativePosition.X < ((FrameworkElement)selectedItemContainer).ActualWidth / 2;
        }

        public static bool IsItemControlOrientationHorizontal(ListBox listBox)
        {
            return true;
        }

        public static bool? IsMousePointerAtTop(ListBox ListBox, Point pt)
        {
            if (pt.Y > 0.0 && pt.Y < 25)
            {
                return true;
            }
            else if (pt.Y > ListBox.ActualHeight - 25 && pt.Y < ListBox.ActualHeight)
            {
                return false;
            }
            return null;
        }

        public static ScrollViewer FindScrollViewer(ListBox ListBox)
        {
            UIElement ele = ListBox;
            while (ele != null)
            {
                if (VisualTreeHelper.GetChildrenCount(ele) == 0)
                {
                    ele = null;
                }
                else
                {
                    ele = VisualTreeHelper.GetChild(ele, 0) as UIElement;
                    if (ele != null && ele is ScrollViewer)
                        return ele as ScrollViewer;
                }
            }
            return null;
        }


        public static double ScrollOffsetUp(double verticaloffset, double offset)
        {
            return verticaloffset - offset < 0.0 ? 0.0 : verticaloffset - offset;
        }
    }
}

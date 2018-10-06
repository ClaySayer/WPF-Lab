using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Behaviors.ListBoxDragDropBehavior
{
    public class ListBoxDragDropBehavior: EventToCommandBehavior<ListBox>
    {
        #region Properties

        public Type ItemType { get; set; }

        public DataTemplate DataTemplate { get; set; }

        #endregion

        private bool _isMouseDown;
        private const int DRAG_WAIT_COUNTER_LIMIT = 10;
        private DragManager _dragManager;

        public ListBoxDragDropBehavior(): base()
        {
            _isMouseDown = false;
            _dragManager = new DragManager();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ListBox_PreviewMouseLeftButtonDown);
            AssociatedObject.PreviewMouseMove += new MouseEventHandler(ListBox_PreviewMouseMove);
            AssociatedObject.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ListBox_PreviewMouseLeftButtonUp);
            AssociatedObject.PreviewDrop += new DragEventHandler(ListBox_PreviewDrop);
            AssociatedObject.PreviewQueryContinueDrag += new QueryContinueDragEventHandler(ListBox_PreviewQueryContinueDrag);
            AssociatedObject.PreviewDragEnter += new DragEventHandler(ListBox_PreviewDragEnter);
            AssociatedObject.PreviewDragOver += new DragEventHandler(ListBox_PreviewDragOver);
            AssociatedObject.DragLeave += new DragEventHandler(ListBox_DragLeave);
            AssociatedObject.GiveFeedback += new GiveFeedbackEventHandler(ListBox_GiveFeedback);
            _dragManager.SetDataTemplate(DataTemplate);
            Converter = new ListBoxDragDropConverter();
            ConverterParameter = ItemType;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.AllowDrop = false;
            AssociatedObject.PreviewMouseLeftButtonDown -= ListBox_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= ListBox_PreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp -= ListBox_PreviewMouseLeftButtonUp;
            AssociatedObject.PreviewDrop -= ListBox_PreviewDrop;
            AssociatedObject.PreviewQueryContinueDrag -= ListBox_PreviewQueryContinueDrag;
            AssociatedObject.PreviewDragEnter -= ListBox_PreviewDragEnter;
            AssociatedObject.PreviewDragOver -= ListBox_PreviewDragOver;
            AssociatedObject.DragLeave -= ListBox_DragLeave;
            AssociatedObject.GiveFeedback -= ListBox_GiveFeedback;
            _dragManager = null;
        }

        #region Button Events

        private void ListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragManager.UpdateSelector();
            _isMouseDown = false;
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                ListBox dragSource = (ListBox)sender;
                Point point = e.GetPosition(dragSource);
                if (_dragManager.InitiateDrag(dragSource, point))
                {
                    _isMouseDown = false;
                }
            }
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            Point p = e.GetPosition(listBox);
            var item = Helper.GetDataObjectFromListBox(listBox, p);
            if (_dragManager.InitializeDrag(p, item))
            {
                _isMouseDown = true;
            }
        }

        #endregion

        #region Drag Events

        private void ListBox_DragLeave(object sender, DragEventArgs e)
        {
            _dragManager.CleanDragFeedback();

            e.Handled = true;
        }

        private void ListBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            UIElement dragItem = (UIElement)sender;
            Point position = e.GetPosition(dragItem);
            _dragManager.UpdateDragFeedBack(position);
            e.Handled = true;
        }

        private void ListBox_GiveFeedback(object sender, System.Windows.GiveFeedbackEventArgs e)
        {
            Mouse.SetCursor(Cursors.Hand);
            e.Handled = true;
        }

        private void ListBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            UIElement dragItem = (UIElement)sender;
            var data = e.Data.GetData(e.Data.GetFormats()[0]);
            Point position = e.GetPosition(dragItem);
            _dragManager.InitializeDragFeedback(dragItem, data, position);
            e.Handled = true;
        }

        private void ListBox_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                _dragManager.CancelDrag(e);
                _isMouseDown = false;
                e.Handled = true;
            }
        }

        private void ListBox_PreviewDrop(object sender, DragEventArgs e)
        {
            _dragManager.Drop();
            UIElement originalSource = (UIElement)e.OriginalSource;
            ListBox source = GetSourceListBox(originalSource);
            source.SelectedItems.Clear();
            _isMouseDown = false;
            //ListBox listBox = (ListBox)sender;
            //DetachAdorners();
            //e.Handled = true;
            //var data = e.Data.GetData(e.Data.GetFormats()[0]);
            //if(data is IList)
            //{
            //    IList items = data as IList;
            //    foreach(var item in items)
            //    {
            //        if(item.GetType() == ItemType)
            //        {
            //            if (!listBox.Items.Contains(item)) {
            //                listBox.Items.Add(item);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    e.Effects = DragDropEffects.None;
            //}
            //ResetState();
            //_data = null;
            //listBox.SelectedItems.Clear();
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
        #endregion
    }
}

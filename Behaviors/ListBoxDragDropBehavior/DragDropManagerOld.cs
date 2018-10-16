using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Behaviors.DragDrop;

namespace Behaviors.ListBoxDragDropBehavior
{
    internal class DragDropManagerOld
    {
        private List<dynamic> _data;
        private dynamic _currentItem = null;
        private Point _dragStartPosition;
        private bool _isDragging = false;
        private static DragAdorner _dragAdorner;
        private DataTemplate _dataTemplate;
        private const int DRAG_WAIT_COUNTER_LIMIT = 10;

        internal DragDropManagerOld()
        {
            _data = new List<dynamic>();
        }

        internal void SetDataTemplate(DataTemplate dataTemplate)
        {
            _dataTemplate = dataTemplate;
        }

        internal bool InitializeDrag(Point point, dynamic item)
        {
            if(item != null)
            {
                _currentItem = item;
                _dragStartPosition = point;
                return true;
            }
            _data.Clear();
            return false;
        }

        internal bool InitiateDrag(ListBox dragSource, Point point)
        {
            if ((_isDragging == false) && (Math.Abs(point.X - _dragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                (Math.Abs(point.Y - _dragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (!_data.Contains(_currentItem)) {
                    _data.Add(_currentItem);
                }
                Console.WriteLine("Add Items:" +_data.Count());
                DragStart(dragSource);
                return true;
            }
            return false;
        }

        internal void InitializeDragFeedback(UIElement dragItem, object data, Point position)
        {
            if (data is IList items)
            {
                dynamic item = items[items.Count - 1];
                if (_dataTemplate != null)
                {
                    if (_dragAdorner == null)
                    {
                        var adornerLayer = AdornerLayer.GetAdornerLayer(dragItem);
                        _dragAdorner = new DragAdorner(item, _dataTemplate, dragItem, adornerLayer);
                        _dragAdorner.UpdatePosition(position.X, position.Y);
                    }
                }
            }
        }

        internal void UpdateDragFeedBack(Point position)
        {
            if(_dragAdorner != null)
            {
                _dragAdorner.UpdatePosition(position.X, position.Y);
            }
        }

        internal void UpdateSelector()
        {
            if (_currentItem != null)
            {
                _data.Add(_currentItem);
                _currentItem = null;
            } else
            {
                _data.Clear();
            }
        }

        internal void CleanDragFeedback()
        {
            RemoveAdorners();
        }

        internal void CancelDrag(QueryContinueDragEventArgs e)
        {
            e.Action = DragAction.Cancel;
            ResetState();
            RemoveAdorners();
        }

        internal void Drop()
        {
            RemoveAdorners();
        }

        private void DragStart(ListBox dragSource)
        {
            _isDragging = true;
            DataObject data = new DataObject(_data.GetType(), _data);
            DragDropEffects e = System.Windows.DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Copy | DragDropEffects.Move);
            dragSource.SelectedItems.Clear();
        }

        private void RemoveAdorners()
        {
            if (_dragAdorner != null)
            {
                _dragAdorner.Destroy();
                _dragAdorner = null;
            }
        }

        private void ResetState()
        {
            _isDragging = false;
        }

        
    }
}

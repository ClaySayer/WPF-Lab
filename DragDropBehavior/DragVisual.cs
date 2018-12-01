using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DragDrop
{
    public class DragVisual
    {
        private AdornerLayer _adornerLayer;
        private List<DragAdorner> _children = new List<DragAdorner>();
        private readonly double _offset = 2;

        public DragVisual(IList data, FrameworkElement context, DataTemplate template, Point position, Size size) {
            _adornerLayer = AdornerLayer.GetAdornerLayer(context);
            _adornerLayer.Opacity = 0.7;
            for (int i = data.Count - 1; i >= 0; i--)
            {
                object item = data[i];
                DragAdorner child = new DragAdorner(context, item, template, _offset * i, position, size);
                AttachAdorner(child);
            }
        }

        public DragVisual(object data, FrameworkElement context, Point position, Size size, FrameworkElement dragVisual)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(context);
            _adornerLayer.Opacity = 0.7;
            if (data is ContentControl item)
            {
                DragAdorner child = new DragAdorner(context, item.Content.ToString(), _offset, position, size, dragVisual)
                {
                    AllowDrop = false
                };
                AttachAdorner(child);
            }
        }

        private void AttachAdorner(DragAdorner adorner)
        {
            _children.Add(adorner);
            _adornerLayer.Add(adorner);
        }

        public void UpdatePosition(double left, double top)
        {
            foreach (DragAdorner child in _children)
            {
                child.UpdatePosition(left, top);
            }
            _adornerLayer.Update();
        }

        public void Destroy()
        {
            foreach (DragAdorner adorner in _children)
            {
                _adornerLayer.Remove(adorner);
            }
            _adornerLayer = null;
        }
    }
}

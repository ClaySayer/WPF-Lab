using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DragandDrop
{
    public class DragVisual
    {
        private AdornerLayer _adornerLayer;
        private List<DragAdorner> _children;
        private readonly double _offset = 2;

        public DragVisual(object data, UIElement context, DataTemplate dataTemplate, Point position, Size size, UIElement dragVisual)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(context);
            _children = new List<DragAdorner>();


            IList list = data as IList;
            if (list != null)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    object item = list[i];
                    DragAdorner child = new DragAdorner(context, item, dataTemplate, _offset * i, position, size, null);
                    _children.Add(child);
                    _adornerLayer.Add(child);
                    _adornerLayer.Opacity = 0.4;
                }
            } else {
                ContentControl item = data as ContentControl;
                if (item != null)
                {
                    DragAdorner child = new DragAdorner(context, item.Content.ToString(), dataTemplate, _offset, position, size, dragVisual);
                    child.AllowDrop = false;
                    _children.Add(child);
                    _adornerLayer.Add(child);
                    _adornerLayer.Opacity = 0.4;
                }
            }
        }

        public void UpdatePosition(double left, double top)
        {
            foreach(DragAdorner child in _children)
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

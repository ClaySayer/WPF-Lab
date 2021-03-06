﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DragDrop
{
    class DragAdorner: Adorner
    {
        private ContentPresenter _contentPresenter;
        private Size _size;
        private double _leftOffset;
        private double _topOffset;
        private readonly double _offset;
        private Point _position;

        public DragAdorner(FrameworkElement context, object data, DataTemplate template, double offset, Point position, Size size): base(context)
        {
            IsHitTestVisible = false;
            _contentPresenter = new ContentPresenter() { Content = data, ContentTemplate = template };
            _size = new Size(size.Width, size.Height);
            _position = position;
            _offset = offset;
        }
        public DragAdorner(FrameworkElement context, object data, double offset, Point position, Size size, FrameworkElement dragVisual) : base(context)
        {
            ContentControl control = dragVisual as ContentControl;
            _contentPresenter = new ContentPresenter() { Content = control };
            _size = new Size(size.Width, size.Height);
            _position = position;
            _offset = offset;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(constraint);
            return _contentPresenter.DesiredSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _contentPresenter;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _contentPresenter.Arrange(new Rect(_size));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }

        public void UpdatePosition(double left, double top)
        {
            _leftOffset = left - _position.X + _offset;
            _topOffset = top - _position.Y - _offset;
        }
    }
}

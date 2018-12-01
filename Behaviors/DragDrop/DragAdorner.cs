using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Behaviors.DragDrop
{
    public class DragAdorner : Adorner
    {
        private ContentPresenter _contentPresenter;
        private AdornerLayer _adornerLayer;
        private double _leftOffset;
        private double _topOffset;
        private Point _offset;

        public DragAdorner(object data, DataTemplate dataTemplate, UIElement adornedElement, AdornerLayer adornerLayer, Point offset)
            : base(adornedElement)
        {
            _adornerLayer = adornerLayer;
            _offset = offset;
            _contentPresenter = new ContentPresenter() { Content = data, ContentTemplate = dataTemplate, Opacity = 0.75 };
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(constraint);
            return _contentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement el = (FrameworkElement)AdornedElement;
            Size size = new Size(el.ActualWidth, finalSize.Height);
            _contentPresenter.Arrange(new Rect(size));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _contentPresenter;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public void UpdatePosition(double left, double top)
        {
            _leftOffset = left -_offset.X;
            _topOffset = top - _offset.Y;
            if (_adornerLayer != null)
            {
                _adornerLayer.Update(this.AdornedElement);
            }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }

        public void Destroy()
        {
            _adornerLayer.Remove(this);
        }
    }
}

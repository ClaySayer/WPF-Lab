using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Behaviors.DragDrop.EventArgs;

namespace Behaviors.DragDrop
{

    public static class DragDropManager
    {
        private static readonly Dictionary<UIElement, DragSource> dragSources = new Dictionary<UIElement, DragSource>();
        private static readonly Dictionary<UIElement, DropTarget> dropTargets = new Dictionary<UIElement, DropTarget>();
        private static DragSource dragSource;
        private static UIElement currentDragSource;
        private static DropTarget dropTarget;
        private static bool isMouseDown = false;
        private static bool isDragging = false;
        private static Point dragStartPosition;
        private static DataTemplate dragVisual;
        private static DragAdorner dragAdorner;

        private class DragSource
        {
            public EventHandler<EventArgs.DragEventArgs> DragInitializeHandler { get; set; }
            public EventHandler<GiveFeedbackEventArgs> GiveFeedbackHandler { get; set; }
            public EventHandler<QueryContinueDragEventArgs> QueryContinueDragHandler { get; set; }
            public EventHandler<EventArgs.DragEventArgs> DropCompletedHandler { get; set; }
        }

        private class DropTarget
        {
            public EventHandler<EventArgs.DragEventArgs> DropHandler { get; set; }
        }

        public static void AddDragSource(
            UIElement element,
            EventHandler<EventArgs.DragEventArgs> dragInitializeHandler,
            EventHandler<GiveFeedbackEventArgs> giveFeedbackHandler,
            EventHandler<QueryContinueDragEventArgs> queryContinueDragHandler,
            EventHandler<EventArgs.DragEventArgs> dropCompletedHandler
            )
        {
            AddDragSourceListeners(element);
            dragSources.Add(element, new DragSource
            {
                DragInitializeHandler = dragInitializeHandler,
                GiveFeedbackHandler = giveFeedbackHandler,
                QueryContinueDragHandler = queryContinueDragHandler,
                DropCompletedHandler = dropCompletedHandler
            });
        }

        public static void AddDropTarget(UIElement element, EventHandler<EventArgs.DragEventArgs> dropHandler)
        {
            dropTargets.Add(element, new DropTarget { DropHandler = dropHandler });
            element.PreviewDrop += new DragEventHandler(UIElement_PreviewDrop);
        }

        public static void RemoveDropTarget(UIElement element, EventHandler<EventArgs.DragEventArgs> dropHandler)
        {
            dropTargets.Remove(element);
            element.PreviewDrop -= UIElement_PreviewDrop;
        }

        public static void RemoveDragSource(UIElement element)
        {
            dragSources.Remove(element);
            RemoveDragSourceListeners(element);
        }

        private static void AddDragSourceListeners(UIElement element)
        {
            element.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(UIElement_PreviewMouseLeftButtonDown);
            element.PreviewMouseMove += new MouseEventHandler(UIElement_PreviewMouseMove);
            element.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(UIElement_PreviewMouseLeftButtonUp);
            element.GiveFeedback += new GiveFeedbackEventHandler(GiveFeedback);
            element.PreviewDragEnter += new DragEventHandler(UIElement_PreviewDragEnter);
            element.PreviewDragOver += new DragEventHandler(UIElement_PreviewDragOver);
            element.PreviewDragLeave += new DragEventHandler(UIElement_PreviewDragLeave);
        }

        private static void RemoveDragSourceListeners(UIElement element)
        {
            element.PreviewMouseLeftButtonDown -= UIElement_PreviewMouseLeftButtonDown;
            element.PreviewMouseMove -= UIElement_PreviewMouseMove;
            element.PreviewMouseLeftButtonUp -= UIElement_PreviewMouseLeftButtonUp;
            element.GiveFeedback -= GiveFeedback;
            element.PreviewDragEnter -= UIElement_PreviewDragEnter;
            element.DragOver -= UIElement_PreviewDragOver;
            element.PreviewDragLeave -= UIElement_PreviewDragLeave;
        }

        #region Mouse Events
        private static void UIElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = (UIElement)sender;
            Point p = e.GetPosition(element);
            InititializeDrag(p);
        }

        private static void UIElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                UIElement source = (UIElement)sender;
                Point p = e.GetPosition(source);
                InitiateDrag(source, p);                
            }
        }

        private static void UIElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
            isDragging = false;
        }
        #endregion

        #region Drag Events

        private static void UIElement_PreviewDragEnter(object sender, System.Windows.DragEventArgs e)
        {
            UIElement element = (UIElement)sender;
            var data = e.Data.GetData(e.Data.GetFormats()[0]);
            Point position = e.GetPosition(element);
            InitializeDragFeedback(element, data, position);
            if (dropTargets[element] != null)
            {
                dropTarget = dropTargets[element];
            }
        }

        private static void UIElement_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            //TODO Select DragDropEffect based on Keys
            //See https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.dodragdrop?view=netframework-4.7.2
            e.Effects = DragDropEffects.None;
            UIElement element = (UIElement)sender;
            if(element != currentDragSource)
            {
                e.Effects = DragDropEffects.Copy;
            }
            Point position = e.GetPosition(element);
            UpdateFeedback(position);
            e.Handled = true;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private static void UIElement_PreviewDragLeave(object sender, System.Windows.DragEventArgs e)
        {
            DetachAdorners();
            e.Handled = true;
        }

        private static void UIElement_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            UIElement element = (UIElement)sender;
            EventArgs.DragEventArgs args = new EventArgs.DragEventArgs(e);
            OnDrop(element, args);
            DetachAdorners();
        }

        #endregion

        private static void GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            UIElement element = (UIElement)sender;
            OnGiveFeedback(element, e);
        }

        private static void InititializeDrag(Point startPoint)
        {
            dragStartPosition = startPoint;
            isMouseDown = true;
        }

        private static void InitiateDrag(UIElement element, Point point)
        {
            if (isMouseDown)
            {
                if (isDragging == false && (Math.Abs(point.X - dragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(point.Y - dragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    dragSource = dragSources[element];
                    if (dragSource != null)
                    {
                        DragStart(element);
                    }
                }
            }
        }

        private static void DragStart(UIElement element)
        {
            isDragging = true;
            EventArgs.DragEventArgs args = new EventArgs.DragEventArgs();
            OnDragInitialize(element, args);
            if(args.DragVisual != null)
            {
                dragVisual = args.DragVisual;
            }
            if (args.Data != null)
            {
                currentDragSource = element;
                DragDropEffects effects = System.Windows.DragDrop.DoDragDrop(element, args.Data, DragDropEffects.Copy|DragDropEffects.Scroll|DragDropEffects.Move);
                EventArgs.DragEventArgs dropCompletedArgs = new EventArgs.DragEventArgs()
                {
                    Data = args.Data,
                    Effects = effects
                };
                if (effects != DragDropEffects.None)
                {
                    OnDropCompleted(element, dropCompletedArgs);
                }
                currentDragSource = null;
            }
            isDragging = false;
            isMouseDown = false;
        }

        private static void InitializeDragFeedback(UIElement element, object data, Point position)
        {
            if(data is IList items)
            {
                dynamic item = items[items.Count - 1];
                if(dragVisual != null)
                {
                    if(dragAdorner == null)
                    {
                        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
                        //TODO remove Adorner creation from here and create in Behavior
                        dragAdorner = new DragAdorner(item, dragVisual, element, adornerLayer);
                        dragAdorner.UpdatePosition(position.X, position.Y);
                    }
                }
            }
        }

        private static void UpdateFeedback(Point position)
        {
            if(dragAdorner != null)
            {
                dragAdorner.UpdatePosition(position.X, position.Y);
            }
        }

        private static void DetachAdorners()
        {
            if(dragAdorner != null)
            {
                dragAdorner.Destroy();
                dragAdorner = null;
            }
        }

        private static void OnDragInitialize(UIElement element, EventArgs.DragEventArgs e)
        {
            dragSource.DragInitializeHandler?.Invoke(element, e);
        }

        private static void OnDropCompleted(UIElement element, EventArgs.DragEventArgs e)
        {
            dragSource.DropCompletedHandler?.Invoke(element, e);
        }
        private static void OnGiveFeedback(UIElement element, GiveFeedbackEventArgs e)
        {
            dragSource.GiveFeedbackHandler?.Invoke(element, e);
        }
        private static void OnDrop(UIElement element, EventArgs.DragEventArgs e)
        {
            dropTarget.DropHandler?.Invoke(element, e);
        }
    }
}
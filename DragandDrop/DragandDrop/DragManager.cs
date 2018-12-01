using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DragandDrop
{
    public static class DragManager
    {
        private static bool _isDragging = false;
        private static DragSource _dragSource;
        private static DragContextInfo _dragContextInfo;
        private static readonly List<DragContextInfo> _dragContexts = new List<DragContextInfo>();
        private static readonly Dictionary<UIElement, DragSource> _dragSources = new Dictionary<UIElement, DragSource>();

        public static Point DragStartPosition { get; set; }

        private class DragSource
        {
            internal EventHandler<DragInitializeEventArgs> DragInitializeHandler { get; set; }
            internal EventHandler<DropCompletedEventArgs> DropCompletedHandler { get; set; }
        }

        public static Panel DragContext { get; set; }

        private class DragContextInfo
        {
            internal Panel Context { get; set; }
            internal EventHandler<DropCompletedEventArgs> DropCompletedHandler { get; set; }
        }

        public static void RegisterDragSource(
            UIElement element,
            EventHandler<DragInitializeEventArgs> dragInitializeEventHandler,
            EventHandler<DropCompletedEventArgs> dropCompleteHandler)
        {
            _dragSources.Add(element, new DragSource
            {
                DragInitializeHandler = dragInitializeEventHandler,
                DropCompletedHandler = dropCompleteHandler
            });
        }

        public static void RegisterDragContext(Panel context, EventHandler<DropCompletedEventArgs> dropCompletedHandler)
        {
            _dragContexts.Add(new DragContextInfo { Context=context, DropCompletedHandler = dropCompletedHandler });
        }

        public static void InitializeDrag(UIElement source, Point startPoint)
        {
            if (_dragSources.ContainsKey(source))
            {
                _dragSource = _dragSources[source];
                _dragContextInfo = FindDragContext(source);
                DragContext = _dragContextInfo.Context;
                DragStartPosition = source.TranslatePoint(startPoint, DragContext);
            }
        }

        public static void InitiateDrag(UIElement source, Point position)
        {
            if (_isDragging == false && ((Math.Abs(position.X - DragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                (Math.Abs(position.Y - DragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance)))
            {
                if (_dragSource != null)
                {
                    DragStart(source);
                }
            }
        }

        public static void FinalizeDrag()
        {
            _isDragging = false;
        }

        private static void DragStart(UIElement source)
        {
            _isDragging = true;
            DragInitializeEventArgs args = new DragInitializeEventArgs
            {
                DragStartPosition = DragStartPosition
            };
            OnDragInitialize(source, args);
            if (args.Data != null)
            {
                DragDropEffects effects = System.Windows.DragDrop.DoDragDrop(source, args.Data, DragDropEffects.All);
                DropCompletedEventArgs dropCompletedArgs = new DropCompletedEventArgs(DragDrop.DropCompletedEvent, effects, args.Data);
                OnDropCompleted(source, dropCompletedArgs);
            }
            _isDragging = false;
            DragContext = null;
            _dragContextInfo = null;
            _dragSource = null;
        }

        private static void OnDropCompleted(UIElement element, DropCompletedEventArgs e)
        {
            element.RaiseEvent(e);
            if (_dragSource != null)
            {
                _dragSource.DropCompletedHandler?.Invoke(element, e);
            }
            if(_dragContextInfo != null)
            {
                _dragContextInfo.DropCompletedHandler?.Invoke(element, e);
            }
            
        }

        private static void OnDragInitialize(UIElement element, DragInitializeEventArgs e)
        {
            _dragSource.DragInitializeHandler?.Invoke(element, e);
        }

        private static DragContextInfo FindDragContext(UIElement element)
        {
            DependencyObject current = element;
            while(current != null)
            {
                if (current.GetType() == typeof(Panel) || current.GetType().IsSubclassOf(typeof(Panel)))
                {
                    Panel panel = current as Panel;
                    foreach(DragContextInfo context in _dragContexts)
                    {
                        if(context.Context == panel)
                        {
                            return context;
                        }
                    }
                }
                current = FindPanel(current);
            }
            return null;
        }

        private static Panel FindPanel(DependencyObject element)
        {
            element = VisualTreeHelper.GetParent(element);
            while (element != null && (element.GetType() != typeof(Panel) && !element.GetType().IsSubclassOf(typeof(Panel))))
            {
                element = VisualTreeHelper.GetParent(element);
            }
            if(element != null)
            {
                return element as Panel;
            }
            return null;
        }
    }
}

using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace Behaviors.DragDrop.EventArgs
{
    public class DragEventArgs : RoutedEventArgs
    {
        public DragDropEffects Effects { get; set; }
        public IDataObject Data { get; set; }
        public DataTemplate DragVisual { get; set; }
        public Point GetPosition(IInputElement element)
        {
            return Mouse.GetPosition(element);
        }
        public DragEventArgs(System.Windows.DragEventArgs args)
        {
            Effects = args.Effects;
            Data = args.Data;
        }
        public DragEventArgs() { }
    }
    public class DragInitializeEventArgs : RoutedEventArgs
    {
        public DragDropEffects AllowedEffects { get; set; }
        public object Data { get; set; }
        public DataTemplate DragVisual { get; set; }
        public string Type { get; set; }
    }

    public class DropCompletedEventArgs : RoutedEventArgs
    {
        public IList Data { get; set; }
        public DragDropEffects Effects { get; set; }
    }
}

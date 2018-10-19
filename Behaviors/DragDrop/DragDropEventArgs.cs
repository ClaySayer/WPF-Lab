using System.Windows;
using System.Windows.Input;

namespace Behaviors.DragDrop.EventArgs
{
    public class DragEventArgs : RoutedEventArgs
    {
        public DragDropEffects Effects { get; set; }
        public IDataObject Data { get; set; }
        public DataTemplate DragVisual { get; set; }
        public UIElement DragElement { get; set; }
        public Point DragOffset { get; set; }
        public Point StartPoint { get; set; }
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
}

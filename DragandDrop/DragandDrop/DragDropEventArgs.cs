using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DragandDrop
{
    public class DragInitializeEventArgs: RoutedEventArgs
    {
        public DragDropEffects AllowedEffects { get; set; }
        public DragDropEffects Effects { get; set; }
        public DataObject Data { get; set; }
        public object DragVisual { get; set; }
        public Point DragVisualOffset { get; set; }
        public Point DragStartPosition { get; set; }
        public Size DragVisualSize { get; set; }
        public Point GetPosition(IInputElement element)
        {
            return Mouse.GetPosition(element);
        }
    }

    public class DropCompletedEventArgs: RoutedEventArgs
    {
        public object Data { get; set; }
        public DragDropEffects Effects { get; set; }
        public DropCompletedEventArgs(RoutedEvent e) : base(e) { }
        public DropCompletedEventArgs(RoutedEvent e, DragDropEffects effects, object data) : base(e)
        {
            Effects = effects;
            Data = data;
        }
        public DropCompletedEventArgs() : base() { }
    }

    public delegate void DropCompletedEventHandler(object sender, DropCompletedEventArgs e);

    //class DragDropEventArgs: RoutedEventArgs
    //{
    //    public DragDropEffects Effects { get; set; }
    //    public IDataObject Data { get; set; }
    //    public DataTemplate DragVisual { get; set; }
    //    public UIElement DragElement { get; set; }
    //    public Point DragOffset { get; set; }
    //    public Point StartPoint { get; set; }
    //    public Point GetPosition(IInputElement element)
    //    {
    //        return Mouse.GetPosition(element);
    //    }
    //    public DragDropEventArgs(System.Windows.DragEventArgs args)
    //    {
    //        Effects = args.Effects;
    //        Data = args.Data;
    //    }
    //    public DragDropEventArgs() { }
    //}
}

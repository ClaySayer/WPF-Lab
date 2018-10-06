using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Behaviors
{
    public class EventToCommandBehavior<T>: Behavior<T> where T : FrameworkElement
    {

        #region private members

        private Delegate _handler;
        private EventInfo _prevEvent;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty EventProperty = 
            DependencyProperty.Register("Event", typeof(string),
                typeof(EventToCommandBehavior<T>),
                new PropertyMetadata(null, OnEventChanged));
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand),
                typeof(EventToCommandBehavior<T>),
                new PropertyMetadata(null));
        public static readonly DependencyProperty PassArgumentsProperty =
            DependencyProperty.Register("PassArguments", typeof(bool),
                typeof(EventToCommandBehavior<T>),
                new PropertyMetadata(false));
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IValueConverter),
                typeof(EventToCommandBehavior<T>),
                new PropertyMetadata(null));
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object),
                typeof(EventToCommandBehavior<T>),
                new PropertyMetadata(null));

        #endregion

        #region Properties

        public String Event
        {
            get { return (string)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public bool PassArguments
        {
            get { return (bool)GetValue(PassArgumentsProperty); }
            set { SetValue(PassArgumentsProperty, value); }
        }

        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty);  }
            set { SetValue(ConverterProperty, value); }
        }

        #endregion

        protected override void OnAttached()
        {
            AttachHandler(Event);
        }


        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (EventToCommandBehavior<T>)d;
            if(behavior.AssociatedObject != null)
            {
                behavior.AttachHandler((string)e.NewValue);
            }
        }

        protected void ExecuteCommand(object sender, EventArgs e)
        {
            object args = this.PassArguments ? e : null;
            if (Command != null)
            {
                if (Converter != null)
                {
                    object parameter = Converter.Convert(e, typeof(object), ConverterParameter, null);
                    if (Command.CanExecute(parameter))
                        Command.Execute(parameter);
                }
                else
                {
                    if (Command.CanExecute(args))
                        Command.Execute(args);
                }
            }
        }

        private void AttachHandler(string eventName)
        {
            if(_prevEvent != null)
            {
                _prevEvent.RemoveEventHandler(AssociatedObject, _handler);
            }
            if (!string.IsNullOrEmpty(eventName))
            {
                EventInfo eventInfo = AssociatedObject.GetType().GetEvent(eventName);
                if(eventInfo != null)
                {
                    MethodInfo methodInfo = GetType().GetMethod("ExecuteCommand", BindingFlags.Instance | BindingFlags.NonPublic);
                    _handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
                    eventInfo.AddEventHandler(AssociatedObject, _handler);
                    _prevEvent = eventInfo;
                } else
                {
                    throw new ArgumentException(string.Format("The event '{0}' was not found on type '{1}'", eventName, AssociatedObject.GetType().Name));
                }
            }
        }

        
    }
}

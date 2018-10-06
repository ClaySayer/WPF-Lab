using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlTests
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _action;

        public DelegateCommand(Action<object> action)
        {
            _action = action;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }

    public class ViewModel: ObservableObject
    {
        public ObservableCollection<Person> People { get; }

        public ICommand TestCommand
        {
            get { return new DelegateCommand(TestMethod); }
        }

        public ViewModel()
        {
            People = new ObservableCollection<Person>
            {
                new Person("Bob"),
                new Person("Fred"),
                new Person("George"),
                new Person("Bill")
            };
        }

        private void TestMethod(object param)
        {
            Console.WriteLine("Hello From TestMethod via Command");
        }
    }
}

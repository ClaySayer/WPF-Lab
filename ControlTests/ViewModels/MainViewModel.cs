using ControlTests.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlTests.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // WORKING WITH OBSERVABLE COLLECTION TRYING VIEWMODELS
        //public ObservableCollection<Person> _building1;
        //public ObservableCollection<Person> _building2;

        //public ObservableCollection<Person> Building1
        //{
        //    get { return _building1; }
        //    set {
        //        if (_building1 != value)
        //        {
        //            _building1 = value;
        //            base.RaisePropertyChanged("Building1");
        //        }
        //    }
        //}

        //public ObservableCollection<Person> Building2
        //{
        //    get { return _building2; }
        //    set
        //    {
        //        if (_building2 != value)
        //        {
        //            _building2 = value;
        //            base.RaisePropertyChanged("Building2");
        //        }
        //    }
        //}

        //public MainViewModel()
        //{
        //    Building1 = new ObservableCollection<Person>();
        //    Building1.Add(new Person("Bob"));
        //    Building1.Add(new Person("Fred"));
        //    Building1.Add(new Person("George"));
        //    Building1.Add(new Person("Jim"));
        //    Building2 = new ObservableCollection<Person>();
        //    Console.WriteLine("HERE");
        //}

        private PeopleViewModel _building1;
        private PeopleViewModel _building2;

        public PeopleViewModel Building1
        {
            get { return _building1 ; }
            set { SetProperty(ref _building1, value); }
        }

        public PeopleViewModel Building2
        {
            get { return _building2; }
            set { SetProperty(ref _building2, value); }
        }

        public MainViewModel()
        {
            Building1 = new PeopleViewModel();
            Building1.Add("Bob");
            Building1.Add("Fred");
            Building1.Add("George");
            Building1.Add("Bill");
            Building2 = new PeopleViewModel();
            //Building1.Add("Bob");
            //Building1.Add("Fred");
            //Building1.Add("George");
            //Building1.Add("Jim");
        }
    }
}

using ControlTests.Commands;
using ControlTests.Models;
using ControlTests.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data;

namespace ControlTests.ViewModels
{
    public class PeopleViewModel: ViewModelBase
    {
        private ObservableCollection<PersonViewModel> _people;

        public PeopleViewModel()
        {
            People = new ObservableCollection<PersonViewModel>();
        }

        public ObservableCollection<PersonViewModel> People
        {
            get { return _people; }
            set { SetProperty(ref _people, value); }
        }

        public void Add(string name)
        {
            People.Add(new PersonViewModel(name));
        }

        public ICommand InsertCommand => new RelayCommand<object>(Insert);
        public ICommand RemoveCommand => new RelayCommand<object>(Remove);

        private void Insert(object param)
        {
            System.Collections.IList items = (System.Collections.IList)param;
            var collection = items.Cast<PersonViewModel>();
            foreach(PersonViewModel item in collection)
            {
                People.Add(item);
            }
        }

        private void Remove(object param)
        {
            IList people = (IList)param;
            List<string> names = new List<string>(); 
            foreach(PersonViewModel p in people)
            {
                names.Add(p.Name);
            }
            for(int i=0; i< names.Count; i++)
            {
                string name = names[i];
                People.Remove(People.Where(p => p.Name == name).Single());
            }
        }
    }
}

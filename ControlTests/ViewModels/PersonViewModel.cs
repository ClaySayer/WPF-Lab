using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlTests.Models;

namespace ControlTests.ViewModels
{
    public class PersonViewModel : ViewModelBase
    {
        private Person _person;
        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        public PersonViewModel(string name)
        {
            _person = new Person(name);
        }

        public string Name
        {
            get => _person.Name;
        }
    }
}

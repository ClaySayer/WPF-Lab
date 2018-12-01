using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlTests.Models
{
    public class Person
    {
        public Person(string name)
        {
            Name = name;
        }

        public Person()
        {
            Name = "Bob";
        }

        public string Name { get; set; }
    }
}

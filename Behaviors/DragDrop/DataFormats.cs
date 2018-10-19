using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Behaviors.DragDrop.DataFormats
{
    public static class LocalDataFormats
    {
        public static readonly string PersonViewModel = "PersonViewModel";
        private static readonly List<DataFormat> DataFormats = new List<DataFormat> {
            new DataFormat("PersonViewModel", 1)

        };
        public static DataFormat GetDataFormat(int id)
        {
            if(DataFormats[id] != null)
            {
                return DataFormats[id];
            }
            throw new ArgumentException($"The Data Format with Id: {id} is not available in the current context");
        }
        public static DataFormat GetDataFormat(string name)
        {
            DataFormat format = DataFormats.Find(f => f.Name == name);
            if(format != null)
            {
                return format;
            }
            throw new ArgumentException($"The Data Format {name} is not available in the current context");
        }
    }
}

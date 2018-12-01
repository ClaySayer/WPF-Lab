using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DragDrop
{
    public static class DragDataManager
    {
        public static readonly string DataFormat = "DragDrop.WPF";

        public static DataObject GenerateDataObject(object data)
        {
            return new DataObject(DataFormat, data);
        }

        public static IList GetData(IDataObject data)
        {
            var items = data.GetData(DataFormat);
            if(items is IList)
            {
                return items as IList;
            }
            Array list = Array.CreateInstance(typeof(object), 1);
            list.SetValue(items, 0);
            return list as IList;
        }
    }
}

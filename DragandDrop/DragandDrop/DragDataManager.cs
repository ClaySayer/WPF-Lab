using System.Collections;
using System.Windows;

namespace DragandDrop
{
    public static class DragDataManager
    {
        public static readonly string DataFormat = "DragDrop.WPF";

        public static DataObject GenerateDataObject(object data)
        {
            return new DataObject(DataFormat, data);
        }

        public static IList GetData(DataObject data)
        {
            var items = data.GetData(DataFormat);
            return items as IList;
        }
    }
}

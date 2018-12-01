using System;
using System.Windows;

namespace DragDropTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_DragCompleted(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Caught Attached Routed DragCompleted Event");
        }
    }
}

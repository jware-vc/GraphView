using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphView
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("Creating Node");
            switch (e.Key)
            {
                case Key.O:
                {
                    NG.AddNode(Mouse.GetPosition(NG), NG.CreateOutNode());
                    break;
                }
                case Key.A:
                {
                    NG.AddNode(Mouse.GetPosition(NG), NG.CreateAdditionNode());
                    break;
                }
                case Key.I:
                {
                    NG.AddNode(Mouse.GetPosition(NG), NG.CreateIntNode());
                    break;
                }
                case Key.Delete:
                {
                    NG.DeleteItemUnderMouse();
                    break;
                }
            }
        }
    }
}

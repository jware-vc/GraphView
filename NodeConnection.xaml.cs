using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Interaction logic for NodeConnection.xaml
    /// </summary>
    public partial class NodeConnection : UserControl
    {
        public Port from, to;
        private Line ln = new Line();
        public NodeConnection()
        {
            InitializeComponent();
            ln.StrokeThickness = 3;
            LineCan.Children.Add(ln);
        }

        public void UpdateValue(object val)
        {
            to?.SetPortValue(val);
            to?.ParentNode.Evaluate();
        }

        public bool IsConnected()
        {
            return from != default && to != default;
        }

        public void RemoveConnection()
        {
            from?.RemoveConnection(this);
            to?.RemoveConnection(this);
        }

        public void ConnectPorts(ref Port f, ref Port t)
        {
            switch (f.Mode)
            {
                case PortMode.Output:
                    from = f;
                    to = t;
                    break;
                case PortMode.Input:
                    to = f;
                    from = t;
                    break;
            }
            UpdatePosition();
        }
        public void UpdatePosition()
        {
            if (IsConnected())
            {
                var ft = from.GlobalPos();
                var tt = to.GlobalPos();
                ln.X1 = ft.X;
                ln.Y1 = ft.Y;
                ln.X2 = tt.X;
                ln.Y2 = tt.Y;
                if (ln.X2 > ln.X1)
                {
                    ln.Stroke = new LinearGradientBrush(from.PortColor(), to.PortColor(), 0f);
                }
                else
                {
                    ln.Stroke = new LinearGradientBrush(to.PortColor(), from.PortColor(), 0f);

                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    /// Interaction logic for Node.xaml
    /// </summary>
    public delegate bool NodeDelegate(Port[] inPorts, Port[] outPorts);
    
    public partial class Node : UserControl
    {
        private Port[] InPorts, OutPorts;
        public NodeDelegate Func;
        public Node(string title, Port[] inPorts, Port[] outPorts, NodeDelegate func)
        {
            InitializeComponent();
            Label.Content = title;
            InPorts = inPorts;
            OutPorts = outPorts;
            Func = func;
            for (var i = 0; i < InPorts.Length; i++)
            {
                var pt = InPorts[i];
                pt.ParentNode = this;
                Canvas.SetLeft(pt, pt.Connection.Width * -0.5f);
                Canvas.SetTop(pt, (i * pt.Connection.Height) + Title.Height);
                NodeCanvas.Children.Add(pt);
            }
            for (var i = 0; i < OutPorts.Length; i++)
            {
                var pt = OutPorts[i];
                pt.ParentNode = this;
                Canvas.SetLeft(pt, Body.Width - (pt.Connection.Width * 0.5f));
                Canvas.SetTop(pt, (i * pt.Connection.Height) + Title.Height);
                NodeCanvas.Children.Add(pt);
            }
        }

        public bool Evaluate()
        {
            if (InPorts.All(p => p.HasConnections) && OutPorts.All(p => p.HasConnections)) return Func(InPorts, OutPorts);
            else return false;
        }

        public void RemoveConnectionsOnPorts()
        {
            foreach (var port in InPorts) port.BreakConnection();
            foreach (var port in OutPorts) port.BreakConnection();
        }

        public Port[] GetPortsUnderMouse()
        {
            var sel = new List<Port>();
            foreach (var i in InPorts) { if (i.IsMouseOver) sel.Add(i); }
            foreach (var o in OutPorts) { if (o.IsMouseOver) sel.Add(o); }
            return sel.ToArray();
        }
        public Point PortPosition(Port pt)
        {
            return new Point(Canvas.GetLeft(this) + Canvas.GetLeft(pt) + 0.5f * pt.Connection.Width, Canvas.GetTop(this) + Canvas.GetTop(pt) + 0.5f * pt.Connection.Height);
        }

        public void Translate(Vector diff)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) - diff.X);
            Canvas.SetTop(this, Canvas.GetTop(this) - diff.Y);
            foreach (var p in InPorts) p.UpdatePosition();
            foreach (var p in OutPorts) p.UpdatePosition();
        }
    }
}

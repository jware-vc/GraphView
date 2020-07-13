using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
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
    /// Interaction logic for NodeGraph.xaml
    /// </summary>
    public class PortCon
    {
        public Port from, to;
    }
    public partial class NodeGraph : UserControl
    {
        private bool isDragging = false;
        private Point prevPos = default;
        private Node[] selection = default;
        private PortCon portCon = default;
        private List<Node> nodes = new List<Node>();
        private List<NodeConnection> connections = new List<NodeConnection>();
        public NodeGraph()
        {
            InitializeComponent();
        }

        public void AddNode(Point pt, Node nd)
        {
            Canvas.SetLeft(nd, pt.X);
            Canvas.SetTop(nd, pt.Y);
            Grid.Children.Add(nd);
            nodes.Add(nd);
        }

        public void Evaluate()
        {
            foreach (var n in nodes) n.Evaluate();
        }

        public Node CreateAdditionNode()
        {
            return new Node(
                "Add",
                new Port[]{
                    new Int32Port( "A", PortMode.Input ),
                    new Int32Port( "B", PortMode.Input )
                },
                new Port[] { 
                    new Int32Port( "O", PortMode.Output )
                },
                (ip, op) => {
                    var a = (Int32Port)ip[0];
                    var b = (Int32Port)ip[1];
                    var o = (Int32Port)op[0];
                    o.UpdatePortValue((Int32)a.value + (Int32)b.value);
                    return true; 
                }
            );
        }

        public Node CreateIntNode()
        {
            return new Node(
                "Int",
                new Port[]{
                },
                new Port[] { 
                    new Int32Port( "O", PortMode.Output )
                },
                (ip, op) => { 
                    Debug.WriteLine("Setting value to 1."); 
                    var o = (Int32Port)op[0];
                    o.UpdatePortValue(1); 
                    return true; 
                }
            );
        }

        public Node CreateOutNode()
        {
            return new Node(
                "Out",
                new Port[]{
                    new Int32Port( "Output", PortMode.Input )
                },
                new Port[] { 
                },
                (ip, op) => { 
                    Debug.WriteLine($"Output: {ip[0].value}"); 
                    return true; 
                }
            );
        }

        public void DeleteItemUnderMouse()
        {
            var nodesToDelete = new List<Node>();
            var connectionsToDelete = new List<NodeConnection>();
            foreach (var node in nodes)
            {
                if (node.IsMouseOver)
                {
                    node.RemoveConnectionsOnPorts();
                    nodesToDelete.Add(node);
                }
            }
            foreach (var conn in connections)
            {
                if (conn.IsMouseOver || conn.IsConnected() == false) 
                { 
                    conn.RemoveConnection();
                    connectionsToDelete.Add(conn); 
                }
            }
            foreach (var node in nodesToDelete)
            {
                Grid.Children.Remove(node);
                nodes.Remove(node);
            }
            foreach (var conn in connectionsToDelete)
            {
                Grid.Children.Remove(conn);
                connections.Remove(conn);
            }
        }

        public Node[] GetNodesUnderMouse()
        {
            var sel = new List<Node>();
            foreach (Node n in nodes)
            {
                if (n.IsMouseOver) sel.Add(n);
            }
            return sel.ToArray();
        }

        public void ConnectNodes(PortCon con)
        {
            var nodeCon = new NodeConnection();
            nodeCon.ConnectPorts(ref con.from, ref con.to);
            connections.Add(nodeCon);
            con.from.AddConnection(nodeCon); 
            con.to.AddConnection(nodeCon); 
            Grid.Children.Add(nodeCon);
            Debug.WriteLine($"Created Connection");
            nodeCon.from.ParentNode.Evaluate();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selection = GetNodesUnderMouse();
            foreach (var n in selection)
            {
                var pts = n.GetPortsUnderMouse();
                if (pts.Length > 0)
                {
                    if (portCon != default && pts[0] != portCon.from && pts[0].Mode != portCon.from.Mode)
                    {
                        portCon.to = pts[0];
                        ConnectNodes(portCon);
                        portCon = default;
                        break;
                    }
                    else
                    {
                        portCon = new PortCon();
                        portCon.from = pts[0];
                        break;
                    }
                }

            }

            isDragging = selection.Length > 0;
            prevPos = e.GetPosition(Grid);
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging) {
                var pos = e.GetPosition(Grid);
                var diff = prevPos - pos;
                foreach (var n in selection)
                {
                    n.Translate(diff);
                }
                prevPos = pos;
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selection = default;
            isDragging = false;
        }
    }
}

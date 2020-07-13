using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    /// Interaction logic for Port.xaml
    /// </summary>
    public enum PortMode { Input, Output };
    public enum PortValue
    {
        BOOL,
        INT,
        STRING,
    }

    public partial class Port : UserControl
    {
        public PortMode Mode;
        public PortValue ValueType;
        public object value;
        public Node ParentNode;
        private List<NodeConnection> connections = new List<NodeConnection>();
        public Port(string name, PortValue pv, PortMode mode)
        {
            InitializeComponent();
            Mode = mode;
            NameLabel.Content = name;
            NameLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            ValueType = pv;
            Connection.Stroke = new SolidColorBrush(PortColor());
            SetupPort();
        }

        public bool HasConnections { get { return connections.Count > 0; } }

        public void UpdatePortValue(object val)
        {
            SetPortValue(val);
            foreach (var c in connections)
            {
                c.UpdateValue(value);
            }
        }

        public void SetPortValue(object val)
        {
            value = val;
        }

        public virtual void SetValueProp(object value) { }

        public void SetupPort()
        {
            switch (Mode)
            {
                case PortMode.Output:
                    Canvas.SetLeft(NameLabel, Canvas.GetLeft(NameLabel) - NameLabel.DesiredSize.Width - (Connection.Width * 0.5f));
                    break;
            }
        }
        public Color PortColor()
        {
            switch (Mode)
            {
                case PortMode.Input:
                    return Colors.DeepSkyBlue;
                case PortMode.Output:
                    return Colors.Orchid;
                default:
                    return Colors.White;
            }
        }
        public Point GlobalPos()
        {
            return ParentNode.PortPosition(this);
        }
        public void BreakConnection()
        {
            foreach (var conn in connections)
            {
                if (conn.from == this) conn.from = default;
                if (conn.to == this) conn.from = default;
            }
        }
        public void RemoveConnection(NodeConnection connection)
        {
            connections.Remove(connection);
        }
        public void AddConnection(NodeConnection connection)
        {
            connections.Add(connection);
        }
        public void UpdatePosition()
        {
            foreach (var conn in connections) conn.UpdatePosition();
        }
    }
    
    public partial class Int32Port: Port
    {
        public Int32Port(string name, PortMode mode) : base(name, PortValue.INT, mode) 
        {

        }
    }
}

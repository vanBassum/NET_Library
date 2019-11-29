using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dkstr
{
    public partial class Form1 : Form
    {
        BindingList<Node> pathDi = new BindingList<Node>();
        BindingList<Node> pathBi = new BindingList<Node>();
        BindingList<Node> nodes = new BindingList<Node>();
        BindingList<Edge> edges = new BindingList<Edge>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.DataSource = nodes;
            listBox2.DataSource = pathDi;
            listBox3.DataSource = pathBi;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            nodes.Clear();
            edges.Clear();
            pathDi.Clear();
            pathBi.Clear();
            foreach(string line in richTextBox1.Lines)
            {
                string[] split = line.Split(' ');

                if (split.Length == 2)
                    nodes.Add(new Node(split[1], int.Parse(split[0])));

                if (split.Length == 3)
                    edges.Add(new Edge
                    {
                        From = nodes.First(n => n.ID == int.Parse(split[0])),
                        To = nodes.First(n => n.ID == int.Parse(split[1])),
                        Weight = uint.Parse(split[2])
                    });

            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pathDi.Clear();
            List<Node> nPath = Dijkstra.Solve(nodes, edges, nodes[0], (Node)listBox1.SelectedItem);
            foreach (Node n in nPath)
                pathDi.Add(n);

            pathBi.Clear();
            List<Node> nPath2 = Dijkstra.SolveBidirectional(nodes, edges, nodes[0], (Node)listBox1.SelectedItem);
            foreach (Node n in nPath2)
                pathBi.Add(n);

        }
    }


    public static class Dijkstra
    {

        public static List<Node> SolveBidirectional(IEnumerable<Node> nodes,
                                                    IEnumerable<Edge> edges,
                                                    Node startNode,
                                                    Node endNode)
        {
            foreach (Node n in nodes)
            {
                n.Done = false;
                n.Prev = null;
                n.Weight = uint.MaxValue;
            }


            Node current = startNode;
            current.Weight = 0;

            while (current != null)
            {
                IEnumerable<Edge> neighbours = edges.Where(e => e.From == current || e.To == current);

                foreach (Edge ed in neighbours)
                {
                    uint nw = current.Weight + ed.Weight;

                    Node to = ed.To == current ? ed.From : ed.To;

                    if (to.Weight > nw)
                    {
                        to.Weight = nw;
                        to.Prev = current;
                    }
                }
                current.Done = true;

                IEnumerable<Node> next = from c in nodes
                                         where !c.Done
                                         where c.Weight != uint.MaxValue
                                         orderby c.Weight
                                         select c;

                current = next.FirstOrDefault();
            }

            current = endNode;

            List<Node> path = new List<Node>();

            while (current != startNode)
            {
                path.Add(current);
                current = current.Prev;
            }
            path.Add(current);

            path.Reverse();
            return path;
        }


        public static List<Node> Solve( IEnumerable<Node> nodes,
                                        IEnumerable<Edge> edges,
                                        Node from,
                                        Node to)
        {
            foreach (Node n in nodes)
            {
                n.Done = false;
                n.Prev = null;
                n.Weight = uint.MaxValue;
            }

            Node current = from;
            current.Weight = 0;

            while (current != null)
            {
                IEnumerable<Edge> neighbours = edges.Where(e => e.From == current);

                foreach(Edge ed in neighbours)
                {
                    uint nw = current.Weight + ed.Weight;
                    if(ed.To.Weight > nw)
                    {
                        ed.To.Weight = nw;
                        ed.To.Prev = current;
                    }
                }
                current.Done = true;

                IEnumerable<Node> next = from c in nodes
                                         where !c.Done
                                         where c.Weight != uint.MaxValue
                                         orderby c.Weight
                                         select c;

                current = next.FirstOrDefault();
            }

            current = to;

            List<Node> path = new List<Node>();

            while(current != from)
            {
                path.Add(current);
                current = current.Prev;
            }
            path.Add(current);

            path.Reverse();
            return path;
        }

    }



    public class Node
    {
        public Node(string name, int id)
        {
            ID = id;
            Name = name;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; } = false;
        public uint Weight { get; set; } = uint.MaxValue;
        public Node Prev { get; set; } = null;

        public override string ToString()
        {
            return Name + " " + ((Weight==uint.MaxValue)?"": Weight.ToString());
        }
    }


    public class Edge
    {

        public Node From { get; set; } = null;
        public Node To { get; set; } = null;
        public uint Weight {get; set; } = uint.MaxValue;
    }

    /*

    public class Node : INode
    {
        public string Name { get; set; }
    }


    public class Edge : IEdge
    {
        public INode From { get; set; }
        public INode To { get; set; }
        public uint Weight { get; set; }
    }


    public interface INode
    {
        string Name { get; set; }
    }

    public interface IEdge
    {
        INode From { get; set; }
        INode To { get; set; }
        uint Weight { get; set; }
    }

    */
}

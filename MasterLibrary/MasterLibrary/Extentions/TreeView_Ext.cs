using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MasterLibrary.Extentions
{
    public static class TreeView_Ext
    {

        public static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                yield return node;

                foreach (var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }
        public static void AddPath(this TreeNode tn, string path, char seperator = '/')
        {
            string relPath = path.Replace(tn.Text, "").TrimStart(seperator);
            string[] split = relPath.Split(seperator);


            TreeNode curNode = tn;
            for (int i = 0; i < split.Length; i++)
            {

                int ind = curNode.Nodes.FindIndex(n => n.Text == split[i]);
                if (ind == -1)
                {
                    TreeNode newNode = new TreeNode(split[i]);
                    newNode.Tag = path;
                    curNode.Nodes.Add(newNode);
                    curNode = newNode;
                }
                else
                {
                    curNode = curNode.Nodes[ind];
                }
            }
        }

        static public int FindIndex(this TreeNodeCollection col, Predicate<TreeNode> predicate)
        {
            for (int i = 0; i < col.Count; i++)
            {
                if (predicate(col[i]))
                    return i;
            }
            return -1;
        }
    }
}

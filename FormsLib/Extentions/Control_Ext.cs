using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FormsLib.Extentions
{
    public static class Control_Ext
    {
        public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
        {
            if (obj.InvokeRequired)
            {
                obj.Invoke(action, new object[0]);
            }
            else
            {
                action();
            }
        }

        public static ToolStripMenuItem AddMenuItem(this ToolStrip menu, string path, Action<ToolStripMenuItem> action) => AddMenuItem(menu.Items, path, action);
        public static ToolStripMenuItem AddMenuItem(this ToolStripMenuItem menu, string path, Action<ToolStripMenuItem> action) => AddMenuItem(menu.DropDown, path, action);
        public static ToolStripMenuItem AddRadioButtonMenuItem(this ToolStrip menu, string path, Action<ToolStripMenuItem> action) => AddRadioButtonMenuItem(menu.Items, path, action);
        public static ToolStripMenuItem AddRadioButtonMenuItem(this ToolStripMenuItem menu, string path, Action<ToolStripMenuItem> action) => AddRadioButtonMenuItem(menu.DropDown, path, action);
        public static ToolStripMenuItem AddCheckboxMenuItem(this ToolStrip menu, string path, Action<ToolStripMenuItem> action) => AddCheckboxMenuItem(menu.Items, path, action);
        public static ToolStripMenuItem AddCheckboxMenuItem(this ToolStripMenuItem menu, string path, Action<ToolStripMenuItem> action) => AddCheckboxMenuItem(menu.DropDown, path, action);



        private static ToolStripMenuItem AddMenuItem(this ToolStripItemCollection collection, string path, Action<ToolStripMenuItem> action)
        {
            ToolStripMenuItem item = GetOrCreateMenuItem(collection, path.Split('/'));
            item.Click += (sender, e) => action(item);
            return item;
        }

        private static ToolStripMenuItem AddRadioButtonMenuItem(this ToolStripItemCollection collection, string path, Action<ToolStripMenuItem> action)
        {
            // Create or retrieve the menu item
            ToolStripMenuItem item = GetOrCreateMenuItem(collection, path.Split('/'));

            // Set properties for radio button behavior
            item.CheckOnClick = true;
            item.CheckState = CheckState.Unchecked;

            // Handle click event
            item.Click += (sender, e) =>
            {
                foreach (ToolStripMenuItem sibling in collection.OfType<ToolStripMenuItem>())
                {
                    sibling.Checked = sibling == item;
                }
                action?.Invoke(item);

                // Ensure at least one item is always selected if needed
                if (collection.OfType<ToolStripMenuItem>().All(x => !x.Checked))
                {
                    item.Checked = true; // Re-check the item if none were checked
                }
            };

            // Initially ensure at least one item is checked if needed
            if (!collection.OfType<ToolStripMenuItem>().Any(x => x.Checked))
            {
                item.Checked = true;
                action?.Invoke(item);
            }

            return item;
        }

        private static ToolStripMenuItem AddCheckboxMenuItem(this ToolStripItemCollection collection, string path, Action<ToolStripMenuItem> action)
        {
            ToolStripMenuItem item = GetOrCreateMenuItem(collection, path.Split('/'));
            item.CheckOnClick = true;

            item.Click += (sender, e) =>
            {
                action?.Invoke(item);
            };

            return item;
        }

        private static ToolStripMenuItem GetOrCreateMenuItem(ToolStripItemCollection parentCollection, string[] path)
        {
            ToolStripMenuItem? item = null;

            if (parentCollection.Find(path[0], false).FirstOrDefault() is ToolStripMenuItem existingItem)
                item = existingItem;

            // If item is not found, create a new one
            if (item == null)
            {
                item = new ToolStripMenuItem(path[0]) { Name = path[0] };
                parentCollection.Add(item);
            }

            // Recursively handle submenus if there are more path segments
            if (path.Length > 1)
            {
                return GetOrCreateMenuItem(item.DropDownItems, path.Skip(1).ToArray());
            }

            return item;
        }
    }
}

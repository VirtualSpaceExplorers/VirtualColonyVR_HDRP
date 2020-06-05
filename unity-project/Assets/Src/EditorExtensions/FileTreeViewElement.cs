using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Assets.Src.EditorExtensions
{
    public class FileTreeViewElement : TreeViewItem
    {
        public FileTreeViewElement(int id, int depth, string displayName) : base(id, depth, displayName)
        {

        }

        public string RelativePath { get; set; }

        public bool Synced { get; set; }

        public bool IsFolder { get; set; }
    }
}

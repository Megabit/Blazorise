using System;
using System.Collections.Generic;

namespace Blazorise.TreeView
{
    public class TreeNode
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Text { get; set; }

        public bool IsExpanded { get; set; }
        public List<TreeNode> Children { get; set; }
    }
}


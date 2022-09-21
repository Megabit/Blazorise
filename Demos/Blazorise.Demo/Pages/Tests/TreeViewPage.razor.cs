using System.Collections.Generic;
using Blazorise.TreeView;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class TreeViewPage : ComponentBase
    {
        private TreeNode selectedNode;
        private bool expandAll = true;

        private IEnumerable<TreeNode> Nodes = new List<TreeNode>
        {
            new TreeNode { Text = "NodeInfo 1" },
            new TreeNode
            {
                Text = "NodeInfo 2",
                Children = new List<TreeNode>
                {
                    new TreeNode { Text = "NodeInfo 2.1" },
                    new TreeNode
                    {
                        Text = "NodeInfo 2.2", Children = new List<TreeNode>
                        {
                            new TreeNode { Text = "NodeInfo 2.2.1" },
                            new TreeNode { Text = "NodeInfo 2.2.2" },
                            new TreeNode { Text = "NodeInfo 2.2.3" },
                            new TreeNode { Text = "NodeInfo 2.2.4" }
                        }
                    },
                    new TreeNode { Text = "NodeInfo 2.3" },
                    new TreeNode { Text = "NodeInfo 2.4" }
                }
            },
            new TreeNode { Text = "NodeInfo 3" },
            new TreeNode
            {
                Text = "NodeInfo 4",
                Children = new List<TreeNode>
                {
                    new TreeNode { Text = "NodeInfo 4.1" },
                    new TreeNode
                    {
                        Text = "NodeInfo 4.2", Children = new List<TreeNode>
                        {
                            new TreeNode { Text = "NodeInfo 4.2.1" },
                            new TreeNode { Text = "NodeInfo 4.2.2" },
                            new TreeNode { Text = "NodeInfo 4.2.3" },
                            new TreeNode { Text = "NodeInfo 4.2.4" }
                        }
                    },
                    new TreeNode { Text = "NodeInfo 4.3" },
                    new TreeNode { Text = "NodeInfo 4.4" }
                }
            },
            new TreeNode { Text = "NodeInfo 5" },
            new TreeNode { Text = "NodeInfo 6" }
        };
    }
}
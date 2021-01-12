using System.Collections.Generic;
using PatternsSearchBor.Model;

namespace PatternsSearchBor.Algorithm
{
    public class InverseTreeCreator
    {

        public InverseTreeCreator()
        {
        }

        public Tree Create(Node startNode, int levelToSearch, HashSet<string> excludedNodes)
        {
            List<Node> nodes = startNode.GetLevelNodes(levelToSearch, excludedNodes);
            Tree inv_T = Tree.GetRootedTree();

            foreach (var node in nodes)
            {
                Node curNode = node;
                var addedNode = inv_T.Root;

                do
                {
                    addedNode = addedNode.AddOrGet(curNode.Value);
                    curNode = curNode.Parent;
                } while (curNode != null && curNode.Level > startNode.Level);
            }

            return inv_T;
        }
    }
}

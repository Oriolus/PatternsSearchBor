using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternsSearchBor.Model;

namespace PatternsSearchBor.Algorithm
{
    public abstract class NodeDeduplicator
    {
        public static readonly int UniqueThreshold = 30;
        public Action<string> Print { get; set; }
        public bool LogEnabled { get; set; }

        public abstract void Deduplicate(Node node);

        protected void MergeToTemplate(Node parentNode, string templateNodeName, List<Node> toMerge)
        {
            Node mergeTo = parentNode.AddOrGet(templateNodeName);

            // Если в списке для мержа есть нода, куда мержить - удалить её иначе parentNode.Remove(mergeFrom)
            if (toMerge.Contains(mergeTo)) toMerge.Remove(mergeTo);

            mergeTo.IsTemplateValue = true;
            mergeTo.Print = Print;
            mergeTo.HintCount += (toMerge.Sum(chld => chld.HintCount) - 1);

            // мержим потомков найденных нод, удаляем шаблон-ноду из входящей ноды
            foreach (var mergeFrom in toMerge)
            {
                // Если нода была скопирована, то удаление (Unchain) - ок
                // Если нода была перемещена, то удалять (Unchain) НЕЛЬЗЯ!
                MergeSubtrees(mergeTo, mergeFrom);
                parentNode.Remove(mergeFrom);
            }
        }

        protected void MergeSubtrees(Node mergeTo, Node mergeFrom)
        {
            foreach (var child in mergeFrom.GetChildren())
            {
                // важная деталь. Если нода новая - то просто добавляем (т.к. добавляется со всем поддеревом)
                // если нет - добавляем все поддерево и скланываем hints
                var found = mergeTo.Get(child.Value);
                if (found == null)
                {
                    mergeTo.AddOrGet(child);
                }
                else
                {
                    var _to = mergeTo.AddOrGet(child);
                    MergeSubtrees(_to, child);
                }
            }
        }

        protected int GetTheDeepestNodeLevel(Node node)
        {
            if (node.GetChildren().Count == 0)
                return node.Level;

            int subNodeMaxLevel = 0;
            foreach (var n in node.GetChildren())
            {
                int nodeLevel = GetTheDeepestNodeLevel(n);
                if (nodeLevel > subNodeMaxLevel)
                    subNodeMaxLevel = nodeLevel;
            }
            return subNodeMaxLevel;
        }

    }
}

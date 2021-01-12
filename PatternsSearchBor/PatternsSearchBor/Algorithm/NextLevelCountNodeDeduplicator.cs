using PatternsSearchBor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsSearchBor.Algorithm
{
    public class NextLevelCountNodeDeduplicator : NodeDeduplicator
    {
        public override void Deduplicate(Node node)
        {
            Merge(node);
        }

        private void Merge(Node node)
        {
            // Берем все child node, у которых hints <= порогового
            // Также выясняем, есть ли конец строки среди child node
            var toUnion = node.GetChildren().Where
            (child =>
                child.HintCount <= UniqueThreshold
                &&
                child.Value != Node.LineEnd
                &&
                child.IsTemplateValue != true
            ).ToList();
            var endOfString = node.GetChildren().FirstOrDefault(child => child.Value == Node.LineEnd);

            // Одиночные node не приводим к шаблону
            if (toUnion.Count > 1)
            {
                // Создаем шаблон-ноду
                string newRootNode = GetTemplateNodeName(node, node);
                if (node.IsTemplateValue)
                {
                    Node ancestor = node.GetLastNotTemplateAncestor();
                    if (ancestor != null)
                    {
                        newRootNode = GetTemplateNodeName(ancestor, node);
                    }
                }

                MergeToTemplate(node, newRootNode, toUnion);

                // Если была конец-нода, то добавляем её отдельно
                if (endOfString != null)
                {
                    node.AddOrGet(endOfString);
                }
            }

            // для всех потомков-нод выполняем этот же метод
            List<Node> childs = new List<Node>(node.GetChildren());
            for (int i = 0; i < childs.Count; i++)
            {
                Merge(childs[i]);
            }
        }

        private string GetTemplateNodeName(Node from, Node to)
        {
            return $"!{from.Value}_{to.Level + 1}";
        }

    }
}

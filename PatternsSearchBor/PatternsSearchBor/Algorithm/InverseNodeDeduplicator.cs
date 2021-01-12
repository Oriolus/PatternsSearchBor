using PatternsSearchBor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsSearchBor.Algorithm
{
    public class InverseNodeDeduplicator : NodeDeduplicator
    {
        private readonly InverseTreeCreator InverseTreeCreator = new InverseTreeCreator();

        public override void Deduplicate(Node node)
        {
            Merge(node);
        }

        public void Merge(Node node)
        {
            // Внимание! toList создает копию массива! По возомжности избавиться

            const int HintCountThreashold = 1;
            const int MaxLevelDelta = 7;
            const int MinLevelDelta = 2;

            if (node.HintCount <= HintCountThreashold) return;

            int nodeMaxLevel = GetTheDeepestNodeLevel(node);
            HashSet<string> merged = new HashSet<string>();

            // если детей больше 1 (есть что объединять)
            // и текущий уровень не больше максимальный уровнь - MinLevelDelta
            if (node.GetChildren().Count > 1 && nodeMaxLevel - node.Level >= MinLevelDelta)
            {
                // Нет смысла делать итерации на уровни больше, чем максимальный уровень поддерева
                int maxLevel = Math.Min(nodeMaxLevel, node.Level + MaxLevelDelta) - node.Level;

                for (int curLevelDelta = MinLevelDelta; curLevelDelta < maxLevel + 1; curLevelDelta++)
                {
                    int levelToSearch = node.Level + curLevelDelta;

                    //if (LogEnabled) Console.WriteLine($"Node: {node.Value}. Current level: {node.Level}. Level to search {levelToSearch}");

                    // Достать все ноды с Level = child.Level + LevelDelta
                    // Построить инвесное дерево (invT) от найденных нод до child (по ссылкам на Parent)

                    Tree inv_T = InverseTreeCreator.Create(node, levelToSearch, merged);

                    //if (LogEnabled) inv_T.BeautyLog(Console.WriteLine);

                    // Возьмем всех детей первого уровня дерева invT с HintCount > 2
                    // Т.е. такие ноды levelToSearch уровня прямого дерева, к которым ведут большое количество предков
                    var inv_MostHintedTs = inv_T.Root.GetChildren().Where(chld => chld.HintCount > 2);

                    foreach (var inv_TopNode in inv_MostHintedTs)
                    {
                        // Для каждого найденного инверсного нода
                        // Водзьмем все листовые ноды и выберем все уникальные имена
                        var _nodes = inv_TopNode.GetLeafNodes(false);
                        var nodeNames = new HashSet<string>(_nodes.Select(n => n.Value).Distinct());

                        // Здесь нужно мержить!
                        // Что мержить. Достать все имена нодов, которые нужно мержить
                        // Исключить уже смерженные ноды
                        var toMerge = node.GetChildren().Where(c => nodeNames.Contains(c.Value) && !merged.Contains(c.Value)).OrderBy(n => n.IsTemplateValue).ToList();

                        if (toMerge.Count > 1)
                        {
                            // куда мержить
                            string newRootNode = GetTemplateNodeName(node, inv_TopNode, levelToSearch);
                            if (node.IsTemplateValue)
                            {
                                Node ancestor = node.GetLastNotTemplateAncestor();
                                if (ancestor != null)
                                {
                                    newRootNode = GetTemplateNodeName(ancestor, inv_TopNode, levelToSearch);
                                }
                            }

                            // сохраняем созданную вершину, чтобы они не попала в новые мержи
                            merged.Add(newRootNode);

                            MergeToTemplate(node, newRootNode, toMerge);
                        }
                    }
                }
            }

            // toList делает array.copy!
            var children = node.GetChildren().Where(c => c.HintCount > HintCountThreashold).ToList();
            for (int i = 0; i < children.Count; i++)
            {
                Merge(children[i]);
            }
        }

        private string GetTemplateNodeName(Node from, Node inv_TopNode, int levelToSearch)
        {
            return $"!{from.Value}({from.Level})_{inv_TopNode.Value}({levelToSearch})";
        }

    }
}

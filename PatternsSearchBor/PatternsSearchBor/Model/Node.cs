using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PatternsSearchBor.Model
{
    public class Node : ICloneable
    {
        private const string NodeIsTemplatePattern = @"\![^\s]+_[0-9]+";

        public static bool IsTemplateNode(string value)
        {
            return Regex.IsMatch(value, NodeIsTemplatePattern);
        }

        public static readonly string LineBegin = "^";
        public static readonly string LineEnd = "$";

        public string Value { get; set; }
        public Node Parent { get; private set; }
        public int Level { get; set; }
        public bool IsTemplateValue { get; set; } = false;
        public Node LastTemplateParent { get; set; }

        public int HintCount { get; set; }

        private Dictionary<string, Node> Children;

        public Action<string> Print;

        public Node(Node parent)
        {
            Children = new Dictionary<string, Node>();
            Parent = parent;
            if (Parent != null)
            {
                Level = Parent.Level + 1;
            }
        }

        public Node(string value, Node parent) : this(parent)
        {
            Value = value;
        }

        internal List<Node> GetLevelNodes(int levelToSearch, HashSet<string> excludeNodes)
        {
            List<Node> nodes = new List<Node>();
            AddToNodeLevelList(levelToSearch, nodes, excludeNodes);
            return nodes;
        }

        private void AddToNodeLevelList(int levelToSearch, List<Node> list, HashSet<string> excludeNodes)
        {
            if (Level == levelToSearch)
            {
                list.Add(this);
            }
            else
            {
                foreach (var child in GetChildren().Where(c => !excludeNodes.Contains(c.Value)))
                {
                    child.AddToNodeLevelList(levelToSearch, list, excludeNodes);
                }
            }
        }

        /// <summary>
        /// Получить листовые ноды с учетом или без конца строки
        /// </summary>
        /// <param name="checkLineEnd">Учитывать символ конца строки $</param>
        /// <returns></returns>
        internal List<Node> GetLeafNodes(bool checkLineEnd = false)
        {
            List<Node> nodes = new List<Node>(HintCount);
            AddLeafNodes(checkLineEnd, nodes);
            return nodes;
        }
        
        private void AddLeafNodes(bool checkLineEnd, List<Node> nodes)
        {
            if (!checkLineEnd && GetChildren().Count == 0)
            {
                nodes.Add(this);
                return;
            }
            else if (checkLineEnd && GetChildren().Any(child => child.Value == LineEnd))
            {
                nodes.Add(this);
            }
            foreach (var child in GetChildren())
            {
                child.AddLeafNodes(checkLineEnd, nodes);
            }
        }

        public Node AddOrGet(string value)
        {
            Node result;

            if (!Children.TryGetValue(value, out result))
            {
                result = new Node(value, this);
                result.HintCount = 1;
                Children.Add(value, result);
            } 
            else
            {
                result.HintCount += 1;
            }

            return result;
        }

        public Node AddOrGet(Node node)
        {
            /* -------------------------------------------------------------------
             * Добавляем готовую ноду к текущему
             * * если у текущего нода добавляемая нода не найдена, то
             * * сетим родителя, уровень, добавляем в коллекцию потомков
             * * если нода найдена
             ------------------------------------------------------------------- */
            Node result;

            if (!Children.TryGetValue(node.Value, out result))
            {
                node.Parent = this;
                node.Level = this.Level + 1;
                Children.Add(node.Value, node);
                result = node;
            }
            else
            {
                node.Parent = this;
                node.Level = this.Level + 1;
                result.HintCount += node.HintCount;
            }

            return result;
        }

        public Node Get(string value)
        {
            Node result;
            Children.TryGetValue(value, out result);
            return result;
        }

        public Node Remove(Node node)
        {
            Node removed;
            if (Children.TryGetValue(node.Value, out removed))
            {
                Children.Remove(node.Value);
            }
            return removed;
        }

        public ICollection<Node> GetChildren()
        {
            return Children.Values;
        }

        public object Clone()
        {
            return null;
        }

        public override string ToString()
        {
            return $"{Value}({HintCount})";
        }

        public void Log(Action<string> logger, string parentPart)
        {
            if (Children.Count > 0)
            {
                foreach (var child in Children.Values)
                {
                    child.Log(logger, $"{parentPart} {Value}");
                }
            }
            else
            {
                logger.Invoke($"{HintCount}\t\t{parentPart} {Value}");
            }
        }

        public void Log(Action<string> logger, string parentPart, long bytes)
        {
            if (Children.Count > 0)
            {
                foreach (var child in Children.Values)
                {
                    child.Log(logger, $"{parentPart} {Value}", bytes + (IsTemplateValue ? 0 : Value.Length * 2));
                }
            }
            else
            {
                logger.Invoke($"{HintCount}\t{bytes * HintCount}\t{parentPart} {Value}");
            }
        }

        public void Log(string parentPart)
        {
            if (Children.Count > 0)
            {
                //Print(parentPart + " " + ToString());
                foreach (var child in Children.Values)
                {
                    child.Log(parentPart + " " + ToString());
                }
            }
            else
            {
                //Console.WriteLine(ToString());
                Print(parentPart + " " + ToString());
            }
        }

        public void Log(StringBuilder buffer, string prefix, string childrenPrefix)
        {
            buffer.Append(prefix);
            buffer.Append(Value);
            buffer.Append($" (H:{HintCount};L:{Level})");

            ICollection<Node> childs = GetChildren().Where(child => child.HintCount > 0).ToList<Node>();

            buffer.AppendLine();

            int currentChild = 0;
            foreach (var child in childs)
            {
                if (currentChild != childs.Count - 1)
                {
                    child.Log(buffer, childrenPrefix + "├── ", childrenPrefix + "│   ");
                }
                else
                {
                    child.Log(buffer, childrenPrefix + "└── ", childrenPrefix + "    ");
                }
                currentChild++;
            }
        }

        public Node GetLastNotTemplateAncestor()
        {
            Node parent = Parent;

            while (parent != null && parent.IsTemplateValue == true)
            {
                parent = parent.Parent;
            }

            return parent;
        }

        public void Unchain()
        {
            foreach (var child in GetChildren())
            {
                child.Unchain();
            }
            Children.Clear();
            LastTemplateParent = null;
            Parent = null;
        }

        public override bool Equals(object obj)
        {
            return obj is Node node &&
                   Value == node.Value &&
                   EqualityComparer<Node>.Default.Equals(Parent, node.Parent) &&
                   Level == node.Level;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternsSearchBor.Model
{
    public class Tree
    {
        public bool LogEnabled { get; set; }

        public static Tree GetRootedTree()
        {
            Tree tree = new Tree();
            tree.Root = new Node(Node.LineBegin, null);
            tree.Root.HintCount = int.MaxValue;

            return tree;
        }

        public static readonly int UniqueThreshold = 30;

        public Node Root { get; set; }

        public Action<string> Print;

        public void Log(Action<string> logger)
        {
            Root.Log(logger, "");
        }

        public void LogBytes(Action<string> logger)
        {
            Root.Log(logger, "", 0);
        }

        public void BeautyLog()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Root.Log(stringBuilder, "", "");
            Print(stringBuilder.ToString());
        }

        public void BeautyLog(Action<string> logger)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Root.Log(stringBuilder, "", "");
            logger.Invoke(stringBuilder.ToString());
        }

    }
}


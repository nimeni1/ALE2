using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALE2
{
    public class Node
    {
        public Node RightChild { get; set; }
        public Node LeftChild { get; set; }
        public Node Parent { get; set; }
        public String Symbol { get; set; }
        public String Label { get; set; }
        public Node(String symbol)
        {
            this.RightChild = null;
            this.LeftChild = null;
            this.Parent = null;
            this.Symbol = symbol;
            this.Label = null;
        }
    }
}
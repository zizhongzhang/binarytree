using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryTree.Program
{
    class Node
    {
        // Stack used to solve for a given tree.
        private Stack<string> stack = new Stack<string>();

        // Solves a tree
        public int Solve()
        {
            /* This method uses a stack to solve the expression. The postfix
             * notation is tokenized and systematically added to the stack.
             * When the stack encounters an operation, it is executed and
             * modifies the contents on stack. The final item left on the 
             * stack (given the expression was valid) is the answer.
             */
            string a, b; // Temporary placeholders for popped values
            string[] tokens = Postfix().Split(' '); // Tokenize the postfix output
            foreach (string e in tokens)
            {
                switch (e)
                {
                    /* For operation cases, the last two items added to the stack are
                     * removed and acted upon. For any other case, the value is pushed
                     * onto the stack.
                     */
                    case "+":
                        b = stack.Pop();
                        a = stack.Pop();
                        stack.Push(Convert.ToString(Convert.ToInt16(a) + Convert.ToInt16(b)));
                        break;
                    case "-":
                        b = stack.Pop();
                        a = stack.Pop();
                        stack.Push(Convert.ToString(Convert.ToInt16(a) - Convert.ToInt16(b)));
                        break;
                    case "/":
                        b = stack.Pop();
                        a = stack.Pop();
                        stack.Push(Convert.ToString(Convert.ToInt16(a) / Convert.ToInt16(b)));
                        break;
                    case "*":
                        b = stack.Pop();
                        a = stack.Pop();
                        stack.Push(Convert.ToString(Convert.ToInt16(a) * Convert.ToInt16(b)));
                        break;
                    case "%":
                        b = stack.Pop();
                        a = stack.Pop();
                        stack.Push(Convert.ToString(Convert.ToInt16(a) % Convert.ToInt16(b)));
                        break;
                    default:
                        stack.Push(e);
                        break;
                }
            }
            // Value left over is the result of the expression
            return Convert.ToInt16(stack.Pop());
        }

        // Returns the prefix notation for the expression
        public string Prefix()
        {
            /* Function recurses through the left then right
             * nodes after its value.
             */
            string res = this.Value + " ";
            if (this.left != null) // If node is not a leaf, then recurse
            {
                res += this.left.Prefix();
                res += this.right.Prefix();
            }
            return res;
        }

        // Returns the postfix notation for the expression
        public string Postfix()
        {
            /* Function recurses through the left then right,
             * bottom-up. All leafs are returned before their
             * parent operators.
             */
            string res = "";
            if (this.left != null) // If node is not a leaf, then recurse
            {
                res += this.left.Postfix() + " ";
                res += this.right.Postfix() + " ";
            }
            res += this.Value;
            return res;
        }

        // Returns the (fully parenthesized) infix notation for the expression
        public string Infix()
        {
            /* Function recurses through left, then returns
             * value, and recurses right. Each expression is
             * nested in parentheses.
             */
            string res = "";
            if (this.left != null)
            {
                res = res + "(" + left.Infix() + " " + Value + " " + right.Infix() + ")";
            }
            else
            {
                res += Value;
            }
            return res;
        }

        // Constructor for subnodes
        public Node(char op, Node l, Node r)
        {
            left = l;
            right = r;
            Value = op.ToString();
        }
        // Constructor for leaf nodes
        public Node(string value)
        {
            // Leaf nodes have no left or right subnodes
            left = null;
            right = null;
            Value = value;
        }

        // Node connected on the left
        private Node left;
        // Node connected on the right
        private Node right;
        // Value (operator or term)
        private string Value;
    }

    // Sample program:
    class Program
    {
        /* The code below demonstrates the use of the Node class. The expression being
         * tested is graphed as shown below. (Make sure you're using a monospace font)
         * (((1-2)-3) + (4*(5+6)))
         *        +
         *      /   \
         *     -     *
         *    / \   / \
         *   -   3 4   +
         *  / \       / \
         * 1   2     5   6
        */

         void Main(string[] args)
        {
            Node root = new Node('+', new Node('-', new Node('-', new Node("1"), new Node("2")), new Node("3")),
                                      new Node('*', new Node("4"), new Node('+', new Node("5"), new Node("6"))));
            Console.WriteLine("Prefix notation: \t" + root.Prefix());
            Console.WriteLine("Postfix notation: \t" + root.Postfix());
            Console.WriteLine("Infix notation: \t" + root.Infix());
            Console.WriteLine("Solution for tree is:\t" + root.Solve());
            Console.ReadKey(true);
        }
    }
}

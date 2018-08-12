using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryTree.Program3
{
    class Program3
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
         void Main()
        {
            Node root = new Node(Token.Add, new Node(Token.Substract, new Node(Token.Substract, new Node(1), new Node(2)), new Node(3)),
                                      new Node(Token.Multiply, new Node(4), new Node(Token.Add, new Node(5), new Node(6))));
            var tree = new Tree();
            Console.WriteLine(tree.Solve(root));
            Console.ReadKey();
        }
    }

    public class Tree
    {
        public int Solve(Node node)
        {
            if (node.HasToken)
            {
                int left = Solve(node.Left);
                int right = Solve(node.Right);
                switch (node.Token)
                {
                    case Token.Add:
                        return left + right;
                    case Token.Substract:
                        return left - right;
                    case Token.Multiply:
                        return left * right;
                    default:
                        break;
                }
            }
            return node.Value;
        }
    }

    public class Node
    {
        public Node Left, Right;
        public int Value;
        public Token Token;
        public bool HasToken => Value == 0;

        public Node(Token token)
        {
            Token = token;
        }
        public Node(int val)
        {
            Value = val;
        }
        public Node(Token token, Node node1, Node node2) : this(token)
        {
            Left = node1;
            Right = node2;
        }
    }

    public enum Token
    {
        Add = 1,
        Substract,
        Multiply
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryTree.Program3Revision
{
    class Program3Revision
    {
        /* The code below demonstrates the use of the Node class. The expression being
        * tested is graphed as shown below. (Make sure you're using a monospace font)
        * A and B or (C or D AND E)
        *        OR
        *      /    \
        *    AND     OR
        *    / \     / \
        *   0   1   0   AND
        *               /  \
        *              1    1
        */
        void Main()
        {
            Node root = new Node(Token.Or, new Node(Token.And, new Node(false), new Node(true)),
                                           new Node(Token.Or, new Node(false), new Node(Token.And, new Node(true), new Node(false))));
            var tree = new Tree();
            Console.WriteLine(tree.Solve(root));
            Console.ReadKey();
        }
    }

    public class Tree
    {
        public bool Solve(Node node)
        {
            if (node.HasToken)
            {
                var left = Solve(node.Left);
                var right = Solve(node.Right);
                switch (node.Token)
                {
                    case Token.And:
                        return left && right;
                    case Token.Or:
                        return left || right;
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
        public bool Value;
        public Token Token;
        public bool HasToken;

        public Node(Token token)
        {
            Token = token;
            HasToken = true;
        }
        public Node(bool val)
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
        And,
        Or
    }
}

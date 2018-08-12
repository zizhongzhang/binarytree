using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BinaryTree.Program2
{
    class Program2
    {
         void Main(string[] args)
        {
            Console.WriteLine(Evaluate("(4+8)*2"));
            //before
            //"c_orgregion"=="au" && "c_count">1
            //after
            //bool val = "au" == "au" && 1 > 1;
            var str = "'au' == 'au' && 1 > 1";
            var validation = bool.Parse(str);
            Console.WriteLine(IsTraitRealized("c_orgregion", "==", _signals));
            Console.ReadKey();
        }
        public static double Evaluate(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }
        public bool IsTraitRealized()
        {
            var trait = new Dictionary<string, string>();
            trait.Add("c_orgregion", "au");
            var signals = new Dictionary<string, string>();
            signals.Add("c_orgregion", "au");
            return signals[trait.Keys.First()] == trait.Values.First();
        }
        public static bool IsTraitRealized(string key, string @operator, IDictionary<string, string> signals)
        {
            if (@operator == "==")
                return signals[key] == _traits[key];
            return signals[key] == _traits[key];
        }
        private static Dictionary<string, string> _traits = new Dictionary<string, string> { { "c_orgregion", "au" } };
        private static Dictionary<string, string> _signals = new Dictionary<string, string> { { "c_orgregion", "jp" } };
    }


    public class Node
    {
        public bool Expression;
        public string Any;
        public bool value;
        public Node Left;
        public Node Right;
        public bool IsLeaf => Left == null && Right == null;
    }


}

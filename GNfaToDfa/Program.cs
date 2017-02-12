using System;

namespace GNfaToDfa
{
    class Program
    {
        static void Main(string[] args)
        {
            Test2();
            //TestNFA.TestMain(args);
        }

        static void Test2()
        {
            RegularExpression a = new Literal("A");
            RegularExpression d = new Literal("D");
            RegularExpression n = new Literal("N");
            RegularExpression e = new Literal("E");
            RegularExpression r = new Literal("R");
            RegularExpression o = new Literal("O");
            RegularExpression t = new Literal("T");
            RegularExpression comma = new Literal(",");

            //RegularExpression notComma = new Alternation(a, d, n, e, r, o, t);
            RegularExpression notComma = new Literal("ADNEROT");
            RegularExpression notCommaStar = new Concatenation(notComma, new KleeneStar(notComma));
            RegularExpression and = new Concatenation(a, n, d);
            RegularExpression near = new Concatenation(n, e, a, r);
            RegularExpression not = new Concatenation(n, o, t);
            RegularExpression or = new Concatenation(o, r);

            RegularExpression blah = new Alternation(near, not, and, or, comma, notCommaStar);

            Nfa nfa = blah.MakeNfa(new Nfa.NameSource());
            Console.WriteLine(nfa);
            Console.WriteLine("-------------------------------");
            Dfa dfa = nfa.ToDfa();
            Console.WriteLine(dfa);
            dfa.WriteDot("lol.dot");
        }
    }
}

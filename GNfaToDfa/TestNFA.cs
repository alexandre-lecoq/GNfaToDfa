using System;

namespace GNfaToDfa
{
    // Trying the RE->NFA->DFA translation on three regular expressions

    class TestNFA
    {
        public static void TestMain(String[] args)
        {
            RegularExpression a = new Literal("A");
            RegularExpression b = new Literal("B");
            RegularExpression c = new Literal("C");
            RegularExpression abStar = new KleeneStar(new Alternation(a, b));
            RegularExpression bb = new Concatenation(b, b);
            RegularExpression r = new Concatenation(abStar, new Concatenation(a, b));
            // The regular expression (a|b)*ab
            BuildAndShow("dfa1.dot", r);
            // The regular expression ((a|b)*ab)*
            BuildAndShow("dfa2.dot", new KleeneStar(r));
            // The regular expression ((a|b)*ab)((a|b)*ab)
            BuildAndShow("dfa3.dot", new Concatenation(r, r));
            // The regular expression (a|b)*abb, from ASU 1986 p 136
            BuildAndShow("dfa4.dot", new Concatenation(abStar, new Concatenation(a, bb)));
            // SML reals: sign?((digit+(\.digit+)?))([eE]sign?digit+)?
            RegularExpression d = new Literal("digit");
            RegularExpression dPlus = new Concatenation(d, new KleeneStar(d));
            RegularExpression s = new Literal("sign");
            RegularExpression sOpt = new Alternation(s, new Epsilon());
            RegularExpression dot = new Literal(".");
            RegularExpression dotDigOpt = new Alternation(new Epsilon(), new Concatenation(dot, dPlus));
            RegularExpression mant = new Concatenation(sOpt, new Concatenation(dPlus, dotDigOpt));
            RegularExpression e = new Literal("e");
            RegularExpression exp = new Alternation(new Epsilon(), new Concatenation(e, new Concatenation(sOpt, dPlus)));
            RegularExpression smlReal = new Concatenation(mant, exp);
            BuildAndShow("dfa5.dot", smlReal);
        }

        public static void BuildAndShow(String filename, RegularExpression r)
        {
            Nfa nfa = r.MakeNfa(new Nfa.NameSource());
            Console.WriteLine(nfa);
            Console.WriteLine("---");
            Dfa dfa = nfa.ToDfa();
            Console.WriteLine(dfa);
            Console.WriteLine("Writing DFA graph to file " + filename);
            dfa.WriteDot(filename);
            Console.WriteLine();
        }
    }
}

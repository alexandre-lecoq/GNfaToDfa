using System;

namespace GNfaToDfa
{
    class Literal : RegularExpression
    {
        String sym;

        public Literal(String sym)
        {
            this.sym = sym;
        }

        // The resulting nfa0 has form s0s -sym-> s0e

        public override Nfa MakeNfa(Nfa.NameSource names)
        {
            int s0s = names.next();
            int s0e = names.next();
            Nfa nfa0 = new Nfa(s0s, s0e);
            nfa0.AddTrans(s0s, sym, s0e);
            return nfa0;
        }
    }
}

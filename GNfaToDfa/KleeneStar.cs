using System.Collections.Generic;

namespace GNfaToDfa
{
    class KleeneStar : RegularExpression
    {
        RegularExpression r;

        public KleeneStar(RegularExpression r)
        {
            this.r = r;
        }

        // If   nfa1 has form s1s ----> s1e 
        // then nfa0 has form s0s ----> s0s
        //                    s0s -eps-> s1s
        //                    s1e -eps-> s0s

        public override Nfa MakeNfa(Nfa.NameSource names)
        {
            Nfa nfa1 = r.MakeNfa(names);
            int s0s = names.next();
            Nfa nfa0 = new Nfa(s0s, s0s);
            foreach (KeyValuePair<int, List<Transition>> entry in nfa1.Trans)
                nfa0.AddTrans(entry);
            nfa0.AddTrans(s0s, null, nfa1.Start);
            nfa0.AddTrans(nfa1.Exit, null, s0s);
            return nfa0;
        }
    }
}

namespace GNfaToDfa
{
    class Epsilon : RegularExpression
    {
        // The resulting nfa0 has form s0s -eps-> s0e

        public override Nfa MakeNfa(Nfa.NameSource names)
        {
            int s0s = names.next();
            int s0e = names.next();
            Nfa nfa0 = new Nfa(s0s, s0e);
            nfa0.AddTrans(s0s, null, s0e);
            return nfa0;
        }
    }
}

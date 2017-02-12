namespace GNfaToDfa
{
    using System;

    class Alternation : RegularExpression
    {
        RegularExpression[] _regularExpressions;

        public Alternation(params RegularExpression[] regularExpressions)
        {
            if (regularExpressions.Length < 2)
            {
                throw new ArgumentException("Constructor needs at least 2 parameters.", "regularExpressions");
            }

            _regularExpressions = regularExpressions;
        }

        // If   nfa1 has form s1s ----> s1e 
        // and  nfa2 has form s2s ----> s2e 
        // then nfa0 has form s0s -eps-> s1s ----> s1e -eps-> s0e
        //                    s0s -eps-> s2s ----> s2e -eps-> s0e

        public override Nfa MakeNfa(Nfa.NameSource names)
        {
            var s0s = names.next();
            var s0e = names.next();

            var nfa0 = new Nfa(s0s, s0e);

            foreach (var regularExpression in _regularExpressions)
            {
                var nfa1 = regularExpression.MakeNfa(names);

                foreach (var entry in nfa1.Trans)
                {
                    nfa0.AddTrans(entry);
                }

                nfa0.AddTrans(s0s, null, nfa1.Start);
                nfa0.AddTrans(nfa1.Exit, null, s0e);
            }

            return nfa0;
        }
    }
}

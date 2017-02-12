using System.Linq;

namespace GNfaToDfa
{
    using System;

    class Concatenation : RegularExpression
    {
        RegularExpression[] _regularExpressions;

        public Concatenation(params RegularExpression[] regularExpressions)
        {
            if (regularExpressions.Length < 2)
            {
                throw new ArgumentException("Constructor needs at least 2 parameters.", "regularExpressions");
            }

            _regularExpressions = regularExpressions;
        }

        // If   nfa1 has form s1s ----> s1e 
        // and  nfa2 has form s2s ----> s2e 
        // then nfa0 has form s1s ----> s1e -eps-> s2s ----> s2e

        //public override Nfa MakeNfa(Nfa.NameSource names)
        //{
        //    var nfa1 = _regularExpressions[0].MakeNfa(names);
        //    var nfa2 = _regularExpressions[1].MakeNfa(names);

        //    var nfa0 = new Nfa(nfa1.Start, nfa2.Exit);

        //    foreach (var entry in nfa1.Trans)
        //        nfa0.AddTrans(entry);

        //    foreach (var entry in nfa2.Trans)
        //        nfa0.AddTrans(entry);

        //    nfa0.AddTrans(nfa1.Exit, null, nfa2.Start);

        //    return nfa0;
        //}

        public override Nfa MakeNfa(Nfa.NameSource names)
        {
            var inputNfas = _regularExpressions.Select(e => e.MakeNfa(names)).ToArray();

            var firstNfa = inputNfas[0];
            var lastNfa = inputNfas[inputNfas.Length - 1];

            var nfa0 = new Nfa(firstNfa.Start, lastNfa.Exit);

            Nfa previousNfa = null;

            foreach (var nfa in inputNfas)
            {
                foreach (var entry in nfa.Trans)
                {
                    nfa0.AddTrans(entry);
                }

                if (previousNfa != null)
                {
                    nfa0.AddTrans(previousNfa.Exit, null, nfa.Start);
                }

                previousNfa = nfa;
            }

            return nfa0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GNfaToDfa
{


    // Class Dfa, deterministic finite automata --------------------------

    /*
      A deterministic finite automaton (DFA) is represented as a Map
      from state number (int) to a Map from label (a String,
      non-null) to a target state (an int).  
    */

    class Dfa
    {
        private readonly int startState;
        private readonly Set<int> acceptStates;
        private readonly IDictionary<int, IDictionary<String, int>> trans;

        public Dfa(int startState, Set<int> acceptStates,
               IDictionary<int, IDictionary<String, int>> trans)
        {
            this.startState = startState;
            this.acceptStates = acceptStates;
            this.trans = trans;
        }

        public int Start { get { return startState; } }

        public Set<int> Accept { get { return acceptStates; } }

        public IDictionary<int, IDictionary<String, int>> Trans
        {
            get { return trans; }
        }

        public override String ToString()
        {
            return string.Format("DFA start={0}\naccept={1}", startState, acceptStates);
        }

        //public Dfa ToMinimizedDfa()
        //{
        //    var startStates = trans.Select(t => t.Key);
        //    var endStates = trans.SelectMany(t => t.Value.Values);
        //    var states = new HashSet<int>(startStates.Union(endStates).Distinct());

        //    var Sigma = trans.SelectMany(a => a.Value.Keys).ToArray();

        //    var s1 = new HashSet<int>(acceptStates);
        //    states.ExceptWith(s1);

        //    var P = new Stack<HashSet<int>>();
        //    P.Push(s1);
        //    P.Push(states);

        //    var W = new Stack<HashSet<int>>();
        //    W.Push(s1);

        //    while (W.Count > 0)
        //    {
        //        var set = W.Pop();

        //        foreach (var symbol in Sigma)
        //        {
        //            //trans.Where(t => t.Value.ContainsKey(symbol)).Select(c => c.Value)
        //        }
        //    }
        //}

        // Write an input file for the dot program.  You can find dot at
        // http://www.research.att.com/sw/tools/graphviz/
        public void WriteDot(String filename)
        {
            TextWriter wr = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));

            wr.WriteLine("// Format this file as a Postscript file with ");
            wr.WriteLine("//    dot {0} -Tps -o out.ps\n", filename);
            wr.WriteLine("digraph dfa {");
            wr.WriteLine("size=\"11,8.25\";");
            wr.WriteLine("graph[dpi = 200];");
            wr.WriteLine("rankdir=LR;");
            wr.WriteLine("n999999 [style=invis];");    // Invisible start node
            wr.WriteLine("n999999 -> n{0}", startState); // Edge into start state

            // Accept states are double circles
            foreach (int state in trans.Keys)
                if (acceptStates.Contains(state))
                    wr.WriteLine("n{0} [peripheries=2];", state);

            // The transitions 
            foreach (KeyValuePair<int, IDictionary<String, int>> entry in trans)
            {
                int s1 = entry.Key;
                foreach (KeyValuePair<String, int> s1Trans in entry.Value)
                {
                    String lab = s1Trans.Key;
                    int s2 = s1Trans.Value;
                    wr.WriteLine("n{0} -> n{1} [label=\"{2}\"];", s1, s2, lab);
                }
            }
            wr.WriteLine("}");
            wr.Close();
        }
    }
}

using System;
using System.Collections.Generic;

namespace GNfaToDfa
{
    // Regular expressions, NFAs, DFAs, and dot graphs
    // sestoft@itu.dk 
    // Java 2001-07-10 * C# 2001-10-22 * Gen C# 2001-10-23, 2003-09-03

    // In the Generic C# 2.0 version we 
    //  use Queue<int> and Queue<Set<int>> for worklists
    //  use Set<int> for pre-DFA states
    //  use List<Transition> for NFA transition relations
    //  use Dictionary<Set<int>, Dictionary<String, Set<int>>>
    //  and Dictionary<int, Dictionary<String, int>> for DFA transition relations

    /* Class Nfa and conversion from NFA to DFA ---------------------------

      A nondeterministic finite automaton (NFA) is represented as a
      Map from state number (int) to a List of Transitions, a
      Transition being a pair of a label lab (a String, null meaning
      epsilon) and a target state (an int).

      A DFA is created from an NFA in two steps:

        (1) Construct a DFA whose each of whose states is composite,
            namely a set of NFA states (Set of int).  This is done by
            methods CompositeDfaTrans and EpsilonClose.

        (2) Replace composite states (Set of int) by simple states
            (int).  This is done by methods Rename and MkRenamer.

      Method CompositeDfaTrans works as follows: 

        Create the epsilon-closure S0 (a Set of ints) of the start
        state s0, and put it in a worklist (a Queue).  Create an
        empty DFA transition relation, which is a Map from a
        composite state (an epsilon-closed Set of ints) to a Map
        from a label (a non-null String) to a composite state.

        Repeatedly choose a composite state S from the worklist.  If it is
        not already in the keyset of the DFA transition relation, compute
        for every non-epsilon label lab the set T of states reachable by
        that label from some state s in S.  Compute the epsilon-closure
        Tclose of every such state T and put it on the worklist.  Then add
        the transition S -lab-> Tclose to the DFA transition relation, for
        every lab.

      Method EpsilonClose works as follows: 

        Given a set S of states.  Put the states of S in a worklist.
        Repeatedly choose a state s from the worklist, and consider all
        epsilon-transitions s -eps-> s' from s.  If s' is in S already,
        then do nothing; otherwise add s' to S and the worklist.  When the
        worklist is empty, S is epsilon-closed; return S.

      Method MkRenamer works as follows: 

        Given a Map from Set of int to something, create an
        injective Map from Set of int to int, by choosing a fresh
        int for every value of the map.

      Method Rename works as follows:

        Given a Map from Set of int to Map from String to Set of
        int, use the result of MkRenamer to replace all Sets of ints
        by ints.

    */


    class Nfa
    {
        private readonly int startState;
        private readonly int exitState;    // This is the unique accept state
        private readonly IDictionary<int, List<Transition>> trans;

        public Nfa(int startState, int exitState)
        {
            this.startState = startState; this.exitState = exitState;
            trans = new Dictionary<int, List<Transition>>();
            if (!startState.Equals(exitState))
                trans.Add(exitState, new List<Transition>());
        }

        public int Start { get { return startState; } }

        public int Exit { get { return exitState; } }

        public IDictionary<int, List<Transition>> Trans
        {
            get { return trans; }
        }

        public void AddTrans(int s1, String lab, int s2)
        {
            List<Transition> s1Trans;
            if (trans.ContainsKey(s1))
                s1Trans = trans[s1];
            else
            {
                s1Trans = new List<Transition>();
                trans.Add(s1, s1Trans);
            }
            s1Trans.Add(new Transition(lab, s2));
        }

        public void AddTrans(KeyValuePair<int, List<Transition>> tr)
        {
            // Assumption: if tr is in trans, it maps to an empty list (end state)
            trans.Remove(tr.Key);
            trans.Add(tr.Key, tr.Value);
        }

        public override String ToString()
        {
            return "NFA start=" + startState + " exit=" + exitState;
        }

        // Construct the transition relation of a composite-state DFA
        // from an NFA with start state s0 and transition relation
        // trans (a Map from int to List of Transition).  The start
        // state of the constructed DFA is the epsilon closure of s0,
        // and its transition relation is a Map from a composite state
        // (a Set of ints) to a Map from label (a String) to a
        // composite state (a Set of ints).

        static IDictionary<Set<int>, IDictionary<String, Set<int>>>
          CompositeDfaTrans(int s0, IDictionary<int, List<Transition>> trans)
        {
            Set<int> S0 = EpsilonClose(new Set<int>(s0), trans);
            Queue<Set<int>> worklist = new Queue<Set<int>>();
            worklist.Enqueue(S0);
            // The transition relation of the DFA
            IDictionary<Set<int>, IDictionary<String, Set<int>>> res =
              new Dictionary<Set<int>, IDictionary<String, Set<int>>>();
            while (worklist.Count != 0)
            {
                Set<int> S = worklist.Dequeue();
                if (!res.ContainsKey(S))
                {
                    // The S -lab-> T transition relation being constructed for a given S
                    IDictionary<String, Set<int>> STrans =
                  new Dictionary<String, Set<int>>();
                    // For all s in S, consider all transitions s -lab-> t
                    foreach (int s in S)
                    {
                        // For all non-epsilon transitions s -lab-> t, add t to T
                        foreach (Transition tr in trans[s])
                        {
                            if (tr.lab != null)
                            {       // Already a transition on lab
                                Set<int> toState;
                                if (STrans.ContainsKey(tr.lab))
                                    toState = STrans[tr.lab];
                                else
                                {                    // No transitions on lab yet
                                    toState = new Set<int>();
                                    STrans.Add(tr.lab, toState);
                                }
                                toState.Add(tr.target);
                            }
                        }
                    }
                    // Epsilon-close all T such that S -lab-> T, and put on worklist
                    Dictionary<String, Set<int>> STransClosed =
                      new Dictionary<String, Set<int>>();
                    foreach (KeyValuePair<String, Set<int>> entry in STrans)
                    {
                        Set<int> Tclose = EpsilonClose(entry.Value, trans);
                        STransClosed.Add(entry.Key, Tclose);
                        worklist.Enqueue(Tclose);
                    }
                    res.Add(S, STransClosed);
                }
            }
            return res;
        }

        // Compute epsilon-closure of state set S in transition relation trans.  

        static Set<int>
          EpsilonClose(Set<int> S, IDictionary<int, List<Transition>> trans)
        {
            // The worklist initially contains all S members
            Queue<int> worklist = new Queue<int>(S);
            Set<int> res = new Set<int>(S);
            while (worklist.Count != 0)
            {
                int s = worklist.Dequeue();
                foreach (Transition tr in trans[s])
                {
                    if (tr.lab == null && !res.Contains(tr.target))
                    {
                        res.Add(tr.target);
                        worklist.Enqueue(tr.target);
                    }
                }
            }
            return res;
        }

        // Compute a renamer, which is a Map from Set of int to int

        static IDictionary<Set<int>, int> MkRenamer(ICollection<Set<int>> states)
        {
            IDictionary<Set<int>, int> renamer = new Dictionary<Set<int>, int>();
            int count = 0;
            foreach (Set<int> k in states)
                renamer.Add(k, count++);
            return renamer;
        }

        // Using a renamer (a Map from Set of int to int), replace
        // composite (Set of int) states with simple (int) states in
        // the transition relation trans, which is assumed to be a Map
        // from Set of int to Map from String to Set of int.  The
        // result is a Map from int to Map from String to int.

        static IDictionary<int, IDictionary<String, int>>
          Rename(IDictionary<Set<int>, int> renamer,
                 IDictionary<Set<int>, IDictionary<String, Set<int>>> trans)
        {
            IDictionary<int, IDictionary<String, int>> newtrans =
              new Dictionary<int, IDictionary<String, int>>();
            foreach (KeyValuePair<Set<int>, IDictionary<String, Set<int>>> entry
                 in trans)
            {
                Set<int> k = entry.Key;
                IDictionary<String, int> newktrans = new Dictionary<String, int>();
                foreach (KeyValuePair<String, Set<int>> tr in entry.Value)
                    newktrans.Add(tr.Key, renamer[tr.Value]);
                newtrans.Add(renamer[k], newktrans);
            }
            return newtrans;
        }

        static Set<int> AcceptStates(ICollection<Set<int>> states,
                         IDictionary<Set<int>, int> renamer,
                         int exit)
        {
            Set<int> acceptStates = new Set<int>();
            foreach (Set<int> state in states)
                if (state.Contains(exit))
                    acceptStates.Add(renamer[state]);
            return acceptStates;
        }

        public Dfa ToDfa()
        {
            IDictionary<Set<int>, IDictionary<String, Set<int>>>
              cDfaTrans = CompositeDfaTrans(startState, trans);
            Set<int> cDfaStart = EpsilonClose(new Set<int>(startState), trans);
            ICollection<Set<int>> cDfaStates = cDfaTrans.Keys;
            IDictionary<Set<int>, int> renamer = MkRenamer(cDfaStates);
            IDictionary<int, IDictionary<String, int>> simpleDfaTrans =
              Rename(renamer, cDfaTrans);
            int simpleDfaStart = renamer[cDfaStart];
            Set<int> simpleDfaAccept = AcceptStates(cDfaStates, renamer, exitState);
            return new Dfa(simpleDfaStart, simpleDfaAccept, simpleDfaTrans);
        }

        // Nested class for creating distinctly named states when constructing NFAs

        public class NameSource
        {
            private static int nextName = 0;

            public int next()
            {
                return nextName++;
            }
        }
    }
}

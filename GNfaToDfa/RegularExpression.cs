namespace GNfaToDfa
{
    // Regular expressions ----------------------------------------------
    //
    // Abstract syntax of regular expressions
    //    r ::= A | r1 r2 | (r1|r2) | r*
    //

    abstract class RegularExpression
    {
        abstract public Nfa MakeNfa(Nfa.NameSource names);
    }
}

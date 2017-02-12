using System;

namespace GNfaToDfa
{
    // Class Transition, a transition from one state to another ----------

    public class Transition
    {
        public String lab;
        public int target;

        public Transition(String lab, int target)
        {
            this.lab = lab; this.target = target;
        }

        public override String ToString()
        {
            return "-" + lab + "-> " + target;
        }
    }
}

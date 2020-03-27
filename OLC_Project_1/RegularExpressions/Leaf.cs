using System;
using System.Collections.Generic;
using System.Text;

namespace OLC_Project_1.RegularExpressions {
    class Leaf:Grammar {
        public Leaf(Token token) : base() {
            //TODO token puede ser id o string
        }

        public override void Check() {

        }










        //For leaf they are empty
        public override void addFirst(Grammar grammar) { }
        public override void addSecond(Grammar grammar) { }
    }
}

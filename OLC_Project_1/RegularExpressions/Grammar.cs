using System;
using System.Collections.Generic;
using System.Text;

namespace OLC_Project_1.RegularExpressions {
    abstract class Grammar {
        public string GrammarName { get; set; }
        public Grammar() {
            
        }
        public abstract void Check();

        public abstract void addFirst(Grammar grammar);
        public abstract void addSecond(Grammar grammar);
    }
}

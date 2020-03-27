using System;
using System.Collections.Generic;
using System.Text;

namespace OLC_Project_1.Anaalysis {
    class Error {
        public enum ErrorType{ 
            Lexical,
            Syntactic
        }

        public ErrorType type { get; }
        public int line { get; }
        public int pos { get; }
        public string description { get; }

        public Error(ErrorType err, int line, int pos, string description) {
            type = err;
            this.line = line;
            this.pos = pos;
            this.description = description;
        }

        public string summary() {
            return "Error type: " + type + ". "+  description + " Line: " + (line + 1) + ". Pos: " + pos;
        }


    }
}

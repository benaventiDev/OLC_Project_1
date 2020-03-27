using System;
using System.Collections.Generic;
using System.Text;

namespace OLC_Project_1.Anaalysis {
    class Token {
        public string lexeme { get; }
        public int row { get; }
        public int column { get; }
        public Sym type { get; set; }

        public int length{ get; set; }

        public Token(string lexeme, int row, int column, int length, Sym type) {
            this.lexeme = lexeme;
            this.row = row;
            this.column = column;
            this.type = type;
            this.length = length;
            //COMMENT
        }

        


    }
}

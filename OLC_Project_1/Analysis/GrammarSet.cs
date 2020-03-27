using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace OLC_Project_1.Anaalysis {
    class GrammarSet {
        public ArrayList charactersTokens { get; } //Array of tokens for a list of CONJ
        public ArrayList characters { get; } //Array of chars 
        public string ConjName { get; set; }
        public bool ConjInError { get; set; }

        public Token firstChar { get; set; }
        public Token lastChar { get; set; }
        public bool SimpleSet { get; set; }// True if its like a~z

        public GrammarSet() {
            firstChar = null;
            SimpleSet = false;
            charactersTokens = new ArrayList();
            characters = new ArrayList();
        }

        public void addChar(Token token) {
            if (firstChar == null) {
                firstChar = token;
            } else if (SimpleSet) {
                lastChar = token;
            }
            charactersTokens.Add(token);
        }

        public void createCharacters() {
            if (SimpleSet) {
                //Aqui no puede ir un todo
                //TODO: validate that TODO is not in any set
                //TODO validate special char
                //ToCharArray(0,1)[0]
                if (firstChar.type == Sym.DIGIT || lastChar.type == Sym.DIGIT) {

                } else { 
                
                
                }

                int init = firstChar.lexeme.ToCharArray(0, 1)[0];
                int last = lastChar.lexeme.ToCharArray(0, 1)[0];
                if (last > init) { } else if (init == last) { } else { }
            } else { 
            
            }


        }

        
        private int  specialCharToInt(Token token) {
            if (String.Compare(token.lexeme, "\\n", true) == 0) {
                return 10;
            } else if (String.Compare(token.lexeme, "\\t", true) == 0) {
                return 9;
            } else if (String.Compare(token.lexeme, "\'", true) == 0) {
                return 39;
            } else if (String.Compare(token.lexeme, "\"", true) == 0) {
                return 34;
            } else if (String.Compare(token.lexeme, "[:todo:]", true) == 0) {
                return -1;
            }
            return -1;
        }



        public bool validateCharacter(char c) {//TODO send a character and see it if its valid or not
            
            foreach (Token character in charactersTokens) {
                //Console.WriteLine(err.summary());
            }
            return false;
        }

        //TODO: delete method is just for testing purposes
        public void Print() {
            Console.Write(ConjName + "  ");
            if (SimpleSet) {
                
                Console.WriteLine(firstChar.type + ":" +  firstChar.lexeme + " *~* " + lastChar.type + ":" + lastChar.lexeme);
                return;
            }
            foreach (Token character in charactersTokens) {
                Console.Write(character.type + ":" + character.lexeme + " ");
            }
            Console.WriteLine();
        }


    }
}

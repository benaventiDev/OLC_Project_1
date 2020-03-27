using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using LexerParserP1Test.RegularExpressions;

namespace OLC_Project_1.Anaalysis {
    class SymbolTable {
    
        public ArrayList SetsList { get; }
        public ArrayList RegularDefinitionsList { get; }
        public SymbolTable() {
            //TODO pass error list to add syntacti errors
            SetsList = new ArrayList();
            RegularDefinitionsList = new ArrayList();
        }

        public void addSet(GrammarSet newSet) {
            //TOOD validate name of set;
            SetsList.Add(newSet);
        }

        public void addGrammar(Grammar grammar) {
            //TOOD validate name of set;
            RegularDefinitionsList.Add(grammar);
        }

        //TODO: test method
        public void PrintSets() {
            foreach (GrammarSet set in SetsList) {
                set.Print();
            }
        }




    }
}

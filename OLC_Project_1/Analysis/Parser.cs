using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using LexerParserP1Test.RegularExpressions;

namespace OLC_Project_1.Anaalysis {
    class Parser {

        private Token token;
        private Lexer lex;
        private Sym lookAhead;
        private bool eof;
        private ArrayList errors;
        private SymbolTable sTable;


        public Parser(string text) {
            sTable = new SymbolTable();
            errors = new ArrayList();
            lex = new Lexer(text, errors);
            token = lex.getNextToken();
            eof = false;
            if (token != null) {
                lookAhead = token.type;
                GRAMMAR();
            }

            //TODO: Delete 
            sTable.PrintSets();
            //foreach (Error err in errors) {
                //Console.WriteLine(err.summary() );
            //}
        }

        private Token match(Sym terminal) {
            if (eof) { return null; }
            SkipNewLine();
            if (terminal == lookAhead) {
                Token returnToken = token;
                if (terminal == Sym.EOF) {
                    eof = true;
                } else {
                    token = lex.getNextToken();
                    if (token != null) {
                        lookAhead = token.type;
                    } else {
                        eof = true;
                        errors.Add(new Error(Error.ErrorType.Syntactic, -1,-1, "Unexpected end of file. Last statement was not finished."));
                    }
                }
                return returnToken;
            } else {
                errors.Add(new Error(Error.ErrorType.Syntactic, token.row , token.column, "Expected " + terminal + ". But received: " + token.type + ". Lexeme: " + token.lexeme));
                Sync();
            }
            return null;
        }

        private void Sync() {
            if (token != null && token.type == Sym.EOF) {
                eof = true;
                return;
            }

            while (token != null && token.type != Sym.EOF ) {
                if (token.type == Sym.NEW_LINE) {
                    GRAMMAR();
                }
                if (token != null) {
                        token = lex.getNextToken();
                    }
                }
        }
        private void SkipNewLine() {
            while (token != null && token.type == Sym.NEW_LINE) {
                token = lex.getNextToken();
                if (token == null) { continue; }
                lookAhead = token.type;
            }
        }

        private void GRAMMAR() {
            //Console.WriteLine("GRAMMAR");
            if (eof) { return; }
            SkipNewLine();
            switch (lookAhead) {
                case Sym.CONJ:
                    CONJ(); GRAMMAR();
                    break;
                case Sym.ID:
                    LINE(); GRAMMAR();
                    break;
                case Sym.EOF:
                    match(Sym.EOF);
                    break;
                default:
                    ERROR();
                    Sync();
                    break;
            }
        }
        private void CONJ() {
            if (eof) { return; }
            SkipNewLine();
            //Console.WriteLine("CONJ");
            switch (lookAhead) {
                case Sym.CONJ:
                    GrammarSet conj = new GrammarSet();
                    sTable.addSet(conj);
                    match(Sym.CONJ); match(Sym.COLON); Token id = match(Sym.ID);
                    if (id != null) { conj.ConjName = id.lexeme; } else { conj.ConjInError = true; }
                    match(Sym.ARROW); CHAR(conj); CONJ_BODY(conj); match(Sym.SEMI_COLON);
                    conj.createCharacters();
                    break;
                default:
                    ERROR();
                    Sync();
                    break;
            }
        }

        private void CONJ_BODY(GrammarSet conj) {
            if (eof) { return; }
            SkipNewLine();            
            switch (lookAhead) {
                case Sym.TILDE:
                    conj.SimpleSet = true;
                    match(Sym.TILDE); CHAR(conj);
                    break;
                case Sym.COMA:
                case Sym.SEMI_COLON:
                    CHAR_LIST(conj);
                    break;
                default:
                    conj.ConjInError = true;
                    ERROR();
                    Sync();
                    break;
            }
        }

        private void CHAR_LIST(GrammarSet conj) {
            if (eof) { return; }
            SkipNewLine();
            //Console.WriteLine("CHAR_LIST");
            switch (lookAhead) {
                case Sym.COMA:
                    match(Sym.COMA); CHAR(conj); CHAR_LIST(conj);
                    break;
                case Sym.SEMI_COLON:
                    //Do nothing EPSILON CASE
                    break;
                default:
                    conj.ConjInError = true;
                    ERROR();
                    Sync();
                    break;
            }
        }

        private void CHAR(GrammarSet conj) {
            if (eof) { return; }
            SkipNewLine();
            switch (lookAhead) {
                case Sym.TILDE:
                    conj.addChar(token);
                    match(Sym.TILDE);
                    break;
                case Sym.SEMI_COLON:
                    conj.addChar(token);
                    match(Sym.SEMI_COLON);
                    break;
                case Sym.CHARACTER:
                    conj.addChar(token);
                    match(Sym.CHARACTER);
                    break;
                case Sym.SPECIAL_CARACTER:
                    conj.addChar(token);
                    match(Sym.SPECIAL_CARACTER);
                    break;
                case Sym.COMA:
                    conj.addChar(token);
                    match(Sym.COMA);
                    break;
                case Sym.DIGIT:
                    conj.addChar(token);
                    match(Sym.DIGIT);
                    break;
                default:
                    conj.ConjInError = true;
                    ERROR();
                    Sync();
                    break;
            }
        }

        private void LINE() {
            if (eof) { return; }
            SkipNewLine();
            switch (lookAhead) {
                case Sym.ID:
                    Token id = match(Sym.ID); EXP(id); match(Sym.SEMI_COLON);
                    break;
                default:
                    ERROR();
                    Sync();
                    break;
            }
        }

        private void EXP(Token id) {
            if (eof) { return; }
            SkipNewLine();
            //Console.WriteLine("EXP");
            switch (lookAhead) {
                case Sym.ARROW:
                    match(Sym.ARROW); Grammar grammar = ERI();
                    if (grammar != null) { grammar.GrammarName = id.lexeme; sTable.addGrammar(grammar); }
                    break;
                case Sym.COLON:
                    //TODO expression
                    match(Sym.COLON); match(Sym.STRING);
                    break;
                default:
                    ERROR();
                    Sync();
                    break;
            }
        }

        private Grammar ERI() {
            if (eof) { return null; }
            SkipNewLine();
            Grammar grammar;
            switch (lookAhead) {
                case Sym.PERIOD:
                    grammar = new CONCATENATION();
                    match(Sym.PERIOD); grammar.addFirst(ERI()); grammar.addSecond( ERI());
                    break;
                case Sym.OR:
                    grammar = new OR();
                    match(Sym.OR); grammar.addFirst(ERI()); grammar.addSecond(ERI());
                    break;
                case Sym.QUESTION_MARK:
                    grammar = new QUESTION();
                    match(Sym.QUESTION_MARK); grammar.addFirst(ERI()); 
                    break;
                case Sym.TIMES:
                    grammar = new Times();
                    match(Sym.TIMES); grammar.addFirst(ERI());
                    break;
                case Sym.PLUS:
                    grammar = new Plus();
                    match(Sym.PLUS); grammar.addFirst(ERI());
                    break;
                case Sym.OPEN_CB:
                    grammar = LEAF();
                    break;
                case Sym.STRING:
                    grammar = LEAF();
                    break;
                default:
                    grammar = null;
                    ERROR();
                    Sync();
                    break;
            }
            return grammar;
        }


        private Grammar LEAF() {
            if (eof) { return null; }
            Leaf grammar = null;
            SkipNewLine();
            switch (lookAhead) {
                case Sym.OPEN_CB:
                    match(Sym.OPEN_CB); grammar = new Leaf( match(Sym.ID)); match(Sym.CLOSE_CB);
                    //TODO: buscar el conjunto y si no existe sintax error
                    break;
                case Sym.STRING:
                    grammar = new Leaf( match(Sym.STRING) ); 
                    break;
                default:
                    ERROR();
                    Sync();
                    break;
            }
            return grammar;
        }

        

        private void ERROR() {
            if (token == null) { return; }
            errors.Add(new Error(Error.ErrorType.Syntactic, token.row, token.column, "Unexpected token " + token.type + ". Lexeme: " + token.lexeme));

        }

    }
}

using System;
using System.Collections;
//TODO crear clase error que retorne errores
namespace OLC_Project_1.Anaalysis {
    class Lexer {
        private State lexState;
        private int state;
        private int size;
        private int lineCounter;
        private int colCounter;
        private int currentPos;
        private ArrayList errors;
        private string errorSummary;
        public string mainText { get; set; }

        enum State {
            INIT,
            CONJ,
            CON_DECL,
            EXP_DECL,
            EXP_IMPL
        }
        public Lexer(string mainText, ArrayList errors) {
            this.mainText = mainText + " \0";
            lexState = 0;
            state = 0;
            size = this.mainText.Length;
            lineCounter = 0;
            colCounter = 0;
            currentPos = 0;
            errorSummary = "";
            this.errors = errors;
        }



        public Token getNextToken() {
            String lexeme = "";
            char c;
            int a;
            int initColCounter = 0;
            Sym sym = Sym.UNDEFINED;


            while (currentPos < size) {
                c = mainText[currentPos];
                a = c;
                //Console.WriteLine(currentPos + " - " + size + ". char: " + c + ". State: " + state + "Lex State: " + lexState);
                switch (state) {
                    case 0:
                        lexeme = "";
                        initColCounter = colCounter;
                        if (c == '\"') { // S_1
                            state = 1;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                            sym = Sym.STRING;
                        } else if (c == '/') { //S_2                           
                            state = 2;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                            sym = Sym.COMMENT;
                        } else if (c == '<') { // S_4
                            state = 4;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                            sym = Sym.MULTI_LCOMMENT;
                        } else if (c == '_' || char.IsLetter(c)) { // S_5
                            if (lexState != State.EXP_IMPL) {
                                state = 5;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.ID;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (char.IsDigit(c)) {
                            if (lexState == State.CON_DECL) {
                                state = 12;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.DIGIT;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '[') { // S_6 
                            if (lexState == State.CON_DECL || lexState == State.EXP_DECL) {
                                state = 6;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.SPECIAL_CARACTER;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '\\') { // S_7 
                            if (lexState == State.EXP_DECL || lexState == State.CON_DECL) {
                                state = 7;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.SPECIAL_CARACTER;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '-') { // S_8
                            state = 8;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                            sym = Sym.ARROW;
                            if (lexState == State.INIT || lexState == State.CONJ) {
                                //if is going empty is normal
                            } else if (lexState == State.CON_DECL) {
                                sym = Sym.CHARACTER;
                                state = 3;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == ',') { // lambda
                            if (lexState == State.CON_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.COMA;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '~') { // lambda
                            if (lexState == State.CON_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.TILDE;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '{') { // lambda
                            if (lexState == State.CON_DECL || lexState == State.EXP_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.OPEN_CB;
                                if (lexState == State.CON_DECL) {
                                    sym = Sym.CHARACTER;
                                }
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '}') { //39 lambda
                            if (lexState == State.CON_DECL || lexState == State.EXP_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.CLOSE_CB;
                                if (lexState == State.CON_DECL) {
                                    sym = Sym.CHARACTER;
                                }
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == ';') { //39 lambda
                            if (lexState == State.CON_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.SEMI_COLON;
                            } else if (lexState == State.EXP_DECL || lexState == State.EXP_IMPL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.SEMI_COLON;
                                lexState = State.INIT;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }

                        } else if (c == ':') { //39 lambda
                            state = 3;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                            if (lexState == State.CONJ) {
                                sym = Sym.COLON;
                            } else if (lexState == State.CON_DECL) {
                                sym = Sym.CHARACTER;
                            } else if (lexState == State.INIT) {
                                sym = Sym.COLON;
                                lexState = State.EXP_IMPL;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '+') { //39 lambda
                            if (lexState == State.CON_DECL || lexState == State.EXP_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                if (lexState == State.CON_DECL) {
                                    sym = Sym.CHARACTER;
                                } else {
                                    sym = Sym.PLUS;
                                }
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }

                        } else if (c == '|') { //39 lambda
                            if (lexState == State.CON_DECL || lexState == State.EXP_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                if (lexState == State.CON_DECL) {
                                    sym = Sym.CHARACTER;
                                } else {
                                    sym = Sym.OR;
                                }
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '*') { //39 lambda
                            if (lexState == State.CON_DECL || lexState == State.EXP_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                if (lexState == State.CON_DECL) {
                                    sym = Sym.CHARACTER;
                                } else {
                                    sym = Sym.TIMES;
                                }
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '.') { //39 lambda
                            if (lexState == State.CON_DECL || lexState == State.EXP_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                if (lexState == State.CON_DECL) {
                                    sym = Sym.CHARACTER;
                                } else {
                                    sym = Sym.PERIOD;
                                }
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '?') { //39 lambda
                            if (lexState == State.CON_DECL || lexState == State.EXP_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                if (lexState == State.CON_DECL) {
                                    sym = Sym.CHARACTER;
                                } else {
                                    sym = Sym.QUESTION_MARK;
                                }
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected character \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '\n') { // space
                            lexState = State.INIT;
                            lineCounter++;
                            colCounter = 0;
                            currentPos++;
                            return new Token("\n", lineCounter - 1, initColCounter, lexeme.Length, Sym.NEW_LINE);
                        } else if (isSpace(c)) { // space
                            colCounter++;
                            currentPos++;
                        } else if (a >= 32 && a <= 125) { //39 lambda  32 al 125
                            if (lexState == State.CON_DECL) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                sym = Sym.CHARACTER;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char between ASCII 32 to 125 but received \'" + c + "\'."));
                                state = 19;
                            }
                        } else if (c == '\0') {
                            sym = Sym.EOF;
                            colCounter++;
                            currentPos++;
                            return new Token("", lineCounter, initColCounter, lexeme.Length, sym);

                        } else {
                            state = 19;
                            sym = Sym.ERROR;
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Character \'" + c + "\' is not recognized within the language."));
                        }
                        break;
                    case 1:
                        if (c != '\"' && c != '\n' && c != '\0' && c != '\r') { // Gamma 
                            state = 1;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else if (c == '\"') {
                            state = 3;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else { // c == New Line Error
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "New line found within a string."));
                            state = 19;
                        }
                        break;
                    case 2:
                        if (c == '/') {
                            state = 9;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else if (lexState == State.CON_DECL) { 
                            sym = Sym.CHARACTER;
                            state = 3;
                        } else {
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \'/\', to continue with a comment but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 3: //ACEPTACION
                        state = 0;
                        if (sym != Sym.COMMENT && sym != Sym.MULTI_LCOMMENT) {
                            return new Token(lexeme, lineCounter, initColCounter, lexeme.Length, sym);
                        }
                        break;
                    case 4:
                        if (c == '!') {
                            state = 10;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else if (lexState == State.CON_DECL) { 
                            sym = Sym.CHARACTER;
                            state = 3;
                        } else { 
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \'!\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 5:
                        if (c == '_' || char.IsDigit(c) || char.IsLetter(c)) {
                            state = 5;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else {// ACEPTACION
                            state = 0;
                            if (lexeme.Equals("CONJ", StringComparison.InvariantCultureIgnoreCase)) {
                                lexState = State.CONJ;
                                return new Token(lexeme, lineCounter, initColCounter, lexeme.Length, Sym.CONJ);
                            } else if(lexState == State.CON_DECL) {
                                if (lexeme.Length == 1) {
                                    return new Token(lexeme, lineCounter, initColCounter, lexeme.Length, Sym.CHARACTER);
                                } else {
                                    errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Found Id within CONJ specification."));
                                    state = 19;
                                }
                            
                            }
                            return new Token(lexeme, lineCounter, initColCounter, lexeme.Length, sym);
                        }
                        break;
                    case 6:
                        if (c == ':') {
                            state = 11;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else {
                            sym = Sym.CHARACTER;
                            state = 3;
                        }
                        break;
                    case 7:
                        if (c == 't' || c == 'T' || c == 'n' || c == 'N' || c == '\'' || c == '"' || c == '\\') {
                            state = 3;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else { // ERROR
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected any of the following chars \'n\', \'t\', \'\\\', \'\"\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 8:
                        if (c == '>') {
                            if (lexState == State.CONJ) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                lexState = State.CON_DECL;
                            } else if (lexState == State.INIT) {
                                state = 3;
                                lexeme += c;
                                colCounter++;
                                currentPos++;
                                lexState = State.EXP_DECL;
                            } else {
                                errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Unexpected char \'>\'."));
                                state = 19;
                            }

                        }/* else if (isSpace(c)) {
                            //TODO: what if it is  -~A; or  -, a, ., +

                        } */else { 
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \'>\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 9:
                        if (c == '\n' || c == '\0') {
                            state = 3;
                        } else { // Alpha
                            state = 9;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        }
                        break;
                    case 10:
                        if (c == '!') {
                            state = 14;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else if (c == '\n') {
                            state = 10;
                            lexeme += c;
                            lineCounter++;
                            colCounter = 0;
                            currentPos++;
                        } else if (c == '\0') {
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "End of file reach before comment was closed."));
                            state = 19;
                        } else {
                            state = 10;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        }
                        break;
                    case 11:
                        if (c == 't' || c == 'T') {
                            state = 13;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else { // ERROR
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \'T\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 12:
                        if (char.IsDigit(c)) {
                            state = 12;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else {
                            // ACEPTACION
                            state = 0;
                            return new Token(lexeme, lineCounter, initColCounter, lexeme.Length, sym);
                        }
                        break;
                    case 13:
                        if (c == 'o' || c == 'O') {
                            state = 15;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else { // ERROR
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \'O\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 14:
                        if (c == '>') {
                            state = 3;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else if (c == '\n') {
                            state = 10;
                            lexeme += c;
                            lineCounter++;
                            colCounter = 0;
                            currentPos++;
                        } else {
                            state = 10;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        }
                        break;
                    case 15:
                        if (c == 'd' || c == 'D') {
                            state = 16;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else { // ERROR
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \'D\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 16:
                        if (c == 'o' || c == 'O') {
                            state = 17;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else { // ERROR
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \'O\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 17:
                        if (c == ':') {
                            state = 18;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else { // ERROR
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \':\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 18:
                        if (c == ']') {
                            state = 3;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        } else { // ERROR
                            errors.Add(new Error(Error.ErrorType.Lexical, lineCounter, colCounter, "Expected char \'[\' but received \'" + c + "\'."));
                            state = 19;
                        }
                        break;
                    case 19:// ERROR
                        if (c == '\n' || c == '\r') {
                            lexState = State.INIT;
                            state = 0;
                            //return new Token(lexeme, lineCounter, initColCounter, lexeme.Length, Sym.ERROR);
                        } else if (isSpace(c) || c == '{' || c == '}' || c == '\"' || c == '\\' || c == '/' || c == '<'
                            || c == '[' || c == '-' || c == ';' || c == ':' || c == '+' || c == '|' || c == '*' || c == '.' || c == '?'
                            || c == '~' || c == ',') {
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                            state = 0;
                            //return new Token(lexeme, lineCounter, initColCounter, lexeme.Length, Sym.ERROR);
                        } else if (c == '\0') {
                            errors.Add(new Error(Error.ErrorType.Lexical, -1, -1, "File ended but couldn't recover from lexical error."));
                        } else {
                            state = 19;
                            lexeme += c;
                            colCounter++;
                            currentPos++;
                        }
                        break;
                }
            }



            return null;
        }


        private bool isSpace(char c) {
            int a = c;
            if (a == 32 || c == '\n' || c == '\t' || a == 9 || a == 10 || a == 11 || a == 12 || a == 13 || c == '\r') {
                return true;
            }
            return false;
        }

    }
}

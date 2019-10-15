using System;

namespace LanguageLexer
{
    public class Lexer
    {
        string input = string.Empty;
        string buffer = String.Empty;
        int line = 1;
        int offset = 0;
        States state;
        char current;
        int id = 0;
        bool err = false;

        public Lexer(string input)
        {
            this.input = input;
        }

        public void lex()
        {
            while (!err && offset < input.Length)
            {

                current = input[offset];

                Console.WriteLine("{0}, {1}", current, state);

                analyse();
                offset++;
            }
        }

        public void analyse()
        {
            switch (state)
            {
                case States.START:
                    lexChar();
                    break;
                case States._NOT:
                    notHandle();
                    break;
                case States._EQ:
                    eqHandle();
                    break;
                case States._L:
                    lHandle();
                    break;
                case States._G:
                    gHandle();
                    break;
                case States._AND:
                    andHandle();
                    break;
                case States._OR:
                    orHandle();
                    break;
                case States._SLASH:
                    slashHandle();
                    break;
                case States.COMM:
                    commHandle();
                    break;
                case States.ML_COMM:
                    mlCommHandle();
                    break;
                case States._COMM_END:
                    commEndHandle();
                    break;
                case States._CHAR:
                    charHandle();
                    break;
                case States._ESC_CH:
                    escChHandle();
                    break;
                case States._CH_VAL:
                    chValHandle();
                    break;
                case States._CH_END:
                    chEndHandle();
                    break;
                default:
                    break;
            }
        }
        public void lexChar()
        {
            switch (state)
            {
                case States.START:
                    lexStart();
                    break;
                default:
                    break;
            }
        }

        public void lexStart()
        {
            switch (current)
            {
                case '+':
                    state = States.PLUS;
                    print();
                    id++;
                    state = States.START;
                    break;
                case '-':
                    state = States.MINUS;
                    print();
                    id++;
                    state = States.START;
                    break;
                case '*':
                    state = States.MULT;
                    print();
                    id++;
                    state = States.START;
                    break;
                case '%':
                    state = States.REM;
                    print();
                    id++;
                    state = States.START;
                    break;
                case '^':
                    state = States.POW;
                    print();
                    id++;
                    state = States.START;
                    break;
                case '(':
                    state = States.PAREN_O;
                    print();
                    id++;
                    state = States.START;
                    break;
                case ')':
                    state = States.PAREN_C;
                    print();
                    id++;
                    state = States.START;
                    break;
                case '{':
                    state = States.BRACE_O;
                    print();
                    id++;
                    state = States.START;
                    break;
                case '}':
                    state = States.BRACE_C;
                    print();
                    id++;
                    state = States.START;
                    break;
                case ';':
                    state = States.SEMI_CLN;
                    print();
                    id++;
                    state = States.START;
                    break;
                case '!':
                    state = States._NOT;
                    break;
                case '=':
                    state = States._EQ;
                    break;
                case '<':
                    state = States._L;
                    break;
                case '>':
                    state = States._G;
                    break;
                case '&':
                    state = States._AND;
                    break;
                case '|':
                    state = States._OR;
                    break;
                case '/':
                    state = States._SLASH;
                    break;
                case '\'':
                    state = States._CHAR;
                    break;
                case '\n':
                    line++;
                    state = States.START;
                    break;
                default:
                    state = States.START;

                    break;
            }
        }

        public void notHandle()
        {
            switch (current)
            {
                case '=':
                    state = States.NOT_EQ;
                    print();
                    id++;
                    state = States.START;
                    break;
                default:
                    state = States.NOT;
                    print();
                    id++;
                    offset--;
                    state = States.START;
                    break;
            }
        }

        public void eqHandle()
        {
            switch (current)
            {
                case '=':
                    state = States.EQ_COMP;
                    print();
                    id++;
                    state = States.START;
                    break;
                default:
                    state = States.EQ_ASGN;
                    print();
                    id++;
                    offset--;
                    state = States.START;
                    break;
            }
        }

        public void lHandle()
        {
            switch (current)
            {
                case '=':
                    state = States.L_EQ;
                    print();
                    id++;
                    state = States.START;
                    break;
                default:
                    state = States.L;
                    print();
                    id++;
                    offset--;
                    state = States.START;
                    break;
            }
        }

        public void gHandle()
        {
            switch (current)
            {
                case '=':
                    state = States.G_EQ;
                    print();
                    id++;
                    state = States.START;
                    break;
                default:
                    state = States.G;
                    print();
                    id++;
                    offset--;
                    state = States.START;
                    break;
            }
        }
        public void andHandle()
        {
            switch (current)
            {
                case '&':
                    state = States.AND;
                    print();
                    id++;
                    state = States.START;
                    break;
                default:
                    error();
                    break;
            }
        }
        public void orHandle()
        {
            switch (current)
            {
                case '|':
                    state = States.OR;
                    print();
                    id++;
                    state = States.START;
                    break;
                default:
                    error();
                    break;
            }
        }

        public void slashHandle()
        {
            switch (current)
            {
                case '/':
                    state = States.COMM;
                    break;
                case '*':
                    state = States.ML_COMM;
                    break;
                default:
                    state = States.DIV;
                    print();
                    id++;
                    offset--;
                    state = States.START;
                    break;
            }
        }

        public void commHandle()
        {
            switch (current)
            {
                case '\n':
                    print();
                    id++;
                    state = States.START;
                    break;
                default:
                    break;
            }
        }
        public void mlCommHandle()
        {
            switch (current)
            {
                case '*':
                    state = States._COMM_END;
                    break;
                case '\n':
                    line++;
                    break;
                default:
                    break;
            }
        }

        public void commEndHandle()
        {
            switch (current)
            {
                case '/':
                    state = States.ML_COMM;
                    print();
                    id++;
                    state = States.START;
                    break;
                default:
                    state = States.ML_COMM;
                    break;
            }
        }

        public void charHandle()
        {
            switch (current)
            {
                case '\\':
                    state = States._ESC_CH;
                    buffer += current;
                    break;
                default:
                    state = States._CH_VAL;
                    buffer += current;
                    break;
            }
        }


        public void escChHandle()
        {
            switch (current)
            {
                case 'n':
                    state = States._CH_END;
                    buffer += current;
                    break;
                case 'b':
                    state = States._CH_END;
                    buffer += current;
                    break;
                case 'r':
                    state = States._CH_END;
                    buffer += current;
                    break;
                case 't':
                    state = States._CH_END;
                    buffer += current;
                    break;
                case '"':
                    state = States._CH_END;
                    buffer += current;
                    break;
                case '\'':
                    state = States._CH_END;
                    buffer += current;
                    break;
                case '\\':
                    state = States._CH_END;
                    buffer += current;
                    break;
                case '0':
                    state = States._CH_END;
                    buffer += current;
                    break;
                default:
                    error();
                    break;
            }
        }

        public void chValHandle()
        {
            switch (current)
            {
                case '\'':
                    error();
                    break;
                case '\"':
                    error();
                    break;
                case '\n':
                    error();
                    break;
                case '\b':
                    error();
                    break;
                case '\r':
                    error();
                    break;
                case '\t':
                    error();
                    break;
                default:
                    state = States._CH_END;
                    buffer += current;
                    break;
            }
        }

        public void chEndHandle()
        {
            switch (current)
            {
                case '\'' :
                    state = States.CHAR;
                    print();
                    id++;
                    buffer = "";
                    state = States.START;
                    break;
                default:
                    error();
                    break;
            }
        }


        public void print()
        {
            Console.WriteLine("{0,4} | {1,4} | {2,10} | {3,10}", id, line, state, buffer);
        }
        public void error()
        {
            err = true;
            Console.WriteLine("ERROR: unexpected character at line {0}", line);
        }

    }

    public enum Keywords
    {

    }

    public enum States
    {
        START,

        PLUS,
        MINUS,
        MULT,
        DIV,
        REM,
        POW,

        NOT,
        NOT_EQ,
        EQ_COMP,
        EQ_ASGN,
        L,
        L_EQ,
        G,
        G_EQ,
        AND,
        OR,

        CHAR,
        STRING,

        PAREN_O,
        PAREN_C,
        BRACE_O,
        BRACE_C,
        SEMI_CLN,

        COMM,
        ML_COMM,

        _NOT,
        _EQ,
        _L,
        _G,
        _AND,
        _OR,
        _SLASH,
        _COMM_END,
        _CHAR,
        _ESC_CH,
        _CH_VAL,
        _CH_END
    }
}

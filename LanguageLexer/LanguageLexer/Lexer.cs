using System;
using System.Collections.Generic;

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
        bool err = false;
        int lineno = 0;
        string fileName;

        List<Token> tokens = new List<Token>();

        public Lexer(string input, string fileName)
        {
            this.input = input;
            this.fileName = fileName;
        }

        public void lex()
        {
            while (state != States.ERROR && offset < input.Length)
            {
                current = input[offset];

                analyse();
                offset++;
            }
            current = ' ';

            analyse();
            analyse();

            if (!err)
            {
                if (state == States.START || state == States.COMM)
                {
                    line++;
                    completeToken(TokenType.EOF);

                    Console.WriteLine(" ID  | LINE |    TYPE    |   VALUE");
                    Console.WriteLine("-----+------+------------+-----------");

                    int counter = 0;
                    foreach (var item in tokens)
                    {
                        Console.WriteLine("{0,4} | {1,4} | {2,10} | {3,10}", counter, item.Line, item.Type, item.Value);
                        counter++;
                    }
                }
                else if (state == States._STRING)
                {
                    error("Unterminated string", true);
                    
                }
                else if (state == States.ML_COMM)
                {
                    error("Unterminated multiline comment", true);
                }
            }
        }

        public void analyse()
        {
            switch (state)
            {
                case States.START:
                    lexStart();
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
                case States._CH_END:
                    chEndHandle();
                    break;
                case States._STRING:
                    stringHandle();
                    break;
                case States._ESC_STR:
                    escStrHandle();
                    break;
                case States._NUM:
                    numHandle();
                    break;
                case States.FLOAT:
                    floatHandle();
                    break;
                case States._EXPONENT:
                    exponentHandle();
                    break;
                case States._NEG_EXPONENT:
                    negExponentHandle();
                    break;
                case States._EXP_END:
                    exponentEndHandle();
                    break;
                case States._REMAINDER:
                    remainderHandle();
                    break;
                case States._IDENT:
                    identHandle();
                    break;
                case States._KEYWORD:
                    keywordHandle();
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
                    completeToken(TokenType.PLUS);
                    break;
                case '-':
                    completeToken(TokenType.MINUS);
                    break;
                case '*':
                    completeToken(TokenType.MULT);
                    break;
                case '%':
                    completeToken(TokenType.REM);
                    break;
                case '^':
                    completeToken(TokenType.POW);
                    break;
                case '(':
                    completeToken(TokenType.PAREN_O);
                    break;
                case ')':
                    completeToken(TokenType.PAREN_C);
                    break;
                case '{':
                    completeToken(TokenType.BRACE_O);
                    break;
                case '}':
                    completeToken(TokenType.BRACE_C);
                    break;
                case ';':
                    completeToken(TokenType.SEMI_CLN);
                    break;
                case ',':
                    completeToken(TokenType.CLN);
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
                case '\"':
                    state = States._STRING;
                    break;
                case '\n':
                    line++;
                    state = States.START;
                    break;
                case char num when Char.IsDigit(num) == true:
                    state = States._NUM;
                    buffer += current;
                    break;
                case '.':
                    state = States._REMAINDER;
                    buffer += current;
                    break;
                case char ch when Char.IsLetter(ch) == true || ch == '_':
                    state = States._IDENT;
                    buffer += current;
                    break;
                default:
                    error("unknown character");
                    break;
            }
        }

        public void notHandle()
        {
            switch (current)
            {
                case '=':
                    completeToken(TokenType.NOT_EQ);
                    break;
                default:
                    completeToken(TokenType.NOT, true);
                    break;
            }
        }

        public void eqHandle()
        {
            switch (current)
            {
                case '=':
                    completeToken(TokenType.EQ_COMP);
                    break;
                default:
                    completeToken(TokenType.EQ_ASGN, true);
                    break;
            }
        }

        public void lHandle()
        {
            switch (current)
            {
                case '=':
                    completeToken(TokenType.L_EQ);
                    break;
                default:
                    completeToken(TokenType.L, true);
                    break;
            }
        }

        public void gHandle()
        {
            switch (current)
            {
                case '=':
                    completeToken(TokenType.G_EQ);
                    break;
                default:
                    completeToken(TokenType.G, true);
                    break;
            }
        }
        public void andHandle()
        {
            switch (current)
            {
                case '&':
                    completeToken(TokenType.AND);
                    break;
                default:
                    error("Incomplete && operator");
                    break;
            }
        }
        public void orHandle()
        {
            switch (current)
            {
                case '|':
                    tokens.Add(new Token(TokenType.OR, "", line));
                    state = States.START;
                    break;
                default:
                    error("Incomplete || operator");
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
                    lineno = line;
                    state = States.ML_COMM;
                    break;
                default:
                    completeToken(TokenType.DIV, true);
                    break;
            }
        }

        public void commHandle()
        {
            switch (current)
            {
                case '\n':
                    line++;
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
                    lineno = 0;
                    state = States.START;
                    break;
                default:
                    state = States.ML_COMM;
                    offset--;
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

                case '\'':
                case '\"':
                case '\n':
                case '\b':
                case '\r':
                case '\t':
                    error();
                    break;

                default:
                    state = States._CH_END;
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
                    buffer = "\n";
                    break;
                case 'b':
                    state = States._CH_END;
                    buffer = "\b";
                    break;
                case 'r':
                    state = States._CH_END;
                    buffer = "\r";
                    break;
                case 't':
                    state = States._CH_END;
                    buffer = "\t";
                    break;
                case '"':
                    state = States._CH_END;
                    buffer = "\"";
                    break;
                case '\'':
                    state = States._CH_END;
                    buffer = "\'";
                    break;
                case '\\':
                    state = States._CH_END;
                    buffer = "\\";
                    break;
                case '0':
                    state = States._CH_END;
                    buffer = "\0";
                    break;
                default:
                    error();
                    break;
            }
        }

        public void chEndHandle()
        {
            switch (current)
            {
                case '\'':
                    completeTokenWithBuffer(TokenType.CHAR, buffer[0]);
                    break;
                default:
                    error("Length of char can only be one character use \" \" for string");
                    break;
            }
        }

        public void stringHandle()
        {
            if (lineno == 0)
            {
                lineno = line;
            }
            switch (current)
            {
                case '\"':
                    tokens.Add(new Token(TokenType.STRING, buffer, lineno));
                    buffer = "";
                    lineno = 0;
                    state = States.START;
                    break;
                case '\\':
                    state = States._ESC_STR;
                    //buffer += current;
                    break;
                case '\n':
                    line++;
                    buffer += current;
                    break;
                default:
                    buffer += current;
                    break;
            }
        }

        public void escStrHandle()
        {
            switch (current)
            {
                case 'n':
                    state = States._STRING;
                    buffer += '\n';
                    break;
                case 'b':
                    state = States._STRING;
                    buffer += '\b';
                    break;
                case 'r':
                    state = States._STRING;
                    buffer += '\r';
                    break;
                case 't':
                    state = States._STRING;
                    buffer += '\t';
                    break;
                case '"':
                    state = States._STRING;
                    buffer += '\"';
                    break;
                case '\'':
                    state = States._STRING;
                    buffer += '\'';
                    break;
                case '\\':
                    state = States._STRING;
                    buffer += '\\';
                    break;
                case '0':
                    state = States._STRING;
                    buffer += '\0';
                    break;
                default:
                    error("unknown escape chatacter: " + current + " "); //printina 3 kartus
                    break;
            }
        }

        public void numHandle()
        {
            switch (current)
            {
                case char num when Char.IsDigit(num) == true:
                    buffer += current;
                    break;
                case '.':
                    state = States.FLOAT;
                    buffer += current;
                    break;
                case 'e':
                    state = States._EXPONENT;
                    buffer += current;
                    break;
                case char ch when Char.IsLetter(ch) == true:
                    state = States.START;
                    error("integer is merged with identifier");
                    break;
                default:
                    completeTokenWithBuffer(TokenType.INT, Int32.Parse(buffer), true);
                    break;
            }
        }

        public void floatHandle()
        {
            switch (current)
            {
                case char num when Char.IsDigit(num) == true:
                    buffer += current;
                    break;
                case 'e':
                    state = States._EXPONENT;
                    buffer += current;
                    break;
                default:
                    completeTokenWithBuffer(TokenType.FLOAT, float.Parse(buffer), true);
                    break;
            }
        }

        public void exponentHandle()
        {
            switch (current)
            {
                case char num when Char.IsDigit(num) == true:
                    state = States._EXP_END;
                    buffer += current;
                    break;
                case '-':
                    state = States._NEG_EXPONENT;
                    buffer += current;
                    break;
                default:
                    error("exponent is not complete "); // printina 3 kartus
                    break;
            }
        }
        public void exponentEndHandle()
        {
            switch (current)
            {
                case char num when Char.IsDigit(num) == true:
                    buffer += current;
                    break;
                default:
                    completeTokenWithBuffer(TokenType.FLOAT, float.Parse(buffer), true);
                    break;
            }
        }

        public void negExponentHandle()
        {
            switch (current)
            {
                case char num when Char.IsDigit(num) == true:
                    state = States._EXP_END;
                    buffer += current;
                    break;
                default:
                    error("exponent is not complete "); // printina 3 kartus
                    break;
            }
        }

        public void remainderHandle()
        {
            switch (current)
            {
                case char num when Char.IsDigit(num) == true:
                    state = States.FLOAT;
                    buffer += current;
                    break;
                case 'e':
                    state = States._EXPONENT;
                    buffer += current;
                    break;
                default:
                    error("float remainder is merged with identifier"); //prints 3 times
                    break;
            }
        }

        public void identHandle()
        {
            switch (current)
            {
                case char ch when Char.IsLetter(ch) == true:
                    buffer += current;
                    break;
                case char num when Char.IsDigit(num) == true:
                    buffer += current;
                    break;
                case '_':
                    buffer += current;
                    break;
                default:
                    offset--;
                    state = States._KEYWORD;
                    break;
            }
        }

        public void keywordHandle()
        {
            switch (buffer)
            {
                case "true":
                    completeTokenWithBuffer(TokenType.BOOL, bool.Parse(buffer), true);
                    break;
                case "false":
                    completeTokenWithBuffer(TokenType.BOOL, bool.Parse(buffer), true);
                    break;
                case "int":
                    completeToken(TokenType.KW_INT, true);
                    break;
                case "float":
                    completeToken(TokenType.KW_FLOAT, true);
                    break;
                case "char":
                    completeToken(TokenType.KW_CHAR, true);
                    break;
                case "string":
                    completeToken(TokenType.KW_STRING, true);
                    break;
                case "bool":
                    completeToken(TokenType.KW_BOOL, true);
                    break;
                case "void":
                    completeToken(TokenType.KW_VOID, true);
                    break;
                case "if":
                    completeToken(TokenType.KW_IF, true);
                    break;
                case "elseif":
                    completeToken(TokenType.KW_ELSEIF, true);
                    break;
                case "else":
                    completeToken(TokenType.KW_ELSE, true);
                    break;
                case "return":
                    completeToken(TokenType.KW_RETURN, true);
                    break;
                case "break":
                    completeToken(TokenType.KW_BREAK, true);
                    break;
                case "continue":
                    completeToken(TokenType.KW_CONTINUE, true);
                    break;
                case "while":
                    completeToken(TokenType.KW_WHILE, true);
                    break;
                default:
                    completeTokenWithBuffer(TokenType.IDENT, buffer, true);
                    break;
            }
        }

        public void error(string message = "unexpected character", bool multiline = false)
        {
            err = true;
            Console.WriteLine("ERROR in {0}: {1} at line {2}",fileName, message, multiline ? lineno :line);
            state = States.ERROR;
        }

        public void completeTokenWithBuffer(TokenType type, dynamic value, bool decrement = false)
        {
            tokens.Add(new Token(type, value, line));
            buffer = "";
            state = States.START;

            if (decrement)
            {
                offset--;
            }
        }

        public void completeToken(TokenType type, bool decrement = false)
        {
            buffer = "";
            tokens.Add(new Token(type, buffer, line));
            state = States.START;

            if (decrement)
            {
                offset--;
            }
        }
    }

    public class Token
    {
        public TokenType Type { get; }
        public dynamic Value { get; }
        public int Line { get; }

        public Token(TokenType type, dynamic value, int line)
        {
            Type = type;
            Value = value;
            Line = line;
        }
    }

    public enum TokenType
    {
        EOF,

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
        INT,
        FLOAT,
        BOOL,

        PAREN_O,
        PAREN_C,
        BRACE_O,
        BRACE_C,
        SEMI_CLN,
        CLN,

        IDENT,

        KW_INT,
        KW_FLOAT,
        KW_CHAR,
        KW_STRING,
        KW_BOOL,
        KW_VOID,

        KW_IF,
        KW_ELSEIF,
        KW_ELSE,

        KW_RETURN,
        KW_BREAK,
        KW_CONTINUE,

        KW_WHILE
    }

    public enum States
    {
        START,
        ERROR,
        EOF,

        FLOAT,

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
        _CH_END,
        _STRING,
        _ESC_STR,
        _NUM,
        _EXPONENT,
        _NEG_EXPONENT,
        _EXP_END,
        _REMAINDER,
        _IDENT,
        _KEYWORD
    }
}

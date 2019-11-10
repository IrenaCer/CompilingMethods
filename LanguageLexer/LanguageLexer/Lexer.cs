﻿using System;
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
                //Console.WriteLine("Im here");
                current = input[offset];

                //Console.WriteLine("{0}, {1}", current, state);

                analyse();
                offset++;
            }
            current = ' ';
            //Console.WriteLine("Last analysing");
            analyse();
            analyse();
            //Console.WriteLine(Char.IsLetter(' '));

            //Console.WriteLine("current:");
            //Console.WriteLine(current);
            //current = ' ';
            //Console.WriteLine(state);
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



            //if (state == States.START)
            //{
            //    state = States.EOF;
            //    print();
            //}
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
                    //completeToken(TokenType.PLUS);
                    tokens.Add(new Token(TokenType.PLUS, "", line));
                    state = States.START;
                    break;
                case '-':
                    tokens.Add(new Token(TokenType.MINUS, "", line));
                    state = States.START;
                    break;
                case '*':
                    tokens.Add(new Token(TokenType.MULT, "", line));
                    state = States.START;
                    break;
                case '%':
                    tokens.Add(new Token(TokenType.REM, "", line));
                    state = States.START;
                    break;
                case '^':
                    tokens.Add(new Token(TokenType.POW, "", line));
                    state = States.START;
                    break;
                case '(':
                    tokens.Add(new Token(TokenType.PAREN_O, "", line));
                    state = States.START;
                    break;
                case ')':
                    tokens.Add(new Token(TokenType.PAREN_C, "", line));
                    state = States.START;
                    break;
                case '{':
                    tokens.Add(new Token(TokenType.BRACE_O, "", line));
                    state = States.START;
                    break;
                case '}':
                    tokens.Add(new Token(TokenType.BRACE_C, "", line));
                    state = States.START;
                    break;
                case ';':
                    tokens.Add(new Token(TokenType.SEMI_CLN, "", line));
                    state = States.START;
                    break;
                case ',':
                    tokens.Add(new Token(TokenType.CLN, "", line));
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
                    state = States.START;

                    break;
            }
        }

        public void notHandle()
        {
            switch (current)
            {
                case '=':
                    tokens.Add(new Token(TokenType.NOT_EQ, "", line));
                    state = States.START;
                    break;
                default:
                    tokens.Add(new Token(TokenType.NOT, "", line));
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
                    tokens.Add(new Token(TokenType.EQ_COMP, "", line));
                    state = States.START;
                    break;
                default:
                    tokens.Add(new Token(TokenType.EQ_ASGN, "", line));
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
                    tokens.Add(new Token(TokenType.L_EQ, "", line));
                    state = States.START;
                    break;
                default:
                    tokens.Add(new Token(TokenType.L, "", line));
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
                    tokens.Add(new Token(TokenType.G_EQ, "", line));
                    state = States.START;
                    break;
                default:
                    tokens.Add(new Token(TokenType.G, "", line));
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
                    tokens.Add(new Token(TokenType.AND, "", line));
                    state = States.START;
                    break;
                default:
                    error("Incomplete && operator"); //prints 2-3 times
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
                    error("Incomplete || operator"); //prints 2-3
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
                    tokens.Add(new Token(TokenType.DIV, "", line));
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

        public void chEndHandle()
        {
            switch (current)
            {
                case '\'':
                    tokens.Add(new Token(TokenType.CHAR, buffer, line));
                    buffer = "";
                    state = States.START;
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
                    //Console.WriteLine(buffer);
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
                    tokens.Add(new Token(TokenType.INT, buffer, line));
                    offset--;
                    buffer = "";
                    state = States.START;
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
                    tokens.Add(new Token(TokenType.FLOAT, buffer, line));
                    offset--;
                    buffer = "";
                    state = States.START;
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
                    tokens.Add(new Token(TokenType.FLOAT, buffer, line));
                    offset--;
                    buffer = "";
                    state = States.START;
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
                    //print();
                    //id++;
                    offset--;
                    //buffer = "";
                    //state = States.START;
                    state = States._KEYWORD;
                    break;
            }
        }

        public void keywordHandle()
        {
            switch (buffer)
            {
                case "true":
                    tokens.Add(new Token(TokenType.BOOL, buffer, line));
                    offset--;
                    buffer = "";
                    state = States.START;
                    break;
                case "false":
                    tokens.Add(new Token(TokenType.BOOL, buffer, line));
                    offset--;
                    buffer = "";
                    state = States.START;
                    break;
                case "int":
                    tokens.Add(new Token(TokenType.KW_INT, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "float":
                    tokens.Add(new Token(TokenType.KW_FLOAT, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "char":
                    tokens.Add(new Token(TokenType.KW_CHAR, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "string":
                    tokens.Add(new Token(TokenType.KW_STRING, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "bool":
                    tokens.Add(new Token(TokenType.KW_BOOL, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "void":
                    tokens.Add(new Token(TokenType.KW_VOID, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "if":
                    tokens.Add(new Token(TokenType.KW_IF, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "elseif":
                    tokens.Add(new Token(TokenType.KW_ELSEIF, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "else":
                    tokens.Add(new Token(TokenType.KW_ELSE, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "return":
                    tokens.Add(new Token(TokenType.KW_RETURN, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "break":
                    tokens.Add(new Token(TokenType.KW_BREAK, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "continue":
                    tokens.Add(new Token(TokenType.KW_CONTINUE, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                case "while":
                    tokens.Add(new Token(TokenType.KW_WHILE, "", line));
                    buffer = "";
                    offset--;
                    state = States.START;
                    break;
                default:
                    //Console.WriteLine("default case of keyword");
                    tokens.Add(new Token(TokenType.IDENT, buffer, line));
                    offset--;
                    buffer = "";
                    state = States.START;
                    break;
            }

        }

        public void error(string message = "unexpected character", bool multiline = false)
        {
            err = true;
            Console.WriteLine("ERROR in {0}: {1} at line {2}",fileName, message, multiline ? lineno :line);
            state = States.ERROR;
        }

        public void completeTokenWithBuffer(TokenType type, bool decrement = false)
        {
            tokens.Add(new Token(type, buffer, line));
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

        public Token(TokenType type, string value, int line)
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

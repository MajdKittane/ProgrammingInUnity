using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public class Lexer
    {
        string input;
        int position;
        int nextPosition;
        byte character;

        public Lexer(string _input)
        {
            this.input = _input;
            this.ReadChar();
        }


        public Token NextToken()
        {
            Token tok;
            
            SkipWhiteSpaces();

            switch (this.character)
            {
                case (byte)'=':
                    if (PeekChar() == '=')
                    {
                        character = (byte) (character + character);
                        ReadChar();
                        tok = new Token(TokenType.Equal,"==");
                    }
                    else
                    {
                        tok = new Token(TokenType.Assign, "=");
                    }
                    break;
                case (byte)'+':
                    tok = new Token(TokenType.Plus, "+");
                    break;
                case (byte)'-':
                    tok = new Token(TokenType.Minus, "-");
                    break;
                case (byte)'!':
                    if (PeekChar() == '=')
                    {
                        character = (byte)(character + character);
                        ReadChar();
                        tok = new Token(TokenType.NotEqual, "!=");
                    }
                    else
                    {
                        tok = new Token(TokenType.Bang, "!");
                    }
                    break;
                case (byte)'*':
                    tok = new Token(TokenType.Asterisk, "*");
                    break;
                case (byte)'/':
                    tok = new Token(TokenType.Slash, "/");
                    break;
                case (byte)'<':
                    tok = new Token(TokenType.LessThan, "<");
                    break;
                case (byte)'>':
                    tok = new Token(TokenType.GreaterThan, ">");
                    break;
                case (byte)',':
                    tok = new Token(TokenType.Comma, ",");
                    break;
                case (byte)';':
                    tok = new Token(TokenType.Semicolon, ";");
                    break;
                case (byte)':':
                    tok = new Token(TokenType.Colon, ":");
                    break;
                case (byte)'(':
                    tok = new Token(TokenType.LeftParen, "(");
                    break;
                case (byte)')':
                    tok = new Token(TokenType.RightParen, ")");
                    break;
                case (byte)'{':
                    tok = new Token(TokenType.LeftBrace, "{");
                    break;
                case (byte)'}':
                    tok = new Token(TokenType.RightBrace, "}");
                    break;
                case (byte)'[':
                    tok = new Token(TokenType.LeftBracket, "[");
                    break;
                case (byte)']':
                    tok = new Token(TokenType.RightBracket, "]");
                    break;
                case (byte)'"':
                    tok = new Token(TokenType.str, ReadString());
                    break;
                case 0:
                    tok = new Token(TokenType.EOF, "");
                    break;
                default:
                    if (Char.IsLetter((char)this.character))
                    {
                        string literal = ReadIdentifier();
                        tok = new Token(Token.LookupIdentifierType(literal), literal);
                        Console.WriteLine(tok.literal + "\t" + tok.tokenType);
                        return tok;
                    }
                    else if (Char.IsDigit((char)this.character))
                    {
                        string literal = ReadNumber();
                        tok = new Token(TokenType.Int, literal);
                        Console.WriteLine(tok.literal + "\t" + tok.tokenType);
                        return tok;
                    }
                    else tok = new Token(TokenType.Illegal, ((char)this.character).ToString());
                    break;
            }
            ReadChar();
            Console.WriteLine(tok.literal + "\t" + tok.tokenType);
            return tok;
        }
        
        
        void SkipWhiteSpaces()
        {
            while (this.character == ' ' || this.character == '\t' || this.character == '\n' || this.character == '\r')
            {
                ReadChar();
            }
        }
        
        void ReadChar()
        {
            this.character = this.PeekChar();
            this.position = this.nextPosition;
            this.nextPosition++;
        }

        byte PeekChar()
        {
            if (this.nextPosition >= this.input.Length)
            {
                return 0;
            }
            else
            {
                return (byte)this.input[this.nextPosition];
            }
        }

        string ReadString()
        {
            string str = "";
            while(true)
            {
                this.ReadChar();
                if (this.character == (byte)'"' || this.character == 0) break;
                str += (char)this.character;
            }
            return str;
        }

        string ReadIdentifier()
        {
            string str = "";
            while (Char.IsLetter((char)this.character))
            {
                str += (char)this.character;
                ReadChar();
            }
            return str;
        }

        string ReadNumber()
        {
            string str = "";
            while (Char.IsDigit((char)this.character))
            {
                str += (char)this.character;
                ReadChar();
            }
            return str;
        }
    }
}

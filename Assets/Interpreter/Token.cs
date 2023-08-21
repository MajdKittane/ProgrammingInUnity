using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
	public class Token
	{
		public TokenType tokenType;
		public string literal;

		static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
		{
			{"fn", TokenType.Function},
			{"let", TokenType.Let},
			{"true", TokenType.True},
			{"false", TokenType.False},
			{"if", TokenType.If},
			{"else", TokenType.Else},
			{"return", TokenType.Return}
		};

		public Token(TokenType _type, string _literal)
        {
			this.tokenType = _type;
			this.literal = _literal;
        }

		public static TokenType LookupIdentifierType(string identifier)
        {
			if (keywords.ContainsKey(identifier))
            {
				return keywords[identifier];
            }
			return TokenType.Identifier;
        }
    }

	public class TokenType
	{
		readonly string value;
		TokenType(string tok) { this.value = tok; }
		public static implicit operator string(TokenType tok) { return tok.value; }
		public static implicit operator TokenType(string v) { return new TokenType(v); }


		public static readonly TokenType Illegal = "Illegal",
					EOF = "EOF",


					Identifier = "Identifier",
					Int = "Int",
					str = "String",


					Assign = "=",
					Plus = "+",
					Minus = "-",
					Bang = "!",
					Asterisk = "*",
					Slash = "/",
					Equal = "==",
					NotEqual = "!=",

					LessThan = "<",
					GreaterThan = ">",


					Comma = ",",
					Semicolon = ";",
					Colon = ":",

					LeftParen = "(",
					RightParen = ")",
					LeftBrace = "{",
					RightBrace = "}",
					LeftBracket = "[",
					RightBracket = "]",


					Function = "Function",
					Let = "Let",
					True = "True",
					False = "False",
					If = "If",
					Else = "Else",
					For = "For",
					While = "While",
					Return = "Return";
					
	}

}

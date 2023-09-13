using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public class Parser
    {
        private Lexer lexer;
        public List<string> errors;
        private Token currentToken;
        private Token peekToken;

        private delegate Expression PrefixParseFn();
        private delegate Expression InfixParseFn(Expression expression);

        private Dictionary<TokenType, PrefixParseFn> prefixParseFns;
        private Dictionary<TokenType, InfixParseFn> infixParseFns;


        private enum Precedence
        {
            Lowest,
            Equals,
            LessOrGreater,
            Sum,
            Product,
            Prefix,
            Call,
            Index
        };

        private Dictionary<TokenType, Precedence> precedences = new Dictionary<TokenType, Precedence>()
        {
            {TokenType.Equal,Precedence.Equals},
            {TokenType.NotEqual,Precedence.Equals},
            {TokenType.LessThan,Precedence.LessOrGreater},
            {TokenType.GreaterThan,Precedence.LessOrGreater},
            {TokenType.Plus,Precedence.Sum},
            {TokenType.Minus,Precedence.Sum},
            {TokenType.Slash,Precedence.Product},
            {TokenType.Asterisk,Precedence.Product},
            {TokenType.Mod,Precedence.Product},
            {TokenType.LeftParen,Precedence.Call},
            {TokenType.LeftBracket,Precedence.Index}
        };

        public Parser(Lexer _lexer)
        {
            this.lexer = _lexer;
            this.errors = new List<string>();

            this.prefixParseFns = new Dictionary<TokenType, PrefixParseFn>();
            this.infixParseFns = new Dictionary<TokenType, InfixParseFn>();

            RegisterPrefix(TokenType.Identifier, ParseIdentifier);
            RegisterPrefix(TokenType.Int, ParseIntegerLiteral);
            RegisterPrefix(TokenType.Bang, ParsePrefixExpression);
            RegisterPrefix(TokenType.Minus, ParsePrefixExpression);
            RegisterPrefix(TokenType.True, ParseBoolean);
            RegisterPrefix(TokenType.False, ParseBoolean);
            RegisterPrefix(TokenType.LeftParen, ParseGroupedExpression);
            RegisterPrefix(TokenType.If, ParseIfExpression);
            RegisterPrefix(TokenType.Function, ParseFunctionLiteral);
            RegisterPrefix(TokenType.str, ParseStringLiteral);
            RegisterPrefix(TokenType.LeftBracket, ParseArrayLiteral);
            RegisterPrefix(TokenType.LeftBrace, ParseHashLiteral);
            RegisterPrefix(TokenType.Loop, ParseLoopExpression);

            RegisterInfix(TokenType.Plus, ParseInfixExpression);
            RegisterInfix(TokenType.Minus, ParseInfixExpression);
            RegisterInfix(TokenType.Slash, ParseInfixExpression);
            RegisterInfix(TokenType.Asterisk, ParseInfixExpression);
            RegisterInfix(TokenType.Mod, ParseInfixExpression);
            RegisterInfix(TokenType.Equal, ParseInfixExpression);
            RegisterInfix(TokenType.NotEqual, ParseInfixExpression);
            RegisterInfix(TokenType.LessThan, ParseInfixExpression);
            RegisterInfix(TokenType.GreaterThan, ParseInfixExpression);
            RegisterInfix(TokenType.LeftParen, ParseCallExpression);
            RegisterInfix(TokenType.LeftBracket, ParseIndexExpression);

            NextToken();
            NextToken();

        }

        public Program ParseProgram()
        {
            Program program = new Program();
            program.statements = new List<Statement>();

            while (currentToken.tokenType != TokenType.EOF)
            {
                Statement statement = ParseStatement();
                if (statement != null)
                {
                    program.statements.Add(statement);
                }
                NextToken();
            }
            return program;
        }

        public List<string> Errors()
        {
            return errors;
        }

        private Statement ParseStatement()
        {
            switch (currentToken.tokenType)
            {
                case "Let":
                    return ParseLetStatement();
                case "Return":
                    return ParseReturnStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private LetStatement ParseLetStatement()
        {
            LetStatement statement = new LetStatement { token = currentToken };

            if (!ExpectPeek(TokenType.Identifier))
            {
                return null;
            }

            statement.name = new Identifier { token = currentToken, value = currentToken.literal };

            if (!ExpectPeek(TokenType.Assign))
            {
                return null;
            }

            NextToken();
            statement.value = ParseExpression((int)Precedence.Lowest);

            if (peekToken.tokenType == TokenType.Semicolon)
            {
                NextToken();
            }

            return statement;

        }

        private ReturnStatement ParseReturnStatement()
        {
            ReturnStatement statement = new ReturnStatement { token = currentToken };

            NextToken();
            statement.returnValue = ParseExpression((int)Precedence.Lowest);

            if (peekToken.tokenType == TokenType.Semicolon)
            {
                NextToken();
            }

            return statement;
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            ExpressionStatement statement = new ExpressionStatement { token = currentToken };
            statement.expression = ParseExpression((int)Precedence.Lowest);

            if (peekToken.tokenType == TokenType.Semicolon)
            {
                NextToken();
            }

            return statement;
        }

        private Expression ParseExpression(int precedence)
        {
            if (!prefixParseFns.TryGetValue(currentToken.tokenType, out PrefixParseFn prefix))
            {
                NoPrefixParseFnError(currentToken.tokenType);
                return null;
            }

            Expression leftExp = prefix();

            while (peekToken.tokenType != TokenType.Semicolon && precedence < PeekPrecedence())
            {
                if (!infixParseFns.TryGetValue(peekToken.tokenType, out InfixParseFn infix))
                {
                    return leftExp;
                }

                NextToken();
                leftExp = infix(leftExp);
            }

            return leftExp;
        }

        private Expression ParseIdentifier()
        {
            return new Identifier { token = currentToken, value = currentToken.literal };
        }

        private Expression ParseIntegerLiteral()
        {
            IntegerLiteral lit = new IntegerLiteral { token = currentToken };

            if (!int.TryParse(currentToken.literal, out int value))
            {
                errors.Add($"Could not parse {currentToken.literal} as Integer.");
                return null;
            }

            lit.value = value;
            return lit;
        }

        private Expression ParsePrefixExpression()
        {
            PrefixExpression expression = new PrefixExpression { token = currentToken, op = currentToken.literal };

            NextToken();

            expression.right = ParseExpression((int)Precedence.Prefix);

            return expression;
        }

        private Expression ParseInfixExpression(Expression _left)
        {
            InfixExpression expression = new InfixExpression { token = currentToken, op = currentToken.literal, left = _left };

            int precedence = CurrentPrecedence();
            NextToken();
            expression.right = ParseExpression(precedence);

            return expression;
        }

        private Expression ParseBoolean()
        {
            return new BooleanLiteral { token = currentToken, value = CurrentTokenIs(TokenType.True) };
        }


        private Expression ParseGroupedExpression()
        {
            NextToken();

            Expression expression = ParseExpression((int)Precedence.Lowest);

            if (!ExpectPeek(TokenType.RightParen))
            {
                return null;
            }

            return expression;
        }

        private Expression ParseIfExpression()
        {
            IfExpression expression = new IfExpression { token = currentToken };

            if (!ExpectPeek(TokenType.LeftParen))
            {
                return null;
            }

            NextToken();
            expression.condition = ParseExpression((int)Precedence.Lowest);

            if (!ExpectPeek(TokenType.RightParen))
            {
                return null;
            }

            if (!ExpectPeek(TokenType.LeftBrace))
            {
                return null;
            }

            expression.consequence = ParseBlockStatement();
            if (PeekTokenIs(TokenType.Else))
            {
                NextToken();

                if (!ExpectPeek(TokenType.LeftBrace))
                {
                    return null;
                }

                expression.alternative = ParseBlockStatement();
            }

            return expression;
        }

        private Expression ParseLoopExpression()
        {
            LoopExpression expression = new LoopExpression { token = currentToken };

            if (!ExpectPeek(TokenType.LeftParen))
            {
                return null;
            }
            NextToken();
            expression.condition = ParseExpression((int)Precedence.Lowest);

            if (!ExpectPeek(TokenType.RightParen))
            {
                return null;
            }

            if (!ExpectPeek(TokenType.LeftBrace))
            {
                return null;
            }

            expression.body = ParseBlockStatement();

            return expression;
        }

        private BlockStatement ParseBlockStatement()
        {
            BlockStatement block = new BlockStatement { token = currentToken };
            block.statements = new List<Statement>();

            NextToken();

            while (!CurrentTokenIs(TokenType.RightBrace) && !CurrentTokenIs(TokenType.EOF))
            {
                Statement statement = ParseStatement();

                if (statement != null)
                {
                    block.statements.Add(statement);
                }
                NextToken();
            }
            return block;
        }

        private Expression ParseFunctionLiteral()
        {
            FunctionLiteral fLit = new FunctionLiteral { token = currentToken };

            if (!ExpectPeek(TokenType.LeftParen))
            {
                return null;
            }

            fLit.parameters = ParseFunctionParameters();

            if (!ExpectPeek(TokenType.LeftBrace))
            {
                return null;
            }

            fLit.body = ParseBlockStatement();

            return fLit;
        }

        private List<Identifier> ParseFunctionParameters()
        {
            List<Identifier> ids = new List<Identifier>();

            if (PeekTokenIs(TokenType.RightParen))
            {
                NextToken();
                return ids;
            }

            NextToken();

            Identifier id = new Identifier { token = currentToken, value = currentToken.literal };
            ids.Add(id);

            while (PeekTokenIs(TokenType.Comma))
            {
                NextToken();
                NextToken();
                id = new Identifier { token = currentToken, value = currentToken.literal };
                ids.Add(id);
            }


            if (!ExpectPeek(TokenType.RightParen))
            {
                return null;
            }

            return ids;
        }


        private Expression ParseCallExpression(Expression fn)
        {
            CallExpression expression = new CallExpression { token = currentToken, function = fn };
            expression.arguments = ParseExpressionList(TokenType.RightParen);
            return expression;
        }

        private List<Expression> ParseExpressionList(TokenType end)
        {
            List<Expression> expressions = new List<Expression>();

            if (PeekTokenIs(end))
            {
                NextToken();
                return expressions;
            }

            NextToken();
            expressions.Add(ParseExpression((int)Precedence.Lowest));

            while (PeekTokenIs(TokenType.Comma))
            {
                NextToken();
                NextToken();
                expressions.Add(ParseExpression((int)Precedence.Lowest));
            }

            if (!ExpectPeek(end))
            {
                return null;
            }

            return expressions;
        }

        private Expression ParseStringLiteral()
        {
            return new StringLiteral { token = currentToken, value = currentToken.literal };
        }

        private Expression ParseArrayLiteral()
        {
            return new ArrayLiteral { token = currentToken, elements = ParseExpressionList(TokenType.RightBracket) };
        }

        private Expression ParseIndexExpression(Expression _left)
        {
            IndexExpression expression = new IndexExpression { token = currentToken, left = _left };

            NextToken();
            expression.index = ParseExpression((int)Precedence.Lowest);

            if (!ExpectPeek(TokenType.RightBracket))
            {
                return null;
            }

            return expression;
        }

        private Expression ParseHashLiteral()
        {
            HashLiteral hash = new HashLiteral { token = currentToken };

            hash.pairs = new Dictionary<Expression, Expression>();

            while (!PeekTokenIs(TokenType.RightBrace))
            {
                NextToken();
                Expression key = ParseExpression((int)Precedence.Lowest);

                if (!ExpectPeek(TokenType.Colon))
                {
                    return null;
                }

                NextToken();

                Expression value = ParseExpression((int)Precedence.Lowest);

                hash.pairs.Add(key, value);

                if (!PeekTokenIs(TokenType.RightBrace) && !ExpectPeek(TokenType.Comma))
                {
                    return null;
                }
            }
            if (!ExpectPeek(TokenType.RightBrace))
            {
                return null;
            }
            return hash;
        }

        private void NextToken()
        {
            currentToken = peekToken;
            peekToken = lexer.NextToken();
        }

        private bool CurrentTokenIs(TokenType type)
        {
            return currentToken.tokenType == type;
        }

        private bool PeekTokenIs(TokenType type)
        {
            return peekToken.tokenType == type;
        }

        private bool ExpectPeek(TokenType type)
        {
            if (PeekTokenIs(type))
            {
                NextToken();
                return true;
            }
            else
            {
                PeekError(type);
                return false;
            }
        }

        private void RegisterPrefix(TokenType type, PrefixParseFn prefix)
        {
            prefixParseFns[type] = prefix;
        }

        private void RegisterInfix(TokenType type, InfixParseFn infix)
        {
            infixParseFns[type] = infix;
        }

        private void NoPrefixParseFnError(TokenType type)
        {
            errors.Add($"No prefix parse function for {type} found.");
        }

        private int PeekPrecedence()
        {
            if (precedences.TryGetValue(peekToken.tokenType, out Precedence precedence))
            {
                return (int)precedence;
            }

            return (int)Precedence.Lowest;
        }

        private int CurrentPrecedence()
        {
            if (precedences.TryGetValue(currentToken.tokenType, out Precedence precedence))
            {
                return (int)precedence;
            }

            return (int)Precedence.Lowest;
        }

        private void PeekError(TokenType type)
        {
            errors.Add($"Expected next token to be {type}, got {peekToken.tokenType} instead.");
        }
    }
}

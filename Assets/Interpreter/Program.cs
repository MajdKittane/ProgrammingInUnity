using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public interface Node
    {
        string TokenLiteral();
        string ToString();
    }

    public interface Statement : Node
    {
        void statementNode();
    }

    public interface Expression : Node
    {
        void expressionNode();
    }
    
    public class Program : Node
    {
        public List<Statement> statements { get; set; }

        public string TokenLiteral()
        {
            if (this.statements.Count > 0)
            {
                return this.statements[0].TokenLiteral();
            }
            return "";
        }

        public override string ToString()
        {
            string output ="";
            foreach (Statement statement in this.statements)
            {
                output += statement.ToString();
            }
            return output;
        }
    }

    public class BlockStatement : Statement
    {
        public Token token { get; set; }
        public List<Statement> statements { get; set; }

        public void statementNode() {}

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";
            foreach (Statement statement in this.statements)
            {
                output += statement;
            }
            return output;
        }

    }

    public class LetStatement : Statement
    {
        public Token token { get; set; }
        public Identifier name { get; set; }
        public Expression value { get; set; }

        public void statementNode() { }
        
        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            output += TokenLiteral() + " ";
            output += name.ToString();
            output += " = ";

            if (value != null)
            {
                output += value.ToString();
            }

            output += (TokenType.Semicolon);
            return output;
        }
    }

    public class ReturnStatement : Statement
    {
        public Token token { get; set; }
        public Expression returnValue { get; set; }

        public void statementNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            output += TokenLiteral() + " ";
            
            if (returnValue != null)
            {
                output += returnValue.ToString();
            }

            output += TokenType.Semicolon;

            return output;
        }
    }

    public class ExpressionStatement : Statement
    {
        public Token token { get; set; }
        public Expression expression { get; set; }

        public void statementNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            if (expression != null)
            {
                return expression.ToString();
            }
            return "";
        }
    }


    public class PrefixExpression : Expression
    {
        public Token token { get; set; }
        public string op { get; set; }
        public Expression right { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            output += TokenType.LeftParen;
            output += op;
            output += right.ToString();
            output += TokenType.RightParen;

            return output;
        }
    }

    public class InfixExpression : Expression
    {
        public Token token { get; set; }
        public Expression left { get; set; }
        public string op { get; set; }
        public Expression right { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            output += TokenType.LeftParen;
            output += left.ToString();
            output += " " + op + " ";
            output += right.ToString();
            output += TokenType.RightParen;

            return output;
        }
    }

    public class IfExpression : Expression
    {
        public Token token { get; set; }
        public Expression condition { get; set; }
        public BlockStatement consequence { get; set; }
        public BlockStatement alternative { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            output += "if";
            output += condition.ToString();
            output += " ";
            output += consequence.ToString();

            if (alternative != null)
            {
                output += "else ";
                output = alternative.ToString();
            }

            return output;
        }
    }

    public class ForExpression : Expression
    {
        public Token token { get; set; }
        public Expression from { get; set; }
        public Expression to { get; set; }
        public BlockStatement body { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";
            output = "for";
            output += from.ToString() + " to ";
            output += to.ToString() + " ";
            output += body.ToString();
            return output;
        }
        
    }

    public class CallExpression : Expression
    {
        public Token token { get; set; }
        public Expression function { get; set; }
        public List<Expression> arguments { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            List<string> args = new List<string>();

            foreach (Expression arg in arguments)
            {
                args.Add(arg.ToString());
            }

            output += function.ToString();
            output += TokenType.LeftParen;
            output += string.Join(TokenType.Comma + " ",args);
            output += TokenType.RightParen;

            return output;
        }
    }

    public class Identifier : Expression
    {
        public Token token { get; set; }
        public string value { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            return value;
        }
    }

    public class BooleanLiteral : Expression
    {
        public Token token { get; set; }
        public bool value { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            return token.literal;
        }
    }


    public class IntegerLiteral : Expression
    {
        public Token token { get; set; }
        public long value { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            return token.literal;
        }
    }

    public class StringLiteral : Expression
    {
        public Token token { get; set; }
        public string value { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            return token.literal;
        }
    }

    public class FunctionLiteral : Expression
    {
        public Token token { get; set; }
        public List<Identifier> parameters { get; set; }
        public BlockStatement body { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            List<string> paramList = new List<string>();
            foreach(Identifier param in parameters)
            {
                paramList.Add(param.ToString());
            }

            output += TokenLiteral();
            output += TokenType.LeftParen;
            output += string.Join(TokenType.Comma + " ", paramList);
            output += TokenType.RightParen;
            output += body.ToString();

            return output;
        }
    }

    public class ArrayLiteral : Expression
    {
        public Token token { get; set; }
        public List<Expression> elements { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            List<string> elems = new List<string>();
            foreach (Identifier element in elements)
            {
                elems.Add(element.ToString());
            }

            output += TokenType.LeftBracket;
            output += string.Join(TokenType.Comma + " ", elems);
            output += TokenType.RightBracket;

            return output;
        }
    }

    public class IndexExpression : Expression
    {
        public Token token { get; set; }
        public Expression left { get; set; }
        public Expression index { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            output += TokenType.LeftParen;
            output += left.ToString();
            output += TokenType.LeftBracket;
            output += index.ToString();
            output += TokenType.RightBracket;
            output += TokenType.RightParen;

            return output;
        }
    }

    public class HashLiteral : Expression
    {
        public Token token { get; set; }
        public Dictionary<Expression, Expression> pairs { get; set; }

        public void expressionNode() { }

        public string TokenLiteral() { return this.token.literal; }

        public override string ToString()
        {
            string output = "";

            List<string> pairlist = new List<string>();
            foreach (var pair in pairs)
            {
                pairlist.Add(pair.Key.ToString() + TokenType.Colon + pair.Value.ToString());
            }

            output += TokenType.LeftBrace;
            output += string.Join(TokenType.Comma + " ", pairlist);
            output += TokenType.RightBrace;

            return output;
        }

    }
}

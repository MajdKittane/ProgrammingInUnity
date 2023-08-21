﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public static class Evaluator
    {
        private const string unknownOperatorError = "unknown operator";
        private const string typeMissMatchError = "type mismatch";
        private const string identifierNotFoundError = "identifier not found";
        private const string notFunctionError = "not a function";

        private static readonly Object NULL = new Null();
        private static readonly Object TRUE = new Boolean { value = true };
        private static readonly Object FALSE = new Boolean { value = false };

        public static Object Eval(Node node, Environment env)
        {
            if (node is Program)
            {
                Program program = (Program)node;
                return EvalProgram(program, env);
            }
            else if (node is ExpressionStatement)
            {
                ExpressionStatement expression = (ExpressionStatement)node;
                return Eval(expression.expression, env);
            }
            else if (node is BlockStatement)
            {
                BlockStatement blockStatement = (BlockStatement)node;
                return EvalBlockStatement(blockStatement, env);
            }
            else if (node is ReturnStatement)
            {
                ReturnStatement returnStatement = (ReturnStatement)node;
                var val = Eval(returnStatement.returnValue, env);
                if (IsError(val))
                {
                    Error error = (Error)val;
                    return error;
                }
                return new ReturnValue { value = val };
            }
            else if (node is LetStatement)
            {
                LetStatement letStatement = (LetStatement)node;
                var val = Eval(letStatement.value, env);
                if (IsError(val))
                {
                    Error error = (Error)val;
                    return error;
                }
                env.Set(letStatement.name.value, val);
            }
            else if (node is IfExpression)
            {
                IfExpression ifExpression = (IfExpression)node;
                return EvalIfExpression(ifExpression, env);
            }
            else if (node is PrefixExpression)
            {
                PrefixExpression prefix = (PrefixExpression)node;
                var right = Eval(prefix.right, env);
                if (right is Error)
                {
                    Error error = (Error)right;
                    return error;
                }
                return EvalPrefixExpression(prefix.op, right);
            }
            else if (node is InfixExpression)
            {
                InfixExpression infix = (InfixExpression)node;
                var left = Eval(infix.left, env);

                if (IsError(left))
                {
                    Error error = (Error)left;
                    return error;
                }

                var right = Eval(infix.right, env);

                if (IsError(right))
                {
                    Error error = (Error)right;
                    return error;
                }

                
                return EvalInfixExpression(infix.op, left, right);
            }
            else if (node is CallExpression)
            {
                CallExpression callExpression = (CallExpression)node;
                var function = Eval(callExpression.function, env);
                
                if (IsError(function))
                {
                    Error error = (Error)function;
                    return error;
                }

                List<Object> args = EvalExpressions(callExpression.arguments,env);

                if (args.Count == 1 && IsError(args[0]))
                {
                    Error error = (Error)args[0];
                    return error;
                }

                return ApplyFunction(function,args);
            }
            else if (node is IndexExpression)
            {
                IndexExpression indexExpression = (IndexExpression)node;
                var left = Eval(indexExpression.left, env);
                if (IsError(left))
                {
                    Error error = (Error)left;
                    return error;
                }
                var index = Eval(indexExpression.index, env);
                if (IsError(index))
                {
                    Error error = (Error)index;
                    return error;
                }
                return EvalIndexExpression(left, index);
            }
            else if(node is Identifier)
            {
                Identifier id = (Identifier)node;
                return EvalIdentifier(id,env);
            }    
            else if(node is FunctionLiteral)
            {
                FunctionLiteral fn = (FunctionLiteral)node;
                var _parameters = fn.parameters;
                var _body = fn.body;
                return new Function { parameters = _parameters, body = _body };
            }
            else if (node is IntegerLiteral)
            {
                IntegerLiteral intLiteral = (IntegerLiteral)node;
                return new Integer { value = intLiteral.value };
            }
            else if (node is StringLiteral)
            {
                StringLiteral strLiteral = (StringLiteral)node;
                return new String { value = strLiteral.value };
            }
            else if (node is Boolean)
            {
                Boolean b = (Boolean)node;
                return NativeBoolToBooleanObject(b.value);
            }
            else if (node is ArrayLiteral)
            {
                ArrayLiteral arrayLiteral = (ArrayLiteral)node;
                List<Object> _elements = EvalExpressions(arrayLiteral.elements, env);
                if (_elements.Count == 1 && IsError(_elements[0]))
                {
                    return (Error)_elements[0];
                }
                return new Array { elements = _elements };
            }
            else if (node is HashLiteral)
            {
                HashLiteral hash = (HashLiteral)node;
                return EvalHashLiteral(hash, env);
            }
            else
            {
                return NULL;
            }
            return null;
        }

        private static Object EvalProgram(Program p,Environment env)
        {
            Object result = null;

            foreach(Statement statement in p.statements)
            {
                result = Eval(statement, env);
                if (result is ReturnValue)
                {
                    ReturnValue returnValue = (ReturnValue)result;
                    return returnValue.value;
                }
                else if (result is Error)
                {
                    Error error = (Error)result;
                    return error;
                }
            }
            return result;
        }


        private static Object EvalBlockStatement(BlockStatement block, Environment env)
        {
            Object result = null;

            foreach (Statement statement in block.statements)
            {
                result = Eval(statement, env);
                if (result != null)
                {
                    Type rt = result.GetType();
                    if (rt == typeof(ReturnValue) || rt == typeof(Error))
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private static Object EvalIfExpression(IfExpression ie, Environment env)
        {
            var condition = Eval(ie.condition, env);
            if (IsError(condition))
            {
                return (Error)condition;
            }

            if (IsTruthy(condition))
            {
                return Eval(ie.consequence, env);
            }

            else if (ie.alternative != null)
            {
                return Eval(ie.alternative, env);
            }
            else return NULL;
        }

        private static Object EvalPrefixExpression(string op, Object right)
        {
            if (op == TokenType.Bang)
            {
                return EvalBangOperatorExpression(right);
            }
            else if (op == TokenType.Minus)
            {
                return EvalMinusPrefixOperatorExpression(right);
            }
            else return new Error { message = $"{unknownOperatorError}: {op}{right.GetType()}" };
        }

        private static Object EvalInfixExpression(string op, Object left, Object right)
        {
            if (op == TokenType.Plus || op == TokenType.Minus || op == TokenType.Asterisk || op == TokenType.Slash)
            {
                return EvalIntegerInfixExpression(op, left, right);
            }
            else if (op == TokenType.LessThan || op == TokenType.GreaterThan || op == TokenType.Equal || op == TokenType.NotEqual)
            {
                return NativeBoolToBooleanObject(((Boolean)EvalIntegerInfixExpression(op, left, right)).value);
            }
            else return new Error { message = $"{unknownOperatorError}: {left.GetType()} {op} {right.GetType()}" };
        }

        private static List<Object> EvalExpressions(List<Expression> exps, Environment env)
        {
            List<Object> result = new List<Object>();

            foreach (var e in exps)
            {
                var evaluated = Eval(e, env);
                if (IsError(evaluated))
                {
                    return new List<Object> { evaluated };
                }
                result.Add(evaluated);
            }
            return result;
        }

        private static Object EvalBangOperatorExpression(Object right)
        {
            if ((right is Boolean bt && bt.value == true) || (right is Integer it && it.value <= 0))
            {
                return TRUE;
            }
            else if ((right is Boolean bf && bf.value == false) || (right is Integer iff && iff.value > 0))
            {
                return FALSE;
            }
            else if (right == null || right == NULL)
            {
                return TRUE;
            }
            else return FALSE;
        }

        private static Object EvalMinusPrefixOperatorExpression(Object right)
        {
            if (!(right is Integer))
            {
                return new Error { message = $"{unknownOperatorError}: -{right.GetType()}" };
            }

            var _value = ((Integer)right).value;
            return new Integer { value = -_value };
        }

        private static Object EvalIntegerInfixExpression(string op, Object left, Object right)
        {
            var leftValue = ((Integer)left).value;
            var rightValue = ((Integer)right).value;

            if (op == TokenType.Plus)
            {
                return new Integer { value = leftValue + rightValue};
            }
            else if (op == TokenType.Minus)
            {
                return new Integer { value = leftValue - rightValue };
            }
            else if (op == TokenType.Asterisk)
            {
                return new Integer { value = leftValue * rightValue };
            }
            else if (op == TokenType.Slash)
            {
                return new Integer { value = leftValue / rightValue };
            }
            else if (op == TokenType.LessThan)
            {
                return new Boolean { value = leftValue < rightValue };
            }
            else if (op == TokenType.GreaterThan)
            {
                return new Boolean { value = leftValue > rightValue };
            }
            else if (op == TokenType.Equal)
            {
                return new Boolean { value = leftValue == rightValue };
            }
            else if (op == TokenType.NotEqual)
            {
                return new Boolean { value = leftValue != rightValue };
            }
            else
            {
                return new Error { message = $"{unknownOperatorError}: {left.GetType()} {op} {right.GetType()}" };
            }
        }

        private static Object EvalStringInfixExpression(string op, Object left, Object right)
        {
            if (op != TokenType.Plus)
            {
                return new Error { message = $"{unknownOperatorError}: {left.GetType()} {op} {right.GetType()}" };
            }

            var leftVal = ((String)left).value;
            var rightVal = ((String)right).value;
            return new String { value = leftVal + rightVal };
        }

        private static Object EvalIndexExpression(Object left, Object index)
        {
            if (left is Array && index is Integer)
            {
                Array array = (Array)left;
                Integer idx = (Integer)index;
                return EvalArrayIndexExpression(array, idx);
            }
            else if (left is Hash)
            {
                Hash hash = (Hash)left;
                return EvalHashIndexExpression(hash, index);
            }
            else return new Error { message = $"index operator not supported: {left.GetType()}" };
        }

        private static Object EvalArrayIndexExpression(Array array, Integer index)
        {
            long idx = index.value;
            long max = array.elements.Count-1;

            if (idx < 0 || idx > max)
            {
                return NULL;
            }

            return array.elements[(int)idx];
        }

        private static Object EvalHashIndexExpression(Hash hash, Object index)
        {
            if (!(index is Hashable))
            {
                return new Error { message = $"unusable as hash key: {index.GetType()}" };
            }

            var key = (Hashable)index;
            if (!hash.pairs.TryGetValue(key.HashKey(), out var pair))
            {
                return NULL;
            }
            return pair.value;
        }


        private static Object EvalIdentifier(Identifier node, Environment env)
        {
            if (env.Get(node.value) != null)
            {
                return env.Get(node.value);
            }

            if (builtins.TryGetValue(node.value, out var builtin))
            {
                return builtin;
            }

            return new Error { message = $"{identifierNotFoundError}: {node.value}" };
        }

        private static Object EvalHashLiteral(HashLiteral node, Environment env)
        {
            var _pairs = new Dictionary<HashKey, HashPair>();
            
            foreach (var keyNode in node.pairs.Keys)
            {
                var valueNode = node.pairs[keyNode];
                var key = Eval(keyNode, env);
                if (IsError(key))
                {
                    return (Error)key;
                }

                if (!(key is Hashable))
                {
                    return new Error { message = $"unusable as hash key: {key.GetType()}" };
                }

                var hashKey = ((Hashable)key).HashKey();
                var _value = Eval(valueNode, env);

                if (IsError(_value))
                {
                    return (Error)_value;
                }

                _pairs[hashKey] = new HashPair { key = key, value = _value };
            }
            return new Hash { pairs = _pairs};
        }

        private static Object ApplyFunction(Object fn, List<Object> args)
        {
            if (fn is Function)
            {
                Function function = (Function)fn;
                var extendedEnv = ExtendFunctionEnvironment(function, args);
                var evaluated = Eval(function.body, extendedEnv);
                return UnwrapReturnValue(evaluated);
            }
            else if (fn is Builtin)
            {
                Builtin builtin = (Builtin)fn;
                return builtin.fn(args);
            }
            else return new Error { message = $"{notFunctionError}: {fn.GetType()}" };
        }

        private static Environment ExtendFunctionEnvironment(Function fn, List<Object> args)
        {
            var env = Environment.NewEnclosedEnvironment(fn.env);

            for (var i = 0; i<fn.parameters.Count; i++)
            {
                env.Set(fn.parameters[i].value, args[i]);
            }

            return env;
        }

        private static Object UnwrapReturnValue(Object obj)
        {
            if (obj is ReturnValue)
            {
                ReturnValue rv = (ReturnValue)obj;
                return rv.value;
            }
            else return obj;
        }

        private static bool IsTruthy(Object obj)
        {
            if (obj is null)
            {
                return false;
            }
            else if (obj is Boolean)
            {
                Boolean b = (Boolean)obj;
                return b.value;
            }
            else return true;
        }

        private static Object NativeBoolToBooleanObject(bool value)
        {
            return value ? TRUE : FALSE;
        }

        private static readonly Dictionary<string, Builtin> builtins = new Dictionary<string, Builtin>();

        private static bool IsError(Object obj)
        {
            return obj != null && obj.GetType() == typeof(Error);
        }
    }
}
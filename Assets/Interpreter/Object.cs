using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public enum ObjectType
    {
        Integer,
        Boolean,
        Null,
        ReturnValue,
        Error,
        Function,
        String,
        Builtin,
        Array,
        Hash,
        CompiledFunction,
        Closure
    }
    public interface Object
    {
        ObjectType Type();
        string Inspect();

    }
    public struct HashKey
    {
        public ObjectType type { get; set; }
        public ulong value { get; set; }

        
    }

    public interface Hashable
    {
        HashKey HashKey();
    }

    public class Integer : Object, Hashable
    {
        public long value { get; set; }
        public ObjectType Type() => ObjectType.Integer;
        public string Inspect() => value.ToString();
        public HashKey HashKey()
        {
            return new HashKey { type = Type(), value = (ulong)value };
        }
    }

    public class Boolean : Object, Hashable
    {
        public bool value { get; set; }
        public ObjectType Type() => ObjectType.Boolean;
        public string Inspect() => value.ToString();
        public HashKey HashKey()
        {
            ulong hashValue = value ? 1u : 0u;
            return new HashKey { type = Type(), value = hashValue };
        }
    }

    public class Null : Object
    {
        public ObjectType Type() => ObjectType.Null;

        public string Inspect() => "null";
    }

    public class ReturnValue : Object
    {
        public Object value { get; set; }

        public ObjectType Type() => ObjectType.ReturnValue;

        public string Inspect() => value.Inspect();
    }

    public class Error : Object
    {
        public string message { get; set; }

        public ObjectType Type() => ObjectType.Error;

        public string Inspect() => "ERROR: " + message;
    }

    public class Function : Object
    {
        public List<Identifier> parameters { get; set; }
        public BlockStatement body { get; set; }
        public Environment env { get; set; }

        public ObjectType Type() => ObjectType.Function;

        public string Inspect()
        {
            string output = "";

            List<string> paramList = new List<string>();
            foreach (string param in paramList)
            {
                paramList.Add(param);
            }

            output += "fn(";
            output += string.Join(", ", paramList);
            output += ") {\n";
            output += body.ToString();
            output += "\n}";

            return output;
        }
    }

    public class String : Object, Hashable
    {
        public string value { get; set; }
        public ObjectType Type() => ObjectType.String;
        public string Inspect() => value;

        public static uint FNVHash(string str)
        {
            const uint fnv_prime = 0x811C9DC5;
            uint hash = 0;
            uint i = 0;

            for (i = 0; i < str.Length; i++)
            {
                hash *= fnv_prime;
                hash ^= ((byte)str[(int)i]);
            }

            return hash;
        }


        public HashKey HashKey()
        {
            var hash = FNVHash(value);
            return new HashKey { type = Type(), value = hash };
        }
    }

    public delegate Object BuiltinFunction(List<Object> args);

    public class Builtin : Object
    {
        public BuiltinFunction fn { set; get; }
        public ObjectType Type() => ObjectType.Builtin;
        public string Inspect() => "builtin function";
    }

    public class Array : Object
    {
        public List<Object> elements { get; set; }
        public ObjectType Type() => ObjectType.Array;
        public string Inspect()
        {
            string output = "";
             
            List<string> elementsStr = new List<string>();
            foreach (Object element in elements)
            {
                elementsStr.Add(element.Inspect());
            }

            output += "[";
            output += string.Join(", ",elementsStr);
            output += "]";

            return output;
        }
        
    }

    public class HashPair
    {
        public Object key;
        public Object value;
    }

    public class Hash : Object
    {
        public Dictionary<HashKey,HashPair> pairs { get; set; }
        public ObjectType Type() => ObjectType.Hash;

        public string Inspect()
        {
            string output = "";

            List<string> pairsStr = new List<string>();
            foreach (HashPair pair in pairs.Values)
            {
                pairsStr.Add($"{pair.key.Inspect()} : {pair.value.Inspect()}");
            }


            output += TokenType.LeftBrace;
            output += string.Join(", ", pairsStr);
            output += TokenType.RightBrace;

            return output;
        }
    }
    

}

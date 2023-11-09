using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public class Builtins
    {
        static Object PrintOutput(List<Object> args)
        {
            foreach(var arg in args)
            {
                Console.WriteLine(arg.Inspect());
            }
            return new Null();
        }

        public static Dictionary<string, Builtin> builtins = new Dictionary<string, Builtin>()
        {
            {"out",new Builtin{fn=PrintOutput}}
        };
    }
}

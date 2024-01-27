using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MyInterpreter
{
    public class Builtins
    {
        static Object PrintOutput(List<Object> args)
        {
            foreach(var arg in args)
            {
                if (Evaluator.observer != null)
                {
                    Evaluator.observer.HandleOutputStream(arg.Inspect());
                }
            }
            return new Null();
        }

        public static Dictionary<string, Builtin> builtins = new Dictionary<string, Builtin>()
        {
            {"out",new Builtin{fn=PrintOutput}}
        };
    }
}

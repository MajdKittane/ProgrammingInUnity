using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    class MainClass
    {
        static void Main(string[] args)
        {

            var input = @"let x = 20 % 7;
";
            /*var input2 = @"if ( 1 < 2)
{
if (4 < 3)
{
let x = 1;
}
else
{
let x = 0;
}
}
";*/
            Console.WriteLine(input);
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();

            /*foreach (var v in program.statements)
            {
                Console.WriteLine(v.ToString());
            }*/


            Environment env = new Environment();
            Evaluator.Eval(program, env);
            foreach (var v in env.store.Keys)
            {
                Console.WriteLine(v + "\t" + env.store[v].Inspect());


            }
        }
    }
}
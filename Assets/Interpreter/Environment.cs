using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public class Environment
    {
        public Dictionary<string, Object> store;
        private Environment outer;

        public Environment()
        {
            store = new Dictionary<string, Object>();
            outer = null;
        }

        private Environment(Dictionary<string,Object> _store, Environment _outer)
        {
            this.store = _store;
            this.outer = _outer;
        }

        public static Environment NewEnvironment()
        {
            return new Environment(new Dictionary<string, Object>(), null);
        }

        public static Environment NewEnclosedEnvironment(Environment outer)
        {
            return new Environment(new Dictionary<string, Object>(), outer);
        }

        public Object Get(string name)
        {
            if (store.TryGetValue(name, out Object obj))
            {
                return obj;
            }
            else if (outer != null)
            {
                return outer.Get(name);
            }
            else return null;
        }

        public void Set(string name, Object val)
        {
            store[name] = val;
        }
    }
}

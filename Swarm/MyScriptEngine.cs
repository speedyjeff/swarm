using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarm
{
    public static class MyScriptEngine
    {
        static ScriptScope m_script;

        static MyScriptEngine()
        {
            ScriptEngine engine;

            engine = Python.CreateEngine();
            m_script = engine.CreateScope();
        }

        public static void DefineVariable(string name, object instance)
        {
            m_script.Engine.GetBuiltinModule().SetVariable(name, instance);
        }

        public static void Execute(string script)
        {
            m_script.Engine.Execute(script);
        }
    }
}

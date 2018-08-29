using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarm
{
    internal class SwarmEngine
    {
        // data
        private PlotColor m_color;
        private string m_script;
        private static SwarmArgs m_args;
        private bool m_executed;

        static SwarmEngine()
        {
            m_args = new SwarmArgs();
            MyScriptEngine.DefineVariable("swarm", m_args);
        }

        public SwarmEngine(PlotColor color)
        {
            m_color = color;
            m_script = "";
            m_executed = false;
        }

        public Move RequestMove(int previousState, int h, int w, int[,] field)
        {
            lock (m_args)
            {
                // execute the script only once
                if (!m_executed)
                {
                    m_script = m_script.Replace("def Move(", "def Move" + m_color + "(");
                    m_script += "\n" +
                        "def Run_Move" + m_color + "():\n" +
                            "  swarm.Return = Move" + m_color + "(swarm.Previous, swarm.H, swarm.W, swarm.Field)\n";
                    MyScriptEngine.Execute( m_script);
                    m_script += "\n" +
                        "Run_Move" + m_color + "()\n";
                    m_executed = true;
                }

                // call the function
                m_args.Return = 0;
                m_args.H = h;
                m_args.W = w;
                m_args.Previous = previousState;
                m_args.Field = field;

                MyScriptEngine.Execute(m_script);

                return (Move)m_args.Return;
            }
        }

        public PlotColor Color { get { return m_color; } }
        public string Script { get { return m_script; } set { m_script = value; m_executed = false; } }
    }
}

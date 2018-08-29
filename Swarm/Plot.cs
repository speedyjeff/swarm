using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarm
{
    internal class Plot
    {
        public PlotState State;
        public PlotColor Color;
        public int WaitDuration;

        public Plot()
        {
            State = PlotState.Forbidden;
            Color = PlotColor.Clear;
            WaitDuration = 0;
        }

        public object Clone()
        {
            Plot p = new Plot();
            p.State = State;
            p.Color = Color;
            p.WaitDuration = WaitDuration;

            return p;
        }
    }
}

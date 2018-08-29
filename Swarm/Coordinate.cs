using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarm
{
    internal struct Coordinate
    {
        public int H;
        public int W;

        public Coordinate(int h, int w)
        {
            H = h;
            W = w;
        }
    }
}

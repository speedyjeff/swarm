using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarm
{
    public enum PlotState
    {
        Forbidden = 0,
        Unoccupied = 1,
        Occupied = 2,
        Defended = 3,
        Duplication = 4,
        Visited = 5,
        Enemy = 6
    }
}

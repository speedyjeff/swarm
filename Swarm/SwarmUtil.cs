using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarm
{
    public static class SwarmUtil
    {
        public static char Seperator { get { return ':'; } }

        public static string Encode(PlotColor color, int h, int w, PlotState state, int waitTime)
        {
            string str;

            // height
            str = Seperator + h.ToString();

            // color
            switch (color)
            {
                case PlotColor.Clear: str += "c"; break;
                case PlotColor.Red: str += "r"; break;
                case PlotColor.Blue: str += "b"; break;
                case PlotColor.Green: str += "g"; break;
                case PlotColor.Yellow: str += "y"; break;
            }

            // waittime
            str += waitTime;

            // state
            switch (state)
            {
                case PlotState.Forbidden: str += "f"; break;
                case PlotState.Unoccupied: str += "u"; break;
                case PlotState.Occupied: str += "o"; break;
                case PlotState.Defended: str += "e"; break;
                case PlotState.Duplication: str += "d"; break;
                case PlotState.Visited: str += "v"; break;
            }

            // width
            str += w;

            return str;
        }

        public static bool Decode(string str, out PlotColor color, out int h, out int w, out PlotState state, out int waitTime)
        {
            // the rest should follow this format
            //  HEIGHTcolorWAITDURATIONstateWIDTH
            int part;
            string numStr;
            char[] chArray;

            part = 0;
            numStr = "";
            color = PlotColor.Clear;
            h = w = 0;
            state = PlotState.Forbidden;
            waitTime = 0;
            chArray = str.ToCharArray();

            // check that this is a valid message
            if (0 == chArray.Length || !Char.IsDigit(chArray[0])) return false;

            foreach (char c in chArray)
            {
                switch (part)
                {
                    case 0:
                        if (Char.IsDigit(c))
                        {
                            numStr += c;
                        }
                        else
                        {
                            // height
                            h = Convert.ToInt32(numStr);
                            numStr = "";
                            // color
                            switch (c)
                            {
                                case 'r': color = PlotColor.Red; break;
                                case 'b': color = PlotColor.Blue; break;
                                case 'g': color = PlotColor.Green; break;
                                case 'y': color = PlotColor.Yellow; break;
                                case 'c': color = PlotColor.Clear; break;
                            }
                            part = 1;
                        }
                        break;
                    case 1:
                        if (Char.IsDigit(c))
                        {
                            numStr += c;
                        }
                        else
                        {
                            // wait time
                            waitTime = Convert.ToInt32(numStr);
                            numStr = "";
                            // state
                            switch (c)
                            {
                                case 'f': state = PlotState.Forbidden; break;
                                case 'u': state = PlotState.Unoccupied; break;
                                case 'o': state = PlotState.Occupied; break;
                                case 'e': state = PlotState.Defended; break;
                                case 'd': state = PlotState.Duplication; break;
                                case 'v': state = PlotState.Visited; break;
                            }
                            part = 2;
                        }
                        break;
                    case 2:
                        numStr += c;
                        break;
                }
            }
            // width
            w = Convert.ToInt32(numStr);

            return true;
        }
    }
}

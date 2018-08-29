using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarm
{
    internal class Field
    {
        // data
        private Plot[,] m_field;
        private string m_transaction;
        private int[] m_count;

        public Field()
        {
            m_field = null;
            m_transaction = "";
            m_count = null;
        }

        public Field(int height, int width)
        {
            m_field = new Plot[height, width];
            m_count = new int[((int)PlotColor.Yellow + 1)];

            // initialize the counts
            m_count[(int)PlotColor.Clear] = height * width;
            m_count[(int)PlotColor.Red] = 0;
            m_count[(int)PlotColor.Blue] = 0;
            m_count[(int)PlotColor.Green] = 0;
            m_count[(int)PlotColor.Yellow] = 0;

            for (int i = 0; i < m_field.GetLength(0); i++)
            {
                for (int j = 0; j < m_field.GetLength(1); j++)
                {
                    m_field[i, j] = new Plot();
                    m_field[i, j].State = PlotState.Unoccupied;
                }
            }
        }

        public bool ChangeState(PlotColor color, int h, int w, PlotState state)
        {
            return ChangeState(color, h, w, state, m_field[h, w].WaitDuration);
        }

        public bool ChangeState(PlotColor color, int h, int w, PlotState state, int waitTime)
        {
            if (h >= 0 && h < m_field.GetLength(0) && w >= 0 && w < m_field.GetLength(1))
            {
                // append transaction
                m_transaction += SwarmUtil.Encode(color, h, w, state, waitTime);

                // upate color counts
                m_count[(int)m_field[h, w].Color]--;
                m_count[(int)color]++;

                m_field[h, w].State = state;
                m_field[h, w].Color = color;
                m_field[h, w].WaitDuration = waitTime;
                return true;
            }

            return false;
        }

        public bool ResetTransaction()
        {
            m_transaction = "";
            return true;
        }

        public int Count(PlotColor color)
        {
            if (PlotColor.Clear <= color && color <= PlotColor.Yellow)
            {
                return m_count[(int)color];
            }
            return 0;
        }

        public int Height { get { return (null == m_field) ? 0 : m_field.GetLength(0); } }
        public int Width { get { return (null == m_field) ? 0 : m_field.GetLength(1); } }
        public string Transaction { get { return m_transaction; } }

        public Plot this[int h, int w]
        {
            get
            {
                if (null != m_field && h >= 0 && h < m_field.GetLength(0) && w >= 0 && w < m_field.GetLength(1))
                {
                    return (Plot)m_field[h, w].Clone();
                }
                else
                {
                    return new Plot();
                }
            }
        }
    }
}

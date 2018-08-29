using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarm
{
    public class SwarmGame
    {
        // constants
        private const int c_width = 50;  // plots
        private const int c_height = 50;  // plots
        private const int c_fieldView = 5;  // plots
        private const int c_initialIndividuals = 5;
        private const int c_defendDuration = 1;    // turns
        private int m_maxIndividuals = 25;
        private int m_duplicationDuration = 5;    // turns
        private float m_winningPercentage = 0.50f;

        // data
        private Field m_field;
        private SwarmEngine[] m_swarm;
        private int m_currentSwarm;
        private bool[] m_validSwarm;

        public SwarmGame()
        {
            m_field = new Field(c_height, c_width);
            m_swarm = new SwarmEngine[((int)PlotColor.Yellow + 1)];
            m_validSwarm = new bool[((int)PlotColor.Yellow + 1)];
            m_currentSwarm = (int)PlotColor.Clear;
            IsQuiting = false;

            for (int i = 0; i < m_swarm.Length; i++) m_swarm[i] = new SwarmEngine((PlotColor)i);
            for (int i = 0; i < m_validSwarm.Length; i++) m_validSwarm[i] = true;
        }

        public bool Configure(PlotColor color, string script, out string transaction)
        {
            int[][] coords;
            coords = new int[(int)PlotColor.Yellow + 1][];
            coords[(int)PlotColor.Clear] = new int[] { 0, 0 };
            coords[(int)PlotColor.Red] = new int[] { 0, m_field.Width - 1 };
            coords[(int)PlotColor.Blue] = new int[] { m_field.Height - c_initialIndividuals, 0 };
            coords[(int)PlotColor.Green] = new int[] { m_field.Height - c_initialIndividuals, m_field.Width - 1 };
            coords[(int)PlotColor.Yellow] = new int[] { 0, 0 };

            transaction = "";

            lock (m_field)
            {
                // reset transaction
                m_field.ResetTransaction();

                // setup the swarm
                m_swarm[(int)color].Script = script;

                // initial swarms
                for (int i = 0; i < c_initialIndividuals; i++)
                {
                    m_field.ChangeState(color, coords[(int)color][0] + i, coords[(int)color][1], PlotState.Occupied);
                }

                transaction = SwarmUtil.Seperator + m_field.Transaction;
            }

            return true;
        }

        public void InitialFieldConfig(FieldConfiguration fieldConfig, out string transaction)
        {
            transaction = "";

            lock (m_field)
            {
                // reset transaction
                m_field.ResetTransaction();

                // clear the board
                for (int w = 0; w < m_field.Width; w++)
                {
                    for (int h = 0; h < m_field.Height; h++)
                    {
                        m_field.ChangeState(PlotColor.Clear, h, w, PlotState.Unoccupied);
                    }
                }

                switch (fieldConfig)
                {
                    case FieldConfiguration.Default:
                        break;
                    case FieldConfiguration.Quads:
                        for (int h = 0; h < (2 * m_field.Height) / 5; h++)
                            m_field.ChangeState(PlotColor.Clear, h, m_field.Width / 2, PlotState.Forbidden);
                        for (int h = m_field.Height - 1; h > (3 * m_field.Height) / 5; h--)
                            m_field.ChangeState(PlotColor.Clear, h, m_field.Width / 2, PlotState.Forbidden);

                        for (int w = 0; w < (2 * m_field.Width) / 5; w++)
                            m_field.ChangeState(PlotColor.Clear, m_field.Height / 2, w, PlotState.Forbidden);
                        for (int w = m_field.Width - 1; w > (3 * m_field.Width) / 5; w--)
                            m_field.ChangeState(PlotColor.Clear, m_field.Height / 2, w, PlotState.Forbidden);
                        break;
                    case FieldConfiguration.Maze:
                        if (50 != m_field.Width || 50 != m_field.Height) return;

                        byte[,] maze = new byte[,] {
                            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1, 1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1, 1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                            {0,0,1,1,1,1,0,0,1,0,0,1,1,1,1,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,1,1,1,1,0,0,1,0,0,1,1,1,1,0,0},
                            {0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,1,1,1,1,0,0,1,1,0,0,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,0,0,1,1,0,0,1,1,1,1,0,0,1,0,0},
                            {0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,1,0,0,1,0,0,1,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,1,0,0,1,0,0,1,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1, 1,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1, 1,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,0,0,1, 1,0,0,1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0},
                            {0,0,1,1,1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0},
                            {0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
                            {0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0},
                            {1,1,1,1,1,1,0,0,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,0,0,1,1,1,1,1,1},
                            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,1, 1,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                            {0,0,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,0,0,1,0,0,1,1,1, 1,1,1,0,0,1,0,0,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0},
                            {0,0,1,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,0,1, 1,0,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,1, 1,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,0,0,1,0,0},
                            {0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0},
                            {0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0},
                            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0, 0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},

                            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0, 0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                            {0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0},
                            {0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0},
                            {0,0,1,0,0,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,1, 1,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,0,1, 1,0,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,1,0,0},
                            {0,0,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,0,0,1,0,0,1,1,1, 1,1,1,0,0,1,0,0,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0},
                            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,1, 1,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                            {1,1,1,1,1,1,0,0,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,0,0,1,1,1,1,1,1},
                            {0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0},
                            {0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
                            {0,0,1,1,1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0},
                            {0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,1,1,0,0,1, 1,0,0,1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1, 1,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1, 1,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,1,0,0,1,0,0,1,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,1,0,0,1,0,0,1,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0},
                            {0,0,1,0,0,1,1,1,1,0,0,1,1,0,0,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,0,0,1,1,0,0,1,1,1,1,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0},
                            {0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,1,0,0},
                            {0,0,1,1,1,1,0,0,1,0,0,1,1,1,1,1,0,0,1,0,0,1,0,0,1, 1,0,0,1,0,0,1,0,0,1,1,1,1,1,0,0,1,0,0,1,1,1,1,0,0},
                            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1, 1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1, 1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
                            };

                        for (int w = 0; w < maze.GetLength(0); w++)
                        {
                            for (int h = 0; h < maze.GetLength(1); h++)
                            {
                                if (1 == maze[h, w]) m_field.ChangeState(PlotColor.Clear, h, w, PlotState.Forbidden);
                            }
                        }
                        break;
                    case FieldConfiguration.Hill:
                        int[][] wbounds = new int[][] {
						//                  WIDTH_VALUE        LOW_HEIGHT           HIGH_HEIGHT               EXCLUDE_H EXCLUDE_H
						new int[]{(   m_field.Width)/10,     (  m_field.Height)/10, (9*m_field.Height)/10-1, 0, 0}, // left
						new int[]{(9*m_field.Width)/10-1, (  m_field.Height)/10, (9*m_field.Height)/10-1, 0, 0}, // right

						new int[]{(2*m_field.Width)/10,    (2*m_field.Height)/10, (8*m_field.Height)/10-1, m_field.Height/2, m_field.Height/2-1}, // left n=2
						new int[]{(8*m_field.Width)/10-1, (2*m_field.Height)/10, (8*m_field.Height)/10-1, m_field.Height/2, m_field.Height/2-1}, // right n=2

						new int[]{(3*m_field.Width)/10,    (3*m_field.Height)/10, (7*m_field.Height)/10-1, 0, 0}, // left n=3
						new int[]{(7*m_field.Width)/10-1, (3*m_field.Height)/10, (7*m_field.Height)/10-1, 0, 0}, // right n=3

						new int[]{(4*m_field.Width)/10,    (4*m_field.Height)/10, (6*m_field.Height)/10-1, m_field.Height/2, m_field.Height/2-1}, // left n=4
						new int[]{(6*m_field.Width)/10-1, (4*m_field.Height)/10, (6*m_field.Height)/10-1, m_field.Height/2, m_field.Height/2-1} // right n=4
						};
                        int[][] hbounds = new int[][] {
						//                HEIGHT_VALUE          LOW_WIDTH           HIGH_WIDTH               EXCLUE_W       EXCLUDE_W
						new int[]{(  m_field.Height)/10,      (  m_field.Width)/10, (9*m_field.Width)/10-1, m_field.Width/2, m_field.Width/2-1}, // top
						new int[]{(9*m_field.Height)/10-1, (  m_field.Width)/10, (9*m_field.Width)/10-1, m_field.Width/2, m_field.Width/2-1}, // bottom

						new int[]{(2*m_field.Height)/10,     (2*m_field.Width)/10, (8*m_field.Width)/10-1, 0, 0}, // top n=2
						new int[]{(8*m_field.Height)/10-1, (2*m_field.Width)/10, (8*m_field.Width)/10-1, 0, 0}, // bottom n=2

						new int[]{(3*m_field.Height)/10,     (3*m_field.Width)/10, (7*m_field.Width)/10-1, m_field.Width/2, m_field.Width/2-1}, // top n=3
						new int[]{(7*m_field.Height)/10-1, (3*m_field.Width)/10, (7*m_field.Width)/10-1, m_field.Width/2, m_field.Width/2-1}, // bottom n=3

						new int[]{(4*m_field.Height)/10,     (4*m_field.Width)/10, (6*m_field.Width)/10-1, 0, 0}, // top n=4
						new int[]{(6*m_field.Height)/10-1, (4*m_field.Width)/10, (6*m_field.Width)/10-1, 0, 0} // bottom n=4
						};

                        for (int w = 0; w < m_field.Width; w++)
                        {
                            for (int h = 0; h < m_field.Height; h++)
                            {
                                foreach (int[] bound in wbounds)
                                {
                                    if (bound[0] == w && bound[1] <= h && h <= bound[2] && (h != bound[3] && h != bound[4]))
                                    {
                                        m_field.ChangeState(PlotColor.Clear, h, w, PlotState.Forbidden);
                                    }
                                }

                                foreach (int[] bound in hbounds)
                                {
                                    if (bound[0] == h && bound[1] <= w && w <= bound[2] && (w != bound[3] && w != bound[4]))
                                    {
                                        m_field.ChangeState(PlotColor.Clear, h, w, PlotState.Forbidden);
                                    }
                                }
                            }
                        }
                        break;
                }

                transaction = SwarmUtil.Seperator + m_field.Transaction;
            }
        }

        public bool Start(int maxIndividuals, int duplicationDuration, int winningPercentage, bool[] players)
        {
            if ((int)PlotColor.Clear == m_currentSwarm)
            {
                m_maxIndividuals = maxIndividuals;
                m_duplicationDuration = duplicationDuration;
                m_winningPercentage = (float)winningPercentage / 100.0f;

                // skip the players that are false in players
                for (int i = 0; i < players.Length; i++) m_validSwarm[i] = players[i];
                m_validSwarm[(int)PlotColor.Clear] = false;

                NextTurnInternal();
                return true;
            }
            else
            {
                return false;
            }
        }

        private int[,] GetFieldView(PlotColor color, int w, int h)
        {
            int[,] field;

            field = new int[2 * c_fieldView + 1, 2 * c_fieldView + 1];

            for (int i = -1 * c_fieldView; i < c_fieldView; i++)
            {
                for (int j = -1 * c_fieldView; j < c_fieldView; j++)
                {
                    if (PlotState.Forbidden == m_field[i + w, j + h].State)
                    {
                        // outside the bounds of the field
                        field[i + c_fieldView, j + c_fieldView] = (int)PlotState.Forbidden;
                    }
                    else if (m_field[i + w, j + h].Color == color)
                    {
                        // colors match
                        field[i + c_fieldView, j + c_fieldView] = (int)m_field[i + w, j + h].State;
                    }
                    else
                    {
                        if (PlotColor.Clear == m_field[i + w, j + h].Color)
                        {
                            // unoccupied
                            field[i + c_fieldView, j + c_fieldView] = (int)PlotState.Unoccupied;
                        }
                        else
                        {
                            // enemy
                            field[i + c_fieldView, j + c_fieldView] = (int)PlotState.Enemy;
                        }
                    }
                }
            }

            return field;
        }

        private List<Coordinate> GetSwarm(PlotColor color)
        {
            // review the field and return a list of coordinates for the given color
            List<Coordinate> coords;

            coords = new List<Coordinate>();

            for (int i = 0; i < m_field.Height; i++)
            {
                for (int j = 0; j < m_field.Width; j++)
                {
                    if (color == m_field[i, j].Color && (PlotState.Occupied == m_field[i, j].State || PlotState.Defended == m_field[i, j].State || PlotState.Duplication == m_field[i, j].State))
                    {
                        coords.Add(new Coordinate(i, j));
                    }
                }
            }

            return coords;
        }

        public bool NextTurn(PlotColor color, out string transaction)
        {
            int[,] field;
            Move move;
            int newH, newW;
            List<Coordinate> swarm;

            transaction = "";

            lock (m_field)
            {
                // reset transaction
                m_field.ResetTransaction();

                // check that the color matches
                if (PlotColor.Clear == color || color != m_swarm[m_currentSwarm].Color) return false;

                // iterate through all the individuals that ask them to move
                swarm = GetSwarm(m_swarm[m_currentSwarm].Color);
                foreach (Coordinate coord in swarm)
                {
                    // check if the individual is in a wait
                    if (1 < m_field[coord.H, coord.W].WaitDuration)
                    {
                        m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W, m_field[coord.H, coord.W].State, m_field[coord.H, coord.W].WaitDuration - 1);
                    }
                    else
                    {
                        // check for a duplication
                        if (PlotState.Duplication == m_field[coord.H, coord.W].State && m_maxIndividuals > swarm.Count)
                        {
                            // add an individual near this point
                            if (PlotState.Unoccupied == m_field[coord.H - 1, coord.W].State) m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H - 1, coord.W, PlotState.Occupied);
                            else if (PlotState.Unoccupied == m_field[coord.H + 1, coord.W].State) m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H + 1, coord.W, PlotState.Occupied);
                            else if (PlotState.Unoccupied == m_field[coord.H, coord.W - 1].State) m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W - 1, PlotState.Occupied);
                            else if (PlotState.Unoccupied == m_field[coord.H, coord.W + 1].State) m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W + 1, PlotState.Occupied);
                            else if (PlotState.Visited == m_field[coord.H - 1, coord.W].State) m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H - 1, coord.W, PlotState.Occupied);
                            else if (PlotState.Visited == m_field[coord.H + 1, coord.W].State) m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H + 1, coord.W, PlotState.Occupied);
                            else if (PlotState.Visited == m_field[coord.H, coord.W - 1].State) m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W - 1, PlotState.Occupied);
                            else if (PlotState.Visited == m_field[coord.H, coord.W + 1].State) m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W + 1, PlotState.Occupied);
                        }

                        // grab the view of the field
                        field = GetFieldView(m_swarm[m_currentSwarm].Color, coord.H, coord.W);

                        // ask for the move
                        move = m_swarm[m_currentSwarm].RequestMove((int)m_field[coord.H, coord.W].State, field.GetLength(0) / 2, field.GetLength(1) / 2, field);

                        // apply the move
                        if (Move.Up == move || Move.Down == move || Move.Left == move || Move.Right == move)
                        {
                            newH = coord.H;
                            newW = coord.W;

                            switch (move)
                            {
                                case Move.Up: newH--; break;
                                case Move.Down: newH++; break;
                                case Move.Left: newW--; break;
                                case Move.Right: newW++; break;
                            }

                            if (PlotState.Defended == m_field[newH, newW].State || PlotState.Forbidden == m_field[newH, newW].State)
                            {
                                // dead
                                m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W, PlotState.Visited);
                            }
                            else
                            {
                                // change the color of this plot
                                m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W, PlotState.Visited);
                                m_field.ChangeState(m_swarm[m_currentSwarm].Color, newH, newW, PlotState.Occupied);
                            }
                        }
                        else if (Move.Defend == move)
                        {
                            m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W, PlotState.Defended, c_defendDuration);
                        }
                        else if (Move.Duplicate == move)
                        {
                            m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W, PlotState.Duplication, m_duplicationDuration);
                        }
                        else
                        {
                            m_field.ChangeState(m_swarm[m_currentSwarm].Color, coord.H, coord.W, PlotState.Occupied);
                        }
                    }
                }

                // advance the user
                NextTurnInternal();

                transaction = SwarmUtil.Seperator + m_field.Transaction;
            } // lock

            return true;
        }

        private void NextTurnInternal()
        {
            // advance the current player [Red..Yellow]
            do
            {
                m_currentSwarm = (m_currentSwarm + 1) % ((int)PlotColor.Yellow + 1);
            }
            while (!m_validSwarm[m_currentSwarm]);
        }

        public bool IsQuiting { get; set; }

        public PlotColor Winner
        {
            get
            {
                lock (m_field)
                {
                    // compute the winning percentage for each color
                    float[] percentage = new float[((int)PlotColor.Yellow + 1)];
                    percentage[(int)PlotColor.Clear] = (float)m_field.Count(PlotColor.Clear) / (float)(c_height * c_width);
                    percentage[(int)PlotColor.Red] = (float)m_field.Count(PlotColor.Red) / (float)(c_height * c_width);
                    percentage[(int)PlotColor.Blue] = (float)m_field.Count(PlotColor.Blue) / (float)(c_height * c_width);
                    percentage[(int)PlotColor.Green] = (float)m_field.Count(PlotColor.Green) / (float)(c_height * c_width);
                    percentage[(int)PlotColor.Yellow] = (float)m_field.Count(PlotColor.Yellow) / (float)(c_height * c_width);

                    for (int i = 1; i < percentage.Length; i++) if (m_winningPercentage <= percentage[i]) return (PlotColor)i;
                    return PlotColor.Clear;
                }
            }
        }
    }
}

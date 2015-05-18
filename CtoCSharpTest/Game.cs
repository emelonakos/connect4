using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;

namespace CtoCSharpTest
{


    class Game
    {
        public static int WIDTH = 7;
        public static int HEIGHT = 6;
        public static int H1 = HEIGHT + 1;
        public static int H2 = HEIGHT + 2;
        public static int SIZE = HEIGHT * WIDTH;
        public static int SIZE1 = H1 * WIDTH;
        public static ulong COL1 = (((ulong)1 << H1) - (ulong)1);
        public static ulong ALL1 = (((ulong)1 << SIZE1) - (ulong)1);
        public static ulong BOTTOM = ALL1 / COL1;
        public static ulong TOP = BOTTOM << HEIGHT;
        public static ulong[] color = new ulong[2];
        public static int[] moves = new int[SIZE];
        public static int nplies;
        public static char[] height = new char[WIDTH];
        public static List<string[]> strDatabase = new List<string[]>();
        public static Dictionary<ulong, int> database = new Dictionary<ulong, int>();
        public static int[] trueheight = new int[7];
        private static ulong[] tempcolor = new ulong[2];
        private static int tempnplies = 0;
        private static char[] tempheight = new char[WIDTH];

        public static void reset()
        {
            int i;
            nplies = 0;
            color[0] = color[1] = 0;
            for (i = 0; i < WIDTH; i++)
            {
                height[i] = (char)(H1 * i);
                trueheight[i] = 0;
            }
                
        }

        public static ulong positioncode()
        {
            //MessageBox.Show(color[nplies & 1].ToString());
            //MessageBox.Show(color[0].ToString());
            //MessageBox.Show(color[1].ToString());
            //MessageBox.Show(BOTTOM.ToString());
            //MessageBox.Show((color[nplies & 1] + color[0] + color[1] + BOTTOM).ToString());
            return color[nplies & 1] + color[0] + color[1] + BOTTOM;
            // color[0] + color[1] + BOTTOM forms bitmap of heights
            // so that positioncode() is a complete board encoding
        }

        // return whether newboard lacks overflowing column
        public static bool islegal(ulong newboard)
        {
            return (newboard & TOP) == 0;
        }

        // return whether columns col has room
        public static bool isplayable(int col)
        {
            return islegal(color[nplies & 1] | ((ulong)1 << height[col]));
        }

        // return non-zero iff newboard includes a win
        public static bool haswon(ulong newboard)
        {
            ulong diag1 = newboard & (newboard >> HEIGHT);
            ulong hori = newboard & (newboard >> H1);
            ulong diag2 = newboard & (newboard >> H2);
            ulong vert = newboard & (newboard >> 1);
            return ((diag1 & (diag1 >> 2 * HEIGHT)) |
                    (hori & (hori >> 2 * H1)) |
                    (diag2 & (diag2 >> 2 * H2)) |
                    (vert & (vert >> 2))) != 0;
        }
        // return whether newboard is legal and includes a win
        public static bool islegalhaswon(ulong newboard)
        {
            return islegal(newboard) && haswon(newboard);
        }
        public static void backmove()
        {
            int n;

            n = moves[--nplies];
            color[nplies & 1] ^= (ulong)1 << --height[n];
        }
        public static void makemove(int n)
        {
            color[nplies & 1] ^= (ulong)1 << height[n]++;
            moves[nplies++] = n;           
        }

        public static void tempmove(int n, int turn)
        {
            tempcolor[turn] ^= (ulong)1 << tempheight[n]++;
            tempnplies++;
        }

        public static ulong temppositioncode()
        {
            return (tempcolor[tempnplies & 1] + tempcolor[0] + tempcolor[1] + BOTTOM);
        }

        public static void resettemp()
        {
            tempnplies = 0;
            tempcolor[0] = tempcolor[1] = 0;
            for (int i = 0; i < WIDTH; i++)
                tempheight[i] = (char)(H1 * i);
        }

        public static void initDatabase()
        {
            resettemp();
            string file = Properties.Resources.connect_4;
            string[] rows = file.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < rows.Length-1; i++ )
            {
                strDatabase.Add(rows[i].Split(','));

                for (int j = 0; j < 42; j++)
                {
                    if (strDatabase[i][j] == "x")
                    {
                        tempmove((j) / 6, 0);
                    }
                    else if (strDatabase[i][j] == "o")
                    {
                        tempmove((j) / 6, 1);
                    }
                }

                int tempresult = 0;
                if (strDatabase[i][42] == "win")
                {
                    tempresult = TransGame.WIN;
                }
                else if (strDatabase[i][42] == "draw")
                {
                    tempresult = TransGame.DRAW;
                }
                else if (strDatabase[i][42] == "loss")
                {
                    tempresult = TransGame.LOSS;
                }
                if (!database.ContainsKey(temppositioncode()))
                {
                    database.Add(temppositioncode(), tempresult);
                }
                else
                {
                    MessageBox.Show("Not in database..forced win?");
                }

                resettemp();

                
            }

        }

    }
}

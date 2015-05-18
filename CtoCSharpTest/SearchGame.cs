using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtoCSharpTest
{
    class SearchGame
    {
        const int BOOKPLY = 0; // additional plies to be searched full-Game.WIDTH
        const int REPORTPLY = 2; // additional plies on which to report value
        public static int[,] history = new int[2, Game.SIZE1];
        public static ulong nodes;
        public static int bookply, reportply;

        public static int min(int x, int y) { return x < y ? x : y; }
        public static int max(int x, int y) { return x > y ? x : y; }

        public static void inithistory()
        {
            int side, i, h;
            for (side = 0; side < 2; side++)
                for (i = 0; i < (Game.WIDTH + 1) / 2; i++)
                    for (h = 0; h < Game.H1 / 2; h++)
                        history[side, Game.H1 * i + h] = history[side, Game.H1 * (Game.WIDTH - 1 - i) + Game.HEIGHT - 1 - h] =
                        history[side, Game.H1 * i + Game.HEIGHT - 1 - h] = history[side, Game.H1 * (Game.WIDTH - 1 - i) + h] =
                         4 + min(3, i) + max(-1, min(3, h) - max(0, 3 - i)) + min(3, min(i, h)) + min(3, h);
        }

        public static int ab(int alpha, int beta)
        {
            int besti, i, j, l, v, val;
            int score;
            uint ttscore;
            bool winontop;
            uint work;
            int nav;
            int[] av = new int[Game.WIDTH];
            ulong poscnt;
            ulong newbrd,other;
            int side,otherside;
            int hashindx;
            uint hashlock;

            if (Game.nplies == 8)
            {
                ulong htmp = Game.positioncode();
                ulong htemp = Game.positioncode();

                // try symmetry recognition by reversing columns
                ulong htemp2 = 0;
                for (htmp = htemp; htmp != 0; htmp >>= Game.H1)
                    htemp2 = htemp2 << Game.H1 | (htmp & Game.COL1);
                //if (htemp2 < htemp)
                htemp = htemp2;

                if (Game.database.ContainsKey(htemp))
                {
                    //MessageBox.Show("worked!");
                    return Game.database[htemp];
                }
                else if (Game.database.ContainsKey(Game.positioncode()))
                {
                    //MessageBox.Show("worked!");
                    return Game.database[Game.positioncode()];
                }
                else
                {
                    //MessageBox.Show("why not?");
                    //MessageBox.Show(Convert.ToString((long)Game.color[0], 2));
                    //MessageBox.Show(Convert.ToString((long)Game.color[1], 2));
                }
            }

            nodes++;
            if (Game.nplies == Game.SIZE-1) // one move left
            return TransGame.DRAW; // by assumption, player to move can't win
            otherside = (side = Game.nplies & 1) ^ 1;
            other = Game.color[otherside];
            for (i = nav = 0; i < Game.WIDTH; i++) {
            newbrd = other | ((ulong)1 << Game.height[i]); // check opponent move
            if (!Game.islegal(newbrd)) 
                continue;
            winontop = Game.islegalhaswon(other | ((ulong)2 << Game.height[i]));
            if (Game.haswon(newbrd)) { // immediate threat
                if (Convert.ToBoolean(winontop)) // can't stop double threat
                return TransGame.LOSS;
                nav = 0; // forced move
                av[nav++] = i;
                while (++i < Game.WIDTH)
                    if (Game.islegalhaswon(other | ((ulong)1 << Game.height[i])))
                    return TransGame.LOSS;
                break;
            }
            if (!winontop)
                av[nav++] = i;
            }
            if (nav == 0)
            return TransGame.LOSS;
            if (Game.nplies == Game.SIZE-2) // two moves left
            return TransGame.DRAW; // opponent has no win either
            if (nav == 1) {
            Game.makemove(av[0]);
            score = (TransGame.LOSSWIN-ab(TransGame.LOSSWIN-beta,TransGame.LOSSWIN-alpha));
            Game.backmove();
            return score;
            }
            ttscore = TransGame.transpose();
            if (ttscore != TransGame.UNKNOWN) {
            if (ttscore == TransGame.DRAWLOSS) {
                if ((beta = TransGame.DRAW) <= alpha)
                return (int)ttscore;
            } else if (ttscore == TransGame.DRAWWIN) {
                if ((alpha = TransGame.DRAW) >= beta)
                return (int)ttscore;
            } else return (int)ttscore; // exact score
            }
            hashindx = TransGame.htindex;
            hashlock = TransGame._lock;
            poscnt = TransGame.posed;
            besti=0;
            score = TransGame.LOSS;
            for (i = 0; i < nav; i++) {
            val = history[side, (int)Game.height[av[l = i]]];
            for (j = i+1; j < nav; j++) {
                v = history[side, (int)Game.height[av[j]]];
                if (v > val) {
                val = v; l = j;
                }
            }
            for (j = av[l]; l>i; l--)
                av[l] = av[l-1];
            Game.makemove(av[i] = j);
            val = (int)(TransGame.LOSSWIN-ab(TransGame.LOSSWIN-beta,TransGame.LOSSWIN-alpha));
            Game.backmove();
            if (val > score) {
                besti = i;
                if ((score=val) > alpha && Game.nplies >= bookply && (alpha=val) >= beta) {
                if (score == TransGame.DRAW && i < nav-1)
                    score = TransGame.DRAWWIN;
                if (besti > 0) {
                    for (i = 0; i < besti; i++)
                    history[side, (int)Game.height[av[i]]]--; // punish bad histories
                    history[side, (int)Game.height[av[besti]]] += besti;
                }
                break;
                }
            }
            }
            if (score == TransGame.LOSSWIN-ttscore) // combine < and >
            score = TransGame.DRAW;
            poscnt = TransGame.posed - poscnt;
            for (work=0; (poscnt>>=1) != 0; work++) ; // work=log #positions stored
            TransGame.transtore(hashindx, hashlock, (uint)score, work);
            return score;
        }

        public static int checkDatabase()
        {
            return 0;
        }

        public static int solve()
        {
            int i;
            int side = Game.nplies & 1;
            int otherside = side ^ 1;
            int score;

            nodes = 0;
            if (Game.haswon(Game.color[otherside]))
                return TransGame.LOSS;
            for (i = 0; i < Game.WIDTH; i++)
            if (Game.islegalhaswon(Game.color[side] | ((ulong)1 << Game.height[i])))
                return TransGame.WIN;
            inithistory();
            reportply = Game.nplies + REPORTPLY;
            bookply = Game.nplies + BOOKPLY;
            score = ab(TransGame.LOSS, TransGame.WIN);
            return score;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtoCSharpTest
{

    class hashentry
    {
        public uint biglock = 0;
        public uint bigwork = 0;
        public uint newlock = 0;
        public uint newscore = 0;
        public uint bigscore = 0;
    }

    class TransGame
    {
        public static int LOCKSIZE = 26;
        public static int TRANSIZE = 8306069;
        public static int SYMMREC = 10;
        public static int UNKNOWN = 0;
        public static int LOSS = 1;
        public static int DRAWLOSS = 2;
        public static int DRAW = 3;
        public static int DRAWWIN = 4;
        public static int WIN = 5;
        public static int LOSSWIN = 6;
        public static int htindex;
        public static uint _lock;
        public static hashentry[] ht = new hashentry[TRANSIZE];
        public static ulong posed;
        public static void trans_init()
        {
            for (int i = 0; i < TRANSIZE; i++)
            {
                ht[i] = new hashentry();
            }
        }

        public static void emptyTT()
        {
            for (int i = 0; i < TRANSIZE; i++)
            {
                ht[i].biglock = 0;
                ht[i].bigwork = 0;
                ht[i].newlock = 0;
                ht[i].newscore = 0;
                ht[i].bigscore = 0;
            }
        }

        public static void hash()
        {
                ulong htmp, htemp = Game.positioncode();
                if (Game.nplies < SYMMREC) { // try symmetry recognition by reversing columns
                    ulong htemp2 = 0;
                    for (htmp=htemp; htmp!=0; htmp>>=Game.H1)
                        htemp2 = htemp2<<Game.H1 | (htmp & Game.COL1);
                    if (htemp2 < htemp)
                        htemp = htemp2;
                }
                _lock = (uint)(Game.SIZE1>LOCKSIZE ? htemp >> (Game.SIZE1-LOCKSIZE) : htemp);
                htindex = (int)(htemp % (ulong)TRANSIZE);
        }

        public static uint transpose()
        {
              hash();
              if (ht[htindex].biglock == _lock)
                return ht[htindex].bigscore;
              if (ht[htindex].newlock == _lock)
                return ht[htindex].newscore;
              return (uint)UNKNOWN;
        }

        public static void transtore(int x, uint _lock, uint score, uint work)
        {
              posed++;
              if (ht[x].biglock == _lock || work >= ht[x].bigwork) {
                    ht[x].biglock = _lock;
                    ht[x].bigscore = score;
                    ht[x].bigwork = work;
              } 
              else {
                    ht[x].newlock = _lock;
                    ht[x].newscore = score;
              }
        }

    }


}

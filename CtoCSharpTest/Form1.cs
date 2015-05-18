using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtoCSharpTest
{
    public partial class Form1 : Form
    {
        List<Button> buttons = new List<Button>();
        public Form1()
        {
            InitializeComponent();
            Game.reset();
            Game.initDatabase();
            TransGame.trans_init();
            TransGame.emptyTT();
            foreach (Control c in this.Controls)
            {
                if (((Button)c).Text == "")
                {
                    buttons.Add((Button)c);

                }
            }
            buttons = buttons.OrderBy(o => o.Name).ToList();
            make_move(3);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Text == "A")
            {
                make_move(0);
            }
            else if (((Button)sender).Text == "B")
            {
                make_move(1);
            }
            else if (((Button)sender).Text == "C")
            {
                make_move(2);
            }
            else if (((Button)sender).Text == "D")
            {
                make_move(3);
            }
            else if (((Button)sender).Text == "E")
            {
                make_move(4);
            }
            else if (((Button)sender).Text == "F")
            {
                make_move(5);
            }
            else if (((Button)sender).Text == "G")
            {
                make_move(6);
            }
            check_winner();
            ai_move();
            check_winner();
        }

        private void ai_move()
        {
            int result;
            //MessageBox.Show("Determining AI move...");
            for (int i = 0; i < 7; i++)
            {
                if (Game.isplayable(i))
                {
                    Game.makemove(i);
                    result = SearchGame.solve();
                    Game.backmove();
                    if (result == 1)
                    {
                        //MessageBox.Show("Moving! " + i.ToString());
                        make_move(i);
                        return;
                    }
                }
            }
        }
        

        private void check_winner()
        {
            if (Game.islegalhaswon(Game.color[0]))
            {
                MessageBox.Show("Player 1 wins!");
            }
            else if (Game.islegalhaswon(Game.color[1]))
            {
                MessageBox.Show("Player 2 wins!");
            }
        }

        private void make_move(int column)
        {
            if (Game.isplayable(column))
            {
                if (!Convert.ToBoolean(Game.nplies & 1))
                {   
                    buttons[column * 6 + Game.trueheight[column]].Text = "X";
                }
                else
                {
                    buttons[column * 6 + Game.trueheight[column]].Text = "O";
                }
                Game.makemove(column);
                Game.trueheight[column]++;
            }
        }
    }
}

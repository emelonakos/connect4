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
        List<List<Button>> buttonGrid = new List<List<Button>>(7);
        List<Button> winning_moves = new List<Button>(4);
        bool gameOn = true;
        Dictionary<string, int> choice_buttons_to_column = new Dictionary<string, int>{{"A", 0}, {"B", 1}, {"C", 2}, {"D", 3}, {"E", 4}, {"F", 5}, {"G", 6}};
        Dictionary<int, Button> choice_column_to_buttons = new Dictionary<int, Button>();
        List<Button> move_history = new List<Button>();
        List<int> column_history = new List<int>();
        public Form1()
        {
            InitializeComponent();
            Game.reset();
            Game.initDatabase();
            TransGame.trans_init();
            TransGame.emptyTT();
            SearchGame.inithistory();
            foreach (Control c in this.Controls.OfType<Button>())
            {
                if (((Button)c).Text == "")
                {
                    buttons.Add((Button)c);
                    ((Button)c).Enabled = false;
                }
            }
            choice_column_to_buttons.Add(0, button_A);
            choice_column_to_buttons.Add(1, button_B);
            choice_column_to_buttons.Add(2, button_C);
            choice_column_to_buttons.Add(3, button_D);
            choice_column_to_buttons.Add(4, button_E);
            choice_column_to_buttons.Add(5, button_F);
            choice_column_to_buttons.Add(6, button_G);
            buttons = buttons.OrderBy(o => o.Name).ToList();
            for (int i = 0; i < 7; i++ )
            {
                buttonGrid.Add(new List<Button>());
                for (int j = 0; j < 6; j++)
                {
                    buttonGrid[i].Add(buttons[i*6+j]);
                }
            }
            make_move(3);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (gameOn)
            {
                int move = choice_buttons_to_column[((Button)sender).Text];
                make_move(move);
                check_winner();
                ai_move();
                check_winner();
            }
            else
            {
                MessageBox.Show("Game is over.");
            }

        }

        private void ai_move()
        {
            if (column_history.Count==2)
            {
                if (column_history[1] == 0)
                {
                    make_move(3);
                    return;
                }
                else if (column_history[1] == 1)
                {
                    make_move(5);
                    return;
                }
                else if (column_history[1] == 2)
                {
                    make_move(5);
                    return;
                }
                else if (column_history[1] == 3)
                {
                    make_move(3);
                    return;
                }
                else if (column_history[1] == 4)
                {
                    make_move(1);
                    return;
                }
                else if (column_history[1] == 5)
                {
                    make_move(1);
                    return;
                }
                else if (column_history[1] == 6)
                {
                    make_move(3);
                    return;
                }
            }
            
            int result;
            for (int i = 0; i < 7; i++)
            {
                if (Game.isplayable(i))
                {
                    Game.makemove(i);
                    result = SearchGame.solve();
                    Game.backmove();
                    if (result == 1 && ((Game.nplies & 1) == 0))
                    {
                        make_move(i);
                        return;
                    }
                }
            }
        }

        private void find_winning_moves()
        {
            //check for horizontal wins
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (buttonGrid[i][j].Text == buttonGrid[i + 1][j].Text && buttonGrid[i + 1][j].Text == buttonGrid[i + 2][j].Text && buttonGrid[i + 2][j].Text == buttonGrid[i + 3][j].Text && buttonGrid[i][j].Enabled)
                    {
                        buttonGrid[i][j].BackColor = Color.LightGreen;
                        buttonGrid[i + 1][j].BackColor = Color.LightGreen;
                        buttonGrid[i + 2][j].BackColor = Color.LightGreen;
                        buttonGrid[i + 3][j].BackColor = Color.LightGreen;
                        winning_moves.Add(buttonGrid[i][j]);
                        winning_moves.Add(buttonGrid[i+1][j]);
                        winning_moves.Add(buttonGrid[i+2][j]);
                        winning_moves.Add(buttonGrid[i+3][j]);
                        return;
                    }
                }
            }

            //check for vertical wins
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttonGrid[i][j].Text == buttonGrid[i][j + 1].Text && buttonGrid[i][j + 1].Text == buttonGrid[i][j + 2].Text && buttonGrid[i][j + 2].Text == buttonGrid[i][j + 3].Text && buttonGrid[i][j].Enabled)
                    {
                        buttonGrid[i][j].BackColor = Color.LightGreen;
                        buttonGrid[i][j + 1].BackColor = Color.LightGreen;
                        buttonGrid[i][j + 2].BackColor = Color.LightGreen;
                        buttonGrid[i][j + 3].BackColor = Color.LightGreen;
                        winning_moves.Add(buttonGrid[i][j]);
                        winning_moves.Add(buttonGrid[i][j+1]);
                        winning_moves.Add(buttonGrid[i][j+2]);
                        winning_moves.Add(buttonGrid[i][j+3]);
                        return;
                    }
                }
            }

            //check forward diagonal
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttonGrid[i][j].Text == buttonGrid[i + 1][j + 1].Text && buttonGrid[i + 1][j + 1].Text == buttonGrid[i + 2][j + 2].Text && buttonGrid[i + 2][j + 2].Text == buttonGrid[i + 3][j + 3].Text && buttonGrid[i][j].Enabled)
                    {
                        buttonGrid[i][j].BackColor = Color.LightGreen;
                        buttonGrid[i + 1][j + 1].BackColor = Color.LightGreen;
                        buttonGrid[i + 2][j + 2].BackColor = Color.LightGreen;
                        buttonGrid[i + 3][j + 3].BackColor = Color.LightGreen;
                        winning_moves.Add(buttonGrid[i][j]);
                        winning_moves.Add(buttonGrid[i+1][j+1]);
                        winning_moves.Add(buttonGrid[i+2][j+2]);
                        winning_moves.Add(buttonGrid[i+3][j+3]);
                        return;
                    }
                }
            }

            //check backward diagonal
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttonGrid[i + 3][j].Text == buttonGrid[i + 2][j + 1].Text && buttonGrid[i + 2][j + 1].Text == buttonGrid[i + 1][j + 2].Text && buttonGrid[i + 1][j + 2].Text == buttonGrid[i][j + 3].Text && buttonGrid[i + 3][j].Enabled)
                    {
                        buttonGrid[i + 3][j].BackColor = Color.LightGreen;
                        buttonGrid[i + 2][j + 1].BackColor = Color.LightGreen;
                        buttonGrid[i + 1][j + 2].BackColor = Color.LightGreen;
                        buttonGrid[i][j + 3].BackColor = Color.LightGreen;
                        winning_moves.Add(buttonGrid[i+3][j]);
                        winning_moves.Add(buttonGrid[i+2][j+1]);
                        winning_moves.Add(buttonGrid[i+1][j+2]);
                        winning_moves.Add(buttonGrid[i][j+3]);
                        return;
                    }
                }
            }
        }

        private void check_winner()
        {
            if (Game.islegalhaswon(Game.color[0]))
            {
                find_winning_moves();
                MessageBox.Show("Player 1 wins!");
                end_game();
            }
            else if (Game.islegalhaswon(Game.color[1]))
            {
                find_winning_moves();
                MessageBox.Show("Player 2 wins!");
                end_game();
            }
            else if (Game.trueheight[0] == 6 && Game.trueheight[0] == Game.trueheight[1] && Game.trueheight[1] == Game.trueheight[2] && Game.trueheight[2] == Game.trueheight[3] && Game.trueheight[3] == Game.trueheight[4] && Game.trueheight[4] == Game.trueheight[5] && Game.trueheight[5] == Game.trueheight[6])
            {
                MessageBox.Show("Draw!");
                end_game();
            }
        }

        private void end_game()
        {
            gameOn = false;
            foreach (Button b in Controls.OfType<Button>())
            {
                if (choice_buttons_to_column.ContainsKey(b.Text))
                {
                    b.Enabled = false;
                }
            }
        }

        private void undo_end_game()
        {
            gameOn = true;
            foreach (Button b in Controls.OfType<Button>())
            {
                if (choice_buttons_to_column.ContainsKey(b.Text) && Game.trueheight[choice_buttons_to_column[b.Text]] < 6)
                {
                    b.Enabled = true;
                }
            }
            if (winning_moves.Count > 0)
            {
                foreach (Button b in winning_moves)
                {
                    b.BackColor = SystemColors.Control;
                }
            }
            winning_moves.Clear();
        }

        private void make_move(int column)
        {
            Button b = buttons[column * 6 + Game.trueheight[column]];
            if (Game.isplayable(column))
            {
                if (!Convert.ToBoolean(Game.nplies & 1))
                {   
                    b.Text = "X";
                }
                else
                {
                    b.Text = "O";
                }
                Game.makemove(column);
                Game.trueheight[column]++;
                if (move_history.Count > 0)
                {
                    move_history[move_history.Count - 1].BackColor = SystemColors.Control;
                    this.Invalidate();
                    this.Update();
                }
                
                b.BackColor = Color.LightGreen;
                b.Enabled = true;
                move_history.Add(b);
                column_history.Add(column);
                if (Game.trueheight[column_history[column_history.Count-1]] == 6)
                {
                    choice_column_to_buttons[column_history[column_history.Count - 1]].Enabled = false;
                }
            }
        }

        private void undo_move()
        {
            if (Game.nplies > 1)
            {
                if (!gameOn)
                {
                    undo_end_game();
                }
                Game.backmove();
                Button last_move = move_history[move_history.Count - 1];
                last_move.Text = "";
                last_move.BackColor = SystemColors.Control;
                last_move.Enabled = false;
                move_history.RemoveAt(move_history.Count - 1);
                if (move_history.Count > 0)
                {
                    move_history[move_history.Count - 1].BackColor = Color.LightGreen;
                }
                Game.trueheight[column_history[column_history.Count - 1]]--;
                if (!choice_column_to_buttons[column_history[column_history.Count-1]].Enabled)
                {
                    choice_column_to_buttons[column_history[column_history.Count - 1]].Enabled = true;
                }
                column_history.RemoveAt(column_history.Count - 1);
            }
            
        }

        private void reset()
        {
            Game.reset();
            Game.resettemp();
            foreach (Button b in buttons)
            {
                b.Text = "";
                b.Enabled = false;
                b.BackColor = SystemColors.Control;
            }
            for (int i = 0; i < 7; i++)
            {
                choice_column_to_buttons[i].Enabled = true;
            }
            gameOn = true;
            make_move(3);
        }

        private void aboutThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Connect 4 against a powerful AI.");
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void button_Undo_Click(object sender, EventArgs e)
        {
            undo_move();
            undo_move();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

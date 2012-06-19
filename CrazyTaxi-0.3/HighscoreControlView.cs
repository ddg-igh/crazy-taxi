using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using AccessDBConector;

namespace CrazyTaxi
{
    public partial class HighscoreControlView : UserControl
    {
        public HighscoreControlView()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize() 
        { 
           /* int rows = tableLayoutPanel1.RowCount;
            int columns = tableLayoutPanel1.ColumnCount;
            HighScore con = new HighScore(@"Resources\Nordwind");
            con.Open();
            List<string[]> highscores = con.GetHighScores();
            con.Close();
            for (int count = 0; count < rows; count++) 
            {

                for (int i = 0; i < columns; i++)
                {
                    Panel textPanel = new Panel();
                    
                    Label label = new Label();
                    if (i == 0)
                    {
                        label.Text = highscores[count][i];
                    }
                    else if (i == 1)
                    {
                        label.Text = highscores[count][i];
                    }
                    else 
                    {
                        label.Text = highscores[count][i];
                    }

                    int labelHeight = (int)label.Font.GetHeight();
                    FontFamily ff = new FontFamily("Haettenschweiler");
                    label.Font = new Font(ff, 16, FontStyle.Regular);
                    textPanel.Controls.Add(label);
                    textPanel.Size = label.Size;
                    label.ForeColor = Color.White;
                    tableLayoutPanel1.Controls.Add(textPanel, i, count);
                } 
            }*/
        }
    }
}

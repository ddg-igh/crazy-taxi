namespace CrazyTaxi
{
    partial class CT_UI
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.scrollImageWorker = new System.ComponentModel.BackgroundWorker();
            this.menuFader = new System.Windows.Forms.Timer(this.components);
            this.Backgroundfader = new System.Windows.Forms.Timer(this.components);
            this.CarFader = new System.Windows.Forms.Timer(this.components);
            this.MoveMotionOne = new System.Windows.Forms.Timer(this.components);
            this.MoveMotionTwo = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // menuFader
            // 
            this.menuFader.Interval = 1;
            this.menuFader.Tick += new System.EventHandler(this.menuFader_Tick);
            // 
            // Backgroundfader
            // 
            this.Backgroundfader.Interval = 30;
            this.Backgroundfader.Tick += new System.EventHandler(this.Backgroundfader_Tick);
            // 
            // MoveMotionOne
            // 
            this.MoveMotionOne.Interval = 10;
            this.MoveMotionOne.Tick += new System.EventHandler(this.MoveMotionOne_Tick);
            // 
            // MoveMotionTwo
            // 
            this.MoveMotionTwo.Interval = 10;
            this.MoveMotionTwo.Tick += new System.EventHandler(this.MoveMotionTwo_Tick);
            // 
            // CT_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(694, 435);
            this.Cursor = System.Windows.Forms.Cursors.PanNorth;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CT_UI";
            this.Text = "CrazyTaxi";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CT_UI_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CT_UI_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CT_UI_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker scrollImageWorker;
        private System.Windows.Forms.Timer menuFader;
        private System.Windows.Forms.Timer Backgroundfader;
        private System.Windows.Forms.Timer CarFader;
        private System.Windows.Forms.Timer MoveMotionOne;
        private System.Windows.Forms.Timer MoveMotionTwo;



    }
}


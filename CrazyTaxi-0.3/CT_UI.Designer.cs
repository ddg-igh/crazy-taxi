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
            this.Backgroundfader = new System.Windows.Forms.Timer(this.components);
            this.Updater = new System.Windows.Forms.Timer(this.components);
            this.gameBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.gameBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Backgroundfader
            // 
            this.Backgroundfader.Interval = 30;
            this.Backgroundfader.Tick += new System.EventHandler(this.Backgroundfader_Tick);
            // 
            // Updater
            // 
            this.Updater.Enabled = true;
            this.Updater.Interval = 30;
            this.Updater.Tick += new System.EventHandler(this.Updater_Tick);
            // 
            // gameBox
            // 
            this.gameBox.Location = new System.Drawing.Point(0, 0);
            this.gameBox.Name = "gameBox";
            this.gameBox.Size = new System.Drawing.Size(596, 380);
            this.gameBox.TabIndex = 0;
            this.gameBox.TabStop = false;
            this.gameBox.Paint += new System.Windows.Forms.PaintEventHandler(this.CT_UI_Paint);
            // 
            // CT_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(678, 435);
            this.Controls.Add(this.gameBox);
            this.Cursor = System.Windows.Forms.Cursors.PanNorth;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CT_UI";
            this.Text = "CrazyTaxi";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CT_UI_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CT_UI_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.gameBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer Backgroundfader;
        private System.Windows.Forms.Timer Updater;
        private System.Windows.Forms.PictureBox gameBox;



    }
}


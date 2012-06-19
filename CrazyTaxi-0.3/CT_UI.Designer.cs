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
            this.SuspendLayout();
            // 
            // Backgroundfader
            // 
            this.Backgroundfader.Interval = 30;
            this.Backgroundfader.Tick += new System.EventHandler(this.Backgroundfader_Tick);
            // 
            // CT_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(678, 435);
            this.Cursor = System.Windows.Forms.Cursors.PanNorth;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CT_UI";
            this.Text = "CrazyTaxi";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CT_UI_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CT_UI_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CT_UI_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer Backgroundfader;



    }
}


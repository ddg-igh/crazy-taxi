using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CTMapUtils;

namespace CTTestForm
{
    public partial class Form1 : Form
    {
        int deg = 0;
        System.Drawing.Point pt = new Point(100, 120);
        System.Threading.Timer t;
        public Form1()
        {
            InitializeComponent();

            /*
             * Den Ordner festlegen, in dem die Bilddateien liegen
             * System.IO.Directory.GetCurrentDirectory() gibt das Verzeichnis zurück, in dem die .exe liegt.
             * */
            MapParser.ImagePath = System.IO.Directory.GetCurrentDirectory() + "\\Ressources";
            //MapParser.Load läd eine Map aus der angegebenen XML-Datei (hier: "Testmap.xml" im Anwendungsverzeichnis
            var map = CTMapUtils.MapParser.Load(10, 10);
            //var map = MapParser.Load(System.IO.Directory.GetCurrentDirectory() + "\\Testmap.xml");
            //Die Map ist ein UserControl, verhält sich also wie jedes Steuerelement auf einer Form (z.B. TextBox, Label, ...)
            //DockStyle.Fill sorgt dafür, dass sie die komplette Form ausfüllt
            map.Dock = DockStyle.Fill;
            //Die Initialize-Methode sorgt dafür, dass die Map ihre einzelnen Bilder erstellt und richtig anordnet
            //Da sie die Größe der Form kennt, kann sie alle Bilder richtig skalieren
            map.Initialize(this.Size);
            //Hier wird die Map noch als UserControl zum Form hinzugefügt
            Controls.Add(map);

            //Zum Testen der Kollisionserkennung
            var entity = map.Collision.AddEntity(new Size(50, 80));
            t = new System.Threading.Timer(tCallback, entity, 1000, 100);
            //MessageBox.Show("Front: " + res.Front + ", Back: " + res.Back + ", Left: " + res.Left + ", Right: " + res.Right);
        }

        private delegate void td(CollisionEntity et);
        private void test(CollisionEntity entity)
        {
            entity.Update(pt, deg, true);
        }

        private void tCallback(object arg)
        {
            var entity = (CollisionEntity)arg;
            deg += 10;
            this.Invoke(new td(test), entity);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Haukcode.Samples;

namespace Hexagones
{
    public partial class FormHexagones : Form
    {
        private const int tailleUnivers = 7;
        private Univers univers = null;
        private Bitmap bmp;
        private System.Collections.Generic.IEnumerable<(IPAddress Address, IPAddress NetMask)> addresses;

        public FormHexagones()
        {
            InitializeComponent();
            this.Show();
        }

        private new void Show()
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            if (univers == null)
                univers = new Univers(tailleUnivers, pictureBox1.Width / 2, pictureBox1.Height / 2);

            univers.drawUniverse(bmp);
            pictureBox1.Image = bmp;
            ArtNetDisplay();
        }

        private void ArtNetDisplay()
        {
            addresses = Haukcode.ArtNet.Helper.GetAddressesFromInterfaceType();
            System.Console.WriteLine(addresses.ToString());
            univers.generateDMXDatas();
            //if (addresses != null) return;

            var addr = addresses.First();
            using (var tester = new SampleCapture(localIp: addr.Address, localSubnetMask: addr.NetMask))
            {
                Console.WriteLine("Start send...");

                // Select show
                tester.sendDatas(univers.generateDMXDatas());

                Console.WriteLine("Sended");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //Fallait pas double cliquer
        }

        private void pictureBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            univers.OnMouseEnter(e.X, e.Y, this);
            this.ChangeColorHexagone(0, 0, 255);
        }

        private void pictureBox1_MouseLeave(object sender, MouseEventArgs e)
        {
            univers.OnMouseEnter(e.X, e.Y, this);
            this.ChangeColorHexagone(0, 0, 0);
        }

        public void OnHexaHit(Hexagone hexa)
        {
            label_q.Text = hexa.Q.ToString();
            label_r.Text = hexa.R.ToString();
            label_id.Text = hexa.ID.ToString();
            trackBar_rouge.Value = hexa.Rouge;
            trackBar_vert.Value = hexa.Vert;
            trackBar_bleu.Value = hexa.Bleu;
            numericUpDown_rouge.Value = hexa.Rouge;
            numericUpDown_vert.Value = hexa.Vert;
            numericUpDown_bleu.Value = hexa.Bleu;
        }

        private void numericUpDown_rouge_ValueChanged(object sender, EventArgs e)
        {
            trackBar_rouge.Value = Convert.ToInt32(numericUpDown_rouge.Value);
            ChangeColorHexagone();
        }

        private void numericUpDown_vert_ValueChanged(object sender, EventArgs e)
        {
            trackBar_vert.Value = Convert.ToInt32(numericUpDown_vert.Value);
            ChangeColorHexagone();
        }

        private void numericUpDown_bleu_ValueChanged(object sender, EventArgs e)
        {
            trackBar_bleu.Value = Convert.ToInt32(numericUpDown_bleu.Value);
            ChangeColorHexagone();
        }

        private void trackBar_rouge_Scroll(object sender, EventArgs e)
        {
            numericUpDown_rouge.Value = trackBar_rouge.Value;
            ChangeColorHexagone();
        }

        private void trackBar_vert_Scroll(object sender, EventArgs e)
        {
            numericUpDown_vert.Value = trackBar_vert.Value;
            ChangeColorHexagone();
        }

        private void trackBar_bleu_Scroll(object sender, EventArgs e)
        {
            numericUpDown_bleu.Value = trackBar_bleu.Value;
            ChangeColorHexagone();
        }

        public void ChangeColorHexagone(int rouge, int vert, blue)
        {
            int id = Convert.ToInt32(label_id.Text);
            int rouge = rouge;
            int vert = vert;
            int bleu = blue;
            univers.ChangeColorHexagone(id, rouge, vert, bleu);
            //On peut faire mieux mais il faut un deuxième thread
            //Il faut éviter d'actualiser en permanence, mais plutôt actualiser toutes les x millisecondes
            this.Show();
        }

        private void button_enregistrer_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Votre message a été enregistré dans le fichier : " + univers.EnregistrerUnivers(),
                "Enregistrement effectué",
                MessageBoxButtons.OK);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        /*Merci Microsoft
         * https://docs.microsoft.com/fr-fr/dotnet/api/system.windows.forms.openfiledialog?view=netframework-4.8
         */
        private void button_ouvrir_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "./";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                        univers.OuvrirUnivers(fileContent, tailleUnivers);
                        this.Show();
                    }
                }
            }
        }
    }
}

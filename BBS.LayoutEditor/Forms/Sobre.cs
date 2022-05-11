using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace BBS.LayoutEditor
{
    //EN-US
    //Made under OpenKH project's resources of structures and informations
    //From Project Kingdom Hearts Birth by Sleep PSP BR Translation by:
    //SoraLeon, Gledson999 & MiguelQueiroz010(Bit.Raiden) - Please sponsor our Project!
    //Free to Use and do not comercialize!

    //PT-BR
    //Feito sob os recursos, informações e estruturas do projeto OpenKH
    //Do Projeto de Tradução Kingdom Hearts Birth by Sleep PSP BR de:
    //SoraLeon, Gledson999 & MiguelQueiroz010(Bit.Raiden) - Apoie o nosso projeto!
    //Livre para uso e não comercializar!
    partial class Sobre : Form
    {
        string raidengithub = "https://github.com/MiguelQueiroz010";
        public Sobre()
        {
            InitializeComponent();
            if (File.Exists("Guia.txt"))
                linkLabel3.Visible = true;
            else
                linkLabel3.Visible = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(raidengithub);
        }

        private void Sobre_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void atalhoslink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var at = new Atalhos();
            at.ShowDialog();
        }

        private void doclink(object sender, LinkLabelLinkClickedEventArgs e)
        {
           Process.Start("Guia.txt");
        }
    }
}

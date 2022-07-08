using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
//using System.Text;
using System.Windows.Forms;

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

namespace BBS.LayoutEditor
{
    public partial class Form1 : Form
    {
        public Size NotMDISize; //Used on Viewport Emu Screen FORM (Screen.cs)
        public Form1()
        {
            InitializeComponent();
            NotMDISize = this.Size;
        }

        public bool DrawAll = false, DrawOne = true; //Control for Draw sequence or group only
        public L2DPage SelectedL2D() => tabControl1.TabCount > 0 ? tabControl1.SelectedTab as L2DPage : null;

        #region Funções

        #region Save/Open/Close SOC
        public void Abrir(bool isDragDroped = false, string[] dragpaths = null)
        {
            if (isDragDroped)
            {
                foreach (var file in dragpaths)
                {
                    if (Path.GetExtension(file) == ".l2d")
                        tabControl1.TabPages.Add(new L2DPage(this, new L2D(File.ReadAllBytes(file)), file));
                }
                UpdateOptions();
            }
            else
            {
                var op = new OpenFileDialog();
                op.Filter = "Layout2Dimensional(*.l2d)|*.l2d";
                op.Title = "Open válid(s) file(s) L2D Birth by Sleep.";
                op.Multiselect = true;
                if (op.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in op.FileNames)
                    {
                        tabControl1.TabPages.Add(new L2DPage(this, new L2D(File.ReadAllBytes(file)), file));
                    }
                    UpdateOptions();
                }
                else 
                {
                    var fail = new OpenFileDialog();
                    if(fail.ShowDialog() == DialogResult.Abort) 
                    {
                        MessageBox.Show("This isn´t a correct file aborting");
                    }
                }
            }
        }
        public void UpdateOptions()
        {
            if (tabControl1.TabCount >= 1)
            {
                if (!fecharToolStripMenuItem.Enabled)
                {
                    fecharToolStripMenuItem.Enabled = true;
                    fecharTodosToolStripMenuItem.Enabled = true;
                    salvarTodosToolStripMenuItem.Enabled = true;
                    salvarToolStripMenuItem.Enabled = true;
                    saveAsToolStripMenuItem.Enabled = true;
                    tabControl1.Visible = true;
                }
            }
            else if (tabControl1.TabCount == 0)
            {
                fecharToolStripMenuItem.Enabled = false;
                fecharTodosToolStripMenuItem.Enabled = false;
                salvarTodosToolStripMenuItem.Enabled = false;
                salvarToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                selecionadoToolStripMenuItem.Visible = false;
                sP2ToolStripMenuItem.Visible = false;
                lY2ToolStripMenuItem.Visible = false;
                tabControl1.Visible = false;
            }
        }
        public void Fechar()
        {
            if (SelectedL2D().ViewPort != null &&
                SelectedL2D().ViewPort.Visible)
                SelectedL2D().ViewPort.Close();
            if (SelectedL2D().imageWindow != null &&
                SelectedL2D().imageWindow.Visible)
                SelectedL2D().imageWindow.Close();
            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
            UpdateOptions();
        }
        public void FecharTodos()
        {
            if (SelectedL2D().ViewPort != null &&
                SelectedL2D().ViewPort.Visible)
                SelectedL2D().ViewPort.Close();
            if (SelectedL2D().imageWindow != null &&
                SelectedL2D().imageWindow.Visible)
                SelectedL2D().imageWindow.Close();
            foreach (L2DPage tab in tabControl1.TabPages)
                tabControl1.TabPages.Remove(tab);
            UpdateOptions();
        }


        public void Salvar(string alternatepath = "N")
        {
            byte[] fileL2D = SelectedL2D().L2DFile.GetRebuild();//Rebuild Entire L2D

            if (alternatepath != "N")
                File.WriteAllBytes(alternatepath, fileL2D);
            else
                File.WriteAllBytes(SelectedL2D().FilePath, fileL2D);

            //Conclusion Message
            MessageBox.Show("Sucessfully saved!\n\n" +
                $"Check the following path for the file: \n{(alternatepath != "N" ? alternatepath : SelectedL2D().FilePath)}!",
                "Layout Editor",MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void SalvarComo()
        {
            var save = new SaveFileDialog();
            save.Filter = "Layout2Dimensional(*.l2d)|*.l2d";
            save.FileName = SelectedL2D().L2DName;
            if (save.ShowDialog() == DialogResult.OK)
            {
                Salvar(save.FileName);
            }
        }
        public void SalvarTodos()
        {
            foreach (L2DPage l2dpage in tabControl1.TabPages)
            {
                byte[] fileL2D = l2dpage.L2DFile.GetRebuild();
                File.WriteAllBytes(l2dpage.FilePath, fileL2D);
            }
            //Conclusion Message
            MessageBox.Show("All files sucessfully saved!\n\n" +
                $"Check the files input path!",
                "Layout Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Export/Import
        public void ExportarTIM2()
        {
            var save = new SaveFileDialog();
            save.Filter = "PortableNetworkGraphics(*.png)|*.png|TIM2 Ps2 Texture(*.tm2)|*.tm2";
            save.FileName = Path.GetFileNameWithoutExtension(SelectedL2D().L2DName);
            if (save.ShowDialog() == DialogResult.OK)
            {
                if (save.FilterIndex == 2)
                {
                    File.WriteAllBytes(save.FileName, SelectedL2D().GetSelectedSQ2P().Texture.TIM2Data);
                }
                else
                    SelectedL2D().GetSelectedSQ2P().Texture._picture[0].GetPNG().Save(save.FileName);
            }
        }
        public void ImportarTIM2()
        {
            var open = new OpenFileDialog();
            open.Filter = "TIM2 Ps2 Texture(*.tm2)|*.tm2";
            if (open.ShowDialog() == DialogResult.OK)
            {
                SelectedL2D().L2DFile.SQ2Ps[SelectedL2D().GetSelectedSQ2P().Index].SetTIM2(File.ReadAllBytes(open.FileName),
                    SelectedL2D().L2DFile.BigEndian);

                RefreshDrawings();

                SelectedL2D().RenderImageWindow();
            }
        }
        public void ExportarSprite()
        {
            var save = new SaveFileDialog();
            save.Filter = "PortableNetworkGraphics(*.png)|*.png";
            save.FileName = $"part_{SelectedL2D().GetSelectedGroup().IndexParts}";
            if (save.ShowDialog() == DialogResult.OK)
            {
                var l2dsele = tabControl1.SelectedTab as L2DPage;
                l2dsele.GetUVPart(l2dsele.GetSelectedSQ2P().Texture._picture[0].GetPNG(),
                    l2dsele.GetSelectedPart(), true).Save(save.FileName);
            }
        }

        public void ImportarSprite()
        {

        }

        public void ImportViewImage()
        {
            var open = new OpenFileDialog();
            open.Filter = "PortableNetworkGraphics(*.png)|*.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                var im = Image.FromFile(open.FileName);
                SelectedL2D().ViewImported = new Bitmap(im, im.Width / 2, im.Height / 2);
                if (SelectedL2D().imageWindow != null && SelectedL2D().imageWindow.Visible)
                {
                    SelectedL2D().imageWindow.Close();
                    SelectedL2D().RenderImageWindow();
                }
                RefreshDrawings();
            }

        }
        #endregion

        #region Drawing Control
        public void DrawGroup()
        {
            var l2dsele = tabControl1.SelectedTab as L2DPage;

            if (l2dsele.ViewPort == null || l2dsele.ViewPort.IsDisposed)
                l2dsele.ViewPort = new Screen(l2dsele);
            if (!l2dsele.ViewPort.Visible)
                l2dsele.ViewPort.Show();


            l2dsele.ViewPort.panel1.Width = 480;
            l2dsele.ViewPort.panel1.Height = 272;

            int WidthViewport = l2dsele.L2DFile.BigEndian ? 1920 : 480;
            int HeightViewport = l2dsele.L2DFile.BigEndian ? 1080 : 272;

            l2dsele.ViewPort.Base = new Size(WidthViewport, HeightViewport);
            int Zoomout = l2dsele.L2DFile.BigEndian ? 2 : 1;

            var layout = l2dsele.GetSelectedLayout();
            var node = l2dsele.GetSelectedNode();
            var sprite = l2dsele.GetSelectedSprite();
            var spritegroup = l2dsele.GetSelectedGroup();

            int x = node.X.SRUtoSRD(WidthViewport) + spritegroup.X0;
            int y = node.Y.SRUtoSRD(0, HeightViewport) + spritegroup.Y0;

            if(WidthViewport==1920)
            {
                x -= 240;
                y -= 135;
                x /= 4;
                y /= 4;
            }

            Bitmap partDraw = l2dsele.GetUVPart(l2dsele.L2DFile.BigEndian ? l2dsele.ViewImported : l2dsele.GetSelectedSQ2P().Texture._picture[0].GetPNG(),
                                    spritegroup.GetPart(l2dsele.L2DFile.SQ2Ps[node.SQ2BaseNumber].SP2Entry.Parts),false, Zoomout);

            int RectWh = Math.Abs(spritegroup.X0 - spritegroup.X1) / Zoomout;
            int RectH = Math.Abs(spritegroup.Y0 - spritegroup.Y1) / Zoomout;

            partDraw = new Bitmap(partDraw, RectWh, RectH);

            l2dsele.ViewPort.RenderAll(new List<Texture>() {
                    new Texture()
                    {
                        Sprite = sprite,
                        Layout = layout,
                        LayoutNode = node,
                        Group = spritegroup,
                        image = partDraw,
                        Originalimage = partDraw,
                        RectLocation = new Point(x,y),
                        PrevRectLocation = new Point(x,y),
                        PrevBoundSize = new Size(RectWh, RectH),
                        BoundSize = new Size(RectWh, RectH)
                    }
                    });

            l2dsele.ViewPort.Selected = l2dsele.ViewPort.Textures[0];

            DrawOne = true;
            DrawAll = false;
        }
        public void DrawAllGroups(int index = 0)
        {
            var l2dsele = tabControl1.SelectedTab as L2DPage;

            if (l2dsele.ViewPort == null || l2dsele.ViewPort.IsDisposed)
                l2dsele.ViewPort = new Screen(l2dsele);
            if (!l2dsele.ViewPort.Visible)
                l2dsele.ViewPort.Show();

            l2dsele.ViewPort.panel1.Width = 480;
            l2dsele.ViewPort.panel1.Height = 272;

            int WidthViewport = l2dsele.L2DFile.BigEndian ? 1920 : 480;
            int HeightViewport = l2dsele.L2DFile.BigEndian ? 1080 : 272;
            l2dsele.ViewPort.Base = new Size(WidthViewport, HeightViewport);
            int Zoomout = l2dsele.L2DFile.BigEndian ? 2 : 1;

            //Get LayoutNode, Sequence and Sprites correlated
            var layout = l2dsele.GetSelectedLayout();
            var node = l2dsele.GetSelectedNode();
            var sequence = node.LinkedSequence(l2dsele.L2DFile.SQ2Ps);
            var controls = sequence.Controls;
            var sprites = new List<SP2.Sprite>();
            foreach (var control in controls)
                foreach (var anim in control.Animations)
                    if (anim.SpriteNumber > 0)
                        sprites.Add(anim.LinkedSprite(l2dsele.L2DFile.SQ2Ps[node.SQ2BaseNumber].SP2Entry.Sprites));
            sprites = sprites.Distinct().ToList();

            //Add the textures struct
            var texts = new List<Texture>();
            foreach (var sprite in sprites)
                foreach (var group in sprite.Groups)
                {
                    //Convert coordinate system and get X and Y as position to RENDER
                    int x = node.X.SRUtoSRD(WidthViewport) + group.X0;
                    int y = node.Y.SRUtoSRD(0, HeightViewport) + group.Y0;

                    if (WidthViewport == 1920)
                    {
                        x -= 240;
                        y -= 135;
                        x /= 4;
                        y /= 4;

                    }

                    //Get the scissored texture UV
                    Bitmap partDraw = l2dsele.GetUVPart(l2dsele.L2DFile.BigEndian ? l2dsele.ViewImported:l2dsele.GetSelectedSQ2P().Texture._picture[0].GetPNG(),
                                            group.GetPart(l2dsele.L2DFile.SQ2Ps[node.SQ2BaseNumber].SP2Entry.Parts), false, Zoomout);

                    //Get rectangle width and height
                    int RectWh = Math.Abs(group.X0 - group.X1) / Zoomout;
                    int RectH = Math.Abs(group.Y0 - group.Y1) / Zoomout;

                    partDraw = new Bitmap(partDraw, RectWh, RectH);

                    //Add textures struct
                    var textVAR = new Texture()
                    {
                        Sprite = sprite,
                        Layout = layout,
                        LayoutNode = node,
                        Group = group,
                        image = partDraw,
                        Originalimage = partDraw,
                        RectLocation = new Point(x, y),
                        PrevRectLocation = new Point(x, y),
                        PrevBoundSize = new Size(RectWh, RectH),
                        BoundSize = new Size(RectWh, RectH),
                    };


                    //Add to render Array
                    if (!texts.Contains(textVAR))
                        texts.Add(textVAR);
                }

            //Render on screen
            l2dsele.ViewPort.RenderAll(texts);

            l2dsele.ViewPort.Selected = l2dsele.ViewPort.Textures[index];

            DrawOne = false;
            DrawAll = true;
        }
        public void RefreshDrawings()
        {
            var selected = SelectedL2D();
            if (selected != null)
                if (selected.treeView1 != null)
                    if (selected.treeView1.SelectedNode != null &&
                        selected.treeView1.SelectedNode.Level == 4)
                    {
                        Texture pasttexture = null;
                        if (selected.ViewPort != null &&
                            selected.ViewPort.Visible
                            && selected.ViewPort.Selected != null)
                        {
                            pasttexture = selected.ViewPort.Selected;
                            selected.ViewPort.DeselectAll();
                        }

                        selected.RenderPart();

                        if (DrawOne)
                            DrawGroup();
                        else if (DrawAll)
                            DrawAllGroups(pasttexture.Index);

                        selected.ViewPort.Refresh();
                    }
        }

        #endregion
        #endregion

        #region Botões e Eventos
        private void abrirL2DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Abrir();
        }
        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sobre about = new Sobre();
            about.ShowDialog();
        }
        private void sairToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (L2DPage page in tabControl1.TabPages)
            {
                if (page.imageWindow != null)
                    if (page.imageWindow.Visible)
                        page.imageWindow.Close();
                if (page.ViewPort != null)
                    if (page.ViewPort.Visible)
                        page.ViewPort.Close();
            }

        }
        private void fecharToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Fechar();
        }
        private void fecharTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FecharTodos();
        }
        private void exportarRecorteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportarSprite();
        }
        private void exportarTexturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportarTIM2();
        }
        private void drawAllPSPEMUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawAllGroups();
        }
        private void verSequencePartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawGroup();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        #region Options Checks
        private void showPartDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showPartDrawToolStripMenuItem.Checked = true;
            showSequenceDrawToolStripMenuItem.Checked = false;
            DrawOne = true;
            DrawAll = false;
            RefreshDrawings();
        }

        private void showSequenceDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showSequenceDrawToolStripMenuItem.Checked = true;
            showPartDrawToolStripMenuItem.Checked = false;
            DrawAll = true;
            DrawOne = false;
            RefreshDrawings();
        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salvar();
        }

        private void importarTexturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportarTIM2();

        }

        private void salvarTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalvarTodos();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalvarComo();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Abrir(true, (string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void importViewImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportViewImage();
        }

        #region Atalhos
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.F2)
            {
                Abrir();
            }
            else if(e.KeyCode == Keys.W && e.Modifiers == Keys.Control)
            {
                if (fecharToolStripMenuItem.Enabled)
                    Fechar();
            }
            else if (e.KeyCode == Keys.Q && e.Modifiers == Keys.Control)
            {
                if (fecharTodosToolStripMenuItem.Enabled)
                    FecharTodos();
            }
            else if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                if (salvarToolStripMenuItem.Enabled)
                    Salvar();
            }
            else if (e.KeyCode == Keys.S && e.Modifiers == Keys.Shift)
            {
                if (salvarTodosToolStripMenuItem.Enabled)
                    SalvarTodos();
            }
            else if (e.KeyCode == Keys.S | e.KeyCode == Keys.Shift | e.KeyCode == Keys.Control)
            {
                if (saveAsToolStripMenuItem.Enabled)
                    SalvarComo();
            }
        }
        #endregion

        private void opçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void modoColoridoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modoColoridoToolStripMenuItem.Checked = !modoColoridoToolStripMenuItem.Checked;
            RefreshDrawings();
        }

        #endregion
        #endregion
    }
}

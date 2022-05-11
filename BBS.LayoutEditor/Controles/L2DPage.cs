using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
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
    public partial class L2DPage : TabPage
    {
        #region Caret DLL
        [DllImport("user32.dll", EntryPoint = "ShowCaret")]
        public static extern long ShowCaret(IntPtr hwnd);
        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern long HideCaret(IntPtr hwnd);
        #endregion

        public L2D L2DFile { get; set; }
        public ImageWindow imageWindow { get; set; }
        public Screen ViewPort { get; set; }

        public Form1 MainForm;

        public Image ViewImported;
        public Encoding FontEncoding;

        public string FilePath { get; set; }
        public string L2DName { get; set; }

        public int ZoomOut = 1;
        public L2DPage(Form1 mainf, L2D L2dFile, string filePath)
        {
            InitializeComponent();

            MainForm = mainf;
            L2DName = Path.GetFileName(filePath);
            this.Text = L2DName;
            FilePath = filePath;
            L2DFile = L2dFile;
            if (L2DFile.BigEndian)
                ZoomOut = 2;
            
            ToTreeView();

            colorDialog1 = new ColorDialog();
            richTextBox1.MouseWheel += richTextBox1_MouseWheel;
            fontEncoding.SelectedIndex = 0;
        }

        #region Funções

        #region Get Selected Things
        public LY2.Layout GetSelectedLayout() => L2DFile.Layout.Layouts[
              treeView1.SelectedNode.Level == 6 ? treeView1.SelectedNode.Parent.Parent.Parent.Parent.Parent.Index
            : treeView1.SelectedNode.Level == 5 ? treeView1.SelectedNode.Parent.Parent.Parent.Parent.Index
            : treeView1.SelectedNode.Level == 4 ? treeView1.SelectedNode.Parent.Parent.Parent.Index
            : treeView1.SelectedNode.Level == 3 ? treeView1.SelectedNode.Parent.Parent.Index
            : treeView1.SelectedNode.Level == 2 ? treeView1.SelectedNode.Parent.Index
            : treeView1.SelectedNode.Level == 1 ? treeView1.SelectedNode.Index : -1];
        public LY2.Node GetSelectedNode() => GetSelectedLayout().Nodes[
              treeView1.SelectedNode.Level == 6 ? treeView1.SelectedNode.Parent.Parent.Parent.Parent.Index
            : treeView1.SelectedNode.Level == 5 ? treeView1.SelectedNode.Parent.Parent.Parent.Index
            : treeView1.SelectedNode.Level == 4 ? treeView1.SelectedNode.Parent.Parent.Index
            : treeView1.SelectedNode.Level == 3 ? treeView1.SelectedNode.Parent.Index
            : treeView1.SelectedNode.Level == 2 ? treeView1.SelectedNode.Index : -1];
        public LY2.FontInfo GetSelectedFont() => GetSelectedNode().FontInfo == 1 ? GetSelectedNode().LinkedFont(L2DFile.Layout.FontInfos) : null;

        public SQ2P GetSelectedSQ2P() => L2DFile.SQ2Ps[GetSelectedNode().SQ2BaseNumber];

        public SP2.Sprite GetSelectedSprite() => GetSelectedSQ2P().SP2Entry.Sprites[treeView1.SelectedNode.Level == 4 ? Convert.ToInt32(treeView1.SelectedNode.Parent.Text.Split(new string[] { "Sprite ", "[", "]" }, StringSplitOptions.RemoveEmptyEntries)[0]) :
            treeView1.SelectedNode.Level == 3 ? Convert.ToInt32(treeView1.SelectedNode.Text.Split(new string[] { "Sprite ", "[", "]" }, StringSplitOptions.RemoveEmptyEntries)[0])
            : -1];
        public SP2.Group GetSelectedGroup() => GetSelectedSprite().Groups[treeView1.SelectedNode.Level == 4 ? treeView1.SelectedNode.Index : -1];

        public SP2.Part GetSelectedPart() => GetSelectedGroup().GetPart(GetSelectedSQ2P().SP2Entry.Parts);
        #endregion

        public void RenderImageWindow()
        {
            if (imageWindow == null || imageWindow.IsDisposed)
                imageWindow = new ImageWindow(this);
            imageWindow.DrawImageWindow(L2DFile.BigEndian ? ViewImported :L2DFile.SQ2Ps[GetSelectedNode().SQ2BaseNumber].Texture._picture[0].GetPNG(),
                L2DFile.SQ2Ps[GetSelectedNode().SQ2BaseNumber].Texture._picture[0].imageType.ToString(),
                L2DFile.SQ2Ps[GetSelectedNode().SQ2BaseNumber].Texture._picture[0].GetColorCount(),
                GetSelectedPart(), ZoomOut);
            if (!imageWindow.Visible)
                imageWindow.Show();
        }
        public void ApplyChanges()
        {
            if (treeView1.SelectedNode.Level == 4)
            {
                #region Apply UV Part Rectangle
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].U0 = (short)(U0num.Value * ZoomOut);
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].U1 = (short)((L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].U0 + uvWidthnum.Value) * ZoomOut);
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].V0 = (short)(V0num.Value * ZoomOut);
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].V1 = (short)((L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].V0 + uvHeightnum.Value) * ZoomOut);
                #endregion

                #region Apply Scissor Group Rectangle

                var sprite = GetSelectedSprite();

                ViewPort.Selected.Group.X0 = (short)(ViewPort.Selected.RectLocation.X - GetSelectedNode().X - GetSelectedLayout().X);                
                ViewPort.Selected.Group.Y0 = (short)(ViewPort.Selected.RectLocation.Y - GetSelectedNode().Y - GetSelectedLayout().Y);
                if (ViewPort.Base.Width == 1920)
                {
                    ViewPort.Selected.Group.X0 *= 4;
                    ViewPort.Selected.Group.X0 += 240;

                    ViewPort.Selected.Group.Y0 *= 4;
                    ViewPort.Selected.Group.Y0 += 135;
                }
                ViewPort.Selected.Group.X0 = (short)ViewPort.Selected.Group.X0.SRDtoSRU(ViewPort.Base.Width);
                ViewPort.Selected.Group.Y0 = (short)ViewPort.Selected.Group.Y0.SRDtoSRU(0, ViewPort.Base.Height);

                ViewPort.Selected.Group.X1 = (short)(ViewPort.Selected.Group.X0 + ViewPort.Selected.BoundSize.Width * ZoomOut);
                ViewPort.Selected.Group.Y1 = (short)(ViewPort.Selected.Group.Y0 + ViewPort.Selected.BoundSize.Height * ZoomOut);
                #endregion

                #region Color Part Set

                //Set new Color from picture boxes
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].SetColor(0, pictureBox3.BackColor);
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].SetColor(1, pictureBox4.BackColor);
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].SetColor(2, pictureBox5.BackColor);
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Parts[ViewPort.Selected.Group.IndexParts].SetColor(3, pictureBox6.BackColor);

                #endregion
                L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Sprites[ViewPort.Selected.Sprite.Index].Groups[ViewPort.Selected.Group.Index - L2DFile.SQ2Ps[ViewPort.Selected.LayoutNode.SQ2BaseNumber].SP2Entry.Sprites[ViewPort.Selected.Sprite.Index].GroupsIndex] = ViewPort.Selected.Group;

                MainForm.RefreshDrawings();
            }
            else if (treeView1.SelectedNode.Level == 3 && treeView1.SelectedNode.Text.Contains("Font"))
            {
                #region Font change Strings
                GetSelectedFont().String = FontEncoding.GetBytes(richTextBox1.Text);
                #endregion

                L2DFile.Layout.FontInfos[GetSelectedNode().FontInfoIndex] = GetSelectedFont();
            }
        }
        private void ToTreeView()
        {
            TreeNode Ly2root = new TreeNode($"L2D: {L2DFile.Name}");
            foreach (var layout in L2DFile.Layout.Layouts)
            {
                #region Layouts
                TreeNode layoutnode = new TreeNode(layout.LayoutName);
                foreach (var node in layout.Nodes)
                {
                    #region Layout Nodes
                    var nodeSeq = node.LinkedSequence(L2DFile.SQ2Ps);
                    TreeNode seqNode = new TreeNode(nodeSeq.Name);


                    #region Font Infos
                    TreeNode Fontnode;
                    if (node.FontInfo == 1)
                    {
                        Fontnode = new TreeNode($"Font [{node.FontInfoIndex}]");
                        seqNode.Nodes.Add(Fontnode);
                    }
                    #endregion

                    #region Sprites
                    var sp2Node = L2DFile.SQ2Ps[node.SQ2BaseNumber].SP2Entry;
                    foreach (var sprite in sp2Node.Sprites)
                    {
                        int groupIndex = sprite.GroupsIndex;

                        if (!Enumerable.Range(0, seqNode.Nodes.Count).Select(x => seqNode.Nodes[x].Text)
                            .Contains($"Sprite [{sprite.Index}]"))
                        {
                            TreeNode spriteNode = new TreeNode($"Sprite [{sprite.Index}]");
                            foreach (var group in sprite.Groups)
                            {
                                TreeNode groupNode = new TreeNode($"Group {groupIndex}");
                                spriteNode.Nodes.Add(groupNode);
                                groupIndex++;
                            }
                            if (nodeSeq.GetSprites(sp2Node.Sprites).Contains(sprite))
                                seqNode.Nodes.Add(spriteNode);

                        }
                    }
                    #endregion


                    layoutnode.Nodes.Add(seqNode);
                    #endregion
                }
                Ly2root.Nodes.Add(layoutnode);
                #endregion
            }
            treeView1.Nodes.Add(Ly2root);
        }
        #endregion

        #region Render
        private void Draw(Bitmap bitmap)
        {
            Clipboard.SetImage(bitmap);
            richTextBox1.ReadOnly = false;
            richTextBox1.Clear();
            richTextBox1.Paste();
            richTextBox1.ReadOnly = true;
            richTextBox1.SelectAll();
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            richTextBox1.DeselectAll();
        }
        private void DrawFontInfo()
        {
            richTextBox1.ReadOnly = false;
            richTextBox1.Clear();
            var fontinfo = GetSelectedFont();
            richTextBox1.Text = fontinfo.String.ConvertTo(FontEncoding);
            richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, (float)GetSelectedFont().Size, FontStyle.Regular);
            richTextBox1.ForeColor = fontinfo.Color;
            this.fontSizenum.Value = fontinfo.Size;
            this.fontColorpic.BackColor = fontinfo.Color;
        }
        public void RenderPart(bool setColor = false)
        {
            var group = GetSelectedGroup();

            if (setColor == true)
            {
                GetSelectedPart().SetColor(0, pictureBox3.BackColor);
                GetSelectedPart().SetColor(1, pictureBox4.BackColor);
                GetSelectedPart().SetColor(2, pictureBox5.BackColor);
                GetSelectedPart().SetColor(3, pictureBox6.BackColor);
            }

            #region Group/Part Info
            //Group Info
            GroupBox.Visible = true;
            GroupBox.Text = $"Group -> {treeView1.SelectedNode.Text.Split(new string[] { "Group" }, StringSplitOptions.RemoveEmptyEntries)[0]}";
            xylb.Text = $"X0: {group.X0},  Y0: {group.Y0},  " +
                $"X1: {group.X1},  Y1: {group.Y1}";
            attrblb.Text = $"   [{group.Attr}]";

            //Part Info
            PartBox.Visible = true;
            PartBox.Text = $"Part -> {group.IndexParts}";
            uvlb.Text = $"U0: {GetSelectedPart().U0},  V0: {GetSelectedPart().V0},  " +
                $"U1: {GetSelectedPart().U1},  V1: {GetSelectedPart().V1}";

            #endregion

            //Render
            Draw(GetUVPart(L2DFile.BigEndian ? ViewImported:GetSelectedSQ2P().Texture._picture[0].GetPNG()
                , GetSelectedPart(), false, ZoomOut));

            #region Color Part Get
            pictureBox3.BackColor = GetSelectedPart().GetColor(0);
            pictureBox4.BackColor = GetSelectedPart().GetColor(1);
            pictureBox5.BackColor = GetSelectedPart().GetColor(2);
            pictureBox6.BackColor = GetSelectedPart().GetColor(3);
            #endregion
        }
        public Bitmap GetUVPart(Image TIM2Texture, SP2.Part part, bool exportMode = false, int zoomOut = 1)
        {
            if (TIM2Texture == null)
                return new Bitmap(64, 64);
            var scissorBitmap = new Bitmap(Math.Abs(part.U0 - part.U1)/ zoomOut, Math.Abs(part.V0 - part.V1)/ zoomOut);
            if (part.U0 > 0)
            {
                var originalBitmap = new Bitmap(TIM2Texture);

                scissorBitmap = originalBitmap.Clone(new Rectangle(Math.Min(part.U0, part.U1) / zoomOut, Math.Min(part.V0, part.V1) / zoomOut,
                Math.Abs(part.U0 - part.U1) / zoomOut, Math.Abs(part.V0 - part.V1) / zoomOut), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                #region Color Filling
                //Color Drawing -> mipmap 4 blocks
                if (MainForm.modoColoridoToolStripMenuItem.Checked && exportMode == false)
                    for (int i = 0; i < 4; i++)
                        for (int x = (scissorBitmap.Width / 4) * i; x < scissorBitmap.Width; x++)
                            for (int y = (scissorBitmap.Height / 4) * i; y < scissorBitmap.Height; y++)
                                if (scissorBitmap.GetPixel(x, y).A > 128) //The game uses the RGBA[Index] Alfa to fill color
                                    scissorBitmap.SetPixel(x, y, part.GetColor(i));
                #endregion
            }
            else
            {
                scissorBitmap = new Bitmap(64, 64);
                Graphics.FromImage(scissorBitmap).DrawString("Inválido!", DefaultFont, Brushes.Red, 0, 0);
            }
            return scissorBitmap;
        }
        public void CloseWindows()
        {
            if (ViewPort != null && ViewPort.Visible || imageWindow != null && imageWindow.Visible)
            {
                ViewPort.Selected = null;
                ViewPort.Refresh();
                ViewPort.Close();

                imageWindow.Selected = null;
                imageWindow.Refresh();
                imageWindow.Close();
            }
        }

        #endregion

        #region Botões

        private void resetBT_Click(object sender, EventArgs e)
        {
            if (ViewPort != null && ViewPort.Visible)
                ViewPort.RestorePosition();
            if (imageWindow != null && imageWindow.Visible)
                imageWindow.RestorePosition();
        }

        private void applyBT_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }

        #endregion

        #region Eventos

        private void NumBoxChange(object sender, EventArgs e)
        {
            var numBox = sender as NumericUpDown;
            switch (numBox.Name)
            {
                //Group Values
                case "X0num":
                    ViewPort.Selected.RectLocation.X = (int)numBox.Value;
                    break;
                case "Y0num":
                    ViewPort.Selected.RectLocation.Y = (int)numBox.Value;
                    break;
                case "gpWidthnum":
                    ViewPort.Selected.BoundSize.Width = (int)numBox.Value;
                    break;

                case "gpHeightnum":
                    ViewPort.Selected.BoundSize.Height = (int)numBox.Value;
                    break;

                //Part Values
                case "U0num":
                    imageWindow.Selected.RectLocation.X = (int)numBox.Value;
                    break;
                case "V0num":
                    imageWindow.Selected.RectLocation.Y = (int)numBox.Value;
                    break;
                case "uvWidthnum":
                    imageWindow.Selected.BoundSize.Width = (int)numBox.Value;
                    break;
                case "uvHeightnum":
                    imageWindow.Selected.BoundSize.Height = (int)numBox.Value;
                    break;
            }
            imageWindow.Refresh();
            ViewPort.Refresh();
        }
        private void L2DPage_Paint(object sender, PaintEventArgs e)
        {

        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            /*Level of Information from L2D Tree
             * 0: Root L2D: + Name
             * 1: Layout
             * 2: LayoutNode
             * 3: Sprites / FontInfos
             * 4: Groups
             */

            #region SQ2P Menu

            if (treeView1.SelectedNode.Level >= 2 && treeView1.SelectedNode.Level < 5)
            {
                if (GetSelectedSQ2P()._Mark == SQ2P.Mark.Tm2DataAreaReduction)
                    MainForm.importViewImageToolStripMenuItem.Visible = true;
                else
                    MainForm.importViewImageToolStripMenuItem.Visible = false;
                MainForm.selecionadoToolStripMenuItem.Visible = true;
            }
            else
            {
                MainForm.selecionadoToolStripMenuItem.Visible = false;
            }
            #endregion

            #region Draw Group + ViewPort Screen
            if (treeView1.SelectedNode.Level == 4)
            {
                fontBox.Visible = false;
                fontOptionsBox.Visible = false;
                if (treeView1.SelectedNode.Text.Contains("Group"))//Groups Render
                {
                    richTextBox1.ReadOnly = true;
                    RenderImageWindow();

                    MainForm.sP2ToolStripMenuItem.Visible = true;
                    GroupBox.Visible = true;
                    MainForm.exportarRecorteToolStripMenuItem.Enabled = true;
                    MainForm.RefreshDrawings();

                    if (MainForm.DrawAll == true)
                        ViewPort.Selected = ViewPort.Textures.Where(x => x.Sprite.Index.ToString() == treeView1.SelectedNode.Parent.Text.Split(new string[] { "Sprite ", "[", "]" }, StringSplitOptions.RemoveEmptyEntries)[0]
                        &&
                        x.Group.Index.ToString() == treeView1.SelectedNode.Text.Split(new string[] { "Group " }, StringSplitOptions.RemoveEmptyEntries)[0]
                        ).ToArray()[0];
                    else
                        ViewPort.Selected = ViewPort.Textures[0];

                    ViewPort.Refresh();

                    U0num.Value = imageWindow.Selected.RectLocation.X;
                    V0num.Value = imageWindow.Selected.RectLocation.Y;
                    uvWidthnum.Value = imageWindow.Selected.BoundSize.Width;
                    uvHeightnum.Value = imageWindow.Selected.BoundSize.Height;
                }

            }
            else if (treeView1.SelectedNode.Level == 3)
            {
                if (treeView1.SelectedNode.Text.Contains("Font"))//Fonts
                {
                    CloseWindows();
                    GroupBox.Visible = false;
                    PartBox.Visible = false;
                    MainForm.sP2ToolStripMenuItem.Visible = false;
                    MainForm.exportarRecorteToolStripMenuItem.Enabled = false;
                    fontBox.Visible = true;
                    workspace.Enabled = true;
                    workspace.Text = $"Selected -> Font Info {GetSelectedNode().FontInfoIndex}";
                    workspace.Controls.Add(fontOptionsBox);
                    fontOptionsBox.Visible = true;
                    selectlbl.Visible = false;
                    resetBT.Visible = true;
                    resetBT.Enabled = true;
                    applyBT.Visible = true;
                    applyBT.Enabled = true;

                    strIndexlbl.Text = $"String Index: {GetSelectedFont().StringIndex}";
                    kcenterlbl.Text = $"Kind: {GetSelectedFont().Kind}  |  Center: {GetSelectedFont().Center}";
                    typelbl.Text = $"Type: {GetSelectedFont().Type}";
                    DrawFontInfo();
                }
                else
                {
                    fontOptionsBox.Visible = false;
                    fontBox.Visible = false;
                    workspace.Text = "";
                    workspace.Enabled = false;
                    selectlbl.Visible = true;
                    resetBT.Visible = false;
                    resetBT.Enabled = false;
                    applyBT.Visible = false;
                    applyBT.Enabled = false;
                }
            }
            else
            {
                workspace.Text = "";
                workspace.Enabled = false;
                fontBox.Visible = false;
                fontOptionsBox.Visible = false;
                applyBT.Visible = false;
                applyBT.Enabled = false;
                resetBT.Visible = false;
                resetBT.Enabled = false;
                selectlbl.Visible = true;
                selectlbl.Enabled = true;
                LayoutBox.Visible = false;
                layoutNodeBox.Visible = false;
                GroupBox.Visible = false;
                PartBox.Visible = false;
                fontBox.Visible = false;
                MainForm.sP2ToolStripMenuItem.Visible = false;
                MainForm.exportarRecorteToolStripMenuItem.Enabled = false;

                CloseWindows();
            }

            #endregion

            #region Layout and LayoutNode Infos

            int levelSelection = treeView1.SelectedNode.Level;

            //Layout
            if (levelSelection > 0)
            {
                //InformationBox
                informationBox.Visible = true;

                //Layout info
                var seleLayout = GetSelectedLayout();
                LayoutBox.Visible = true;
                LayoutBox.Text = $"Layout -> {seleLayout.LayoutName}";
                xyLayoutlbl.Text = $"X: {seleLayout.X} ,  Y: {seleLayout.Y}";
                layerLYlbl.Text = $"Layer: {seleLayout.Layer}";
            }
            else
            {
                //InformationBox
                informationBox.Visible = false;

                //Layout info
                LayoutBox.Visible = false;
            }

            //Layout Node
            if (levelSelection > 1)
            {
                //Layout node info
                var seleLayoutNode = GetSelectedNode();
                layoutNodeBox.Visible = true;
                layoutNodeBox.Text = $"Layout Node -> {seleLayoutNode.LinkedSequence(L2DFile.SQ2Ps).Name}";
                xyLayoutnodelbl.Text = $"X: {seleLayoutNode.X} ,  Y: {seleLayoutNode.Y}";
                afcollbl.Text = $"ColorEFF: {seleLayoutNode.AffectColor}  |  Font: {seleLayoutNode.FontInfo}";
                transllbl.Text = $"TranslationEFF: {seleLayoutNode.AffectTranslation}";
            }
            else
            {
                //Layout node info
                layoutNodeBox.Visible = false;
            }

            #endregion
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox2.Focus();
        }

        private void richTextBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            //HideCaret(richTextBox1.Handle);
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {

        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            //HideCaret(richTextBox1.Handle);
        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            //HideCaret(richTextBox1.Handle);
        }

        private void richTextBox1_MouseHover(object sender, EventArgs e)
        {
            //HideCaret(richTextBox1.Handle);
        }

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //HideCaret(richTextBox1.Handle);
            richTextBox1.DeselectAll();
        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //HideCaret(richTextBox1.Handle);
            richTextBox1.DeselectAll();
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            var pbx = sender as PictureBox;
            colorDialog1.Color = pbx.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pbx.BackColor = colorDialog1.Color;

                //Refresh
                RenderPart(true);

                MainForm.RefreshDrawings();
            }
        }
        private void fontEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combobox = sender as ComboBox;

            switch (combobox.SelectedIndex)
            {
                case 0:
                    FontEncoding = Encoding.Default;
                    break;

                case 1:
                    FontEncoding = Encoding.UTF8;
                    break;

                case 2:
                    FontEncoding = Encoding.GetEncoding(932);
                    break;
            }
            try { DrawFontInfo(); } catch (Exception) { }
        }
        private void fontSizenum_ValueChanged(object sender, EventArgs e)
        {
            GetSelectedFont().Size = (byte)fontSizenum.Value;
            DrawFontInfo();
        }
        private void fontColorpic_Click(object sender, EventArgs e)
        {
            var pbx = sender as PictureBox;
            colorDialog1.Color = pbx.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pbx.BackColor = colorDialog1.Color;
                GetSelectedFont().Color = pbx.BackColor;
                DrawFontInfo();
            }
        }

        #endregion
    }
}

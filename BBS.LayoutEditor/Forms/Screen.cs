using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
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
    public partial class Screen : Form
    {
        public SelectionRectangle Selection;
        public Size PinsSize = new Size(5, 5);
        public Size Base;

        public Graphics Context;
        public List<Texture> Textures;
        public Texture Selected;

        PosSizRect posRect;
        private enum PosSizRect
        {
            Norte,
            Sul,
            Leste,
            Oeste,
            Nordeste,
            Sudeste,
            Noroeste,
            Sudoeste,
            Move
        };

        private Point MouseLastPoint;
        bool isMouseDown = false, isMove = false, lockMove = false, lockSize = false;

        L2DPage l2dpage;
        public Screen(L2DPage page)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            l2dpage = page;
            panel1.GetType().GetMethod("SetStyle",
    System.Reflection.BindingFlags.Instance |
    System.Reflection.BindingFlags.NonPublic).Invoke(panel1,
        new object[]
        {
            System.Windows.Forms.ControlStyles.UserPaint |
            System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
            System.Windows.Forms.ControlStyles.DoubleBuffer, true
        });

            this.MdiParent = l2dpage.MainForm;
            l2dpage.MainForm.Size = new Size(l2dpage.MainForm.NotMDISize.Width + this.Width,
                l2dpage.MainForm.NotMDISize.Height);
            StartPosition = FormStartPosition.Manual;
            this.Location = new Point(l2dpage.MainForm.tabControl1.Width,
                0);
        }

        public void RenderAll(List<Texture> textures)
        {
            Textures = textures;

            int index = 0;
            foreach (var texture in textures)
            {
                texture.Bounds = new Rectangle(texture.RectLocation,
                    texture.BoundSize);
                texture.Index = index;
                Refresh();
                index++;
            }

        }
        public void RestorePosition()
        {
            Selected.RectLocation = Selected.PrevRectLocation;
            Selected.Bounds.Location = Selected.RectLocation;
            Selected.BoundSize = Selected.PrevBoundSize;
            Selected.Bounds.Size = Selected.BoundSize;
            Selection.SetSelection(Selected.Bounds, PinsSize);
            Textures[Selected.Index] = Selected;

            RenderAll(Textures);
        }
        public void DeselectAll()
        {
            Selected = null;
        }

        #region Rectangle and Painting
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            #region Draw Image and Rectangle
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Context = e.Graphics;
            if (Textures != null)
                foreach (var texture in Textures)
                    e.Graphics.DrawImage(texture.image, texture.RectLocation);
            #endregion

            if (Selected != null)
            {
                #region L2DPAGE Info Update
                l2dpage.workspace.Text = $"Selected -> Sprite {Selected.Sprite.Index}, Group {Selected.Group.Index}";
                l2dpage.workspace.Enabled = true;
                l2dpage.selectlbl.Visible = false;
                l2dpage.resetBT.Enabled = true;
                l2dpage.resetBT.Visible = true;
                l2dpage.applyBT.Enabled = true;
                l2dpage.applyBT.Visible = true;

                //Edit things
                l2dpage.screenGpBox.Visible = true;
                l2dpage.scissorGpbox.Visible = true;
                l2dpage.lockMoveChckBox.Visible = true;
                l2dpage.lockSizeChckbox.Visible = true;

                #region Fill numbox Infos and get Flags
                //Flags
                lockMove = l2dpage.lockMoveChckBox.Checked;
                lockSize = l2dpage.lockSizeChckbox.Checked;

                #region SCREEN RECTANGLE GROUP
                l2dpage.X0num.Value = Selected.RectLocation.X;
                l2dpage.Y0num.Value = Selected.RectLocation.Y;

                l2dpage.gpWidthnum.Value = Selected.BoundSize.Width;
                l2dpage.gpHeightnum.Value = Selected.BoundSize.Height;
                #endregion

                #endregion

                #endregion
                #region Draw Selection and Plot Infos
                Selection.SetSelection(new Rectangle(Selected.RectLocation, Selected.BoundSize), PinsSize);
                Selection.DrawSelection(e.Graphics, Brushes.Black, Brushes.Red);

                //Info
                origposlbl.Text = $"Original Position: X: {Selected.PrevRectLocation.X}  |  Y: {Selected.PrevRectLocation.Y}";
                poslbl.Text = $"Position: X: {Selected.RectLocation.X}  |  Y: {Selected.RectLocation.Y}";
                origsize.Text = $"Original Size: {Selected.PrevBoundSize.Width}x{Selected.PrevBoundSize.Height}";
                size.Text = $"Size: {Selected.BoundSize.Width}x{Selected.BoundSize.Height}";
                #endregion
            }
            else
            {
                #region L2DPAGE Info Update
                l2dpage.workspace.Text = "";
                l2dpage.workspace.Enabled = false;
                l2dpage.selectlbl.Visible = true;
                l2dpage.resetBT.Enabled = false;
                l2dpage.resetBT.Visible = false;
                l2dpage.applyBT.Enabled = false;
                l2dpage.applyBT.Visible = false;

                //Edit things
                l2dpage.screenGpBox.Visible = false;
                l2dpage.scissorGpbox.Visible = false;
                l2dpage.lockMoveChckBox.Visible = false;
                l2dpage.lockSizeChckbox.Visible = false;
                #endregion
            }


        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            //Get Selected Texture - Just one now!
            var findSelected = Textures.Where(x => x.Bounds.Contains(panel1.PointToClient(Control.MousePosition))).ToArray();
            if (findSelected.Length > 0)
            {
                Selected = findSelected[0];
                Selection.SetSelection(new Rectangle(Selected.RectLocation, Selected.BoundSize), PinsSize);

                if (RectangleTools.IsMouseOn(Selected.Bounds, panel1) && lockMove == false)
                    isMove = true;
            }

            MouseLastPoint = e.Location;
            Refresh();
            l2dpage.Refresh();

            //Set bool button pressed
            isMouseDown = true;
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = $"SRD:   X:{e.X}  |  Y: {e.Y}";
            label2.Text = $"SRU:   X:{e.X.SRDtoSRU(480)}  |  Y: {e.Y.SRDtoSRU(0, 272)}";

            #region Move and Resize Operations/Cursor
            if (Selected != null &&
                ClientRectangle.Contains(PointToClient(Control.MousePosition)))
            {
                if (isMouseDown == true && isMove == false && lockSize == false)
                {
                    switch (posRect)
                    {
                        case PosSizRect.Norte:
                            Selected.Bounds.Y += e.Y - MouseLastPoint.Y;
                            Selected.BoundSize.Height -= e.Y - MouseLastPoint.Y;
                            break;
                        case PosSizRect.Sul:
                            Selected.BoundSize.Height += e.Y - MouseLastPoint.Y;
                            break;
                        case PosSizRect.Leste:
                            Selected.BoundSize.Width += e.X - MouseLastPoint.X;
                            break;
                        case PosSizRect.Oeste:
                            Selected.Bounds.X += e.X - MouseLastPoint.X;
                            Selected.BoundSize.Width -= e.X - MouseLastPoint.X;
                            break;

                        case PosSizRect.Nordeste:
                            Selected.Bounds.X += e.X - MouseLastPoint.X;
                            Selected.Bounds.Y += e.Y - MouseLastPoint.Y;
                            Selected.BoundSize.Width -= e.X - MouseLastPoint.X;
                            Selected.BoundSize.Height -= e.Y - MouseLastPoint.Y;
                            break;
                        case PosSizRect.Sudeste:
                            Selected.BoundSize.Width -= e.X - MouseLastPoint.X;
                            Selected.Bounds.X += e.X - MouseLastPoint.X;
                            Selected.BoundSize.Height += e.Y - MouseLastPoint.Y;
                            break;
                        case PosSizRect.Noroeste:
                            Selected.Bounds.Y += e.Y - MouseLastPoint.Y;
                            Selected.BoundSize.Width += e.X - MouseLastPoint.X;
                            Selected.BoundSize.Height -= e.Y - MouseLastPoint.Y;
                            break;
                        case PosSizRect.Sudoeste:
                            Selected.BoundSize.Width += e.X - MouseLastPoint.X;
                            Selected.BoundSize.Height += e.Y - MouseLastPoint.Y;
                            break;

                    }
                    Selected.Bounds.Size = Selected.BoundSize;
                    Selection.SetSelection(Selected.Bounds, PinsSize);
                }
                if (isMove == true && lockMove == false)
                {
                    posRect = PosSizRect.Move;
                }

                #region Directions and Move Cursor
                if (RectangleTools.IsMouseOn(Selection.Norte, panel1) && lockSize == false ||
                        RectangleTools.IsMouseOn(Selection.Sul, panel1) && lockSize == false)
                {
                    //North/South
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNS;

                    if (isMouseDown == true && isMove == false && lockSize == false)
                    {
                        if (RectangleTools.IsMouseOn(Selection.Norte, panel1))
                        {
                            posRect = PosSizRect.Norte;
                        }
                        else
                        {
                            posRect = PosSizRect.Sul;
                        }
                    }
                }
                else if (RectangleTools.IsMouseOn(Selection.Leste, panel1) && lockSize == false ||
                        RectangleTools.IsMouseOn(Selection.Oeste, panel1) && lockSize == false)
                {
                    //East/West
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeWE;

                    if (isMouseDown == true && isMove == false && lockSize == false)
                    {
                        if (RectangleTools.IsMouseOn(Selection.Leste, panel1))
                        {
                            posRect = PosSizRect.Leste;
                        }
                        else
                        {
                            posRect = PosSizRect.Oeste;
                        }
                    }
                }
                else if (RectangleTools.IsMouseOn(Selection.Sudeste, panel1) && lockSize == false ||
                        RectangleTools.IsMouseOn(Selection.Noroeste, panel1) && lockSize == false)
                {
                    //NW/SE
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNESW;

                    if (isMouseDown == true && isMove == false && lockSize == false)
                    {
                        if (RectangleTools.IsMouseOn(Selection.Sudeste, panel1))
                        {
                            posRect = PosSizRect.Sudeste;
                        }
                        else
                        {
                            posRect = PosSizRect.Noroeste;
                        }
                    }
                }
                else if (RectangleTools.IsMouseOn(Selection.Nordeste, panel1) && lockSize == false ||
                        RectangleTools.IsMouseOn(Selection.Sudoeste, panel1) && lockSize == false)
                {
                    //NE/SW
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNWSE;

                    if (isMouseDown == true && isMove == false && lockSize == false)
                    {
                        if (RectangleTools.IsMouseOn(Selection.Nordeste, panel1))
                        {
                            posRect = PosSizRect.Nordeste;
                        }
                        else
                        {
                            posRect = PosSizRect.Sudoeste;
                        }
                    }
                }
                else if (RectangleTools.IsMouseOn(Selected.Bounds, panel1) && lockMove == false)
                {
                    //Move
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeAll;

                    if (isMouseDown == true && isMove == true && lockMove == false)
                    {
                        //Move cursor inside
                        posRect = PosSizRect.Move;

                        Selected.RectLocation.X = e.Location.X - Selected.image.Width / 2;
                        Selected.RectLocation.Y = e.Location.Y - Selected.image.Height / 2;
                        Selected.Bounds.Location = Selected.RectLocation;
                        Selection.SetSelection(Selected.Bounds, PinsSize);

                        //Check rectangle bounds to not go out the control
                        if (Selection.Rectangle.Right > panel1.Width || Selected.Bounds.Right > panel1.Width)
                        {
                            Selected.RectLocation.X = panel1.Width - Selected.Bounds.Width;
                            Selected.Bounds.Location = Selected.RectLocation;
                            Selection.SetSelection(Selected.Bounds, PinsSize);
                        }
                        if (Selection.Rectangle.Top < 0 || Selected.Bounds.Top < 0)
                        {
                            Selected.RectLocation.Y = 0;
                            Selected.Bounds.Location = Selected.RectLocation;
                            Selection.SetSelection(Selected.Bounds, PinsSize);
                        }
                        if (Selection.Rectangle.Left < 0 || Selected.Bounds.Left < 0)
                        {
                            Selected.RectLocation.X = 0;
                            Selected.Bounds.Location = Selected.RectLocation;
                            Selection.SetSelection(Selected.Bounds, PinsSize);
                        }
                        if (Selection.Rectangle.Bottom > panel1.Height || Selected.Bounds.Bottom > panel1.Height)
                        {
                            Selected.RectLocation.Y = panel1.Height - Selected.Bounds.Height;
                            Selected.Bounds.Location = Selected.RectLocation;
                            Selection.SetSelection(Selected.Bounds, PinsSize);
                        }



                    }
                }
                else
                {
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                }
                #endregion

                //Return modifications to array
                Selected.image = new Bitmap(Selected.Originalimage, Selected.BoundSize);//Image with new dimensions to render
                Textures[Selected.Index] = Selected;//Back to Array
                MouseLastPoint = e.Location;//Last Point set to Mouse Cursor

                Refresh();
            }
            #endregion
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            //Get Selected Texture - Just one now!
            var findSelected = Textures.Where(x => x.Bounds.Contains(panel1.PointToClient(Control.MousePosition))).ToArray();
            if (findSelected.Length == 0)
                if (System.Windows.Forms.Cursor.Current == System.Windows.Forms.Cursors.Default)
                    DeselectAll();

            l2dpage.Refresh();
            Refresh();
            isMove = false;
            isMouseDown = false;
        }


        #endregion
        private void PSPScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            l2dpage.MainForm.Size = l2dpage.MainForm.NotMDISize;
        }
    }
}

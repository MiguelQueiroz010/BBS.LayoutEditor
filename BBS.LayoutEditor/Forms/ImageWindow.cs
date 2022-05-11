using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
    public partial class ImageWindow : Form
    {
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

        Image current, original;
        public Texture texture, Selected;
        PosSizRect posRect;
        private Point MouseLastPoint;
        bool isMouseDown = false, isMove = false, lockMove = false, lockSize = false;

        public SelectionRectangle Selection;
        Size PinsSize = new Size(5, 5);

        L2DPage l2dpage;

        public ImageWindow(L2DPage page)
        {
            InitializeComponent();

            l2dpage = page;
            //Double Buffered pictureBox
            pictureBox1.GetType().GetMethod("SetStyle",
    System.Reflection.BindingFlags.Instance |
    System.Reflection.BindingFlags.NonPublic).Invoke(pictureBox1,
        new object[]
        {
            System.Windows.Forms.ControlStyles.UserPaint |
            System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
            System.Windows.Forms.ControlStyles.DoubleBuffer, true
        });

            //Info set
            xlabel.Text = $"X: {0}";
            ylabel.Text = $"Y: {0}";
            bpplbl.Text = $"Bpp: {0} | Width: {0}";
            colorlb.Text = $"Colors: {0} | Height: {0}";
        }
        public void Render()
        {
            texture.Bounds = new Rectangle(texture.RectLocation,
                texture.BoundSize);
            Refresh();
        }
        public void RestorePosition()
        {
            Selected.RectLocation = Selected.PrevRectLocation;
            Selected.Bounds.Location = Selected.RectLocation;
            Selected.BoundSize = Selected.PrevBoundSize;
            Selected.Bounds.Size = Selected.BoundSize;
            Selection.SetSelection(Selected.Bounds, PinsSize);

            Render();
        }
        public void DeselectAll()
        {
            Selected = null;
        }
        public void DrawImageWindow(Image image, string bpp, int colors, SP2.Part part, int zoomOut = 1)
        {
            original = image;
            current = image;
            pictureBox1.Image = current;

            //Rectangle info
            try
            {
                texture = new Texture()
                {
                    image = image,
                    Originalimage = image,
                    RectLocation = new Point(part.U0 / zoomOut, part.V0/ zoomOut),
                    PrevRectLocation = new Point(part.U0 / zoomOut, part.V0 / zoomOut),
                    PrevBoundSize = new Size((part.U1 - part.U0) / zoomOut, (part.V1 - part.V0) / zoomOut),
                    BoundSize = new Size((part.U1 - part.U0) / zoomOut, (part.V1 - part.V0) / zoomOut)
                };
                texture.Bounds = new Rectangle(texture.RectLocation, texture.BoundSize);
                Selected = texture;
            }
            catch (Exception) { }


            //Info Set
            bpplbl.Text = $"Bpp: {bpp} | Width: {image.Width}";
            colorlb.Text = $"Colors: {colors} | Height: {image.Height}";
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //Information(Mouse Position)
            xlabel.Text = $"X: {e.X}";
            ylabel.Text = $"Y: {e.Y}";

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

                    Selected.RectLocation.X = MouseLastPoint.X;
                    Selected.RectLocation.Y = MouseLastPoint.Y;
                    Selected.Bounds.Location = Selected.RectLocation;
                    Selection.SetSelection(Selected.Bounds, PinsSize);

                    //Check rectangle bounds to not go out the control
                    if (Selection.Rectangle.Right > pictureBox1.Width || Selected.Bounds.Right > pictureBox1.Width)
                    {
                        Selected.RectLocation.X = pictureBox1.Width - Selected.Bounds.Width;
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
                    if (Selection.Rectangle.Bottom > pictureBox1.Height || Selected.Bounds.Bottom > pictureBox1.Height)
                    {
                        Selected.RectLocation.Y = pictureBox1.Height - Selected.Bounds.Height;
                        Selected.Bounds.Location = Selected.RectLocation;
                        Selection.SetSelection(Selected.Bounds, PinsSize);
                    }


                }

                #region Directions and Move Cursor
                if (RectangleTools.IsMouseOn(Selection.Norte, pictureBox1) && lockSize == false ||
                        RectangleTools.IsMouseOn(Selection.Sul, pictureBox1) && lockSize == false)
                {
                    //North/South
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNS;

                    if (isMouseDown == true && isMove == false && lockSize == false)
                    {
                        if (RectangleTools.IsMouseOn(Selection.Norte, pictureBox1))
                        {
                            posRect = PosSizRect.Norte;
                        }
                        else
                        {
                            posRect = PosSizRect.Sul;
                        }
                    }
                }
                else if (RectangleTools.IsMouseOn(Selection.Leste, pictureBox1) && lockSize == false ||
                        RectangleTools.IsMouseOn(Selection.Oeste, pictureBox1) && lockSize == false)
                {
                    //East/West
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeWE;

                    if (isMouseDown == true && isMove == false && lockSize == false)
                    {
                        if (RectangleTools.IsMouseOn(Selection.Leste, pictureBox1))
                        {
                            posRect = PosSizRect.Leste;
                        }
                        else
                        {
                            posRect = PosSizRect.Oeste;
                        }
                    }
                }
                else if (RectangleTools.IsMouseOn(Selection.Sudeste, pictureBox1) && lockSize == false ||
                        RectangleTools.IsMouseOn(Selection.Noroeste, pictureBox1) && lockSize == false)
                {
                    //NW/SE
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNESW;

                    if (isMouseDown == true && isMove == false && lockSize == false)
                    {
                        if (RectangleTools.IsMouseOn(Selection.Sudeste, pictureBox1))
                        {
                            posRect = PosSizRect.Sudeste;
                        }
                        else
                        {
                            posRect = PosSizRect.Noroeste;
                        }
                    }
                }
                else if (RectangleTools.IsMouseOn(Selection.Nordeste, pictureBox1) && lockSize == false ||
                        RectangleTools.IsMouseOn(Selection.Sudoeste, pictureBox1) && lockSize == false)
                {
                    //NE/SW
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNWSE;

                    if (isMouseDown == true && isMove == false && lockSize == false)
                    {
                        if (RectangleTools.IsMouseOn(Selection.Nordeste, pictureBox1))
                        {
                            posRect = PosSizRect.Nordeste;
                        }
                        else
                        {
                            posRect = PosSizRect.Sudoeste;
                        }
                    }
                }
                else if (RectangleTools.IsMouseOn(Selected.Bounds, pictureBox1) && lockMove == false)
                {
                    //Move
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeAll;

                    if (isMouseDown == true && isMove == true && lockMove == false)
                    {
                        //Move cursor inside
                        posRect = PosSizRect.Move;



                    }
                }
                else
                {
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                }
                #endregion

                MouseLastPoint = e.Location;//Last Point set to Mouse Cursor
                Refresh();
            }
            #endregion

            //Numboxes Values
            l2dpage.U0num.Value = Selected.RectLocation.X;
            l2dpage.V0num.Value = Selected.RectLocation.Y;
            l2dpage.uvWidthnum.Value = Selected.BoundSize.Width;
            l2dpage.uvHeightnum.Value = Selected.BoundSize.Height;

            if (l2dpage.ViewPort != null && l2dpage.ViewPort.Visible)
                l2dpage.ViewPort.Selected.BoundSize = Selected.BoundSize;

            l2dpage.Refresh();
        }

        private void ImageWindow_Paint(object sender, PaintEventArgs e)
        {
            #region Draw Image and Rectangle
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            #endregion

            //Flags
            lockMove = l2dpage.lockMoveChckBox.Checked;
            lockSize = l2dpage.lockSizeChckbox.Checked;

            if (Selected != null)
            {
                #region Draw Selection and Plot Infos
                Selection.SetSelection(new Rectangle(Selected.RectLocation, Selected.BoundSize), PinsSize);
                Selection.DrawSelection(e.Graphics, Brushes.Black, Brushes.Green);
                #endregion
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMove = false;
            isMouseDown = false;
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //Selection.SetSelection(Selected.Bounds, PinsSize);
            if (RectangleTools.IsMouseOn(Selected.Bounds, pictureBox1) && lockMove == false)
                isMove = true;

            MouseLastPoint = e.Location;
            Refresh();
            l2dpage.Refresh();

            isMouseDown = true;
        }
    }
}

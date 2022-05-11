using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

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
public struct SelectionRectangle
{
    public Rectangle Rectangle;

    public Rectangle Norte, Sul, Leste, Oeste;
    public Rectangle Nordeste, Sudeste, Noroeste, Sudoeste;

    internal void SetSelection(Rectangle rectangle, Size PinsSize)
    {
        Rectangle = rectangle;

        //Normais
        Sul = new Rectangle(rectangle.X + rectangle.Width/2, rectangle.Y + rectangle.Height, PinsSize.Width, PinsSize.Height);
        Norte = new Rectangle(rectangle.X + rectangle.Width/2, rectangle.Y - PinsSize.Height, PinsSize.Width, PinsSize.Height);
        Leste = new Rectangle(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height/2, PinsSize.Width, PinsSize.Height);
        Oeste = new Rectangle(rectangle.X- PinsSize.Width, rectangle.Y + rectangle.Height/2, PinsSize.Width, PinsSize.Height);

        //Extremos
        Sudeste = new Rectangle(rectangle.X - PinsSize.Width, rectangle.Y + rectangle.Height, PinsSize.Width, PinsSize.Height);
        Sudoeste = new Rectangle(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, PinsSize.Width, PinsSize.Height);
        Nordeste = new Rectangle(rectangle.X - PinsSize.Width, rectangle.Y - PinsSize.Height, PinsSize.Width, PinsSize.Height);
        Noroeste = new Rectangle(rectangle.X + rectangle.Width, rectangle.Y - PinsSize.Height, PinsSize.Width, PinsSize.Height);
    }
    public void DrawSelection(Graphics e, Brush PinsColor, Brush SelectionColor)
    {
        e.DrawRectangle(new Pen(SelectionColor),Rectangle);
        e.DrawRectangles(new Pen(PinsColor), new Rectangle[] {
            Norte,Sul, Leste, Oeste,
            Nordeste, Sudeste, Noroeste, Sudoeste
        });
    }

}

public class RectangleTools
{
    public static bool IsMouseOn(Rectangle rect, Control control) => rect.Contains(control.PointToClient(Control.MousePosition));

}


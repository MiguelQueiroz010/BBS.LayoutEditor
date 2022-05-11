using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

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
    public class Texture
    {
        public Image image;
        public Image Originalimage;
        public Rectangle Bounds;

        public Point PrevRectLocation;
        public Point RectLocation;

        public Size PrevBoundSize;
        public Size BoundSize;
        public int Index;

        public LY2.Layout Layout;
        public LY2.Node LayoutNode;
        public SP2.Sprite Sprite;
        public SP2.Group Group;

    }
}

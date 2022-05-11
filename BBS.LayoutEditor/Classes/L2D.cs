using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using static IOextent;
using Dxt;
using Rainbow.ImgLib.Encoding;

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
public class L2D //Layout 2 Dimensional Header
{
    private const string Magic = "L2D@";
    private string Version;

    private uint Reserved1, Reserved2;
    private byte[] Reserved3;
    private int SQ2PCount;
    private uint SQ2PBaseOffset;
    private uint LY2Offset;
    public bool BigEndian = false;

    public List<Info> SQ2PInfos;
    public SQ2P[] SQ2Ps;
    public LY2 Layout;

    public string Name;
    public uint FileSize;
    //public DateTime InternalTime; NOT USED ANYMORE
    private byte[] Date;

    public class Info
    {
        public uint Offset { get; set; }
        public uint Tamanho { get; set; }

        public Info(uint offs, uint size = 0)
        {
            Offset = offs;
            Tamanho = size;
        }
    }

    public L2D(byte[] Input)
    {
        #region Verificar Tipo de Arquivo
        byte[] magic = Input.ReadBytes(0, 4);
        if (magic[0] == 0x40)//"@D2L"
        {
            BigEndian = true;
            Array.Reverse(magic);
        }

        if (magic.ConvertTo(Encoding.Default)!=Magic)
            throw new Exception($"Tipo de arquivo não suportado!\r\n" +
                $"Magic esperado: 0x{Magic}\r\nEncontrado: {magic.ConvertTo(Encoding.Default)}");
        #endregion
        #region Desserializar dados Header
        Version = Input.ReadBytes(4, 4).ConvertTo(Encoding.Default);
        //InternalTime = Input.ReadBytes(8, 8).GetDateTimeDir();
        Date = Input.ReadBytes(8, 8);
        Name = Input.ReadBytes(0x10, 4).ConvertTo(Encoding.Default);

        //Reservados
        Reserved1 = Input.ReadUInt(0x14, 32, BigEndian);
        Reserved2 = (uint)Input.ReadULong(0x18, BigEndian);

        //Counts/Offsets
        SQ2PCount = (int)Input.ReadUInt(0x20, 32, BigEndian);
        SQ2PBaseOffset = Input.ReadUInt(0x24, 32, BigEndian);
        LY2Offset = Input.ReadUInt(0x28, 32, BigEndian);
        FileSize = Input.ReadUInt(0x2C, 32, BigEndian);

        Reserved3 = Input.ReadBytes(0x30, 0x10);
        #endregion

        #region Desserializar LY2
        Layout = LY2.Read(new MemoryStream(Input.ReadBytes((int)LY2Offset, (int)(FileSize - LY2Offset))),BigEndian);
        #endregion

        #region Offsets/Tamanhos SQ2P
        byte[] SEQ2InfoInput = Input.ReadBytes((int)SQ2PBaseOffset, (int)Input.ReadUInt((int)SQ2PBaseOffset, 32,BigEndian));
        if (SQ2PCount > 0)
        {
            SQ2PInfos = new List<Info>();

            //Offsets first
            for (int i = 0, suboffs = 0; i < SQ2PCount; i++, suboffs += 4)
            {
                if (i == 0)//Primeira Entrada
                    SQ2PInfos.Add(new Info(SEQ2InfoInput.ReadUInt(0, 32, BigEndian),
                        SEQ2InfoInput.ReadUInt(4, 32, BigEndian) - SQ2PBaseOffset));
                else
                {
                    uint offs = SEQ2InfoInput.ReadUInt(suboffs, 32, BigEndian);
                    SQ2PInfos.Add(new Info(offs));
                }

            }

            //then the Sizes calculation offsets based
            for (int i = 0; i < SQ2PInfos.Count; i++)
            {
                uint Size = 0;
                if (i == SQ2PInfos.Count - 1)
                {
                    Size = (LY2Offset - 0x40) - SQ2PInfos[i].Offset;
                }
                else
                {
                    Size = SQ2PInfos[i + 1].Offset - SQ2PInfos[i].Offset;
                }
                SQ2PInfos[i].Tamanho = Size;
            }
        }
        #endregion

        #region Desserializar SQ2P
        byte[] L2SQHeaderless = Input.ReadBytes((int)SQ2PBaseOffset, (int)(LY2Offset - SQ2PBaseOffset));

        SQ2Ps = Enumerable.Range(0, SQ2PCount).Select(x =>
        SQ2P.Read(L2SQHeaderless.ReadBytes((int)SQ2PInfos[x].Offset, (int)SQ2PInfos[x].Tamanho),x, BigEndian)
        ).ToArray();

        #endregion
    }

    internal byte[] GetRebuild(bool LittleEndianOnly = false)
    {
        var result = new List<byte>();
        result.AddRange(Encoding.Default.GetBytes(Magic).Endianess(LittleEndianOnly == false ? BigEndian: false));
        result.AddRange(Encoding.Default.GetBytes(Version));
        result.AddRange(Date);
        //result.AddRange(BitConverter.GetBytes((UInt64)InternalTime.ToBinary()).Endianess(LittleEndianOnly == false ? BigEndian : false));

        result.AddRange(Encoding.Default.GetBytes(Name));//Name
        result.AddRange(BitConverter.GetBytes((UInt32)Reserved1).Endianess(LittleEndianOnly == false ? BigEndian : false));
        result.AddRange(BitConverter.GetBytes((UInt64)Reserved2).Endianess(LittleEndianOnly == false ? BigEndian : false));
        
        //SQ2Ps Offset Table
        int TableSize = SQ2Ps.Count();
        while (TableSize % 0x10 != 0)
            TableSize++;

        //LY2
        byte[] LayoutData = Layout.GetData(LittleEndianOnly == false ? BigEndian : false);

        //SQ2Ps Data
        var OffsSQ2P = new List<int>();
        var SQ2PsData = new List<byte>();
        int offset = TableSize;
        foreach (var _sq2p in SQ2Ps)
        {
            byte[] Sq2pData = _sq2p.GetData(LittleEndianOnly == false ? BigEndian : false);
            SQ2PsData.AddRange(Sq2pData);
            OffsSQ2P.Add(offset);

            offset += Sq2pData.Length;
        }

        //Counts and Offsets
        result.AddRange(BitConverter.GetBytes((UInt32)SQ2Ps.Count()).Endianess(LittleEndianOnly == false ? BigEndian : false));//SQ2P Count
        result.AddRange(BitConverter.GetBytes((UInt32)0x40).Endianess(LittleEndianOnly == false ? BigEndian : false));//SQ2P Offset (Table offset)
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40+TableSize+SQ2PsData.Count())).Endianess(LittleEndianOnly == false ? BigEndian : false));//LY2 Offset

        result.AddRange(BitConverter.GetBytes((UInt32)(0x40+TableSize+SQ2PsData.Count() + LayoutData.Length)).Endianess(LittleEndianOnly == false ? BigEndian : false));//Entire File Size

        //Reserved Mark Data
        result.AddRange(Reserved3);

        //Build Offset Table
        var OffsTable = new List<byte>();
        foreach(var _offs in OffsSQ2P)
            OffsTable.AddRange(BitConverter.GetBytes((UInt32)_offs).Endianess(LittleEndianOnly == false ? BigEndian : false));

        while (OffsTable.Count() < TableSize)
            OffsTable.Add(0); ;

        //Datas
        result.AddRange(OffsTable.ToArray());//SQ2Ps Offs Table
        result.AddRange(SQ2PsData.ToArray());//SQ2Ps
        result.AddRange(LayoutData);//LY2

        return result.ToArray();
    }
}
public class LY2
{
    public class FontInfo
    {
        public uint StringIndex { get; set; }
        public Color Color { get; set; }
        public byte Size { get; set; }
        public byte Kind { get; set; }
        public byte Center { get; set; }
        public byte Type { get; set; }
        public uint Reserved { get; set; }
        public byte[] String { get; set; }
        internal static FontInfo Read(byte[] FontData, byte[] StringsData, bool bigendian =false) => new FontInfo()
        {
            StringIndex = FontData.ReadUInt(0, 32, bigendian),
            Color = Color.FromArgb(FontData[7], FontData[4], FontData[5], FontData[6]),
            Size = FontData[8],
            Kind = FontData[9],
            Center = FontData[10],
            Type = FontData[11],
            Reserved = FontData.ReadUInt(0xC, 32, bigendian),
            String = StringsData != null ?  StringsData.Skip((int)FontData.ReadUInt(0, 32, bigendian)).TakeWhile(x=>x!=0).ToArray(): null
        };

        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)StringIndex).Endianess(bigendian));
            result.AddRange(Color.ColorToData(bigendian));
            result.Add(Size);
            result.Add(Kind);
            result.Add(Center);
            result.Add(Type);
            result.AddRange(BitConverter.GetBytes((UInt32)Reserved).Endianess(bigendian));
            return result.ToArray();
        }
    }
    public class Layout
    {
        public uint MaxFrame { get; set; }
        public uint NodeStartIndex { get; set; }
        public ushort NodeCount { get; set; }
        public ushort Layer { get; set; }
        public short X { get; set; }
        public short Y { get; set; }

        public string LayoutName { get; set; }

        public Node[] Nodes;

        internal static Layout Read(string name, byte[] LayoutData, Node[] AllNodes, bool bigendian = false) => new Layout()
        {
            MaxFrame = LayoutData.ReadUInt(0, 32, bigendian),
            NodeStartIndex = LayoutData.ReadUInt(4, 32, bigendian),
            NodeCount = (ushort)LayoutData.ReadUInt(8, 16, bigendian),
            Layer = (ushort)LayoutData.ReadUInt(0xA, 16, bigendian),
            X = (short)LayoutData.ReadUInt(0xC, 16, bigendian),
            Y = (short)LayoutData.ReadUInt(0xE, 16, bigendian),
            LayoutName = name,
            Nodes = AllNodes.Skip((int)LayoutData.ReadUInt(4, 32, bigendian)).Take((int)LayoutData.ReadUInt(8, 16, bigendian)).ToArray()
        };

        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)MaxFrame).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)NodeStartIndex).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt16)NodeCount).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt16)Layer).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)X).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)Y).Endianess(bigendian));

            return result.ToArray();
        }
    }
    public class Node
    {
        public uint MaxFrame { get; set; }
        public int SQ2BaseNumber { get; set; }
        public int SequenceIndex { get; set; }
        public byte AffectTranslation { get; set; }
        public byte AffectColor { get; set; }
        public byte Reservado1 { get; set; }
        public byte Reservado2 { get; set; }

        public uint FontInfo { get; set; }
        public uint FontInfoIndex { get; set; }

        public short ParentIndex { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short ID { get; set; }

        public uint Reservado3 { get; set; }

        public SQ2 LinkedSQ2(SQ2P[] sq2ps) => sq2ps[SQ2BaseNumber].SQ2Entry;

        public FontInfo LinkedFont(FontInfo[] AllFonts) => FontInfo == 1 ? AllFonts[FontInfoIndex] : null;

        public SQ2.Sequence LinkedSequence(SQ2P[] sq2ps) => LinkedSQ2(sq2ps).Sequences[SequenceIndex];

        internal static Node Read(byte[] NodeData, bool bigendian=false) => new Node()
        {
            MaxFrame = NodeData.ReadUInt(0, 32, bigendian),
            SQ2BaseNumber = (int)NodeData.ReadUInt(4, 16, bigendian),
            SequenceIndex = (int)NodeData.ReadUInt(6, 16, bigendian),
            AffectTranslation = NodeData[8],
            AffectColor = NodeData[9],
            Reservado1 = NodeData[0xA],
            Reservado2 = NodeData[0xB],
            FontInfo = NodeData.ReadUInt(0xC, 32, bigendian),
            FontInfoIndex = NodeData.ReadUInt(0x10, 32, bigendian),
            ParentIndex = (short)NodeData.ReadUInt(0x14, 16, bigendian),
            X = (short)NodeData.ReadUInt(0x16, 16, bigendian),
            Y = (short)NodeData.ReadUInt(0x18, 16, bigendian),
            ID = (short)NodeData.ReadUInt(0x1A, 16, bigendian),
            Reservado3 = NodeData.ReadUInt(0x1c, 32, bigendian)
        };

        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)MaxFrame).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt16)SQ2BaseNumber).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt16)SequenceIndex).Endianess(bigendian));
            result.Add(AffectTranslation);
            result.Add(AffectColor);
            result.Add(Reservado1);
            result.Add(Reservado2);
            result.AddRange(BitConverter.GetBytes((UInt32)FontInfo).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)FontInfoIndex).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)ParentIndex).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)X).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)Y).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)ID).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)Reservado3).Endianess(bigendian));
            return result.ToArray();
        }
    }


    private const uint Magic = 0x4032594C;//LY2@
    public string Version;
    private uint Reserved;

    public uint LayoutCount, LayoutOffset;
    public uint ControlCount, ControlOffset;
    public uint LayoutNodeCount, LayoutNodeOffset;
    public uint LayoutFontCount, LayoutFontOffset;

    public uint StringCount, StringOffset;
    public uint LayoutNameOffset, LayoutIDOffset;

    public FontInfo[] FontInfos;
    public Layout[] Layouts;
    public string[] LayoutNames;
    public byte[] LayoutIDs;

    byte[] GetLYNamesArray()
    {
        var result = new List<byte>();
        foreach(var str in LayoutNames)
        {
            result.AddRange(Encoding.Default.GetBytes(str));
            while (result.Count % 0x10 != 0)
                result.Add(0);
        }

        return result.ToArray();
    }
    internal static LY2 Read(Stream LY2Input, bool bigendian = false) => new LY2()
    {
        Version = LY2Input.ReadBytes(4, 4, bigendian).ConvertTo(Encoding.Default),
        Reserved = (uint)LY2Input.ReadULong(8, bigendian),
        LayoutCount = LY2Input.ReadUInt(0x10, 32, bigendian),
        LayoutOffset = LY2Input.ReadUInt(0x14, 32, bigendian),
        ControlCount = LY2Input.ReadUInt(0x18, 32, bigendian),
        ControlOffset = LY2Input.ReadUInt(0x1c, 32, bigendian),
        LayoutNodeCount = LY2Input.ReadUInt(0x20, 32, bigendian),
        LayoutNodeOffset = LY2Input.ReadUInt(0x24, 32, bigendian),
        LayoutFontCount = LY2Input.ReadUInt(0x28, 32, bigendian),
        LayoutFontOffset = LY2Input.ReadUInt(0x2c, 32, bigendian),
        StringCount = LY2Input.ReadUInt(0x30, 32, bigendian),
        StringOffset = LY2Input.ReadUInt(0x34, 32, bigendian),
        LayoutNameOffset = LY2Input.ReadUInt(0x38, 32, bigendian),
        LayoutIDOffset = LY2Input.ReadUInt(0x3c, 32, bigendian),

        LayoutNames = Enumerable.Range(0, (int)LY2Input.ReadUInt(0x10, 32, bigendian)).Select(x =>
        LY2Input.ReadBytes((int)LY2Input.ReadUInt(0x38, 32, bigendian) + (x * 0x10), 0x10).ConvertTo(Encoding.Default)).ToArray(),

        LayoutIDs = LY2Input.ReadBytes((int)LY2Input.ReadUInt(0x3c, 32, bigendian), (int)(LY2Input.Length - LY2Input.ReadUInt(0x3c, 32, bigendian))),

        FontInfos = Enumerable.Range(0, (int)LY2Input.ReadUInt(0x28, 32, bigendian)).Select(x =>
        FontInfo.Read(LY2Input.ReadBytes((int)(LY2Input.ReadUInt(0x2c, 32, bigendian) + (x * 0x10)), 0x10),
            (int)LY2Input.ReadUInt(0x30, 32, bigendian) > 0 ? LY2Input.ReadBytes((int)LY2Input.ReadUInt(0x34, 32, bigendian),
            (int)(LY2Input.ReadUInt(0x38, 32, bigendian) - LY2Input.ReadUInt(0x34, 32, bigendian))) : null
            , bigendian)).ToArray(),

        Layouts = Enumerable.Range(0, (int)LY2Input.ReadUInt(0x10, 32, bigendian)).Select(x =>
        Layout.Read(Enumerable.Range(0, (int)LY2Input.ReadUInt(0x10, 32, bigendian)).Select(y =>
        LY2Input.ReadBytes((int)LY2Input.ReadUInt(0x38, 32, bigendian) + (y * 0x10), 0x10).ConvertTo(Encoding.Default)).ToArray()[x],

            LY2Input.ReadBytes((int)(LY2Input.ReadUInt(0x14, 32, bigendian) + (x * 0x10)), 0x10),
            
            Enumerable.Range(0, /*(int)LY2Input.ReadUInt(0x20, 32, bigendian)*/(int)(LY2Input.ReadUInt(0x2c, 32, bigendian) - LY2Input.ReadUInt(0x24, 32, bigendian) / 0x20)).Select(n=> 
            Node.Read(LY2Input.ReadBytes((int)LY2Input.ReadUInt(0x24, 32, bigendian) + (n*0x20),0x20), bigendian)).ToArray()
            ,bigendian)).ToArray()
    };

    internal byte[] GetData(bool bigendian = false)
    {
        var result = new List<byte>();
        result.AddRange(BitConverter.GetBytes((UInt32)Magic));
        result.AddRange(Encoding.Default.GetBytes(Version).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt64)Reserved).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)LayoutCount).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)LayoutOffset).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)ControlCount).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)ControlOffset).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)LayoutNodeCount).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)LayoutNodeOffset).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)LayoutFontCount).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)LayoutFontOffset).Endianess(bigendian));

        result.AddRange(BitConverter.GetBytes((UInt32)StringCount).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)StringOffset).Endianess(bigendian));

        #region Font Build

        //FontInfos + Strings Array
        var stringArr = new List<byte>();

        var fontinf = new List<byte>();
        int indOffs = 0;
        foreach (var font in FontInfos)
        {
            stringArr.AddRange(font.String);
            stringArr.Add(0);

            font.StringIndex = (uint)indOffs;
            indOffs += font.String.Length + 1;
            fontinf.AddRange(font.GetData(bigendian));
        }

        while (stringArr.Count % 0x10 != 0)
            stringArr.Add(0);

        LayoutNameOffset = StringOffset + (uint)stringArr.Count;
        LayoutIDOffset = LayoutNameOffset + (LayoutCount * 0x10);
        #endregion


        result.AddRange(BitConverter.GetBytes((UInt32)LayoutNameOffset).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)LayoutIDOffset).Endianess(bigendian));
        while (result.Count < LayoutOffset)
            result.Add(0);

        //Layouts
        foreach (var layout in Layouts)
            result.AddRange(layout.GetData(bigendian));
        while (result.Count < ControlOffset)
            result.Add(0);

        //Nodes
        var nodes = new List<Node>();
        foreach (var layout in Layouts)
            nodes.AddRange(layout.Nodes);

        foreach (var node in nodes.Distinct())
            result.AddRange(node.GetData(bigendian));
        while (result.Count < LayoutFontOffset)
            result.Add(0);

        //Font infos
        result.AddRange(fontinf.ToArray());
        while (result.Count < StringOffset)
            result.Add(0);

        //Strings & IDS
        if (StringCount > 0)
            result.AddRange(stringArr.ToArray());

        result.AddRange(GetLYNamesArray());
        result.AddRange(LayoutIDs);

        return result.ToArray();
    }
}

public class SQ2P
{
    private const uint Magic = 0x50325153;//SQ2P
    private string Version;
    public byte[] Reserved;

    public uint SP2Offset, SQ2Offset, TM2Offset;
    public byte[] MarkData;//36 bytes

    public SP2 SP2Entry;
    public SQ2 SQ2Entry;
    public TM2 Texture;

    public int Index;

    public Mark _Mark;

    public enum Mark
    {
        Normal,
        Tm2DataAreaReduction
    };

    internal static SQ2P Read(byte[] L2Input, int index = -1, bool bigendian = false) => new SQ2P()
    {
        Index = index,

        Version = L2Input.ReadBytes(4, 4).ConvertTo(Encoding.Default),
        Reserved = L2Input.ReadBytes(8, 8),

        SP2Offset = L2Input.ReadUInt(0x10, 32, bigendian),
        SQ2Offset = L2Input.ReadUInt(0x14, 32, bigendian),
        TM2Offset = L2Input.ReadUInt(0x18, 32, bigendian),

        MarkData = L2Input.Skip(0x1c).TakeWhile(x=>x!=0).ToArray(),

        _Mark = L2Input.Skip(0x1c).TakeWhile(x => x != 0).ToArray().ConvertTo(Encoding.Default) == "Tm2DataAreaReductionMark" ? Mark.Tm2DataAreaReduction: Mark.Normal,

        SP2Entry = SP2.Read(new MemoryStream(L2Input.ReadBytes((int)L2Input.ReadUInt(0x10, 32, bigendian),
            (int)(L2Input.ReadUInt(0x14, 32, bigendian) - L2Input.ReadUInt(0x10, 32, bigendian)))), bigendian),

        SQ2Entry = SQ2.Read(new MemoryStream(L2Input.ReadBytes((int)L2Input.ReadUInt(0x14, 32, bigendian),
            (int)(L2Input.ReadUInt(0x18, 32, bigendian) - L2Input.ReadUInt(0x14, 32, bigendian)))), bigendian),

        Texture = TM2.Read(new MemoryStream(L2Input.ReadBytes((int)L2Input.ReadUInt(0x18, 32, bigendian),
            (int)L2Input.Length - (int)L2Input.ReadUInt(0x18, 32, bigendian))), L2Input.ReadBytes((int)L2Input.ReadUInt(0x18, 32, bigendian),
            (int)L2Input.Length - (int)L2Input.ReadUInt(0x18, 32, bigendian)), bigendian)
    };

    public void SetTIM2(byte[] tim2file, bool bigendian = false)
    {
        Texture = TM2.Read(new MemoryStream(tim2file), tim2file, bigendian);
    }
    internal byte[] GetData(bool bigendian = false)
    {
        var result = new List<byte>();
        result.AddRange(BitConverter.GetBytes((UInt32)Magic).Endianess(bigendian));
        result.AddRange(Encoding.Default.GetBytes(Version));
        result.AddRange(Reserved);

        byte[] SP2 = SP2Entry.GetData(bigendian);
        byte[] SQ2 = SQ2Entry.GetData(bigendian);

        //Offsets
        result.AddRange(BitConverter.GetBytes((UInt32)0x40).Endianess(bigendian));//SP2
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40 + SP2.Length)).Endianess(bigendian));//SQ2
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40 + SP2.Length + SQ2.Length)).Endianess(bigendian));//TIM2

        //MarkData
        if (_Mark == Mark.Normal)
            result.AddRange(new byte[36]);
        else
        {
            byte[] mark = new byte[36];
            Array.Copy(Encoding.Default.GetBytes("Tm2DataAreaReductionMark"), mark, 0x18);
            result.AddRange(mark);
        }

        //Datas
        result.AddRange(SP2);
        result.AddRange(SQ2);

        if(bigendian)
            result.AddRange(Texture.BETIM2Data);
        else
            result.AddRange(Texture.TIM2Data);

        return result.ToArray();
    }
}
public class SP2
{
    public struct Part
    {
        public short U0, V0, U1, V1;
        public Color[] RGBA;

        internal static Part Read(byte[] Part, bool bigendian=false) => new Part()
        {
            U0 = (short)Part.ReadUInt(0, 16, bigendian),
            V0 = (short)Part.ReadUInt(2, 16, bigendian),
            U1 = (short)Part.ReadUInt(4, 16, bigendian),
            V1 = (short)Part.ReadUInt(6, 16, bigendian),
            RGBA = new Color[4] { 
            Part.ReadUInt(8,32).FromUint(bigendian),
            Part.ReadUInt(0xC,32).FromUint(bigendian),
            Part.ReadUInt(0x10,32).FromUint(bigendian),
            Part.ReadUInt(0x14,32).FromUint(bigendian)
            }
        };

        internal Color GetColor(int index)
        {
            var color = Color.FromArgb(255,
                RGBA[index].R,
                RGBA[index].G,
                RGBA[index].B
                );
            return color;
        }

        internal void SetColor(int index, Color newColor)
        {
            var color = Color.FromArgb(0x80,
                newColor.R,
                newColor.G,
                newColor.B
                );

            RGBA[index] = color;
        }

        internal byte[] GetData(bool bigendian=false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((Int16)U0).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)V0).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)U1).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)V1).Endianess(bigendian));
            result.AddRange(RGBA[0].ColorToData(bigendian));
            result.AddRange(RGBA[1].ColorToData(bigendian));
            result.AddRange(RGBA[2].ColorToData(bigendian));
            result.AddRange(RGBA[3].ColorToData(bigendian));
            return result.ToArray();
        }
    }

    public struct Group
    {
        public short X0, Y0, X1, Y1;
        public ushort IndexParts;

        public Attribute Attr;//uint16 - ushort
        public Part GetPart(Part[] AllParts) => AllParts[IndexParts];
        public enum Attribute : ushort
        {
            DEFAULT = 0,
            ATTR_XYUV = 0x100,
            AATR_SCISSOR_ON = 0x200,
            ATTR_SCISSOR_OFF = 0x400
        };

        public int Index;
        internal static Group Read(byte[] group, int index = -1, bool bigendian = false) => new Group()
        {
            Index = index,
            X0 = (short)group.ReadUInt(0, 16, bigendian),
            Y0 = (short)group.ReadUInt(2, 16, bigendian),
            X1 = (short)group.ReadUInt(4, 16, bigendian),
            Y1 = (short)group.ReadUInt(6, 16, bigendian),
            IndexParts = (ushort)group.ReadUInt(8, 16, bigendian),
            Attr = (Attribute)((ushort)(group.ReadUInt(0xA, 16, bigendian))),
        };
        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((Int16)X0).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)Y0).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)X1).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)Y1).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt16)IndexParts).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt16)Attr).Endianess(bigendian));

            return result.ToArray();
        }
    }

    public struct Sprite
    {
        public ushort GroupCount, GroupsIndex;

        public Group[] Groups;

        public int Index;
        internal static Sprite Read(byte[] sprite, Group[] AllGroups, int index =-1, bool bigendian = false) => new Sprite()
        {
            Index = index,
            GroupCount = (ushort)sprite.ReadUInt(0, 16, bigendian),
            GroupsIndex = (ushort)sprite.ReadUInt(2, 16, bigendian),
            Groups = AllGroups.Skip((int)sprite.ReadUInt(2, 16, bigendian)).Take((int)sprite.ReadUInt(0, 16, bigendian)).ToArray()
        };
        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt16)GroupCount).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt16)GroupsIndex).Endianess(bigendian));
            return result.ToArray();
        }
    }

    private const uint Magic = 0x40325053;//SP2@
    private string Version;
    public byte[] Reserved;

    public int PartsCount, PartsOffset;
    public int GroupsCount, GroupsOffset;
    public int SpriteCount, SpriteOffset;

    public byte[] Reserved1;//24 bytes

    public Sprite[] Sprites;
    public Part[] Parts;

    internal static SP2 Read(Stream SP2Input, bool bigendian = false) => new SP2()
    {
        Version = SP2Input.ReadBytes(4, 4).ConvertTo(Encoding.Default),
        Reserved = SP2Input.ReadBytes(8, 8),

        PartsCount = (int)SP2Input.ReadUInt(0x10, 32, bigendian),
        PartsOffset = (int)SP2Input.ReadUInt(0x14, 32, bigendian),
        GroupsCount = (int)SP2Input.ReadUInt(0x18, 32, bigendian),
        GroupsOffset = (int)SP2Input.ReadUInt(0x1c, 32, bigendian),
        SpriteCount = (int)SP2Input.ReadUInt(0x20, 32, bigendian),
        SpriteOffset = (int)SP2Input.ReadUInt(0x24, 32, bigendian),

        Reserved1 = SP2Input.ReadBytes(0x28, 24),

        Parts = Enumerable.Range(0, (int)SP2Input.ReadUInt(0x10, 32, bigendian)).Select(p =>
        Part.Read(SP2Input.ReadBytes((int)SP2Input.ReadUInt(0x14, 32, bigendian) + (p * 0x18), 0x18), bigendian)
        ).ToArray(),

        Sprites = Enumerable.Range(0, (int)SP2Input.ReadUInt(0x20, 32, bigendian)).Select(x =>
        Sprite.Read(SP2Input.ReadBytes((int)(SP2Input.ReadUInt(0x24, 32, bigendian) + (x * 4)), 4),
            Enumerable.Range(0, (int)SP2Input.ReadUInt(0x18, 32, bigendian)).Select(g =>
        Group.Read(SP2Input.ReadBytes((int)SP2Input.ReadUInt(0x1c, 32, bigendian) + (g * 0xC), 0xC),
             g, bigendian)
        ).ToArray(), x, bigendian)).ToArray()
    };

    internal byte[] GetData(bool bigendian = false)
    {
        var result = new List<byte>();
        result.AddRange(BitConverter.GetBytes((UInt32)Magic).Endianess(bigendian));
        result.AddRange(Encoding.Default.GetBytes(Version));
        result.AddRange(Reserved);

        //Parts
        result.AddRange(BitConverter.GetBytes((UInt32)Parts.Count()).Endianess(bigendian));
        var PartsData = new List<byte>(); //Parts
        foreach (var _part in Parts)
            PartsData.AddRange(_part.GetData(bigendian));
        PartsData.Padd();
        result.AddRange(BitConverter.GetBytes((UInt32)0x40).Endianess(bigendian));//Offset

        //Sprites and Groups
        var SpritesData = new List<byte>();
        var GroupsData = new List<byte>();
        foreach (var _sprite in Sprites)
        {
            SpritesData.AddRange(_sprite.GetData(bigendian));

            foreach (var _group in _sprite.Groups)
            {
                GroupsData.AddRange(_group.GetData(bigendian));
            }
        }
        SpritesData.Padd();
        GroupsData.Padd();


        //Continue on Sps and Grps
        result.AddRange(BitConverter.GetBytes((UInt32)GroupsCount).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40+ PartsData.Count())).Endianess(bigendian));

        result.AddRange(BitConverter.GetBytes((UInt32)Sprites.Count()).Endianess(bigendian));
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40+ PartsData.Count() + GroupsData.Count())).Endianess(bigendian));

        result.AddRange(Reserved1);

        //Datas
        result.AddRange(PartsData.ToArray());
        result.AddRange(GroupsData.ToArray());
        result.AddRange(SpritesData.ToArray());

        

        return result.ToArray();
    }
}
public class SQ2
{
    public struct Sequence
    {
        public int ControlIndex;
        public short ControlNumber, LayerNumber;

        public string Name;
        public Control[] Controls;

        internal static Sequence Read(string name, byte[] seq, Control[] AllControls, bool bigendian = false) => new Sequence()
        {
            Name = name,
            ControlIndex = (int)seq.ReadUInt(0, 32, bigendian),
            ControlNumber = (short)seq.ReadUInt(4, 16, bigendian),
            LayerNumber = (short)seq.ReadUInt(6, 16, bigendian),
            Controls = AllControls.Skip((int)seq.ReadUInt(0, 32, bigendian)).Take((int)seq.ReadUInt(4, 16, bigendian)).ToArray()
        };

        internal SP2.Sprite[] GetSprites(SP2.Sprite[] AllSprites)
        {
            var sprites = new List<SP2.Sprite>();
            foreach (var control in Controls)
                foreach (var anim in control.Animations)
                    if(anim.SpriteNumber>=0)
                    sprites.Add(AllSprites[anim.SpriteNumber]);

            return sprites.ToArray();
        }
        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)ControlIndex).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)ControlNumber).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)LayerNumber).Endianess(bigendian));
            return result.ToArray();
        }
    }

    public struct Control
    {
        public int MaxFrame, ReturnFrame, AnimIndex;
        public short LoopNumber, AnimNumber;

        public Animation[] Animations;
        internal static Control Read(byte[] control, Animation[] AllAnim, bool bigendian = false) => new Control()
        {
            MaxFrame = (int)control.ReadUInt(0, 32, bigendian),
            ReturnFrame = (int)control.ReadUInt(4, 32, bigendian),
            AnimIndex = (int)control.ReadUInt(8, 32, bigendian),
            LoopNumber = (short)control.ReadUInt(0xC, 16, bigendian),
            AnimNumber = (short)control.ReadUInt(0xE, 16, bigendian),
            Animations = AllAnim.Skip((int)control.ReadUInt(8, 32, bigendian)).Take((int)control.ReadUInt(0xE, 16, bigendian)).ToArray()
        };
        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)MaxFrame).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)ReturnFrame).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)AnimIndex).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)LoopNumber).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)AnimNumber).Endianess(bigendian));
            return result.ToArray();
        }
    }

    public struct Animation
    {
        public int MaxFrameNumber;
        public short SpriteNumber, nOfsKeyData;

        public char[] KeyData;

        public Kind kind;
        public BlendType blend;
        public BitFlag[] flags;
        private byte Flags;

        public byte ScissorNumber, ZDepth;
        public enum Kind: byte
        {
            Parent = 0,
            Normal = 1,
            Font = 2,
            Max = 3
        };

        public enum BlendType: byte
        {
            Blend = 0,
            Add = 1,
            Sub = 2
        };

        public enum BitFlag//BitArray
        {
            Null,
            Dummy,
            DitherOff,
            Bilinear
        };

        internal static Animation Read(byte[] anim, bool bigendian = false) => new Animation()
        {
            MaxFrameNumber = (int)anim.ReadUInt(0, 32, bigendian),
            SpriteNumber = (short)anim.ReadUInt(4, 16, bigendian),
            nOfsKeyData = (short)anim.ReadUInt(6, 16, bigendian),

            KeyData = anim.ReadBytes(8, 11).ConvertTo(Encoding.Default).ToCharArray(),

            kind = (Kind)anim[0x13],

            blend = (BlendType)anim[0x14],

            Flags = anim[0x15],

            flags = new List<BitFlag>() {
                    (anim[0x15] & 1) == 1 ? BitFlag.Bilinear :
                    (anim[0x15] & 2) == 2 ? BitFlag.DitherOff :
                    (anim[0x15]& 0xfc) < 0xFC && (anim[0x15] & 0xfc) > 2 ? BitFlag.Dummy:
                    BitFlag.Null
                    }.ToArray(),

            ScissorNumber = anim[0x16],
            ZDepth = anim[0x17]
        };
        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)MaxFrameNumber).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)SpriteNumber).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)nOfsKeyData).Endianess(bigendian));
            result.AddRange(Encoding.Default.GetBytes(new String(KeyData)));
            result.Add((byte)kind);
            result.Add((byte)blend);

            result.Add((byte)Flags);//Not correctly serialized

            result.Add((byte)ScissorNumber);
            result.Add((byte)ZDepth);
            return result.ToArray();
        }
        public SP2.Sprite LinkedSprite(SP2.Sprite[] Sprites) => Sprites[SpriteNumber]; 
    }

    public struct Key
    {
        public Single key;
        public Value value;//uint32
        public byte[] Data;//4

        public enum Value : uint
        {
            DefColor = 0xFF808080,
            DefScaleX = 0x3F800000,
            DefScaleY = 0X3F800000,
            DefBaseY = 0,
            DefRotateY = 0,
            DefRotateX = 0,
            TypeLinear = 0,
            DefRotateZ = 0,
            KindStatus = 0,
            DefStatus = 0,
            DefBaseX = 0,
            DefOffsetX = 0,
            DefOffsetY = 0,
            KindBaseX = 1,
            KindBaseY = 2,
            KindOffsetX = 3,
            KindOffsetY = 4,
            KindRotateX = 5,
            KindRotateY = 6,
            KindRotateZ = 7,
            KindScaleX = 8,
            KindScaleY = 9,
            KindColor = 0xA,
            KindMax = 0xB,
            TypeSpline = 0x40,
            TypeSway = 0x80,
            TypePoint = 0xC0,
            TypeMax = 0xC1
        };

        internal static Key Read(byte[] keyData, bool bigendian = false) => new Key()
        {
            key = (Single)keyData.ReadUInt(0, 32, bigendian),
            value = (Value)keyData.ReadUInt(4, 32, bigendian),
            Data = keyData.ReadBytes(8, 4)
        };

        internal byte[] GetData(bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)key).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)value).Endianess(bigendian));
            result.AddRange(Data);
            return result.ToArray();
        }
    }

    private const uint Magic = 0x40325153;//SQ2@
    private string Version;
    public byte[] Reserved;

    public int SequenceCount, SequenceOffset;
    public int ControlCount, ControlOffset;
    public int AnimationCount, AnimationOffset;
    public int KeyCount, KeyOffset;

    public int SequenceNameOffset, SequenceIDOffset;

    public byte[] Reserved1;

    public Sequence[] Sequences;
    public Key[] Keys;

    public string[] SequenceNames;
    public byte[] SequenceIDs;

    internal static SQ2 Read(Stream SQ2Input, bool bigendian = false) => new SQ2()
    {
        Version = SQ2Input.ReadBytes(4, 4).ConvertTo(Encoding.Default),
        Reserved = SQ2Input.ReadBytes(8, 8),

        SequenceCount = (int)SQ2Input.ReadUInt(0x10, 32, bigendian),
        SequenceOffset = (int)SQ2Input.ReadUInt(0x14, 32, bigendian),
        ControlCount = (int)SQ2Input.ReadUInt(0x18, 32, bigendian),
        ControlOffset = (int)SQ2Input.ReadUInt(0x1c, 32, bigendian),
        AnimationCount = (int)SQ2Input.ReadUInt(0x20, 32, bigendian),
        AnimationOffset = (int)SQ2Input.ReadUInt(0x24, 32, bigendian),
        KeyCount = (int)SQ2Input.ReadUInt(0x28, 32, bigendian),
        KeyOffset = (int)SQ2Input.ReadUInt(0x2c, 32, bigendian),

        SequenceNameOffset = (int)SQ2Input.ReadUInt(0x30, 32, bigendian),
        SequenceIDOffset = (int)SQ2Input.ReadUInt(0x34, 32, bigendian),

        Reserved1 = SQ2Input.ReadBytes(0x38, 8),

        Sequences = Enumerable.Range(0, (int)SQ2Input.ReadUInt(0x10, 32, bigendian)).Select(x =>
        Sequence.Read(Enumerable.Range(0, (int)SQ2Input.ReadUInt(0x10, 32, bigendian)).Select(k =>
        
        SQ2Input.ReadBytes((int)SQ2Input.ReadUInt(0x30, 32, bigendian) + (k * 0x10), 0x10).
        ConvertTo(Encoding.Default)).ToArray()[x],
            
        SQ2Input.ReadBytes((int)SQ2Input.ReadUInt(0x14, 32, bigendian) + (x * 8), 8),

        Enumerable.Range(0, (int)SQ2Input.ReadUInt(0x18, 32, bigendian)).Select(c =>
        Control.Read(SQ2Input.ReadBytes((int)SQ2Input.ReadUInt(0x1c, 32, bigendian) + (c * 0x10), 0x10),

            Enumerable.Range(0, (int)SQ2Input.ReadUInt(0x20, 32, bigendian)).Select(a =>
            Animation.Read(SQ2Input.ReadBytes((int)SQ2Input.ReadUInt(0x24, 32, bigendian) + (a * 0x18), 0x18),bigendian)).ToArray(), bigendian)
            ).ToArray(), bigendian)
        ).ToArray(),

        Keys = Enumerable.Range(0, (int)(SQ2Input.ReadUInt(0x30, 32, bigendian) - SQ2Input.ReadUInt(0x2C, 32, bigendian)) / 0xC).Select(x =>
        Key.Read(SQ2Input.ReadBytes((int)SQ2Input.ReadUInt(0x2c, 32, bigendian) + (x * 0xC), 0xC), bigendian)
        ).ToArray(),

        SequenceNames = Enumerable.Range(0, (int)SQ2Input.ReadUInt(0x10, 32, bigendian)).Select(x =>
        SQ2Input.ReadBytes((int)SQ2Input.ReadUInt(0x30, 32, bigendian) + (x * 0x10), 0x10).ConvertTo(Encoding.Default)).ToArray(),

        SequenceIDs = SQ2Input.ReadBytes((int)SQ2Input.ReadUInt(0x34, 32, bigendian), (int)SQ2Input.Length - (int)SQ2Input.ReadUInt(0x34, 32, bigendian))
    };

    internal byte[] GetData(bool bigendian = false)
    {
        var result = new List<byte>();
        result.AddRange(BitConverter.GetBytes((UInt32)Magic).Endianess(bigendian));
        result.AddRange(Encoding.Default.GetBytes(Version));
        result.AddRange(Reserved);

        //Sequences, controls and Animations
        var SequencesData = new List<byte>(); //Sequences
        var ControlsData = new List<byte>(); //Controls
        var AnimsData = new List<byte>(); //Animations
        foreach (var _sequence in Sequences)
        {
            SequencesData.AddRange(_sequence.GetData(bigendian));
            foreach(var _control in _sequence.Controls)
            {
                ControlsData.AddRange(_control.GetData(bigendian));
                foreach(var _anim in _control.Animations)
                {
                    AnimsData.AddRange(_anim.GetData(bigendian));
                }
            }
        }

        //Keys
        var KeysData = new List<byte>();
        foreach (var _key in Keys)
            KeysData.AddRange(_key.GetData(bigendian));

        SequencesData.Padd();

        result.AddRange(BitConverter.GetBytes((UInt32)Sequences.Count()).Endianess(bigendian));//Sequences Count
        result.AddRange(BitConverter.GetBytes((UInt32)0x40).Endianess(bigendian));//Sequence Offset

        result.AddRange(BitConverter.GetBytes((UInt32)ControlsData.Count() / 0x10).Endianess(bigendian));//Controls Count
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40 + SequencesData.Count())).Endianess(bigendian));//Controls Offset
        ControlsData.Padd();

        result.AddRange(BitConverter.GetBytes((UInt32)AnimsData.Count() / 0x18).Endianess(bigendian));//Animations Count
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40 + SequencesData.Count() + ControlsData.Count())).Endianess(bigendian));//Animations Offset
        AnimsData.Padd();

        result.AddRange(BitConverter.GetBytes((UInt32)KeyCount).Endianess(bigendian));//Keys Count
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40 + SequencesData.Count() + ControlsData.Count() + AnimsData.Count())).Endianess(bigendian));//Keys Offset
        KeysData.Padd();


        //Sequence Name/ID
        result.AddRange(BitConverter.GetBytes((UInt32)(0x40 + SequencesData.Count() + ControlsData.Count() + AnimsData.Count() + KeysData.Count())).Endianess(bigendian));//Names Offset
        var NamesData = new List<byte>();
        foreach (var _name in SequenceNames)
            NamesData.AddRange(Encoding.Default.GetBytes(_name));

        result.AddRange(BitConverter.GetBytes((UInt32)(0x40 + SequencesData.Count() + ControlsData.Count() + AnimsData.Count() + KeysData.Count() + NamesData.Count())).Endianess(bigendian));//IDs Offset
        
        result.AddRange(Reserved1);

        //Datas
        result.AddRange(SequencesData.ToArray());
        result.AddRange(ControlsData.ToArray());
        result.AddRange(AnimsData.ToArray());
        result.AddRange(KeysData.ToArray());
        result.AddRange(NamesData.ToArray());
        result.AddRange(SequenceIDs);

        return result.ToArray();
    }
}
public class TM2
{
    public struct Header
    {
        private const uint MagicCode = 0x324D4954U;
        private const byte Version = 4;

        private byte Format;
        public short ImageCount;

        internal static Header Read(byte[] Header, bool bigendian=false) => new Header()
        {
            Format = Header[5],
            ImageCount = (short)Header.ReadUInt(6, 16, bigendian)
        };
        internal byte[] GetData(int alignment, bool bigendian = false)
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((UInt32)MagicCode));
            result.Add(Version);
            result.Add(Format);
            result.AddRange(BitConverter.GetBytes((Int16)ImageCount).Endianess(bigendian));

            //Alignment/Padding (16 or 128 bytes)
            while (result.Count % alignment != 0)
                result.Add(0);
            return result.ToArray();
        }

    }

    public struct Picture
    {
        private int TotalSize, ClutSize, ImageSize;
        private short HeaderSize;
        public short ClutColorCount;

        public byte PictureFormat, MipMapCount, ClutType;
        public ImageType imageType;
        public short Width, Height;

        private ulong GSTEX0, GSTEX1;
        private int GsRegs, GsClut;

        public byte[] Pixels, CLT;

        public enum ImageType : byte
        {
            PSMCT32 = 0,
            PSMCT24 = 1,
            PSMCT16 = 2,
            PSMCT8 = 5,
            PSMCT4 = 6
        };

        public int GetColorCount() => imageType == ImageType.PSMCT32 ? Width * Height : ClutColorCount;
        internal static Picture Read(byte[] PictureData, bool bigendian=false) => new Picture()
        {
            TotalSize = (int)PictureData.ReadUInt(0, 32, bigendian),
            ClutSize = (int)PictureData.ReadUInt(4, 32, bigendian),
            ImageSize = (int)PictureData.ReadUInt(8, 32, bigendian),

            HeaderSize = (short)PictureData.ReadUInt(0xC, 16, bigendian),
            ClutColorCount = (short)PictureData.ReadUInt(0xE, 16, bigendian),

            PictureFormat = PictureData[0x10],
            MipMapCount = PictureData[0x11],
            ClutType = PictureData[0x12],
            imageType = (ImageType)PictureData[0x13],

            Width = (short)PictureData.ReadUInt(0x14, 16, bigendian),
            Height = (short)PictureData.ReadUInt(0x16, 16, bigendian),

            GSTEX0 = PictureData.ReadULong(0x18, bigendian),
            GSTEX1 = PictureData.ReadULong(0x20, bigendian),

            GsRegs = (int)PictureData.ReadUInt(0x28, 32, bigendian),
            GsClut = (int)PictureData.ReadUInt(0x2C, 32, bigendian),

            Pixels = PictureData.ReadBytes((int)PictureData.ReadUInt(0xC, 16, bigendian),
                (int)PictureData.ReadUInt(8, 32, bigendian)),

            CLT = PictureData.ReadBytes((int)PictureData.ReadUInt(8, 32, bigendian) + (int)PictureData.ReadUInt(0xC, 16, bigendian),
                (int)PictureData.ReadUInt(4, 32, bigendian))
        };
        internal byte[] GetData(int alignment, bool bigendian = false)
        {
            var result = new List<byte>();

            while (HeaderSize % alignment != 0)
                HeaderSize++;
            TotalSize = HeaderSize + ImageSize + ClutSize;

            result.AddRange(BitConverter.GetBytes((UInt32)TotalSize).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)ClutSize).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)ImageSize).Endianess(bigendian));

            

            result.AddRange(BitConverter.GetBytes((Int16)HeaderSize).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)ClutColorCount).Endianess(bigendian));

            result.Add(PictureFormat);
            result.Add(MipMapCount);
            result.Add(ClutType);

            result.Add((byte)imageType);

            result.AddRange(BitConverter.GetBytes((Int16)Width).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((Int16)Height).Endianess(bigendian));

            result.AddRange(BitConverter.GetBytes((UInt64)GSTEX0).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt64)GSTEX1).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)GsRegs).Endianess(bigendian));
            result.AddRange(BitConverter.GetBytes((UInt32)GsClut).Endianess(bigendian));

            //Alignment/Padding (16 or 128 bytes)
            while (result.Count % alignment != 0)
                result.Add(0);

            //Pixels and CLT
            result.AddRange(Pixels);

            if(ClutSize!=0)
                result.AddRange(CLT);

            return result.ToArray();
        }
        #region BGR+I
        public static Color[] unswizzlePalette(Color[] palette)
        {
            if (palette.Length == 256)
            {
                Color[] unswizzled = new Color[palette.Length];

                int j = 0;
                for (int i = 0; i < 256; i += 32, j += 32)
                {
                    copy(unswizzled, i, palette, j, 8);
                    copy(unswizzled, i + 16, palette, j + 8, 8);
                    copy(unswizzled, i + 8, palette, j + 16, 8);
                    copy(unswizzled, i + 24, palette, j + 24, 8);
                }
                return unswizzled;
            }
            else
            {
                return palette;
            }
        }
        public static Color[] swizzlePalette(Color[] palette)
        {
            if (palette.Length == 256)
            {
                Color[] unswizzled = new Color[palette.Length];

                int j = 0;
                for (int i = 0; i < 256; i += 32, j += 32)
                {
                    copySW(palette, i, unswizzled, j, 8);
                    copySW(palette, i + 16, unswizzled, j + 8, 8);
                    copySW(palette, i + 8, unswizzled, j + 16, 8);
                    copySW(palette, i + 24, unswizzled, j + 24, 8);
                }
                return unswizzled;
            }
            else
            {
                return palette;
            }
        }
        private static void copy(Color[] unswizzled, int i, Color[] swizzled, int j, int num)
        {
            for (int x = 0; x < num; ++x)
            {
                unswizzled[i + x] = swizzled[j + x];
            }
        }
        private static void copySW(Color[] unswizzled, int i, Color[] swizzled, int j, int num)
        {
            for (int x = 0; x < num; ++x)
            {
                swizzled[j + x] = unswizzled[i + x];
            }
        }
        #endregion

        Color[] GetPalette(byte[] CLTData) => unswizzlePalette(Enumerable.Range(0, (int)(ClutSize / 4)).Select(x =>
        Color.FromArgb((int)CLTData[(x * 4) + 3], (int)CLTData[(x * 4)], (int)CLTData[(x * 4) + 1], (int)CLTData[(x * 4) + 2])).ToArray());


        public Image GetPNG()
        {
            if (ClutSize == 0) {
                byte[] pixs = new byte[Width * Height * 4]; 
                Pixels.CopyTo(pixs, 0);
                Pixels = pixs;
            }
            return ClutSize == 0 ? new ImageDecoderDirectColor(Pixels, Width, Height, Rainbow.ImgLib.Encoding.ColorCodec.CODEC_32BIT_RGBA).DecodeImage() :
            new ImageDecoderIndexed(Pixels, Width, Height, IndexCodec.FromNumberOfColors(ClutColorCount), GetPalette(CLT)).DecodeImage();
        }

    }

    public Header _header;
    public Picture[] _picture;
    public byte[] GetTIM2(int alignment = 16, bool bigendian=false)
    {
        var TIM2 = new List<byte>();

        //Header
        TIM2.AddRange(_header.GetData(alignment, bigendian));

        //Pictures
        foreach(var _pic in _picture)
            TIM2.AddRange(_pic.GetData(alignment, bigendian));

        return TIM2.ToArray();
    }
    public byte[] TIM2Data { get => GetTIM2(); }
    public byte[] BETIM2Data { get => GetTIM2(16,true); }
            
    internal static TM2 Read(Stream TM2, byte[] tm2data, bool bigendian =false) => new TM2()
    {
        _header = Header.Read(TM2.ReadBytes(0, 0x10), bigendian),
        _picture = Enumerable.Range(0, (int)Header.Read(TM2.ReadBytes(0, 0x10), bigendian).ImageCount).Select(x =>
        Picture.Read(new MemoryStream(TM2.ReadBytes(0x10, (int)TM2.Length - 0x10)).ReadBytesTM2(bigendian),bigendian)).ToArray()
    };
    
}

public class GTF
{
    public struct Header
    {
        public uint Version;
        public uint DDSBlockSize;
        public uint DDSCount;
        private uint Unk1;

        public uint HeaderSize;
        public uint FileSize;
        private uint Unk2;
        public uint RemapsUnk;

        //DDS Section
        public uint Width, Height;
        public uint ImgSizeDepth;//Unknow purpose...
        private byte[] UnkData;

        internal static Header Read(byte[] header, bool bigendian = true)=> new Header()
        {
            Version = header.ReadUInt(0,32,bigendian),
            DDSBlockSize = header.ReadUInt(4,32,bigendian),
            DDSCount = header.ReadUInt(8,32,bigendian),
            Unk1 = header.ReadUInt(0xC,32,bigendian),

            HeaderSize = header.ReadUInt(0x10,32,bigendian),
            FileSize = header.ReadUInt(0x14,32,bigendian),
            Unk2 = header.ReadUInt(0x18,32,bigendian),
            RemapsUnk = header.ReadUInt(0x1C,32,bigendian),

            Width = header.ReadUInt(0x20, 16, bigendian),
            Height = header.ReadUInt(0x22, 16, bigendian),
            ImgSizeDepth = header.ReadUInt(0x24, 16, bigendian),
            UnkData = header.ReadBytes(0x26, 0x5a)
        };
    }

    public struct DDS
    {
        public uint Width, Height;
        public byte[] DXTData;

        public Image GetPNG()
        {
            Image outResult = null;
            byte[] outresult = new byte[(Width * Height) *4];
            DxtDecoder.DecompressDXT5(DXTData, (int)Width, (int)Height, outresult);
            outResult = new ImageDecoderDirectColor(outresult, (int)Width, (int)Height, Rainbow.ImgLib.Encoding.ColorCodec.CODEC_32BIT_BGRA).DecodeImage();
            return outResult;
        }
        internal static DDS Read(Stream DDSData, Header header, bool bigendian = true)=>new DDS()
        {
            DXTData = DDSData.ReadBytes((int)header.HeaderSize, (int)header.DDSBlockSize),
            Width = header.Width,
            Height = header.Height
        };
    }

    public Header _header;
    public DDS _dds;

    internal static GTF Read(Stream GTF, bool bigendian = true) => new GTF()
    {
        _header = Header.Read(GTF.ReadBytes(0, (int)GTF.ReadUInt(0x10, 32, bigendian)),bigendian),
        _dds = DDS.Read(GTF, 
            Header.Read(GTF.ReadBytes(0, (int)GTF.ReadUInt(0x10, 32, bigendian)), bigendian), 
            bigendian)
    };
}



using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using JRPC_Client;



namespace IW6_MP
{
    using BYTES;

    public struct Vec3
    {
        public float x;
        public float y;
        public float z;

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class GAME
    {
        public GAME(JRPC XBOX)
        {
            Console = XBOX;
        }
        public GAME()
        {
            //Maybe do shit with this later
        }
        private static JRPC Console;
        public bool FPS 
        {
            get { return (Console.Memory.ReadUInt32(0x8251CD1C) == 0x60000000); } 
            set { Console.Memory.WriteUInt32(0x8251CD1C, (uint)(value ? 0x60000000 : 0x419A008C)); } 
        }
        public string Version
        {
            get
            {
                int temp = Console.CallByte(0x82437E30, new object[1]);
                if (temp != 0)
                {
                    uint major = Console.CallUInt(0x82437D20, new object[] { temp });
                    return Console.Memory.ReadString(0x82037BB0, 4) + Console.CallUInt(0x824277E8, new object[] { major }) + (Convert.ToUInt16(major) & 0xFFFF).ToString();
                }
                else
                {
                    return Console.Memory.ReadString(0x82037BB0, 4) + Console.CallUInt(0x824277E8, new object[] { temp });
                }
            }
            set
            {
                bool temp = Convert.ToBoolean(Console.CallByte(0x82437E30, new object[1]));
                char[] backup = (value).ToCharArray();
                Array.Resize<char>(ref backup, 8);
                Console.Memory.WriteString(temp ? 0x82049460 : 0x82049458, temp ? value + "\0" : new string(backup) + "\0");
            }

        }
        public bool UnlimitedAmmo 
        {
            get { return (Console.Memory.ReadUInt32(0x82235BC0) == 0x60000000); }
            set { Console.Memory.WriteUInt32(0x82235BC0, (uint)(value ? 0x60000000 : 0x7D4B182E)); } 
        }
        public bool FullAuto
        {
            get { return (!Console.Memory.ReadBool(0x82230EF7)); }
            set { FullAuto = value; Console.Memory.WriteSByte(0x82230EF7, value ? (sbyte)0 : (sbyte)1); }
        }
        public bool FallDamage 
        {
            get { return (Console.Memory.ReadUInt16(0x8221E824) == 0x4800); }
            set { Console.Memory.WriteUInt16(0x8221E824, (ushort)(value ? 0x4198 : 0x4800)); } 
        }
        public float JumpHeight
        {
            get { return Console.Memory.ReadFloat(0x820151F8); }
            set
            {
                Console.Memory.WriteFloat(0x820151F8, JumpHeight);
            }
        }

        public void Cbuf_AddText(int Client, string text)
        {
            if (Client == -1)
                for (int i = 0; i < 4; i++)
                    Console.CallVoid(0x82433528, new object[] { i, text });
            else
                Console.CallVoid(0x82433528, new object[] { Client, text });
        }
        public void SV_GameSendServerCommand(int Client, int svscmd_type, string text)
        {
            Console.CallVoid(0x824B8C08, new object[] { Client, svscmd_type, text });
        }

        public class dvar_s
        {
            public UInt32 Address;
            byte[] Data;

            public string name;

            public string StringValue;

            uint EnumCount;
            string[] StringEnum;

            public dvar_s(UInt32 address, bool load)
            {
                Address = address;
                Data = new byte[0x48];
                if (load)
                    Load();
            }
            public dvar_s(string Name, string value)
            {
                name = Name;
                StringValue = value;
            }

            public void Load()
            {
                uint x;
                Console.Memory.xbConsole.DebugTarget.GetMemory(Address, 0x48, Data, out x);
                name = Console.Memory.ReadString(Data.GetUInt32(0), 32);

                if (Data[8] == 7)
                {
                    StringValue = Console.Memory.ReadString(Data.GetUInt32(0xC), 32);
                }
                else if (Data[8] == 6)//Not sure if this works, not sure how limit works which is related to this
                {
                    EnumCount = Data.GetUInt32(0x3C);
                    StringEnum = new string[EnumCount];
                    for (int i = 0; i < EnumCount; i++)
                        StringEnum[i] = Console.Memory.ReadString((uint)(Data.GetUInt32(0x40) + (i * 4)), 32);
                }
            }

            Object GetValue()
            {
                switch (Data[8])
                {
                    case 0: return (Data[0xC] > 0);
                    case 1: return Data.GetSingle(0xC);
                    case 2: goto VECTOR;
                    case 3: goto VECTOR;
                    case 4: goto VECTOR;
                    VECTOR:
                        Single[] vec;
                        vec = new Single[Data[8]];
                        for (int i = 0; i < vec.Length; i++)
                            vec[i] = Data.GetSingle(0xC + (i * 4));
                        return vec;
                    case 5: return Data.GetUInt32(0xC);
                    case 6: return Data.GetUInt32(0xC);
                    case 8: return Data.GetColor(0xC);
                    case 7: 
                        if(StringValue == "")
                            StringValue = Console.Memory.ReadString(Data.GetUInt32(0xC), 32);
                        return StringValue;
                }
                return null;
            }

            void SetValue(Object value)
            {
                switch (Data[8])
                {
                    case 0: 
                        Data[0xC] = (byte)value;
                        Console.Memory.WriteBool(Address + 0xC, (bool)value);
                        break;
                    case 1:
                        Data.SetSingle((float)value, 0xC);
                        Console.Memory.WriteFloat(Address + 0xC, (float)value);
                        break;
                    case 2: goto VECTOR;
                    case 3: goto VECTOR;
                    case 4: 
                        goto VECTOR;
                    VECTOR:
                        for (int i = 0; i < Data[8]; i++)
                        {
                            Data.SetSingle(((Single[])value)[i], 0xC + (i * 4));
                            Console.Memory.WriteFloat((uint)(Address + 0xC + (i * 4)), ((Single[])value)[i]);
                        }
                    break;
                    case 5:
                        Data.SetUInt32((uint)value, 0xC);
                        Console.Memory.WriteUInt32(Address + 0xC, (uint)value);
                        break;
                    case 6:
                        Data.SetUInt32((uint)value, 0xC);
                        Console.Memory.WriteUInt32(Address + 0xC, (uint)value);
                        break;
                    case 8:
                        Data.SetColor((Color)value, 0xC);
                        Console.Memory.WriteUInt32(Address + 0xC, (uint)((Color)value).ToArgb());
                        break;
                    case 7:
                        StringValue = (string)value;
                        Console.Memory.WriteString(Data.GetUInt32(0xC), StringValue);
                        break;
                }
            }

            public uint Flags
            {
                get { return Data.GetUInt32(4); }
                set
                {
                    Data.SetUInt32(value, 4);
                    Console.Memory.WriteUInt32(Address + 4, value);
                }
            }

            public byte Type
            {
                get { return Data[8]; }
            }

            public bool Modified
            {
                get { return Data[9] > 0; }
                set
                {
                    Data[9] = value ? (byte)1 : (byte)0;
                    Console.Memory.WriteBool(Address + 9, value);
                }
            }

            public Object Value
            {
                get { return GetValue(); }
                set { SetValue(value); }

            }

        }

        public class pState_s
        {
            public uint getPlayerState(int Client)
            {
                return (uint)(0x8328A600 + (Client * 0x3600));
            }
            public uint getGentity(int Client)
            {
                return (uint)(0x832F1A00 + (Client * 0x280));
            }

            public string getClantag(int Client, bool fmt)
            {
                if (Console.Memory.ReadByte(getPlayerState(Client) + 0x300C) != 0)
                    return (fmt) ? "[" + Console.Memory.ReadString(getPlayerState(Client) + 0x300C, 6) + "]" : Console.Memory.ReadString(getPlayerState(Client) + 0x300C, 6);
                return "";
            }
            public string getGamertag(int Client)
            {
                if (Console.Memory.ReadByte(getPlayerState(Client) + 0x2F9C) != 0)
                    return Console.Memory.ReadString(getPlayerState(Client) + 0x2F9C, 32);
                return "";
            }
            public string getCleanGamertag(int Client)
            {
                if (Console.Memory.ReadByte(getPlayerState(Client) + 0x2F1C) != 0)
                    return Console.Memory.ReadString(getPlayerState(Client) + 0x2F1C, 32);
                return "";
            }
            public string getFormatedGamertag(int Client)
            {
                return getClantag(Client, true) + getCleanGamertag(Client);
            }

            public void writeClantag(int Client, string Clantag)
            {
                Console.Memory.WriteString(getPlayerState(Client) + 0x300C, Clantag + "\0");
            }
            public void writeGamertag(int Client, string Gamertag)
            {
                Console.Memory.WriteString(getPlayerState(Client) + 0x2F9C, Gamertag + "\0");
            }

            public Vec3 getOrigin(int Client)
            {
                return new Vec3(Console.Memory.ReadFloat(getPlayerState(Client) + 0x1C), Console.Memory.ReadFloat(getPlayerState(Client) + 0x20), Console.Memory.ReadFloat(getPlayerState(Client) + 0x24));
            }
            public void setOrigin(int Client, Vec3 Origin)
            {
                Console.Memory.WriteFloat(getPlayerState(Client) + 0x1C, Origin.x);
                Console.Memory.WriteFloat(getPlayerState(Client) + 0x20, Origin.y);
                Console.Memory.WriteFloat(getPlayerState(Client) + 0x24, Origin.z);
            }

            public void setSpread(int Client, bool Enabled)
            {
                if (Enabled)
                    Console.Memory.WriteUInt32(getPlayerState(Client) + 0x434, 1);
                else
                {
                    Console.Memory.WriteInt32(getPlayerState(Client) + 0x430, 0);
                    Console.Memory.WriteUInt32(getPlayerState(Client) + 0x434, 2);
                }
            }
            public void setSpread(int Client, uint Spread)
            {
                Console.Memory.WriteUInt32(getPlayerState(Client) + 0x430, Spread);
                Console.Memory.WriteUInt32(getPlayerState(Client) + 0x434, 2);
            }

            public void setSpeed(int Client, float Speed)
            {
                Console.Memory.WriteFloat(getPlayerState(Client) + 0x2F50, Speed);
            }

            public void Visibility(int Client, bool Enabled)
            {
                Console.Memory.WriteUInt32(getGentity(Client) + 0x11C, 0);
                if (Enabled)
                    Console.Memory.AND_Int32(getPlayerState(Client) + 0x124, ~0x20);
                else
                    Console.Memory.OR_UInt32(getPlayerState(Client) + 0x124, 0x20);
            }

        }

    }
}

namespace BYTES
{
    public static class ByteArrayExtensions
    {
        public static Color GetColor(this Byte[] buffer, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = new Byte[4];
            Array.Copy(buffer, pos, copybuffer, 0, 4);

            return Color.FromArgb(
                copybuffer[3],
                copybuffer[0],
                copybuffer[1],
                copybuffer[2]);
        }

        public static void SetColor(this Byte[] buffer, Color value, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = new Byte[4];
            copybuffer[3] = value.A;
            copybuffer[0] = value.R;
            copybuffer[1] = value.G;
            copybuffer[2] = value.B;

            Array.Copy(copybuffer, 0, buffer, pos, 4);
        }

        public static UInt16 GetUInt16(this Byte[] buffer, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = new Byte[2];
            Array.Copy(buffer, pos, copybuffer, 0, 2);
            Array.Reverse(copybuffer);

            return BitConverter.ToUInt16(copybuffer, 0);
        }

        public static void SetUInt16(this Byte[] buffer, UInt16 value, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = BitConverter.GetBytes(value);
            Array.Reverse(copybuffer);
            Array.Copy(copybuffer, 0, buffer, pos, 2);
        }

        public static Int32 GetInt32(this Byte[] buffer, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = new Byte[4];
            Array.Copy(buffer, pos, copybuffer, 0, 4);
            Array.Reverse(copybuffer);

            return BitConverter.ToInt32(copybuffer, 0);
        }

        public static void SetInt32(this Byte[] buffer, Int32 value, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = BitConverter.GetBytes(value);
            Array.Reverse(copybuffer);
            Array.Copy(copybuffer, 0, buffer, pos, 4);
        }

        public static UInt32 GetUInt32(this Byte[] buffer, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = new Byte[4];
            Array.Copy(buffer, pos, copybuffer, 0, 4);
            Array.Reverse(copybuffer);

            return BitConverter.ToUInt32(copybuffer, 0);
        }

        public static void SetUInt32(this Byte[] buffer, UInt32 value, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = BitConverter.GetBytes(value);
            Array.Reverse(copybuffer);
            Array.Copy(copybuffer, 0, buffer, pos, 4);
        }

        public static Single GetSingle(this Byte[] buffer, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = new Byte[4];
            Array.Copy(buffer, pos, copybuffer, 0, 4);
            Array.Reverse(copybuffer);

            return BitConverter.ToSingle(copybuffer, 0);
        }

        public static void SetSingle(this Byte[] buffer, Single value, Int32 pos)
        {
            Byte[] copybuffer;

            copybuffer = BitConverter.GetBytes(value);
            Array.Reverse(copybuffer);
            Array.Copy(copybuffer, 0, buffer, pos, 4);
        }
    }
}

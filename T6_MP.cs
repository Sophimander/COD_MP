using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JRPC_Client;

namespace T6_MP
{

    public class xBox
    {
        public void SetLED(int tLeft, int tRight, int bLeft, int bRight, JRPC Console)//Never tested but should work just find and needs to be cleaned up
        {
            byte[] m_SMCMessage = new byte[16];
            byte[] m_SMCReturn = new byte[16];

            m_SMCMessage[0] = 0x99;
            m_SMCMessage[1] = 0x1;
            m_SMCMessage[2] = (byte)((tLeft >> 3) | (tRight >> 2) | (bLeft >> 1) | bRight);
            Console.CallVoid(Console.ResolveFunction("xboxkrnl.exe", 0x29), new object[] { m_SMCMessage, m_SMCReturn });
        }
    }

    public class gameFunctions : gEnt
    {
        public void setMemory(uint Offset, byte[] Data, JRPC Console)
        {
            uint x;
            Console.Memory.xbConsole.DebugTarget.SetMemory(Offset, (uint)Data.Length, Data, out x);
        }

        public byte[] getMemory(uint Offset, uint size, JRPC Console)
        {
            uint x;
            byte[] Data = new byte[size];
            Console.Memory.xbConsole.DebugTarget.GetMemory(Offset, size, Data, out x);
            return Data;
        }

        public enum sessionState_t : int
        {
            SESS_STATE_PLAYING,
            SESS_STATE_DEAD,
            SESS_STATE_SPECTATOR,
            SESS_STATE_INTERMISSION
        };

        public enum connectionState_t : int
        {
            CON_DISCONNECTED,
            CON_CONNECTING,
            CON_CONNECTED
        };

        public enum svscmd_type : int
        {
            SV_CMD_CAN_IGNORE,
            SV_CMD_RELIABLE
        };

        public enum Button_Flags : uint
        {
            None = 0,
            Crouched = 0x400000,
            Prone = 0x800000,
            Right_Stick = 0x40040000,
            Left_Stick = 0x20000000,
            Right_Bumper = 0x10000,
            Left_Bumper = 0x20000,
            Right_Trigger = 0x100080,
            Left_Trigger = 0x80000000,
            Y = 0x8,
            B = 0x8000,
            A = 0x200000,
            X = 0x4000000
        };//820E5312 forgot what this is

        public enum HudOverlay : int
        {
            Toggles_Infrared = 0xA00,
            Vision_Infrared = 0x1000
        };

        public enum ActionSlotType : int
        {
            none,
            weapon,
            altmode,
            nightvision
        };

        public bool IsInGame(JRPC Console)
        {
            return Console.Memory.ReadBool(0x83C43B00 + 0x18);
        }

        public void ErrorMessage(string Message, JRPC Console)
        {
            Console.Memory.WriteString(0x8207920C, Message + "\0");
            Console.CallVoid(0x825CF530, new object[1]);
            Console.Memory.WriteString(0x8207920C, "No Error\0");
        }

        public uint getCamoIndexForName(string Camo)
        {
            switch (Camo)
            {
                case "None": return 0;
                case "Dev Gru": return 1;
                case "A-Tacs AU": return 2;
                case "ERDL": return 3;
                case "Siberia": return 4;
                case "Choco": return 5;
                case "Blue Tiger": return 6;
                case "Bloodshot": return 7;
                case "Ghostex": return 8;
                case "Kryptek": return 9;
                case "Carbon Fiber": return 10;
                case "Cherry Blossom": return 11;
                case "Art of War": return 12;
                case "Ronin": return 13;
                case "Skulls": return 14;
                case "Gold": return 15;
                case "Diamond": return 16;
                case "Elite": return 17;
                case "CE Digital": return 18;
                case "Jungle Warfare": return 19;
                case "Rainbow": return 20;
                case "Benjamins": return 21;
                case "Dia de Muertos": return 22;
                case "Graffiti": return 23;
                case "Kawaii": return 24;
                case "Party Rock": return 25;
                case "Zombies": return 26;
                case "Viper": return 27;
                case "Bacon": return 28;
                case "Ghost": return 29;
                case "Paladin": return 30;
                case "Comics": return 31;
                case "Dragon": return 32;
                case "Cyborg": return 33;
                case "Aqua": return 34;
                case "Breach": return 35;
                case "Coyote": return 36;
                case "Glam": return 37;
                case "Rogue": return 38;
                case "Pack-a-punch": return 39;
            }
            return 0;
        }

        public string getCamoNameForIndex(int Camo)
        {
            switch (Camo)
            {
                case 0: return "None";
                case 1: return "Dev Gru";
                case 2: return "A-Tacs AU";
                case 3: return "ERDL";
                case 4: return "Siberia";
                case 5: return "Choco";
                case 6: return "Blue Tiger";
                case 7: return "Bloodshot";
                case 8: return "Ghostex";
                case 9: return "Kryptek";
                case 10: return "Carbon Fiber";
                case 11: return "Cherry Blossom";
                case 12: return "Art of War";
                case 13: return "Ronin";
                case 14: return "Skulls";
                case 15: return "Gold";
                case 16: return "Diamond";
                case 17: return "Elite";
                case 18: return "CE Digital";
                case 19: return "Jungle Warfare";
                case 20: return "Rainbow";
                case 21: return "Benjamins";
                case 22: return "Dia de Muertos";
                case 23: return "Graffiti";
                case 24: return "Kawaii";
                case 25: return "Party Rock";
                case 26: return "Zombies";
                case 27: return "Viper";
                case 28: return "Bacon";
                case 29: return "Ghost";
                case 30: return "Paladin";
                case 31: return "Comics";
                case 32: return "Dragon";
                case 33: return "Cyborg";
                case 34: return "Aqua";
                case 35: return "Breach";
                case 36: return "Coyote";
                case 37: return "Glam";
                case 38: return "Rogue";
                case 39: return "Pack-a-punch";
            }
            return "None";
        }

        public void SV_GameSendServerCommand(int pIndex, svscmd_type type, string text, JRPC Console)
        {
            Console.CallVoid(0x8242FAE0, pIndex, (int)type, text);
        }

        public void Cbuf_AddText(int localIndex, string text, JRPC Console)
        {
            Console.CallVoid(0x82401518, localIndex, text);
        }

        public void QuickNop(uint Offset, JRPC Console)
        {
            Console.Memory.WriteUInt32(Offset, 0x60000000);
        }

        public class HudElems
        {
            private uint HudElem_Alloc = 0x8232A570;
            private uint G_MaterialIndex = 0x82388FE0;
            private uint G_LocalizeStringIndex = 0x82388DC0;
            private uint Level_Time = 0x83540A98;

            public enum align : byte
            {
                TOP_RIGHT = 0xA8,
                TOPLEFT = 0x88,
                TOP_CENTERED = 0x78,
                BOTTOM_RIGHT = 0xAA,
                BOTTOM_LEFT = 0x8A,
                BOTTOM_CENTER = 0x9A,
                CENTER_MIDDLE = 0x92,
                CENTER_LEFT = 0x82,
                CENTER_RIGHT = 0xA2
            };

/*typedef struct{
	float X; //0x0000
	float Y; //0x0004
	float Z; //0x0008
	float fontScale; //0x000C
	float fromFontScale; //0x0010
	__int32 fontScaleStartTime; //0x0014
	hud_color color; //0x0018
	hud_color fromColor; //0x001C
	__int32 fadeStartTime; //0x0020
	__int32 scaleStartTime; //0x0024
	float fromX; //0x0028
	float fromY; //0x002C
	__int32 moveStartTime; //0x0030
	__int32 time; //0x0034
	__int32 duration; //0x0038
	__int32 value; //0x003C
	float sort; //0x0040
	hud_color glowColor; //0x0044
	__int32 fxBirthTime; //0x0048
	__int32 flags; //0x004C
	__int16 targetEntNum; //0x0050
	__int16 fontScaleTime; //0x0052
	__int16 fadeTime; //0x0054
	__int16 label; //0x0056
	__int16 Width; //0x0058
	__int16 Height; //0x005A
	__int16 fromWidth; //0x005C
	__int16 fromHeight; //0x005E
	__int16 scaleTime; //0x0060
	__int16 moveTime; //0x0062
	__int16 text; //0x0064
	__int16 fxLetterTime; //0x0066
	__int16 fxDecayStartTime; //0x0068
	__int16 fxDecayDuration; //0x006A
	__int16 fxRedactDecayStartTime; //0x006C
	__int16 fxRedactDecayDuration; //0x006E
	BYTE Type; //0x0070
	BYTE textStyle; //0x0071
	BYTE AlignOrg; //0x0072
	BYTE AlignScreen; //0x0073
	BYTE MaterialIndex; //0x0074
	BYTE OffscreenMaterialIndex; //0x0075
	BYTE fromAlignOrg; //0x0076
	BYTE fromAlignScreen; //0x0077
	BYTE soundID; //0x0078
	BYTE ui3dWindow; //0x0079
	__int16 flags2; //0x007A
	__int32 Client; //0x007C
	__int32 team; //0x0080
	__int32 abilityFlag; //0x0084
} game_hudelem_s; //Size = 0x88*/

            public uint Alloc_HudElem(JRPC Console)
            {
                return Console.CallUInt(HudElem_Alloc, new object[] { 0 });
            }

            public uint getMaterialIndex(string Material, JRPC Console)
            {
                return Console.CallUInt(G_MaterialIndex, new object[] { Material });
            }

            public ushort getStringIndex(string Text, JRPC Console)
            {
                return (ushort)Console.CallUInt(G_LocalizeStringIndex, new object[] { Text });
            }

            public uint SpawnTextElem(uint Elem, uint PlayerIndex, string Text, float x, float y, sbyte font, float fontScale, byte[] Color, byte[] glowColor, byte AlignOrg, byte AlignScreen, float sort, JRPC Console)
            {
                Console.Memory.WriteUInt32(Elem + 0x7C, PlayerIndex);
                Console.Memory.WriteSByte(Elem + 0x70, 1);
                Console.Memory.WriteSByte(Elem + 0x71, font);
                Console.Memory.WriteByte(Elem + 0x72, AlignOrg);
                Console.Memory.WriteByte(Elem + 0x73, AlignScreen);
                Console.Memory.WriteFloat(Elem + 0x3C, sort);
                Console.Memory.WriteFloat(Elem, x);
                Console.Memory.WriteFloat(Elem + 4, y);
                Console.Memory.WriteFloat(Elem + 0x10, fontScale);
                new gameFunctions().setMemory(Elem + 0x18, Color, Console);
                new gameFunctions().setMemory(Elem + 0x44, glowColor, Console);
                Console.Memory.WriteUInt16(Elem + 0x64, getStringIndex(Text, Console));
                return Elem;
            }

            public uint SpawnTextElem(uint PlayerIndex, string Text, float x, float y, sbyte font, float fontScale, byte[] Color, byte[] glowColor, byte AlignOrg, byte AlignScreen, float sort, JRPC Console)
            {
                uint Elem = Console.CallUInt(HudElem_Alloc, new object[] { PlayerIndex });
                //Console.WriteUInt32(Elem + 0x7C, PlayerIndex);
                Console.Memory.WriteSByte(Elem + 0x70, 1);
                Console.Memory.WriteSByte(Elem + 0x71, font);
                Console.Memory.WriteByte(Elem + 0x72, AlignOrg);
                Console.Memory.WriteByte(Elem + 0x73, AlignScreen);
                Console.Memory.WriteFloat(Elem + 0x3C, sort);
                Console.Memory.WriteFloat(Elem, x);
                Console.Memory.WriteFloat(Elem + 4, y);
                Console.Memory.WriteUInt32(Elem + 0x10, (uint)fontScale);
                new gameFunctions().setMemory(Elem + 0x18, Color, Console);
                new gameFunctions().setMemory(Elem + 0x44, glowColor, Console);
                Console.Memory.WriteUInt16(Elem + 0x64, getStringIndex(Text, Console));
                return Elem;
            }

            public uint SpawnShaderElem(uint Elem, uint PlayerIndex, string Material, float x, float y, ushort Width, ushort Height, byte[] Color, byte AlignOrg, byte AlignScreen, float sort, JRPC Console)
            {
                Console.Memory.WriteUInt32(Elem + 0x7C, PlayerIndex);
                Console.Memory.WriteSByte(Elem + 0x70, 8);
                Console.Memory.WriteByte(Elem + 0x74, (byte)getMaterialIndex(Material, Console));
                Console.Memory.WriteByte(Elem + 0x72, AlignOrg);
                Console.Memory.WriteByte(Elem + 0x73, AlignScreen);
                Console.Memory.WriteFloat(Elem + 0x3C, sort);
                Console.Memory.WriteFloat(Elem, x);
                Console.Memory.WriteFloat(Elem + 4, y);
                new gameFunctions().setMemory(Elem + 0x18, Color, Console);
                Console.Memory.WriteUInt16(Elem + 0x58, Width);
                Console.Memory.WriteUInt16(Elem + 0x5A, Height);
                return Elem;
            }

            public uint SpawnShaderElem(uint PlayerIndex, string Material, float x, float y, ushort Width, ushort Height, byte[] Color, byte AlignOrg, byte AlignScreen, float sort, JRPC Console)
            {
                uint Elem = Console.CallUInt(HudElem_Alloc, new object[] { PlayerIndex });
                Console.Memory.WriteSByte(Elem + 0x70, 8);
                Console.Memory.WriteByte(Elem + 0x74, (byte)getMaterialIndex(Material, Console));
                Console.Memory.WriteByte(Elem + 0x72, AlignOrg);
                Console.Memory.WriteByte(Elem + 0x73, AlignScreen);
                Console.Memory.WriteFloat(Elem + 0x3C, sort);
                Console.Memory.WriteFloat(Elem, x);
                Console.Memory.WriteFloat(Elem + 4, y);
                new gameFunctions().setMemory(Elem + 0x18, Color, Console);
                Console.Memory.WriteUInt16(Elem + 0x58, Width);
                Console.Memory.WriteUInt16(Elem + 0x5A, Height);
                return Elem;
            }

            public uint MoveOverTime(uint Elem, ushort Time, float X, float Y, JRPC Console)
            {
                Console.Memory.WriteSByte(Elem + 0x76, Console.Memory.ReadSByte(Elem + 0x72));
                Console.Memory.WriteSByte(Elem + 0x77, Console.Memory.ReadSByte(Elem + 0x73));
                Console.Memory.WriteFloat(Elem + 0x28, Console.Memory.ReadFloat(Elem));
                Console.Memory.WriteFloat(Elem + 0x2C, Console.Memory.ReadFloat(Elem + 0x4));
                Console.Memory.WriteUInt32(Elem + 0x30, Console.Memory.ReadUInt32(Level_Time));
                Console.Memory.WriteUInt16(Elem + 0x62, (ushort)System.Math.Floor(Time * 1000 + 0.5));
                Console.Memory.WriteFloat(Elem, X);
                Console.Memory.WriteFloat(Elem + 4, Y);
                return Elem;
            }

            public uint FadeOverTime(uint Elem, ushort Time, byte[] Color, JRPC Console)
            {
                new gameFunctions().setMemory(Elem + 0x1C, new gameFunctions().getMemory(Elem + 0x18, 4, Console), Console);
                Console.Memory.WriteUInt32(Elem + 0x20, Console.Memory.ReadUInt32(Level_Time));
                Console.Memory.WriteUInt16(Elem + 0x54, (ushort)System.Math.Floor(Time * 1000 + 0.5));
                new gameFunctions().setMemory(Elem + 0x18, Color, Console);
                return Elem;
            }

            public uint FontScaleOverTime(uint Elem, ushort Time, float fontScale, JRPC Console)
            {
                Console.Memory.WriteFloat(Elem + 0x10, Console.Memory.ReadFloat(Elem + 0xC));
                Console.Memory.WriteUInt32(Elem + 0x14, Console.Memory.ReadUInt32(Level_Time));
                Console.Memory.WriteUInt16(Elem + 0x52, (ushort)System.Math.Floor(Time * 1000 + 0.5));
                Console.Memory.WriteFloat(Elem + 0xC, fontScale);
                return Elem;
            }

            public uint ScaleOverTime(uint Elem, ushort Time, ushort Width, ushort Height, JRPC Console)
            {
                Console.Memory.WriteUInt16(Elem + 0x5C, Console.Memory.ReadUInt16(Elem + 0x58));
                Console.Memory.WriteUInt16(Elem + 0x5E, Console.Memory.ReadUInt16(Elem + 0x5A));
                Console.Memory.WriteUInt32(Elem + 0x24, Console.Memory.ReadUInt32(Level_Time));
                Console.Memory.WriteUInt16(Elem + 0x60, (ushort)System.Math.Floor(Time * 1000 + 0.5));
                Console.Memory.WriteUInt16(Elem + 0x58, Width);
                Console.Memory.WriteUInt16(Elem + 0x5A, Height);
                return Elem;
            }

            public uint PulseFX(uint Elem, ushort Time, ushort DecayStart, ushort DecayDuration, JRPC Console)//May or may not work.
            {
                Console.Memory.WriteUInt32(Elem + 0x48, Console.Memory.ReadUInt32(Level_Time));
                Console.Memory.WriteUInt16(Elem + 0x66, (ushort)System.Math.Floor(Time * 1000 + 0.5));
                Console.Memory.WriteUInt16(Elem + 0x68, (ushort)System.Math.Floor(DecayStart * 1000 + 0.5));
                Console.Memory.WriteUInt16(Elem + 0x66, (ushort)System.Math.Floor(DecayDuration * 1000 + 0.5));
                return Elem;
            }

            public byte[] ColorToByteArray(System.Drawing.Color Color)
            {
                return new byte[] { Color.R, Color.G, Color.B, Color.A };
            }
        }

        public class rFiles
        {
            struct RawFile
            {
                UInt32 NamePointer;
                UInt32 Length;
                UInt32 DataPointer;
            }

            public string getFileName(uint Index, JRPC Console)
            {
                return Console.Memory.ReadString(Console.Memory.ReadUInt32(0x832DA220 + (Index * 0xC)), 32);
            }

            public uint getFileLength(uint Index, JRPC Console)
            {
                return Console.Memory.ReadUInt32(0x832DA220 + (Index * 0xC) + 0x4);
            }

            public string getFileData(uint Index, JRPC Console)
            {
                return new string(Encoding.ASCII.GetChars(new gameFunctions().getMemory(Console.Memory.ReadUInt32(0x832DA220 + (Index * 0xC) + 0x8), getFileLength(Index, Console), Console)));
            }

            public void WriteFile(uint Index, string Data, JRPC Console)
            {
                uint FileOffset = 0x832DA220 + (Index * 0xC);
                Console.Memory.WriteUInt32(FileOffset + 0x4, (uint)Data.Length + 1);
                new gameFunctions().setMemory(Console.Memory.ReadUInt32(FileOffset + 0x8), Encoding.ASCII.GetBytes(Data + "\0"), Console);
            }

        }
    }

    public class pState
    {

        public uint getPlayerState(int pIndex)
        {
            return 0x8354A310 + ((uint)pIndex * 0x57F8);
        }

        #region Origin and Angles and Velocity

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
        };

        public Vec3 getOrigin(int pIndex, JRPC Console)
        {
            return new Vec3(Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x28), Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x2C), Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x30));
        }

        public void setOrigin(int pIndex, Vec3 Origin, JRPC Console)
        {
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x28, Origin.x);
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x2C, Origin.y);
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x30, Origin.z);
        }

        public Vec3 getVelocity(int pIndex, JRPC Console)
        {
            return new Vec3(Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x34), Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x38), Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x3C));
        }

        public void setVelocity(int pIndex, Vec3 Velocity, JRPC Console)
        {
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x34, Velocity.x);
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x38, Velocity.y);
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x3C, Velocity.z);
        }

        public Vec3 getAngles(int pIndex, JRPC Console)
        {
            return new Vec3(Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x1F8), Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x1FC), Console.Memory.ReadFloat(getPlayerState(pIndex) + 0x200));
        }

        public void setAngles(int pIndex, Vec3 Angles, JRPC Console)
        {
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x1F8, Angles.x);
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x1FC, Angles.y);
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x200, Angles.z);
        }

        public void Vec3ToNumericArray(Vec3 Vector, System.Windows.Forms.NumericUpDown[] Numeric)
        {
            if (Numeric.Count() == 3)
            {
                Numeric[0].Value = (decimal)Vector.x;
                Numeric[1].Value = (decimal)Vector.y;
                Numeric[2].Value = (decimal)Vector.z;
            }
        }

        public Vec3 NumericArrayToVec3(System.Windows.Forms.NumericUpDown[] Numeric)
        {
            return new Vec3((float)Numeric[0].Value, (float)Numeric[1].Value, (float)Numeric[2].Value);
        }

        #endregion

        #region Gamertag

        public string getClantag(int pIndex, JRPC Console, bool fmt)
        {
            if (Console.Memory.ReadByte(getPlayerState(pIndex) + 0x55A0) != 0)
                return (fmt) ? "[" + Console.Memory.ReadString(getPlayerState(pIndex) + 0x55A0, 6) + "]" : Console.Memory.ReadString(getPlayerState(pIndex) + 0x55A0, 6);
            return "";
        }

        public string getGamertag(int pIndex, JRPC Console)
        {
            if (Console.Memory.ReadByte(getPlayerState(pIndex) + 0x5534) != 0)
                return Console.Memory.ReadString(getPlayerState(pIndex) + 0x5534, 32);
            return "";
        }

        public string getCleanGamertag(int pIndex, JRPC Console)
        {
            if (Console.Memory.ReadByte(getPlayerState(pIndex) + 0x54AC) != 0)
                return Console.Memory.ReadString(getPlayerState(pIndex) + 0x54AC, 32);
            return "";
        }

        public string getFormatedGamertag(int pIndex, JRPC Console)
        {
            return getClantag(pIndex, Console, true) + getCleanGamertag(pIndex, Console);
        }

        public void writeClantag(int pIndex, string Clantag, JRPC Console)
        {
            new gameFunctions().setMemory(getPlayerState(pIndex) + 0x55A0, System.Text.Encoding.ASCII.GetBytes(Clantag + "\0"), Console);
        }

        public void writeGamertag(int pIndex, string Gamertag, JRPC Console)
        {
            new gameFunctions().setMemory(getPlayerState(pIndex) + 0x5534, System.Text.Encoding.ASCII.GetBytes(Gamertag + "\0"), Console);
        }

        public int getIndexForGamertag(string Gamertag, JRPC Console)
        {
            for (int i = 0; i < 18; i++)
                if (Console.Memory.ReadString(getPlayerState(i) + 0x54AC, 32).Contains(Gamertag))
                    return i;
            return -1;
        }

        #endregion

        #region Standard

        public uint getPressedButtons(int pIndex, JRPC Console)
        {
            Console.Memory.xbConsole.DebugTarget.InvalidateMemoryCache(true, getPlayerState(pIndex) + 0x568C, 4);
            return Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0x568C);
        }

        public gameFunctions.sessionState_t getSessionState(int pIndex, JRPC Console)
        {
            return (gameFunctions.sessionState_t)Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0x5410);
        }

        public gameFunctions.connectionState_t getConnectionState(int pIndex, JRPC Console)
        {
            return (gameFunctions.connectionState_t)Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0x5428);
        }

        public void Visibility(int pIndex, bool Enabled, JRPC Console)
        {
            Console.Memory.WriteUInt32(new gEnt().getGentity(pIndex) + 0x11C, 0);
            if (Enabled)
            {
                Console.Memory.AND_Int32(getPlayerState(pIndex) + 0xFC, ~0x20);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Visiblitity: ^2On\"", Console);
            }
            else
            {
                Console.Memory.OR_UInt32(getPlayerState(pIndex) + 0xFC, 0x20);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Visiblitity: ^1Off\"", Console);
            }
        }

        public void God(int pIndex, bool Enabled, JRPC Console)
        {
            if (Enabled)
            {
                Console.Memory.OR_UInt32(getPlayerState(pIndex) + 0x18, 1);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3God: ^2On\"", Console);
            }
            else
            {
                Console.Memory.AND_Int32(getPlayerState(pIndex) + 0x18, ~1);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3God: ^1Off\"", Console);
            }
        }

        public void setSpread(int pIndex, bool Enabled, JRPC Console)
        {
            if (Enabled)
            {
                Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x1EC, 1);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Spread: ^2On\"", Console);
            }
            else
            {

                Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x1E8, 0);
                Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x1EC, 2);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Spread: ^1Off\"", Console);
            }
        }

        public void setSpread(int pIndex, float Spread, JRPC Console)
        {
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x1E8, Spread);
            Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x1EC, 2);
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Spread: ^7" + Spread.ToString() + "\"", Console);
        }

        public void givePlayerWeapon(int pIndex, string Weapon, int Camo, bool Akimbo, JRPC Console)
        {
            int weaponIndex = (int)Console.CallUInt(0x823ABBB8, new object[] { Weapon });
            Console.CallVoid(0x823ACE78, new object[] { getPlayerState(pIndex), weaponIndex, Akimbo, Camo });
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_RELIABLE, "h " + weaponIndex.ToString(), Console);
            //Console.CallVoid(0x82311E80, new object[] { new gEnt().getGentity(pIndex), weaponIndex, 0, 999 });
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_RELIABLE, "; \"^3Weapon: ^7" + Weapon + "\"", Console);
        }

        public void TakeAllWeapons(int pIndex, JRPC Console)
        {
            for (int i = 0; i < 0x10; i++)
                Console.Memory.WriteUInt32((uint)(getPlayerState(pIndex) + 0x248 + (i * 0x1C)), 0);
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3All Weapons Taken\"", Console);
        }

        public void Test(JRPC Console)//Forgot what this is
        {
            for (int i = 0; i < 0x10; i++)
            {
                Console.Memory.WriteUInt32((uint)(getPlayerState(0) + 0x230 + (i * 0x2F)), 0xFF);
                Console.Memory.WriteUInt32((uint)(getPlayerState(0) + 0x22C + (i * 0x2F)), 0xFF);
            }
        }

        public void setCamo(int pIndex, int Camo, JRPC Console)
        {
            for (int i = 0; i < 0x10; i++)
                Console.Memory.WriteByte((uint)(getPlayerState(pIndex) + 0x2BF + (i * 0x1C)), 0);
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Camo: ^7" + new gameFunctions().getCamoNameForIndex(Camo) + "\"", Console);
        }

        public void GiveInfrared(int pIndex, int Type, JRPC Console)
        {
            if (Type == 0)
                Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x18, 0x1000);//Vision w/ effect
            else if (Type == 1)
                Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x18, 0x1A00);//Effect w/o vision
            else
            {
                Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x18, 0x1000);//Gives Vision w/ effect
                Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x18, 0xA00);//Removes effect
            }
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Infrared: ^7" + ((Type == 0) ? "Full" : (Type == 1) ? "Effect" : "Vision") + "\"", Console);
        }

        public void GiveKillStreaks(int pIndex, int Amount, JRPC Console)
        {
            Console.Memory.WriteByte(getPlayerState(pIndex) + 0x42E, (byte)Amount);
            Console.Memory.WriteByte(getPlayerState(pIndex) + 0x432, (byte)Amount);
            Console.Memory.WriteByte(getPlayerState(pIndex) + 0x437, (byte)Amount);
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3KillStreaks Given\"", Console);
        }

        public bool Freeze(int pIndex, bool Enabled, JRPC Console)
        {
            if (Enabled)
                Console.Memory.OR_UInt32(getPlayerState(pIndex) + 0x5684, 0x4);
            else
                Console.Memory.AND_Int32(getPlayerState(pIndex) + 0x5684, ~0x4);
            return Enabled;
        }

        public bool FreezeWithLook(int pIndex, bool Enabled, JRPC Console)
        {
            if (Enabled)
                Console.Memory.OR_UInt32(getPlayerState(pIndex) + 0x5684, 0x10);
            else
                Console.Memory.AND_Int32(getPlayerState(pIndex) + 0x5684, ~0x10);
            return Enabled;
        }

        public void setSpeed(int pIndex, float Speed, JRPC Console)
        {
            Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x54E0, Speed);
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Speed: ^7" + Speed.ToString() + "\"", Console);
        }

        public void EMP(int pIndex, bool Enabled, JRPC Console)
        {
            if (Enabled)
            {
                Console.Memory.OR_UInt32(getPlayerState(pIndex) + 0x18, 0x40);
                Console.Memory.OR_UInt32(getPlayerState(pIndex) + 0x100, 0x8000);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3EMP: ^2On\"", Console);
            }
            else
            {
                Console.Memory.AND_Int32(getPlayerState(pIndex) + 0x18, ~0x40);
                Console.Memory.AND_Int32(getPlayerState(pIndex) + 0x100, ~0x8000);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3EMP: ^1Off\"", Console);
            }
        }

        public void UAV(int pIndex, int Type, JRPC Console)
        {
            Console.Memory.WriteByte(getPlayerState(pIndex) + 0x5607, (byte)Type);
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Hud: ^7" + ((Type == 0) ? "Hardcore" : (Type == 1) ? "Normal" : "UAV Always") + "\"", Console);
        }

        public void SetActionSlot(int pIndex, int ActionSlot, gameFunctions.ActionSlotType aType, JRPC Console)
        {
            Console.Memory.WriteUInt32((uint)(getPlayerState(pIndex) + 0x550 + (ActionSlot * 0x4)), (uint)aType);
        }

        public void NightVision(int pIndex, JRPC Console)
        {
            SetActionSlot(pIndex, 0, gameFunctions.ActionSlotType.nightvision, Console);
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_RELIABLE, "2 1060 Infrared 0", Console);
            new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_RELIABLE, "; \"^3Press [{+actionslot 1}] to toggle nightvision\"", Console);
        }

        public int getTeam(int pIndex, JRPC Console)
        {
            return Console.Memory.ReadInt32(getPlayerState(pIndex) + 0x54F8);/*0-3 or 4 teams*/
        }

        #endregion

        #region Toggles

        public void Visibility(int pIndex, JRPC Console)
        {
            Console.Memory.WriteUInt32(new gEnt().getGentity(pIndex) + 0x11C, 0);
            Console.Memory.XOR_UInt32(getPlayerState(pIndex) + 0xFC, 0x20);
            if ((Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0xFC) & 0x20) != 0)
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Visiblitity: ^1Off\"", Console);
            else
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Visiblitity: ^2On\"", Console);
        }

        public void God(int pIndex, JRPC Console)
        {
            Console.Memory.XOR_UInt32(getPlayerState(pIndex) + 0x18, 1);
            if ((Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0x18) & 1) != 0)
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3God: ^2On\"", Console);
            else
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3God: ^1Off\"", Console);
        }

        public void setSpread(int pIndex, JRPC Console)
        {
            if (Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0x1EC) == 1)
            {
                Console.Memory.WriteFloat(getPlayerState(pIndex) + 0x1E8, 0);
                Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x1EC, 2);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Spread: ^1Off\"", Console);
            }
            else
            {
                Console.Memory.WriteUInt32(getPlayerState(pIndex) + 0x1EC, 1);
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Spread: ^2On\"", Console);
            }
        }

        public void Freeze(int pIndex, JRPC Console)
        {
            Console.Memory.XOR_UInt32(getPlayerState(pIndex) + 0x5684, 4);
            if ((Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0x5684) & 4) != 0)
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Movement: ^7Frozen\"", Console);
            else
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Movement: ^7Default\"", Console);
        }

        public void FreezeWithLook(int pIndex, JRPC Console)
        {
            Console.Memory.XOR_UInt32(getPlayerState(pIndex) + 0x5684, 0x10);
            if ((Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0x5684) & 0x10) != 0)
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Movement: ^7Frozen with look\"", Console);
            else
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3Movement: ^7Default\"", Console);
        }

        public void EMP(int pIndex, JRPC Console)
        {
            Console.Memory.OR_UInt32(getPlayerState(pIndex) + 0x18, 0x40);
            Console.Memory.XOR_UInt32(getPlayerState(pIndex) + 0x100, 0x8000);
            if ((Console.Memory.ReadUInt32(getPlayerState(pIndex) + 0x100) & 0x8000) != 0)
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3EMP: ^2On\"", Console);
            else
                new gameFunctions().SV_GameSendServerCommand(pIndex, gameFunctions.svscmd_type.SV_CMD_CAN_IGNORE, "; \"^3EMP: ^2On\"", Console);
        }

        #endregion

    }

    public class gEnt : pState
    {
        //Need to work on gentity and entity linking later.
        public uint getGentity(int pIndex)
        {
            return 0x833C8F40 + ((uint)pIndex * 0x31C);
        }
    }
}
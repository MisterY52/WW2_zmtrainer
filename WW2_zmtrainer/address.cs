using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zm
{
    class address
    {
        public static Int64 baseadd = 0;
        public static Int64 soldi;
        public static Int64[] ammo = new Int64[14];
        public static Int64[] ammo_init = new Int64[14];
        public static Int64 vita;
        public static Int64 special;
        public static Int64[] vz = new Int64[21];
        public static Int64[] posz_X = new Int64[21];
        public static Int64[] posz_Y = new Int64[21];
        public static Int64[] posz_Z = new Int64[21];
        public static Int64 give1;
        public static Int64 give2;
        public static Int64 give3;
        public static Int64 rapid;
        public static Int64 jump;
        public static Int64 X;
        public static Int64 Y;
        public static Int64 Z;
        public static Int64 norecoil;
        public static Int64 curweap;
        public static Int64 timescale;
        public static Int64 round;
        public static Int64 nome;

        public class player
        {
            public Int64 soldi;
            public Int64[] ammo = new Int64[14];
            public Int64[] ammo_init = new Int64[14];
            public Int64 vita;
            public Int64 special;
            public Int64 give1;
            public Int64 give2;
            public Int64 give3;
            public Int64 rapid;
            public Int64 X;
            public Int64 Y;
            public Int64 Z;
            public Int64 nome;
        }

        public static player p2 = new player();
        public static player p3 = new player();
        public static player p4 = new player();

        public static void findadd(string s)
        {
            memory m = new memory();
            m.AttackProcess("s2_mp64_ship");
            
            try
            {
                switch (s)
                {
                    case "money_p1":
                        soldi = m.FindPattern(baseadd + 0xA000000, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "????xxx????xxxxxx?");
                        soldi = m.FindPattern(soldi+1, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "????xxx????xxxxxx?");
                        break;
                    case "money_p2":
                        p2.soldi = m.FindPattern(baseadd + 0xA000000, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "???????????xxxxxx?");
                        break;
                    case "money_p3":
                        p3.soldi = m.FindPattern(baseadd + 0xA000000, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "???????????xxxxxx?");
                        p3.soldi = m.FindPattern(p3.soldi+1, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "???????????xxxxxx?");
                        p3.soldi = m.FindPattern(p3.soldi + 1, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "???????????xxxxxx?");
                        break;
                    case "money_p4":
                        p4.soldi = m.FindPattern(baseadd + 0xA000000, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "???????????xxxxxx?");
                        p4.soldi = m.FindPattern(p4.soldi + 1, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "???????????xxxxxx?");
                        p4.soldi = m.FindPattern(p4.soldi + 1, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "???????????xxxxxx?");
                        p4.soldi = m.FindPattern(p4.soldi + 1, baseadd + 0xF000000, "\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x06\x6B\x00\x00\x31\x80", "???????????xxxxxx?");
                        break;
                    case "round":
                        round = m.FindPattern(baseadd + 0xA000000, baseadd + 0xF000000, "\x03\x00\x00\x00\x00\x00\x00\x00\xE6\x29\x00\x00\x06\xC0\x00\x00\x54\x80", "????xxxx????xxxxx?");
                        break;
                    default: break;
                }
            }
            catch
            {
            }
        }
        //+119600, vita
        //+11A600, ammo, rapid, coord, give
        //+29ACF0
        //+29AFF0  3C9A80
        public static void setadd()
        {
            memory m = new memory();
            m.AttackProcess("s2_mp64_ship");
            baseadd = memory.GetBaseAddress("s2_mp64_ship").ToInt64();
            for (int i = 0; i < ammo.Length; i++)
            {
                ammo[i] = baseadd + 0xA40D688 + i * 0x18; 
                ammo_init[i] = ammo[i] + 0x8;
            }
            vita = baseadd + 0xA024B0C; 
            special = baseadd + 0xA40EF88;
            give1 = baseadd + 0xA40D35C;
            give2 = give1 + 0x24; //baseadd + 0xA10E440;
            give3 = give2 + 0xC; //baseadd + 0x9DBBECC;
            rapid = baseadd + 0xA40D30C;
            jump = baseadd + 0xC33F48;
            X = baseadd + 0xA40CFA4;
            Y = X + 0x4;
            Z = X + 0x8;
            norecoil = baseadd + 0x51B71;
            curweap = baseadd + 0x5CC7480;
            timescale = baseadd + 0x278FB7C; //00 00 80 3F 01 00 00 00 00 00 00 00
            for (int i = 0; i < vz.Length; i++)
            {
                vz[i] = baseadd + 0xA030F8C + 0x418 * i; //150 default, round 1
            }
            for (int i = 0; i < posz_X.Length; i++)
            {
                posz_X[i] = baseadd + 0xA534324 + 0x5CC8 * i;
                posz_Y[i] = posz_X[i] + 0x4;
                posz_Z[i] = posz_X[i] + 0x8;
            }
            nome = baseadd + 0x9DC149A;
            //Player 2
            //+6244, ammo, coord, give, rapid, special
            //+6640, soldi
            //+418, vita
            p2.vita = vita + 0x418;
            for (int i = 0; i < ammo.Length; i++)
            {
                p2.ammo[i] = ammo[i] + 0x6268;
                p2.ammo_init[i] = ammo_init[i] + 0x6268;
            }
            p2.X = X + 0x6268;
            p2.Y = p2.X + 0x4;
            p2.Z = p2.X + 0x8;
            p2.give1 = give1 + 0x6268;
            p2.give2 = give2 + 0x6268;
            p2.give3 = give3 + 0x6268;
            p2.rapid = rapid + 0x6268;
            p2.special = special + 0x6268;
            p2.nome = nome + 0x6268;
            //Player 3
            p3.vita = vita + 0x418 * 2;
            for (int i = 0; i < ammo.Length; i++)
            {
                p3.ammo[i] = ammo[i] + 0x6268 * 2;
                p3.ammo_init[i] = ammo_init[i] + 0x6268 * 2;
            }
            p3.X = X + 0x6268 * 2;
            p3.Y = p3.X + 0x4;
            p3.Z = p3.X + 0x8;
            p3.give1 = give1 + 0x6268 * 2;
            p3.give2 = give2 + 0x6268 * 2;
            p3.give3 = give3 + 0x6268 * 2;
            p3.rapid = rapid + 0x6268 * 2;
            p3.special = special + 0x6268 * 2;
            p3.nome = nome + 0x6268 * 2;
            //Player 4
            p4.vita = vita + 0x418 * 3;
            for (int i = 0; i < ammo.Length; i++)
            {
                p4.ammo[i] = ammo[i] + 0x6268 * 3;
                p4.ammo_init[i] = ammo_init[i] + 0x6268 * 3;
            }
            p4.X = X + 0x6268 * 3;
            p4.Y = p4.X + 0x4;
            p4.Z = p4.X + 0x8;
            p4.give1 = give1 + 0x6268 * 3;
            p4.give2 = give2 + 0x6268 * 3;
            p4.give3 = give3 + 0x6268 * 3;
            p4.rapid = rapid + 0x6268 * 3;
            p4.special = special + 0x6268 * 3;
            p4.nome = nome + 0x6268 * 3;
        }
    }
}

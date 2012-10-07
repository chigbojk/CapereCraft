using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCLawl
{
    public class Mine
    {
        public static List<Mine> Mines = new List<Mine>();

        public ushort[] loc = new ushort[3] { 0, 0, 0 };
        public Player owner;
        public Level levelOn;
        public bool active;

        public Mine(ushort[] loc, Player owner, Level levelon, bool active)
        {
            loc = this.loc;
            owner = this.owner;
            levelon = this.levelOn;
        }

        public static void placeMine(Mine m, Player p, Level l, ushort[] loc)
        {
            try
            {
                Mines.Add(m);

            }
            catch { }

        }
        public static void deleteMine(Mine m, Player p, Level l, ushort[] loc, bool silent)
        {
            try
            {
                Mines.Remove(m);
                l.SetTile(loc[0], loc[1], loc[2], Block.air);
                if (!silent) p.SendMessage("Mine defused.");
            }
            catch { }
        }
        public static Mine Find(Level levelOn1, ushort[] loc1)
        {
            Mine Z = null; //MineZ... you geddit? LOL... ok maybe it isn't that funny.
            foreach (Mine m in Mines)
            {
                if (m.loc == loc1 && levelOn1 == m.levelOn)
                {
                    return Z;
                }
            }
            return null;
        }
    }
}

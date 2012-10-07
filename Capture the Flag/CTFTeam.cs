using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCLawl
{
    public class CTFTeam
    {

        public string name = "";
        public int captures = 0;
        public string teamColor = null;
        public ushort[] flagSpawn = new ushort[3] { 0, 0, 0 };
        public ushort[] teamSpawn = new ushort[3] { 0, 0, 0 };
        public List<Player> players = new List<Player>();
        public bool hasFlag = false;
        public bool flagAtBase = false;
        public byte blockColor;

        public void spawnPlayers()
        {
            foreach (Player p in players.ToArray()) { spawnPlayer(p, true); }
        }
        public void openSpawn()
        {
            ushort x = teamSpawn[0], y = (ushort)teamSpawn[1]--, yy = (ushort)teamSpawn[1]--, z = (ushort)teamSpawn[2];
            yy--;

            CTF.gameLevel.SetTile(x, y, z, Block.air); CTF.gameLevel.SetTile(x, yy, z, Block.air);
            CTF.gameStarted = true;
            CTF.Message("The game has started! Go go go!");

        }
        public void takeFlag(Player p, CTFTeam taker)
        {
            CTF.Message(p.getTeam().teamColor + " took the " + (taker == CTF.redTeam ? "%cred" : "%9blue") + c.def + " flag!");
            taker.hasFlag = true;
            p.carryingFlag = true;
            p.opponentTeam().flagAtBase = false;

            if (p.opponentTeam().hasFlag)
            {
                CTF.antiStalemate = true;
                CTF.Message(c.green + "Anti-Stalemate mode activated!"); 
                CTF.Message("If someone is tagged, their team's flag-carrier automatically drops the flag!");
            }
        }

        
        public void addMember(Player p)
        {
            if (players.Contains(p)) { return; }

            players.Add(p);
            p.spectator = false;
            Player.GlobalDie(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            spawnPlayer(p, true);
        }

        public void removeMember(Player p)
        {
            if (!players.Contains(p)) { return; }
            players.Remove(p);
            p.spectator = true;
        }

        public void spawnPlayer(Player p, bool announce)
        {
            addMember(p);
            p.setLoc(teamSpawn);
            if (announce)
            {
                CTF.Message(p.getTeam().teamColor + p.name + c.def + " joined the "
                    + p.getTeam().teamColor + p.getTeam().name + c.def + " team");
            }

        }

        public void teamChat(Player p, string msg)
        {
            foreach (Player m in players.ToArray())
            {
                m.SendMessage(teamColor + "<TEAM> " + p.name + ": " + c.white + msg);
            }
        }

        public string capitalName() { return ((name.ToLower().StartsWith("r")) ? "Red" : "Blue"); }





    }
}

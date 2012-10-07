using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace MCLawl
{
    class CTF
    {
        //Maps

        public static bool antiStalemate = false;
        public static int maxCaptures = 5;
        public static bool gameStarted = false;
        public static List<Player> referees = new List<Player>();
        public static CTFTeam redTeam;
        public static CTFTeam blueTeam;
        public static Level gameLevel;

        public static ushort[] redFlag { get { return redTeam.flagSpawn; } }
        public static ushort[] blueFlag { get { return blueTeam.flagSpawn; } }

        public static string[] maps = new string[2];
        public static int[] votes = new int[2];
        public static string lastMap = Server.mainLevel.name;

        public static bool doublePoints = false;
        public static bool voting = false;

        public static void nextLevel()
        {
            string nextLevel = "";
            var rand = new Random();

            votes[0] = 0;
            votes[1] = 0;

            DirectoryInfo di = new DirectoryInfo("levels");
            var maps = di.GetFiles("*.lvl");
            redo:
            CTF.maps[0] = maps[rand.Next(maps.Length)].Name.ToLower().Replace(".lvl", "");
            CTF.maps[1] = maps[rand.Next(maps.Length)].Name.ToLower().Replace(".lvl", "");
            #region We don't want this happening now, do we?
            {
                if (CTF.maps[0] == lastMap) { goto redo; }
                if (CTF.maps[1] == lastMap) { goto redo; }
                if (CTF.maps[0] == CTF.maps[1]) { goto redo; }
                if (CTF.maps[0] == "main") { goto redo; }
                if (CTF.maps[1] == "main") { goto redo; }
                if (CTF.maps[0] == Server.mainLevel.name) { goto redo; }
                if (CTF.maps[1] == Server.mainLevel.name)
                if (string.IsNullOrEmpty(CTF.maps[0])) { goto redo; }
                if (string.IsNullOrEmpty(CTF.maps[1])) { goto redo; }
            }
            #endregion
            voting = true;
            Message("Vote for a new map! Say 1 or 2 to vote for a map!");
            Message(c.lime + "[1] " + CTF.maps[0]);
            Message(c.lime + "[2] " + CTF.maps[1]);
            Thread.Sleep(10000);
            Message("Voting ends in 15 seconds!");
            Thread.Sleep(15000);
            voting = false;

            if (votes[0] > votes[1])
            {
                Message(maps[0] + " won the vote!");
                nextLevel = CTF.maps[0];
            }
            if (votes[0] < votes[1])
            {
                Message(maps[1] + " won the vote!");
                nextLevel = CTF.maps[1];
            }
            if (votes[0] == votes[1])
            {
                Message("There was a tie, so " + maps[0] + " won by default!");
                nextLevel = CTF.maps[0];
            }
            


        }

        public static void initateGame()
        {
            redTeam.teamColor = c.red;
            blueTeam.teamColor = c.blue;
            redTeam.blockColor = Block.red;
            blueTeam.blockColor = Block.blue;

            Player.GlobalMessage("Spawns opening in 15 seconds!");
            Thread.Sleep(10000);
            gameStarted = true;

            Thread mineThread = new Thread(new ThreadStart(mineProximity));
            Thread tagThread = new Thread(new ThreadStart(playerTagging));
            mineThread.Start();
            tagThread.Start();
        }

        public static void captureFlag(Player returner, CTFTeam team)
        {
            Message(team.teamColor + team.name + c.def + " flag returned by " + team.teamColor + returner.name + c.def + "!");
            Message(team.teamColor + team.name + " captures: " + team.captures.ToString() + " - " + returner.opponentTeam().teamColor + returner.opponentTeam().name + " captures: " + c.def + returner.opponentTeam().captures);

            returner.flagsCaptured++;
            team.captures++;
            returner.points += 20;
            if (team.captures >= maxCaptures)
            {
                if (team.name.ToLower() == "blue")
                    End(blueTeam, redTeam);
                else
                    End(redTeam, blueTeam);

            }

        }
        public static void End(CTFTeam winTeam, CTFTeam loseTeam)
        {
            Message("The " + winTeam.teamColor + winTeam.name + c.def + " team won the game!");
            Message(winTeam.teamColor + winTeam.name + " captures: " + c.white + winTeam.captures.ToString() + " - " + loseTeam.teamColor + loseTeam.name + " captures: " + c.white + loseTeam.captures.ToString());
            foreach (Player p in winTeam.players.ToArray())
            {
                p.points += 20;
                p.wins++;
                p.gamesPlayed++;

            }
            foreach (Player p in Player.players.ToArray())
            {
                p.pointsThisGame = 0;
                p.hasTNT = false;
                p.hasMine = false;
            }

        }
        public static void KillPlayer(Player killer, Player victim, KillType killtype)
        {
            if (victim.carryingFlag)
            {
                string col = victim.getTeam().teamColor;
                CTF.Message(col + victim.name + c.def + " dropped the " + killer.getTeam().teamColor + killer.getTeam().name + c.def + " flag.");
                victim.carryingFlag = false;
                victim.getTeam().hasFlag = false;
                killer.points += 5;
                killer.money += 5;
                killer.pointsThisGame += 10;
            }
            switch (killtype)
            {
                case KillType.Explode:

                    Player.GlobalMessage(c.white + "- " + killer.getTeam().teamColor + killer.name + c.def + getDeathMessage() + killer.getTeam().teamColor + victim.name);
                    killer.explodes++;
                    break;
                case KillType.Mine:
                    killer.mines++;
                    Player.GlobalMessage(c.white + "- " + killer.getTeam().teamColor + killer.name + c.def + " mined " + killer.getTeam().teamColor + victim.name);
                    break;
                case KillType.Tag:
                    killer.tags++;
                    Player.GlobalMessage(c.white + "- " + killer.getTeam().teamColor + killer.name + c.def + " tagged " + killer.getTeam().teamColor + victim.name);
                    break;
            }
            killer.pointsThisGame += 5;
            if (!doublePoints) { killer.points += 5; } else { killer.points += 10; }

            killer.opponentTeam().spawnPlayer(victim, false);



        }

        public static void ExplodeTNT(Player p, ushort x, ushort y, ushort z, int size)
        {
            if (!p.hasTNT) return;
            for (int cx = x - size; cx <= x + size; cx++)
            {
                for (int cy = y - size; cy <= y + size; cy++)
                {
                    for (int cz = z - size; cz <= z + size; cz++)
                    {
                        if (canExplode(p, (ushort)cx, (ushort)cy, (ushort)cz)) { p.level.Blockchange(p, (ushort)cx, (ushort)cy, (ushort)cz, Block.air); }
                        playerProximity(p, (ushort)cz, (ushort)cy, (ushort)cz);
                        tntMineProximity(p, (ushort)cz, (ushort)cy, (ushort)cz);
                    }
                }
            }
        }



        public enum KillType : int
        {
            Explode = 1,
            Tag = 2,
            Mine = 3
        }

        public static string getDeathMessage()
        {
            string[] msgs = { " exploded ", " mutilated ", " blew up ", " ended ", " blasted ", " pulverised ", " destroyed ", " annihilated ", " slaughtered ", " murdered ", " eliminated ", " exploded ", " exploded " }; //We cant give it a 1/11 chance, can we?
            var rand = new Random();
            return msgs[rand.Next(msgs.Length)];
        }

        public static void Message(string message)
        {
            Player.GlobalMessage(c.white + "- " + Server.DefaultColor + message);
        }

        public static bool canExplode(Player p, ushort x, ushort y, ushort z)
        {
            return !(p.level.GetTile(x,y,z) == Block.blackrock && p.level.GetTile(x, y, z) == Block.air && Block.OPBlocks(p.level.GetTile(x, y, z)) && x == redFlag[0] && y == redFlag[1] && z == redFlag[2] && blueFlag[0] == x && blueFlag[1] == y && blueFlag[2] == z);
        }

        public static void playerTagging()
        {
            foreach (Player rplayer in redTeam.players)
            {
                foreach (Player bplayer in blueTeam.players)
                {
                    if (!rplayer.hasShield && !bplayer.hasShield)
                    {
                        if (rplayer.footLocation == bplayer.footLocation || rplayer.headLocation == bplayer.headLocation)
                        {
                            Player tagged = null, tagger = null;
                            if (rplayer.getLoc(false)[0] <= gameLevel.width)
                            {

                                tagged = bplayer; tagger = rplayer;
                                if (antiStalemate) { bplayer.dropFlag(); }
                                Message(rplayer.ctfColor + bplayer.name + c.def + " dropped the " + bplayer.opponentTeam().teamColor + bplayer.opponentTeam().name + c.def + " flag!");
                                KillPlayer(tagger, tagged, KillType.Tag);
                                antiStalemate = false;
                            }
                            else
                            {

                                if (antiStalemate) { bplayer.dropFlag(); }
                                tagged = rplayer; tagger = bplayer;
                                Message(bplayer.ctfColor + bplayer.name + c.def + " dropped the " + bplayer.opponentTeam().teamColor + bplayer.opponentTeam().name + c.def + " flag!");
                                KillPlayer(tagger, tagged, KillType.Tag);
                                antiStalemate = false;
                            }
                        }
                    }
                }
            }
        }

        public static void mineProximity()
        {
            foreach(Mine m in Mine.Mines.ToArray())
            {
            int size = 2;
            int x = m.loc[0];
            int y = m.loc[1];
            int z = m.loc[2];
            for (int cx = x - size; cx <= x + size; cx++)
            {
                for (int cy = y - size; cy <= y + size; cy++)
                {
                    for (int cz = z - size; cz <= z + size; cz++)
                    {
                        foreach (Player p in m.owner.opponentTeam().players.ToArray())
                        {
                            if ((p.footLocation[0] == cx && p.footLocation[1] == y && p.footLocation[2] == z) || (p.headLocation[0] == x && p.headLocation[1] == y && p.headLocation[2] == z))
                            {
                                Message(m.owner.ctfColor + m.owner.name + c.def + " mined " + p.ctfColor + p.name);
                                Mine.deleteMine(m, m.owner, gameLevel, m.loc, true);
                                m.owner.hasMine = false;
                            }

                        }
                    }

                }

            }

                }

            }


        
        public static void playerProximity(Player p, ushort x, ushort y, ushort z)
        {
            int count = 0;
            List<Player> peopleKilled = new List<Player>();
            foreach (Player o in p.opponentTeam().players.ToArray())
            {

                if ((o.getLoc(false)[0] == x && o.getLoc(false)[1] == y && o.getLoc(false)[2] == z) ||
                    (o.getLoc(true)[0] == x && o.getLoc(true)[1] == y && o.getLoc(true)[2] == z))
                { count++; peopleKilled.Add(o); } if (count == 0) { return; }
                if (peopleKilled.Count > 0)
                {
                    foreach (Player m in peopleKilled.ToArray())
                    {
                        KillPlayer(p, m, KillType.Explode);
                    }
                }
            }
            if (count > 1)
            {
                switch (count)
                {
                    case 2:
                        CTF.Message(p.getTeam().teamColor + "DOUBLE KILL!");
                        break;
                    case 3:
                        CTF.Message(p.getTeam().teamColor + "TRIPLE KILL!");
                        break;
                    case 4:
                        CTF.Message(p.getTeam().teamColor + "QUAD KILL!");
                        break;
                    default:
                        CTF.Message(p.getTeam().teamColor + "x" + count.ToString() + "KILL!");
                        break;
                }
            }
            peopleKilled.Clear();
        }

        public static void tntMineProximity(Player p, ushort x, ushort y, ushort z)
        {
            int size = 2;
            if (p.hasBigTNT) { size = 4; }
            Player owner = null;
            foreach (Mine m in Mine.Mines)
            {
                for (int cx = x - size; cx <= x + size; cx++)
                {
                    for (int cy = y - size; cy <= y + size; cy++)
                    {
                        for (int cz = z - size; cz <= z + size; cz++)
                        {
                            if (m.loc[0] == cx && m.loc[1] == cy && m.loc[2] == cz)
                            {
                                ushort[] loc = new ushort[3];
                                loc[0] = (ushort)cx; loc[1] = (ushort)cy; loc[2] = (ushort)cz;
                                owner = m.owner;
                                Mine.deleteMine(m, p, CTF.gameLevel, loc, true);
                                Message(owner.ctfColor + possessionRule(owner.name) + " mine was defused by " + p.ctfColor + p.name);
                                p.points += 3;

                            }

                        }

                    }

                }

            }




        }
        public static string possessionRule(string str)
        {
            return @str + (str.EndsWith("s") ? "'" : "'s");
        }


    }
}

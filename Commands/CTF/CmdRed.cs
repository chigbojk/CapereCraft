/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl) Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.osedu.org/licenses/ECL-2.0
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
using System;

namespace MCLawl
{
    public class CmdRed : Command
    {
        public override string name { get { return "red"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return ""; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdRed() { }

        public override void Use(Player p, string message)
        {
            bool unbalanced;
            int red = CTF.redTeam.players.Count;
            int blu = CTF.blueTeam.players.Count; //TF2 style c:
            int diff = red - blu;
            if (p.getTeam() == CTF.redTeam)
            {
                //if (unbalanced && red > blu)

            }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/red - Join the %cRed " + c.def + "team");
        }
    }
}
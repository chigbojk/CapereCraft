using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCLawl
{
    class MapSettings
    {
        public static string ctfPath = "CTF/";
        public static string configPath = "CTF/config/";

        public static void createTemplate()
        {
            if (File.Exists(configPath + "template.config")) { return; }

            List<string> template = new List<string>();
            string l = Environment.NewLine;

            #region Long stuff is long.
            template.Add("#This is the template you should use when setting up new CTF maps.");
            template.Add(@"#Copy and paste this file and rename it to 'mapname.config' in the config folder.");
            template.Add("#Any line starting with a # will not be read by the server");
            template.Add(l);
            template.Add("#Welcome to the custom CTF map config file!");
            template.Add("#Below is a demonstration for what each setting does.");
            template.Add(l);
            template.Add("#red.flag.x = number - The x coordinate for the red flag");
            template.Add("#red.flag.y = number - The y coordinate for the red flag");
            template.Add("#red.flag.z = number - The z coordinate for the red flag");
            template.Add(l);
            template.Add("#blue.flag.x = number - The x coordinate for the blue flag");
            template.Add("#blue.flag.y = number - The y coordinate for the blue flag");
            template.Add("#blue.flag.z = number - The z coordinate for the blue flag");
            template.Add(l);
            template.Add("#red.team.x = number - The x coordinate for the red team spawn");
            template.Add("#red.team.y = number - The y coordinate for the red team spawn");
            template.Add("#red.team.z = number - The z coordinate for the red team spawn");
            template.Add(l);
            template.Add("#blue.team.x = number - The x coordinate for the blue team spawn");
            template.Add("#blue.team.y = number - The y coordinate for the blue team spawn");
            template.Add("#blue.team.z = number - The z coordinate for the blue team spawn");
            template.Add("#Below is the actual config! Make sure not to leave a space between the setting and option, so the correct way would be:red.flag.x = 0 - not red.flag.x=0!");
            template.Add(l);
            template.Add(l);
            template.Add("red.flag.x = 0");
            template.Add("red.flag.y = 0");
            template.Add("red.flag.z = 0");
            template.Add(l);
            template.Add("blue.flag.x = 0");
            template.Add("blue.flag.y = 0");
            template.Add("blue.flag.z = 0");
            template.Add(l);
            template.Add("red.team.x = 0");
            template.Add("red.team.y = 0");
            template.Add("red.team.z = 0");
            template.Add(l);
            template.Add("blue.team.x = 0");
            template.Add("blue.team.y = 0");
            template.Add("blue.team.z = 0");
            #endregion

            File.WriteAllLines(configPath + "template.config", template.ToArray());
        }

        public static void retrieveMapSettings(string str)
        {
            try
            {
                if (!File.Exists(configPath + str)) { Server.s.Log("Config file is empty!"); return; }

                foreach (string line in File.ReadAllLines(configPath + str))
                {
                    if (line != "" && line[0] != '#')
                    {
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();

                        switch (key.ToLower())
                        {
                            #region Blue flag spawn

                            case "blue.flag.x":
                                CTF.redTeam.flagSpawn[0] = ushort.Parse(value);
                                break;
                            case "blue.flag.y":
                                CTF.redTeam.flagSpawn[1] = ushort.Parse(value);
                                break;
                            case "blue.flag.z":
                                CTF.redTeam.flagSpawn[2] = ushort.Parse(value);
                                break;
                            #endregion

                            #region Blue team spawn
                            case "blue.team.x":
                                CTF.blueTeam.teamSpawn[0] = ushort.Parse(value);
                                break;
                            case "blue.team.y":
                                CTF.blueTeam.teamSpawn[1] = ushort.Parse(value);
                                break;
                            case "blue.team.z":
                                CTF.blueTeam.teamSpawn[2] = ushort.Parse(value);
                                break;


                            #endregion


                            #region Red flag spawn
                            case "red.flag.x":
                                CTF.redTeam.flagSpawn[0] = ushort.Parse(line);
                                break;
                            case "red.flag.y":
                                CTF.redTeam.flagSpawn[1] = ushort.Parse(line);
                                break;
                            case "red.flag.z":
                                CTF.redTeam.flagSpawn[2] = ushort.Parse(line);
                                break;
                            #endregion

                            #region Red team spawn
                            case "red.team.x":
                                CTF.redTeam.teamSpawn[0] = ushort.Parse(value);
                                break;
                            case "red.team.y":
                                CTF.redTeam.teamSpawn[1] = ushort.Parse(value);
                                break;
                            case "red.team.z":
                                CTF.redTeam.teamSpawn[2] = ushort.Parse(value);
                                break;


                            #endregion
                        }

                    }


                }


            }
            catch (Exception ex)
            {
                Server.ErrorLog(ex);
            }
        }

        public static void retrieveGameSettings()
        {
            try
            {
                string[] settings = new string[]{
                    "#Welcome to the CTF game config file!",
                    "#Here you may toggle different things on the server to your liking ",
                    "#Anything starting with a # symbol will NOT be read by the server!",
                    "",
                    "#Here is a guide to the settings:",
                    "#doublePoints = true/false - Players can earn 2x the amount of points they normally do per kill",
                    "#Make sure you leave a space between the setting and the option, so the correct way would be doublePoints = true, not doublePoints=true!",
                    "#ACTUAL SETTING(S) BELOW",
                    "",
                    "doublePoints = false",
                    "",
                    ""
                };
                File.WriteAllLines(ctfPath + "settings.config", settings);

                foreach (string line in File.ReadAllLines(configPath))
                {
                    if (line != "" && line[0] != '#')
                    {
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();

                        switch (key.ToLower())
                        {
                            case "doublePoints":
                                CTF.doublePoints = bool.Parse(key);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Server.ErrorLog(ex);
            }
        }

        



    }
}

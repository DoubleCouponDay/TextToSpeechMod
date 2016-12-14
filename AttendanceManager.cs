using System.Collections.Generic;

using VRage.Game.ModAPI;
using Sandbox.ModAPI;

namespace SETextToSpeechMod
{
    static class AttendanceManager
    {   
        private static List <IMyPlayer> playersField = new List <IMyPlayer>();      
        public static IList <IMyPlayer> Players
        {
            get
            {
                return playersField.AsReadOnly();
            }
        }        

        /// <summary>
        /// CAN BE NULL
        /// </summary>
        public static IMyPlayer LocalPlayer;

        private static Dictionary <string, bool> muteStatusesField = new Dictionary <string, bool>();
        public static IReadOnlyDictionary <string, bool> PlayersMuteStatuses
        {
            get
            {
                return muteStatusesField as IReadOnlyDictionary <string, bool>;
            }
        }                      
        public static bool Debugging {get; set; }

        /// <summary>
        /// Mutes or unmutes the requests player based on your bool input.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mutePlayer"></param>
        /// <returns></returns>
        public static void ChangeMuteStatusOfPlayer (string player, bool newMuteStatus)
        {
            UpdatePlayers();

            for (int i = 0; i < playersField.Count; i++)
            {
                if (playersField[i].DisplayName == player)
                {
                    if (muteStatusesField.ContainsKey (player))
                    {
                        muteStatusesField[player] = newMuteStatus;
                    }

                    else
                    {
                        muteStatusesField.Add (player, newMuteStatus);
                    }

                    switch (newMuteStatus)
                    {
                        case true:
                            MyAPIGateway.Utilities.ShowMessage ("", "Muted '" + player + "', Ya cunt...");
                            break;

                        case false:
                            MyAPIGateway.Utilities.ShowMessage ("", "Unmuted '" + player + "'.");
                            break;
                    }
                    return;
                }
            }                     
            MyAPIGateway.Utilities.ShowMessage ("", "Player: '" + player + "' could not be found.");
        }

        public static void UpdatePlayers()
        {
            playersField.Clear(); //GetPlayers() just adds without overwriting so list must be cleared every time.

            if (Debugging == false)
            {
                MyAPIGateway.Multiplayer.Players.GetPlayers (playersField);

                for (int i = 0; i < playersField.Count; i++)
                {
                    if (muteStatusesField.ContainsKey (playersField[i].DisplayName) == false)
                    {
                        muteStatusesField.Add (playersField[i].DisplayName, new bool());                            
                    }

                    if (playersField[i].SteamUserId == MyAPIGateway.Multiplayer.MyId)
                    {
                        LocalPlayer = playersField[i]; 
                    }
                }
            }    
        }
    }
}

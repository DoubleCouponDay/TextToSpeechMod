using System.Collections.Generic;
using System.Collections.ObjectModel;

using VRage.Game.ModAPI;
using Sandbox.ModAPI;

namespace SETextToSpeechMod
{
    static class AttendanceManager
    {       
        public static IList <IMyPlayer> Players
        {
            get
            {
                playersField.Clear(); //GetPlayers() just adds without overwriting so list must be cleared every time.

                if (OutputManager.Debugging == false)
                {
                    MyAPIGateway.Multiplayer.Players.GetPlayers (playersField); //everytime the project needs to see all players, this will update. Little heavier on performance but its polymorphic. 

                    for (int i = 0; i < playersField.Count; i++)
                    {
                        if (muteStatusesField.ContainsKey (Players[i].DisplayName) == false)
                        {
                            muteStatusesField.Add (Players[i].DisplayName, new bool());
                        }
                    }
                }                
                return playersField.AsReadOnly();
            }
        }

        /// <summary>
        /// NOT GUARANTEED TO CONTAIN EVERY PLAYER. CHECK IF ITS THERE FIRST.
        /// </summary>
        public static IReadOnlyDictionary <string, bool> PlayersMuteStatuses
        {
            get
            {
                return muteStatusesField as IReadOnlyDictionary <string, bool>;
            }
        }
        private static List <IMyPlayer> playersField = new List <IMyPlayer>();        
        private static Dictionary <string, bool> muteStatusesField = new Dictionary <string, bool>();

        /// <summary>
        /// Mutes or unmutes the requests player based on your bool input. prints outcome which depends on if the input player existed or not. (check for typos)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mutePlayer"></param>
        /// <returns></returns>
        public static void ChangeMuteStatusOfPlayer (string player, bool newMuteStatus)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].DisplayName == player)
                {
                    if (muteStatusesField.ContainsKey (player))
                    {
                        muteStatusesField[player] = newMuteStatus;
                    }

                    else
                    {
                        muteStatusesField.Add (player, newMuteStatus);
                    }

                    muteStatusesField.Add ("", true);

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
            MyAPIGateway.Utilities.ShowMessage ("", "Player: '" + player + "' does not exist.");
        }
    }
}

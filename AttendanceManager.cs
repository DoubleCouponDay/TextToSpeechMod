using System.Collections.Generic;

using VRage.Game.ModAPI;
using Sandbox.ModAPI;

namespace SETextToSpeechMod
{
    public class AttendanceManager
    {   
        private List <IMyPlayer> playersField = new List <IMyPlayer>();      
        public IList <IMyPlayer> Players
        {
            get
            {
                return playersField.AsReadOnly();
            }
        }        

        /// <summary>
        /// CAN BE NULL
        /// </summary>
        public IMyPlayer LocalPlayer { get; private set; }

        private Dictionary <string, bool> muteStatusesField = new Dictionary <string, bool>();
        public IReadOnlyDictionary <string, bool> PlayersMuteStatuses
        {
            get
            {
                return (IReadOnlyDictionary<string, bool>)muteStatusesField;
            }
        }         
        
        private static AttendanceManager instance = new AttendanceManager();

        private AttendanceManager()
        {

        }

        public static AttendanceManager GetSingleton()
        {
            return instance;
        }

        /// <summary>
        /// Mutes or unmutes the requests player based on your bool input.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mutePlayer"></param>
        /// <returns></returns>
        public void ChangeMuteStatusOfPlayer (string player, bool newMuteStatus)
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

            if (OutputManager.IsDebugging == false)
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

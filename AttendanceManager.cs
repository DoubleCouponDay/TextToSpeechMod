using System.Collections.Generic; 

using VRage.Game.ModAPI;
using Sandbox.ModAPI;

namespace SETextToSpeechMod
{
    static class AttendanceManager
    {
        private static List <IMyPlayer> playersField;

        public static List <IMyPlayer> Players
        {
            get
            {
                playersField.Clear(); //GetPlayers() just adds without overwriting so list must be cleared every time.
                MyAPIGateway.Multiplayer.Players.GetPlayers (playersField); //everytime the project needs to see all players, this will update. Little heavier on performance but its polymorphic. 
                return playersField;
            }
        }    

        static AttendanceManager()
        {
            playersField = new List <IMyPlayer>();    
        }
    }
}

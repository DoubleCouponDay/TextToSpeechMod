using System;
using System.Text; //location of encoding/decoding.

using Sandbox.ModAPI; //location of MyAPIGateway.
using VRage.Game.Components; //location of MySessionComponentBase.

namespace SETextToSpeechMod
{   
    [MySessionComponentDescriptor (MyUpdateOrder.BeforeSimulation)] //adds an attribute tag telling the game to run my script.
    class MessageEventHandler : MySessionComponentBase //this is also the entry point of the mod.
    {
        const ushort packet_ID = 60452; //the convention is to use the last 4-5 digits of your steam mod as packet ID

        bool initialised;

        private Encoding encode = Encoding.Unicode; //encoding is necessary to convert message into correct format.

        public override void UpdateBeforeSimulation()
        {
            if (initialised == false)
            {
                Initialise();
            }
            OutputManager.Run();             
        }

        void Initialise() //this wouldnt work as a constructor because im guessing some assets arent available during load time.
        {
            initialised = true;           
            MyAPIGateway.Utilities.MessageEntered += OnMessageEntered; //subscribes my method to the MessageEntered event.
            MyAPIGateway.Multiplayer.RegisterMessageHandler (packet_ID, OnReceivedPacket); //registers a multiplayer packet receiver.
            SoundPlayer.InitialiseEmitter();
            OutputManager.FactoryReset();
            MyAPIGateway.Utilities.ShowMessage ("TextToSpeechMod", "If you find a broken word, please tell the designer.");            
        }

        public void OnMessageEntered (string messageText, ref bool sendToOthers)  //event handler method will run when this client posts a chat message.
        {                    
            string noEscapes = string.Format (@"{0}", messageText);
            string fixedCase = noEscapes.ToUpper(); //capitalize all letters of the input sentence so that comparison is made easier.                
            string[] choppedCommands = fixedCase.Split (' ');

            if (choppedCommands.Length >= COMMANDS.CONVENTIONAL_WORD_LIMIT)
            {
                switch (choppedCommands[0] + " " + choppedCommands[1])
                {
                    case COMMANDS.CHANGE_VOICE_TO_MAREK:
                        OutputManager.LocalPlayersVoice = POSSIBLE_OUTPUTS.MarekType;
                        break;

                    case COMMANDS.CHANGE_VOICE_TO_HAWKING:
                        OutputManager.LocalPlayersVoice = POSSIBLE_OUTPUTS.HawkingType;
                        break;

                    case COMMANDS.CHANGE_VOICE_TO_GLADOS:
                        OutputManager.LocalPlayersVoice = POSSIBLE_OUTPUTS.GLADOSType;
                        break;

                    case COMMANDS.MUTE_PLAYER:
                        AttendanceManager.ChangeMuteStatusOfPlayer (choppedCommands[2], true);
                        break;

                    case COMMANDS.UNMUTE_PLAYER:
                        AttendanceManager.ChangeMuteStatusOfPlayer (choppedCommands[2], false);
                        break;
                }
            }
            string signatureBuild = OutputManager.LocalPlayersVoice.ToString();
            int leftoverSpace = POSSIBLE_OUTPUTS.AutoSignatureSize - OutputManager.LocalPlayersVoice.ToString().Length;

            for (int i = 0; i < leftoverSpace; i++)
            {
                signatureBuild += " ";
            }
            fixedCase = signatureBuild + fixedCase; 
            byte[] bytes = encode.GetBytes (fixedCase);

            for (int i = 0; i < AttendanceManager.Players.Count; i++)
            {                                       
                if (AttendanceManager.PlayersMuteStatuses[AttendanceManager.Players[i].DisplayName] == false)
                {
                    MyAPIGateway.Multiplayer.SendMessageTo (packet_ID, bytes, AttendanceManager.Players[i].SteamUserId, true); //everyone will get this trigger including you.
                }                
            }
        }

        public void OnReceivedPacket (byte[] bytes) //action type method which handles the received packets from other players.
        { 
            string decoded = encode.GetString (bytes);
            string signature = ExtractSignatureFromPacket (ref decoded);

            if (decoded.Length > OutputManager.MAX_LETTERS && //letter limit for mental health concerns.
                OutputManager.Debugging == false) 
            {
                MyAPIGateway.Utilities.ShowMessage (OutputManager.MAX_LETTERS.ToString(), " LETTER LIMIT REACHED");
            }

            else
            {
                Type signatureConverted = Type.GetType (signature);
                OutputManager.CreateNewSpeech (signatureConverted, decoded);
            }   
        }

        //dont blame me if your string gets cut the fuck up if it doesnt contain a signature!
        private string ExtractSignatureFromPacket (ref string packet)
        {
            char[] dividedMessage = packet.ToCharArray();
            char[] signatureChars = new char[POSSIBLE_OUTPUTS.AutoSignatureSize];

            for (int i = 0; i < signatureChars.Length; i++)
            {
                signatureChars[i] = dividedMessage[i];
            }
            string voiceSignature = new string (signatureChars);

            packet = packet.Remove (0, POSSIBLE_OUTPUTS.AutoSignatureSize);
            return voiceSignature;
        }

        protected override void UnloadData() //will run when the session closes to prevent my assets from doubling up.
        {
            initialised = false;
            MyAPIGateway.Utilities.MessageEntered -= OnMessageEntered;
            MyAPIGateway.Multiplayer.UnregisterMessageHandler (packet_ID, OnReceivedPacket);            
        }
    }

    public struct COMMANDS
    {
        public const string CHANGE_VOICE_TO_MAREK = "[ MAREK";
        public const string CHANGE_VOICE_TO_HAWKING = "[ MADDEN";
        public const string CHANGE_VOICE_TO_GLADOS = "[ GLADOS";
        public const string MUTE_PLAYER = "[ MUTE ";
        public const string UNMUTE_PLAYER = "[ UNMUTE ";

        public const int CONVENTIONAL_WORD_LIMIT = 3;
    }
}


using System.Text; //location of encoding/decoding.

using Sandbox.ModAPI; //location of MyAPIGateway.
using VRage.Game.Components; //location of MySessionComponentBase.
using System.Threading.Tasks;
using System;

namespace SETextToSpeechMod
{   
    [MySessionComponentDescriptor (MyUpdateOrder.BeforeSimulation)] //adds an attribute tag telling the game to run my script.
    public class ChatManager : MySessionComponentBase 
    {
        const ushort packet_ID = 60452; //the convention is to use the last 4-5 digits of your steam mod as packet ID

        bool initialised;
        private bool debugging = false;
                
        private Task asyncCallAllowed;
        private Encoding encode = Encoding.Unicode; //encoding is necessary to convert message into correct format.        
        private SoundPlayer soundPlayer;
        public OutputManager OutputManager { get; private set; }

        public ChatManager(){}

        public ChatManager (bool isDebugging)
        {
            debugging = isDebugging;           
        }

        public override void UpdateBeforeSimulation()
        {
            if (initialised == false)
            {
                Initialise();
            }

            if (asyncCallAllowed == null || 
               (asyncCallAllowed?.IsCompleted != null && //Apprently && operator will not continue evaluating if its previous evaluation was false
                asyncCallAllowed?.IsCompleted == true))
            {
                asyncCallAllowed = OutputManager.RunAsync();
            }
        }

        public void Initialise() //this wouldnt work as a constructor because im guessing some assets arent available during load time.
        {
            initialised = true;   
            soundPlayer = new SoundPlayer (debugging, true);        
            OutputManager = new OutputManager (soundPlayer, debugging);    

            if (debugging == false)
            {
                MyAPIGateway.Utilities.MessageEntered += OnMessageEntered; //subscribes my method to the MessageEntered event.
                MyAPIGateway.Multiplayer.RegisterMessageHandler (packet_ID, OnReceivedPacket); //registers a multiplayer packet receiver.
                AttendanceManager.UpdatePlayers();            
                MyAPIGateway.Utilities.ShowMessage ("TextToSpeechMod:", "If you find a broken word, please tell the designer.");
            }
        } 

        public void OnMessageEntered (string messageText, ref bool sendToOthers)  //event handler method will run when this client posts a chat message.
        {                                                 
            string noEscapes = string.Format (@"{0}", messageText);
            string fixedCase = noEscapes.ToUpper(); //capitalize all letters of the input sentence so that comparison is made easier.                
            ExecuteCommandIfValid (fixedCase);
            string signatureBuild = OutputManager.LocalPlayersVoice.ToString();
            int leftoverSpace = PossibleOutputs.AutoSignatureSize - OutputManager.LocalPlayersVoice.ToString().Length;

            for (int i = 0; i < leftoverSpace; i++)
            {
                signatureBuild += " ";
            }
            fixedCase = signatureBuild + fixedCase; 
            byte[] ConvertedToPacket = encode.GetBytes (fixedCase);

            if (MyAPIGateway.Multiplayer.MultiplayerActive)
            {
                for (int i = 0; i < AttendanceManager.Players.Count; i++)
                {
                    if (AttendanceManager.PlayersMuteStatuses[AttendanceManager.Players[i].DisplayName] == false)
                    {
                        MyAPIGateway.Multiplayer.SendMessageTo(packet_ID, ConvertedToPacket, AttendanceManager.Players[i].SteamUserId, true); //everyone will get this trigger including you.
                    }
                }
            }

            else
            {
                OnReceivedPacket (ConvertedToPacket);
            }
        }

        private void ExecuteCommandIfValid (string upperCaseSentence)
        {
            for (int i = 0; i < COMMANDS.VoiceCollection.Length; i++)
            {
                if (upperCaseSentence.Contains (COMMANDS.VoiceCollection[i]))
                {
                    OutputManager.LocalPlayersVoice = PossibleOutputs.Collection[i];
                    return;
                }
            }
            string[] choppedCommand = upperCaseSentence.Split (' ');

            if (choppedCommand.Length >= COMMANDS.MUTING_MIN_SIZE)
            {
                int startOfNameIndex = COMMANDS.MUTING_MIN_SIZE - 1;
                string interpretedInput = choppedCommand[startOfNameIndex];

                for (int i = startOfNameIndex; i < choppedCommand.Length; i++)
                {
                    interpretedInput +=  " " + choppedCommand[i];
                }

                if (upperCaseSentence.Contains (COMMANDS.MUTE_PLAYER))
                {
                    AttendanceManager.ChangeMuteStatusOfPlayer (interpretedInput, true);
                }

                else if (upperCaseSentence.Contains (COMMANDS.UNMUTE_PLAYER))
                {
                    AttendanceManager.ChangeMuteStatusOfPlayer (interpretedInput, false);
                }

                else if (upperCaseSentence.Contains (COMMANDS.CHANGE_VOLUME))
                {
                    float attemptedConversion; 

                    if (float.TryParse (interpretedInput, out attemptedConversion))
                    {
                        soundPlayer.UpdateVolume (attemptedConversion);
                    }                 
                    
                    else
                    {
                        MyAPIGateway.Utilities.ShowMessage ("", "Could not determine a valid volume from: " + interpretedInput);
                    }   
                }
            }
        }

        public void OnReceivedPacket (byte[] receivedPacket) //action type method which handles the received packets from other players.
        { 
            string decoded = encode.GetString (receivedPacket);
            string signature = ExtractSignatureFromPacket (ref decoded);

            if (decoded.Length > OutputManager.MAX_LETTERS && //letter limit for mental health concerns.
                debugging == false) 
            {
                MyAPIGateway.Utilities.ShowMessage (OutputManager.MAX_LETTERS.ToString(), " LETTER LIMIT REACHED");
            }

            else
            {
                OutputManager.CreateNewSpeech (signature, decoded);
            }   
        }

        //dont blame me if your string gets cut the fuck up if it doesnt contain a signature!
        private string ExtractSignatureFromPacket (ref string packet)
        {
            char[] dividedMessage = packet.ToCharArray();
            char[] signatureChars = new char[PossibleOutputs.AutoSignatureSize];

            for (int i = 0; i < signatureChars.Length; i++)
            {
                signatureChars[i] = dividedMessage[i];
            }
            string voiceSignature = new string (signatureChars);

            packet = packet.Remove (0, PossibleOutputs.AutoSignatureSize);
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
        //public const string CHANGE_VOICE_TO_HAWKING = "[ JOHN MADDEN";
        public const string CHANGE_VOICE_TO_GLADOS = "[ GLADOS";       

        public static readonly string[] VoiceCollection = {
            CHANGE_VOICE_TO_MAREK,
            //CHANGE_VOICE_TO_HAWKING,
            CHANGE_VOICE_TO_GLADOS,
        };

        public const string MUTE_PLAYER = "[ MUTE";
        public const string UNMUTE_PLAYER = "[ UNMUTE";
        public const int MUTING_MIN_SIZE = 3;

        public const string CHANGE_VOLUME = "[ VOLUME";
    }
}


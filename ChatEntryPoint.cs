using System.Text; //location of encoding/decoding.

using Sandbox.ModAPI; //location of MyAPIGateway.
using VRage.Game.Components; //location of MySessionComponentBase.
using System.Threading.Tasks;
using System;
using SETextToSpeechMod.LookUpTables;
using SETextToSpeechMod.Processing;

namespace SETextToSpeechMod
{   
    [MySessionComponentDescriptor (MyUpdateOrder.BeforeSimulation)] //adds an attribute tag telling the game to run my script.
    public class ChatEntryPoint : MySessionComponentBase 
    {
        const ushort packet_ID = 60452; //the convention is to use the last 4-5 digits of your steam mod as packet ID

        bool initialised;
        private readonly bool debugging;
                        
        private Encoding encode = Encoding.Unicode; //encoding is necessary to convert message into correct format.        
        private SoundPlayer soundPlayer;
        public OutputManager OutputManager { get; private set; }
        private AttendanceManager attendanceManager = AttendanceManager.GetSingleton();
        private Commands commands = Commands.GetSingleton();

        public ChatEntryPoint(){}

        public ChatEntryPoint (bool isDebugging)
        {
            debugging = isDebugging;           
        }

        public override void UpdateBeforeSimulation()
        {
            if (initialised == false)
            {
                Initialise();
            }
            OutputManager.Run();
        }

        public void Initialise() //this wouldnt work as a constructor because some assets arent available during load time.
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
                for (int i = 0; i < attendanceManager.Players.Count; i++)
                {
                    if (attendanceManager.PlayersMuteStatuses[attendanceManager.Players[i].DisplayName] == false)
                    {
                        MyAPIGateway.Multiplayer.SendMessageTo(packet_ID, ConvertedToPacket, attendanceManager.Players[i].SteamUserId, true); //everyone will get this trigger including you.
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
            for (int i = 0; i < commands.VoiceCollection.Length; i++)
            {
                if (upperCaseSentence.Contains (commands.VoiceCollection[i]))
                {
                    OutputManager.LocalPlayersVoice = PossibleOutputs.Collection[i];
                    return;
                }
            }
            string[] choppedCommand = upperCaseSentence.Split (' ');

            if (choppedCommand.Length >= commands.MUTING_MIN_SIZE)
            {
                int startOfNameIndex = commands.MUTING_MIN_SIZE - 1;
                string interpretedInput = choppedCommand[startOfNameIndex];

                for (int i = startOfNameIndex; i < choppedCommand.Length; i++)
                {
                    interpretedInput +=  " " + choppedCommand[i];
                }

                if (upperCaseSentence.Contains (commands.MUTE_PLAYER))
                {
                    attendanceManager.ChangeMuteStatusOfPlayer (interpretedInput, true);
                }

                else if (upperCaseSentence.Contains (commands.UNMUTE_PLAYER))
                {
                    attendanceManager.ChangeMuteStatusOfPlayer (interpretedInput, false);
                }

                else if (upperCaseSentence.Contains (commands.CHANGE_VOLUME))
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
            Sentence inputSentence = new Sentence(decoded);

            if (decoded.Length > OutputManager.MAX_LETTERS && //letter limit for mental health concerns.
                debugging == false) 
            {
                MyAPIGateway.Utilities.ShowMessage (OutputManager.MAX_LETTERS.ToString(), " LETTER LIMIT REACHED");
            }

            else
            {
                OutputManager.CreateNewSpeech (signature, inputSentence);
            }   
        }

        /// <summary>
        /// Gets the serialized header within a transmission packet.
        /// Dont blame me if your string is destroyed when it doesn't contain a signature!
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
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
            OutputManager.DisposeOfUnsafe();         
        }

        public void Dispose()
        {
            initialised = false;
            OutputManager.DisposeOfUnsafe();
        }
    }
}


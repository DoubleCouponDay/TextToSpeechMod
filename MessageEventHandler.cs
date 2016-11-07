using System;
using System.Collections.Generic; //allows specialized Enumerables.
using System.Text; //location of encoding/decoding.

using Sandbox.ModAPI; //location of MyAPIGateway.
using VRage.Game.Components; //location of MySessionComponentBase.
using VRage.Game.ModAPI; //location of IMyPlayer.

namespace SETextToSpeechMod
{   
    [MySessionComponentDescriptor (MyUpdateOrder.BeforeSimulation)] //adds an attribute tag telling the game to run my script.
    class MessageEventHandler : MySessionComponentBase //MySessionComponentBase is inherited and allows me to override its methods.
    {
        const string VERSION = "0";
        const int MAX_LETTERS = 100;
        const int UPDATES_INTERVAL = 60;
        const ushort packet_ID = 60452; //the convention is to use the last 4-5 digits of steam mod as packet ID.

        bool initialised;
        bool runUpdates;
        public bool debugging {private get; set;}
        int timer = 0;        

        private Encoding encode = Encoding.Unicode; //encoding is necessary to convert message into correct format.
        private List <IMyPlayer> players = new List <IMyPlayer>();
        public List <SentenceFactory> speeches = new List <SentenceFactory>(); //this must be a global variable for OptionalDebugger() to work.

        public override void UpdateBeforeSimulation()
        {
            if (initialised == false)
            {
                Initialise();
            }
            
            else if (runUpdates == true)
            {
                if (timer == 0) //im hoping that a little distance will prevent the oscillating position whch annoys ear drums.
                {
                    timer = UPDATES_INTERVAL;
                    players.Clear(); //GetPlayers() just adds without overwriting so list must be cleared every time.
                    MyAPIGateway.Multiplayer.Players.GetPlayers (players);
                    SoundPlayer.UpdatePosition (players);   
                }

                else
                {
                    timer--;
                }               

                for (int i = 0; i < speeches.Count; i++) 
                {
                    speeches[i].Load();

                    if (speeches[i].finished == true)
                    {
                        speeches.RemoveAt (i);
                        i--; //the for loop is about to increment so i dont want to skip a speech.
                    }
                }

                if (speeches.Count == 0)
                {
                    runUpdates = false;
                    timer = 0;
                }
            }
        }

        void Initialise() //this wouldnt work as a constructor because im guessing some assets arent available during load time.
        {
            initialised = true;           
            MyAPIGateway.Utilities.MessageEntered += OnMessageEntered; //subscribes my method to the MessageEntered event.
            MyAPIGateway.Multiplayer.RegisterMessageHandler (packet_ID, OnReceivedPacket); //registers a multiplayer packet receiver.
            SoundPlayer.InitialiseEmitter();
            MyAPIGateway.Utilities.ShowMessage ("TextToSpeechMod", "If you find a broken word, please tell the designer.");
        }

        public void OnMessageEntered (string messageText, ref bool sendToOthers)  //event handler method will run when this client posts a chat message.
        {      
            try //some messages may be too small when i try to access out of bounds.
            {
                if (messageText[0] == '[' && messageText[1] == ' ')
                {     
                    string noEscapes = string.Format (@"{0}", messageText); // @ prevents user's regex inputs.
                    string fixedCase = noEscapes.ToUpper(); //capitalize all letters of the input sentence so that comparison is made easier.                
                    byte[] bytes = encode.GetBytes (fixedCase);
                    players.Clear(); 
                    MyAPIGateway.Multiplayer.Players.GetPlayers (players);

                    if (MyAPIGateway.Multiplayer.MultiplayerActive == true)
                    {
                        for (int i = 0; i < players.Count; i++) //performance danger
                        {                                       
                            bool packetSizeSucceeded = MyAPIGateway.Multiplayer.SendMessageTo (packet_ID, bytes, players[i].SteamUserId, true); //everyone will get this trigger including you.

                            if (packetSizeSucceeded == false)
                            {
                                MyAPIGateway.Utilities.ShowMessage ("", "TRANSMISSION FAILED DUE TO PACKET SIZE LIMIT");
                                break;
                            }
                        }
                    }
                        
                    else
                    {
                        OnReceivedPacket (bytes); //sends it to just you :^)
                    }         
                }
            }  

            catch
            {
                ;
            }
        }

        public void OnReceivedPacket (byte[] bytes) //action type method which handles the received packets from other players.
        { 
            string decoded = encode.GetString (bytes);

            if (decoded == "[ STOP") //the killswitch for whatever reason (looping the fuck out of your sim speed).
            {
                speeches.Clear();
            }
/*
            else if (decoded == "[ VERSION")
            {
                MyAPIGateway.Utilities.ShowMessage ("TTS mod version: ", "2." + VERSION);
            }
*/            
            else if (decoded.Length > MAX_LETTERS && debugging == false) //letter limit for mental health concerns.
            {
                MyAPIGateway.Utilities.ShowMessage (Convert.ToString (MAX_LETTERS), " LETTER LIMIT REACHED");
            }

            else
            {
                speeches.Add (new MarekVoice (decoded));
                runUpdates = true; //in case this is the first sentence.
            }   
        }

        protected override void UnloadData() //will run when the session closes to prevent my assets from doubling up.
        {
            initialised = false;
            runUpdates = false;
            timer = 0;
            speeches.Clear();
            MyAPIGateway.Utilities.MessageEntered -= OnMessageEntered;
            MyAPIGateway.Multiplayer.UnregisterMessageHandler (packet_ID, OnReceivedPacket);
        }
    }
}


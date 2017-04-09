using VRage.ModAPI;
using VRage.Game.Components;
using VRage.ObjectBuilders;

using Sandbox.ModAPI.Interfaces.Terminal;
using Sandbox.ModAPI;
using System.Collections.Generic;

namespace SETextToSpeechMod
{
    //[MyEntityComponentDescriptor (typeof (MyObjectBuilder_), new string[] {"Speech Block (Small)", "Speech Block (Large)"})]
    public class SpeechBlock : MyGameLogicComponent
    {        
        private MyObjectBuilder_EntityBase objectbuilder;
        private SoundPlayer soundPlayer;
        private OutputManager outputManager;

        private IMyTerminalControlCombobox voiceList;
        private IMyTerminalControlTextbox inputField;

        public override void Init (MyObjectBuilder_EntityBase inputObjectBuilder)
        {
            this.objectbuilder = inputObjectBuilder;
            base.Init (inputObjectBuilder);
            Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
            soundPlayer = new SoundPlayer (false, false);
            outputManager = new OutputManager (soundPlayer, false);            
        }

        //Saves the state of the block's entity to disk.
        public override MyObjectBuilder_EntityBase GetObjectBuilder (bool copy = false)
        {
            return (copy) ? (MyObjectBuilder_EntityBase) objectbuilder.Clone() : objectbuilder;
        }

        public override void UpdateBeforeSimulation()
        {
            outputManager.RunAsync();
            
        }

        public void OnCustomTerminalControlGet (IMyTerminalBlock block, List <IMyTerminalControl> controls)
        {

        }

        public override void Close()
        {
            base.Close();
        }
    }
}

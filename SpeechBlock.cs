using VRage.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.ObjectBuilders;

using Sandbox.ModAPI.Interfaces.Terminal;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Text;
using VRage.Utils;

namespace SETextToSpeechMod
{
    [MyEntityComponentDescriptor (typeof (MyObjectBuilder_TerminalBlock), new string[] {"Speech Block (Small)", "Speech Block (Large)"})]
    class SpeechBlock : MyGameLogicComponent, IMyTerminalControlTextbox
    {        
        //Fields
        //------------------------------------------------------------------------------------------------------------------------------//

        private MyObjectBuilder_EntityBase objectbuilder;
        private SoundPlayer soundPlayer;
        private OutputManager outputManager;

        //Properties
        //------------------------------------------------------------------------------------------------------------------------------//

        string IMyTerminalControl.Id { get { throw new NotImplementedException(); } }
        string ITerminalProperty.Id { get { throw new NotImplementedException(); } }
        string ITerminalProperty.TypeName { get { throw new NotImplementedException(); } }


        MyStringId IMyTerminalControlTitleTooltip.Title
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); } 
        }

        MyStringId IMyTerminalControlTitleTooltip.Tooltip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); } 
        }

        bool IMyTerminalControl.SupportsMultipleBlocks
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); } 
        }

        Func<IMyTerminalBlock, bool> IMyTerminalControl.Enabled { set { throw new NotImplementedException(); } }
        Func<IMyTerminalBlock, bool> IMyTerminalControl.Visible { set { throw new NotImplementedException(); } }

        Func<IMyTerminalBlock, StringBuilder> IMyTerminalValueControl<StringBuilder>.Getter
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); } 
        }

        Action<IMyTerminalBlock, StringBuilder> IMyTerminalValueControl<StringBuilder>.Setter
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); } 
        }                

        //Methods
        //------------------------------------------------------------------------------------------------------------------------------//

        public override void Init (MyObjectBuilder_EntityBase inputObjectBuilder)
        {
            this.objectbuilder = inputObjectBuilder;
            base.Init (inputObjectBuilder);
            Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
            soundPlayer = new SoundPlayer (false);
            outputManager = new OutputManager (soundPlayer, false);
        }

        //Saves the state of the block's entity to disk.
        public override MyObjectBuilder_EntityBase GetObjectBuilder (bool copy = false)
        {
            return (copy) ? (MyObjectBuilder_EntityBase) objectbuilder.Clone() : objectbuilder;
        }

        public override void UpdateBeforeSimulation()
        {
            outputManager.Run();
        }

        public override void Close()
        {
            base.Close();
        }

        void IMyTerminalControl.RedrawControl()
        {
            throw new NotImplementedException();
        }

        void IMyTerminalControl.UpdateVisual()
        {
            throw new NotImplementedException();
        }
    }
}

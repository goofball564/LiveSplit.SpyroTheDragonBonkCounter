using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.SpyroTheDragonMusicPlayer.Hook
{
    public abstract class Emulator
    {
        protected Process emulatorProcess;

        public abstract IntPtr BaseRAMAddress
        {
            get;
        }

        public Emulator(Process emulatorProcess)
        {
            this.emulatorProcess = emulatorProcess;
        }
    }
}

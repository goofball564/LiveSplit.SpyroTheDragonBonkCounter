using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;
using System.Linq;

namespace LiveSplit.SpyroTheDragonMusicPlayer.Hook
{
    class Duckstation : Emulator
    {
        // Apparently this works for most or all versions of Duckstation.
        // BaseRAMAddress is IntPtr.Zero if no matching memory page is found,
        // which means Duckstation is loaded but a game isn't.
        public override IntPtr BaseRAMAddress
        {
            get => emulatorProcess.MemoryPages(true).Where(p => p.Type == MemPageType.MEM_MAPPED && p.RegionSize == (UIntPtr)0x200000).FirstOrDefault().BaseAddress;
        }

        public Duckstation(Process emulatorProcess) : base(emulatorProcess)
        {
        }
    }
}

using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;

namespace LiveSplit.SpyroTheDragonBonkCounter.Hook
{
    class EPSXe : Emulator
    {
        private IntPtr _baseRAMAddress = IntPtr.Zero;

        public override IntPtr BaseRAMAddress
        {
            get => _baseRAMAddress;
        }

        public EPSXe(Process emulatorProcess) : base(emulatorProcess)
        {
            DetermineBaseRAMAddress();
        }

        public override void Dispose()
        {
        }

        private void DetermineBaseRAMAddress()
        {
            ProcessModuleWow64Safe mainModule = emulatorProcess.MainModuleWow64Safe();
            switch (mainModule.ModuleMemorySize)
            {
                case 0x9d3000:
                    // version = "1.9.0";
                    _baseRAMAddress = mainModule.BaseAddress + 0x6579A0;
                    break;
                case 0xa08000:
                    // version = "1.9.25";
                    _baseRAMAddress = mainModule.BaseAddress + 0x68b6a0;
                    break;
                case 0x1359000:
                    // version = "2.0.0";
                    _baseRAMAddress = mainModule.BaseAddress + 0x81a020;
                    break;
                default:
                    _baseRAMAddress = IntPtr.Zero;
                    break;
            }
        }
    }
}

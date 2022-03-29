using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;

namespace LiveSplit.SpyroTheDragonBonkCounter.Hook
{
    class Mednafen : Emulator
    {
        private IntPtr _baseRAMAddress = IntPtr.Zero;

        public override IntPtr BaseRAMAddress
        {
            get => _baseRAMAddress;
        }

        public Mednafen(Process emulatorProcess) : base(emulatorProcess)
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
                case 0x42c9000:
                    // version = "1.24.1 32bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x1c96560;
                    break;
                case 0x5eef000:
                    // version = "1.24.1 64bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x25bf280;
                    break;
                case 0x42c6000:
                    // version = "1.24.2 32bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x1c93560;
                    break;
                case 0x5eec000:
                    // version = "1.24.2/1.24.3 64bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x25bc280;
                    break;
                case 0x42c7000:
                    // version = "1.24.3/1.26.1 32bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x1c94560;
                    break;
                case 0x5e83000:
                    //  version = "1.26.1 64 bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x2553280;
                    break;
                case 0x3a44000:
                    // version = "1.27.1 32bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x13ff160;
                    break;
                case 0x55f1000:
                    // version = "1.27.1 64bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x1cade80;
                    break;
                case 0x3C81000:
                    // version = "1.29.0 32bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x1438160;
                    break;
                case 0x574B000:
                    // version = "1.29.0 64bit";
                    _baseRAMAddress = mainModule.BaseAddress + 0x1C03E80;
                    break;
                default:
                    _baseRAMAddress = IntPtr.Zero;
                    break;
            }
        }
    }
}

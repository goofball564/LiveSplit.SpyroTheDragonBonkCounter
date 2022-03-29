using System;
using System.Diagnostics;
using LiveSplit.ComponentUtil;
using PropertyHook;

namespace LiveSplit.SpyroTheDragonBonkCounter.Hook
{
    public class SpyroTheDragonHook : PHook
    {
        private const string MEDNAFEN_NAME = "mednafen";
        private const string DUCKSTATION_NOGUI_NAME = "duckstation-nogui-x64-ReleaseLTCG";
        private const string DUCKSTATION_QT_NAME = "duckstation-qt-x64-ReleaseLTCG";
        private const string EPSXE_NAME = "ePSXe";

        private const int ANIMATION_OFFSET = 0x76E90;
        private const ushort BONK_ANIMATION = 0xF;

        // Arguments to PHook constructor.
        private const int REFRESH_INTERVAL = 5000;
        private const int MIN_LIFESPAN = 5000;
        private static Func<Process, bool> processSelector = (p) =>
        {
            return (p.ProcessName == MEDNAFEN_NAME)
            || (p.ProcessName == DUCKSTATION_NOGUI_NAME)
            || (p.ProcessName == DUCKSTATION_QT_NAME)
            || (p.ProcessName == EPSXE_NAME);
        };

        public bool IsBonking
        {
            get => ReadGameMemory<ushort>(ANIMATION_OFFSET, out ushort animation) ? animation == BONK_ANIMATION : false;
        }
        public bool Ready
        {
            get => Hooked && Emulator != null;
        }

        private Emulator Emulator { get; set; }

        public SpyroTheDragonHook() : base(REFRESH_INTERVAL, MIN_LIFESPAN, processSelector)
        {
            OnHooked += Emulator_OnHooked;
            OnUnhooked += Emulator_OnUnhooked;
            Start();
        }

        public void Dispose()
        {
            Emulator?.Dispose();
            Stop();
        }

        private bool ReadGameMemory<T>(int offset, out T memoryValue) where T : struct
        {
            try
            {
                if (Ready)
                {
                    IntPtr baseRAMAddress = Emulator.BaseRAMAddress;
                    if (baseRAMAddress != IntPtr.Zero)
                    {
                        memoryValue = Process.ReadValue<T>(baseRAMAddress + offset);
                        return true;
                    }
                }
            }
            catch (NullReferenceException)
            {
                memoryValue = default;
                return false;
            }

            memoryValue = default;
            return false;
        }

        private void Emulator_OnHooked(object sender, PHEventArgs e)
        {
            switch (Process.ProcessName)
            {
                case MEDNAFEN_NAME:
                    Emulator = new Mednafen(Process);
                    break;
                case DUCKSTATION_NOGUI_NAME:
                case DUCKSTATION_QT_NAME:
                    Emulator = new Duckstation(Process);
                    break;
                case EPSXE_NAME:
                    Emulator = new EPSXe(Process);
                    break;
                default:
                    Emulator = null;
                    break;
            }
        }

        private void Emulator_OnUnhooked(object sender, PHEventArgs e)
        {
            Emulator?.Dispose();
            Emulator = null;
        }
    }
}

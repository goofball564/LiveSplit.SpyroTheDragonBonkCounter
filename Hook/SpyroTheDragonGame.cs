using System;
using System.Diagnostics;
using System.Threading;
using LiveSplit.ComponentUtil;
using PropertyHook;

namespace LiveSplit.SpyroTheDragonMusicPlayer.Hook
{
    public class SpyroTheDragonGame : PHook
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
            || (p.ProcessName == DUCKSTATION_QT_NAME);
        };

        private IntPtr _emulatorBaseRAMAddress = IntPtr.Zero;

        private Thread _refreshBaseRAMAddressThread;
        private CancellationTokenSource _threadCancellationSource;

        public int RamAddressPollingInterval { get; set; }

        private ushort _oldAnimation = 0;
        public bool IsBonking
        {
            get
            {
                bool returnVal = false;
                ushort currentAnimation = 0;

                if (Ready)
                    currentAnimation = Process.ReadValue<ushort>(_emulatorBaseRAMAddress + ANIMATION_OFFSET);
               
                if (currentAnimation == BONK_ANIMATION)
                {
                    if (currentAnimation != _oldAnimation)
                        _emulatorBaseRAMAddress = Emulator.BaseRAMAddress;
                    if (_emulatorBaseRAMAddress != IntPtr.Zero)
                        returnVal = true;
                }

                _oldAnimation = currentAnimation;
                return returnVal;
            }
        }
        public bool Ready
        {
            get => Hooked && Emulator != null && _emulatorBaseRAMAddress != IntPtr.Zero;
        }

        private Emulator Emulator { get; set; }

        public SpyroTheDragonGame(int ramAddressPollingInterval) : base(REFRESH_INTERVAL, MIN_LIFESPAN, processSelector)
        {
            RamAddressPollingInterval = ramAddressPollingInterval;
            OnHooked += Emulator_OnHooked;
            OnUnhooked += Emulator_OnUnhooked;
            Start();
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
            StartRAMAddressPollingThread();
        }

        private void Emulator_OnUnhooked(object sender, PHEventArgs e)
        {
            StopRAMAddressPollingThread();
            Emulator = null;
            _emulatorBaseRAMAddress = IntPtr.Zero;
        }

        private void StartRAMAddressPollingThread()
        {
            if (_refreshBaseRAMAddressThread == null)
            {
                _threadCancellationSource = new CancellationTokenSource();
                var threadStart = new ThreadStart(() => PollBaseRAMAddress(_threadCancellationSource.Token));
                _refreshBaseRAMAddressThread = new Thread(threadStart);
                _refreshBaseRAMAddressThread.IsBackground = true;
                _refreshBaseRAMAddressThread.Start();
            }
        }

        private void StopRAMAddressPollingThread()
        {
            if (_refreshBaseRAMAddressThread != null)
            {
                _threadCancellationSource.Cancel();
                _refreshBaseRAMAddressThread = null;
                _threadCancellationSource = null;
            }
        }

        // Periodically poll for Base RAM Address of game in emulator's memory; this is done instead of simply
        // determining the Base RAM Address once when the emulator starts because Duckstation's Base RAM Address
        // changes when the game is stopped and restarted.
        private void PollBaseRAMAddress(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                _emulatorBaseRAMAddress = Emulator.BaseRAMAddress;
                Thread.Sleep(RamAddressPollingInterval);
            }
        }

        public void Dispose()
        {
            StopRAMAddressPollingThread();
            Stop();
        }

    }
}

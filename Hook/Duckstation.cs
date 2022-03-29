using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace LiveSplit.SpyroTheDragonBonkCounter.Hook
{
    class Duckstation : Emulator
    {
        private const int RAM_ADDRESS_POLLING_INTERVAL = 1500;

        private IntPtr _baseRAMAddress = IntPtr.Zero;
        private Thread _refreshBaseRAMAddressThread;
        private CancellationTokenSource _threadCancellationSource;

        private readonly object locker = new object();

        public override IntPtr BaseRAMAddress
        {
            get
            {
                lock (locker)
                {
                    if (VerifyBaseRAMAddress())
                        return _baseRAMAddress;
                    else
                        return IntPtr.Zero;
                }
            }
        }

        public Duckstation(Process emulatorProcess) : base(emulatorProcess)
        {
            StartRAMAddressPollingThread();
        }

        public override void Dispose()
        {
            StopRAMAddressPollingThread();
        }

        private bool VerifyBaseRAMAddress()
        {
            return emulatorProcess.ReadPointer(_baseRAMAddress, out _);
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
        // Apparently this works for most or all versions of Duckstation.
        // IntPtr.Zero if no matching memory page is found,
        // which means Duckstation is loaded but a game isn't.
        private void PollBaseRAMAddress(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                IntPtr temp = emulatorProcess.MemoryPages(true).Where(p => p.Type == MemPageType.MEM_MAPPED && p.RegionSize == (UIntPtr)0x200000).FirstOrDefault().BaseAddress;
                lock (locker)
                {
                    _baseRAMAddress = temp;
                }
                Thread.Sleep(RAM_ADDRESS_POLLING_INTERVAL);
            }
        }
    }
}

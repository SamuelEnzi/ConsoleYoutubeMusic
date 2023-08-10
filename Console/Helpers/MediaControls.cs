using System.Runtime.InteropServices;

namespace Console.Helpers
{
    internal class MediaControls
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_NONE = 0;
        private const uint VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const uint VK_MEDIA_STOP = 0xB2;
        private const uint VK_MEDIA_NEXT_TRACK = 0xB0;
        private const uint VK_MEDIA_PREV_TRACK = 0xB1;
        private const uint VK_VOLUME_UP = 0xAF;
        private const uint VK_VOLUME_DOWN = 0xAE;

        public event EventHandler? PlayPausePressed;
        public event EventHandler? StopPressed;
        public event EventHandler? NextTrackPressed;
        public event EventHandler? PrevTrackPressed;
        public event EventHandler? VolumeUpPressed;
        public event EventHandler? VolumeDownPressed;

        private Thread? messageThread;

        public void Start()
        {
            messageThread = new Thread(MessageThread);
            messageThread.Start();
        }

        private void MessageThread()
        {
            IntPtr hWnd = CreateMessageWindow();

            RegisterHotKey(hWnd, 1, MOD_NONE, VK_MEDIA_PLAY_PAUSE);
            RegisterHotKey(hWnd, 2, MOD_NONE, VK_MEDIA_STOP);
            RegisterHotKey(hWnd, 3, MOD_NONE, VK_MEDIA_NEXT_TRACK);
            RegisterHotKey(hWnd, 4, MOD_NONE, VK_MEDIA_PREV_TRACK);
            RegisterHotKey(hWnd, 5, MOD_NONE, VK_VOLUME_UP);
            RegisterHotKey(hWnd, 6, MOD_NONE, VK_VOLUME_DOWN);

            while (true)
            {
                try
                {
                    GetMessage(out var msg, IntPtr.Zero, 0, 0);
                    if (msg.message == WM_HOTKEY)
                    {
                        switch (msg.wParam.ToInt32())
                        {
                            case 1:
                                PlayPausePressed?.Invoke(this, EventArgs.Empty);
                                break;
                            case 2:
                                StopPressed?.Invoke(this, EventArgs.Empty);
                                break;
                            case 3:
                                NextTrackPressed?.Invoke(this, EventArgs.Empty);
                                break;
                            case 4:
                                PrevTrackPressed?.Invoke(this, EventArgs.Empty);
                                break;
                            case 5:
                                VolumeUpPressed?.Invoke(this, EventArgs.Empty);
                                break;
                            case 6:
                                VolumeDownPressed?.Invoke(this, EventArgs.Empty);
                                break;
                        }
                    }

                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
                catch
                {

                }
            }
        }

        private IntPtr CreateMessageWindow()
        {
            var wc = new WNDCLASS
            {
                lpfnWndProc = WndProcs,
                lpszClassName = "MediaKeyMessageWindow",
                hInstance = GetModuleHandle(null) // Get the handle of the current module
            };
            RegisterClass(ref wc);
            return CreateWindowEx(0, wc.lpszClassName, "", 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, wc.hInstance, IntPtr.Zero);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short RegisterClass(ref WNDCLASS wc);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage(ref MSG lpMsg);

        private IntPtr WndProcs(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WNDCLASS
        {
            public uint style;
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)] public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)] public string lpszClassName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hWnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
            public uint lPrivate; // Added this field
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }
    }
}

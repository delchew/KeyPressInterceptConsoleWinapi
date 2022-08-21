using System;
using System.Runtime.InteropServices;

namespace KeyPressInterceptConsoleWinapi
{
	public class Hook : IDisposable
	{
        private delegate IntPtr KeyboardHookProc(int code, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_KEYDOWN = 0x0100;

        // код клавиши на которую ставим хук
        private int _key;
        public event Action KeyPressed;

        private readonly KeyboardHookProc _proc;
        private IntPtr _hHook = IntPtr.Zero;

        public Hook(int keyCode)
        {
            _key = keyCode;
            _proc = HookProc;
        }

        public void SetHook()
        {
            var hInstance = LoadLibrary("User32");
            _hHook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        public void UnHook() => UnhookWindowsHookEx(_hHook);


        public void Dispose() => UnHook();


        private IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WH_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                KeyPressed?.Invoke();
            }

            // пробрасываем хук дальше
            return CallNextHookEx(_hHook, code, (int)wParam, lParam);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);
    }
}
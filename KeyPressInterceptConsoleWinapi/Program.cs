using System;
using System.Runtime.InteropServices;

namespace KeyPressInterceptConsoleWinapi
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        static void Main()
        {
            keybd_event(0x90, 0x45, 0x1, (UIntPtr)0);

            Console.WriteLine("Счетчик нажатия клавиши Caps Lock.\nДля завершения программы и вывода количества нажатий нажмите Esc.");

            //https://docs.microsoft.com/ru-ru/windows/win32/inputdev/virtual-key-codes
            int сapsLockKeyCode = 0x14;
            int counter = 0;

            using var hook = new Hook(сapsLockKeyCode);
            hook.KeyPressed += () => counter++;
            hook.SetHook();

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Escape)
                    break;
            }

            Console.WriteLine("Клавиша Caps Lock была нажата {0} раз.", counter);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SurfacePenNavigator
{
    internal class KeyboardHook : IDisposable
    {
        private int _hookHandle = 0;
        private List<Keys> _watchedKeys = new List<Keys>();

        /// <summary>
        /// Occurs when the watched key was triggered (pressed)
        /// </summary>
        public event EventHandler<Keys> KeyTriggered;

        /// <summary>
        /// Gets whether the hook is enabled (installed)
        /// </summary>
        public bool Enabled => _hookHandle != 0;

        public void Dispose()
        {
            Disable();
        }

        /// <summary>
        /// Enable the hook
        /// </summary>
        /// <returns></returns>
        public bool Enable()
        {
            var hookProcedure = new NativeMethods.HookProc(KeyHookProc);

            _hookHandle = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, hookProcedure, IntPtr.Zero, 0);

            return _hookHandle != 0;
        }

        /// <summary>
        /// Disable the hook
        /// </summary>
        public void Disable()
        {
            if (_hookHandle != 0)
                NativeMethods.UnhookWindowsHookEx(_hookHandle);

            _hookHandle = 0;
        }

        /// <summary>
        /// Add a key to watch for
        /// </summary>
        /// <param name="key"></param>
        public void AddKeyToWatch(Keys key)
        {
            _watchedKeys.Add(key);
        }

        private int KeyHookProc(int nCode, int wParam, IntPtr lParam)
        {
            // Marshall the data from the callback.
            var data = Marshal.PtrToStructure<NativeMethods.KeyboardHookStruct>(lParam);
            var key = (Keys)data.vkCode;

            if (_watchedKeys.Contains(key))
            {
                if (wParam == NativeMethods.WM_KEYDOWN)
                    KeyTriggered?.Invoke(this, key);

                return 1;
            }

            return NativeMethods.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }
    }
}

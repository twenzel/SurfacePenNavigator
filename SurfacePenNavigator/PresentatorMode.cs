using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SurfacePenNavigator
{
    /// <summary>
    /// The Surface pen presentator mode
    /// </summary>
    class PresentatorMode : IDisposable
    {
        private KeyboardHook _hook;

        /// <summary>
        /// Gets whether the presentator mode is enabled
        /// </summary>
        public bool Enabled => _hook.Enabled;

        public PresentatorMode()
        {
            InitHook();
        }     

        public void Dispose()
        {
            _hook.Dispose();
        }

        /// <summary>
        /// Toggles the mode (enabled/disabled)
        /// </summary>
        public void Toggle()
        {
            if (_hook.Enabled)
                _hook.Disable();
            else
                _hook.Enable();
        }

        /// <summary>
        /// Disables the presentation mode
        /// </summary>
        public void Disable()
        {
            if (_hook.Enabled)
                _hook.Disable();
        }

        private void InitHook()
        {
            _hook = new KeyboardHook();
            _hook.KeyTriggered += _hook_KeyTriggered;
            _hook.AddKeyToWatch(Keys.F18);
            _hook.AddKeyToWatch(Keys.F19);
            _hook.AddKeyToWatch(Keys.F20);

            // required because this key is also triggered when pressing the pen
            // we should suppress this key otherwise the start menu will be opened
            _hook.AddKeyToWatch(Keys.LWin);
        }

        private void _hook_KeyTriggered(object sender, Keys e)
        {           
            if (e == Keys.F20)
                SendKeys.Send("{DOWN}"); // Next slide
            else if (e == Keys.F19)
                SendKeys.Send("{UP}"); // Previous slide
            else if (e == Keys.F18)
                SendKeys.Send("{HOME}"); // First slide
        }
    }
}

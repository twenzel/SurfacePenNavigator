using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SurfacePenNavigator
{
    /// <summary>
    /// Application context to host a notify icon only (without hosting a Form)
    /// </summary>
    class TrayIconApplicationContext : ApplicationContext
    {        
        private NotifyIcon _notifyIcon;
        private MenuItem _toggleMenuItem;
        private PresentatorMode _presentatorMode;        

        public TrayIconApplicationContext()
        {
            _presentatorMode = new PresentatorMode();
            CreateNotifyIcon();
            SubscribeSystemEvents();
        }

        private void SubscribeSystemEvents()
        {
            SystemEvents.PowerModeChanged += (s, e) => {
                if (e.Mode == PowerModes.Suspend)
                    DisablePresentationMode();
            };

            SystemEvents.SessionEnded += (s, e) => {
                DisablePresentationMode();
            };

            SystemEvents.SessionSwitch += (s, e) => {
                if (e.Reason == SessionSwitchReason.SessionLogoff)
                    DisablePresentationMode();
            };

        }

        private void CreateNotifyIcon()
        {
            _toggleMenuItem = new MenuItem("Enable Presenter Mode", new EventHandler(ToggleMode));
            MenuItem aboutMenuItem = new MenuItem("About", new EventHandler(ShowAbout));           
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = SurfacePenNavigator.Properties.Resources.pen_white;
            _notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { _toggleMenuItem, new MenuItem("-"), aboutMenuItem, exitMenuItem });
            _notifyIcon.Visible = true;
            _notifyIcon.DoubleClick += new EventHandler(ToggleMode);
            UpdateModeTexts();
        }

        private void ToggleMode(object sender, EventArgs e)
        {
            _presentatorMode.Toggle();
            UpdateModeTexts();
        }

        private void UpdateModeTexts()
        {
            _toggleMenuItem.Text = _presentatorMode.Enabled ? "Disable Presenter Mode" : "Enable Presenter Mode";
            _notifyIcon.Text = "Presenter mode: " + (_presentatorMode.Enabled ? "enabled" : "disabled");
            _notifyIcon.Icon = _presentatorMode.Enabled ? Properties.Resources.pen_white_filled : Properties.Resources.pen_white;
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            using (var form = new AboutForm())
            {
                form.ShowDialog();
            }          
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _presentatorMode.Dispose();

            base.Dispose(disposing);
        }

        private void DisablePresentationMode()
        {
            _presentatorMode.Disable();
            UpdateModeTexts();
        }

        private void Exit(object sender, EventArgs e)
        {
            DisablePresentationMode();

            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            _notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}

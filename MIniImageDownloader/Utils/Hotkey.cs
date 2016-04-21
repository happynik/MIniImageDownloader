using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml.Serialization;
using Application = System.Windows.Forms.Application;

namespace MIniImageDownloader.Utils
{
    public class Hotkey : IMessageFilter
    {
        #region Interop

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);

        [DllImport("user32.dll", SetLastError=true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        private const uint WM_HOTKEY = 0x312;

        private const uint MOD_ALT = 0x1;
        private const uint MOD_CONTROL = 0x2;
        private const uint MOD_SHIFT = 0x4;
        private const uint MOD_WIN = 0x8;

        private const uint ERROR_HOTKEY_ALREADY_REGISTERED = 1409;

        #endregion

        private static int _currentId;
        private const int MaximumId = 0xBFFF;
        
        private Keys _keyCode;
        private bool _shift;
        private bool _control;
        private bool _alt;
        private bool _windows;

        [XmlIgnore]
        private int _id;
        [XmlIgnore]
        private bool _registered;
        [XmlIgnore]
        //private Control _windowControl;
        private Window _windowControl;

        private HwndSource _source;

        public event HandledEventHandler Pressed;

        public Hotkey() : this(Keys.None, false, false, false, false)
        {
            // No work done here!
        }
        
        public Hotkey(Keys keyCode, bool shift, bool control, bool alt, bool windows)
        {
            // Assign properties
            KeyCode = keyCode;
            Shift = shift;
            Control = control;
            Alt = alt;
            Windows = windows;

            // Register us as a message filter
            Application.AddMessageFilter(this);
        }

        ~Hotkey()
        {
            // Unregister the hotkey if necessary
            if (Registered)
            { Unregister(); }
        }

        public Hotkey Clone()
        {
            // Clone the whole object
            return new Hotkey(_keyCode, _shift, _control, _alt, _windows);
        }

        public bool GetCanRegister(Window windowControl)
        {
            // Handle any exceptions: they mean "no, you can't register" :)
            try
            {
                // Attempt to register
                if (!Register(windowControl))
                { return false; }

                // Unregister and say we managed it
                Unregister();
                return true;
            }
            catch (Win32Exception)
            { return false; }
            catch (NotSupportedException)
            { return false; }
        }

        public bool Register(Window windowControl)
        {
            // Check that we have not _registered
            if (_registered)
            { throw new NotSupportedException("You cannot register a hotkey that is already _registered"); }
        
            // We can't register an empty hotkey
            if (Empty)
            { throw new NotSupportedException("You cannot register an empty hotkey"); }

            // Get an ID for the hotkey and increase current ID
            _id = _currentId;
            _currentId = _currentId + 1 % MaximumId;

            // Translate modifier keys into unmanaged version
            uint modifiers = (Alt ? MOD_ALT : 0) | (Control ? MOD_CONTROL : 0) |
                            (Shift ? MOD_SHIFT : 0) | (Windows ? MOD_WIN : 0);

            // Register the hotkey
            var helper = new WindowInteropHelper(windowControl);
            var currentHandle = helper.Handle.ToInt64();
            var handle = currentHandle == 0 ? helper.EnsureHandle() : helper.Handle;
            _source = HwndSource.FromHwnd(handle);
            _source.AddHook(HwndHook);
            if (RegisterHotKey(helper.Handle, _id, modifiers, _keyCode) == 0)
            {
                // Is the error that the hotkey is _registered?
                if (Marshal.GetLastWin32Error() == ERROR_HOTKEY_ALREADY_REGISTERED)
                { return false; }
                throw new Win32Exception();
            }

            // Save the _control reference and register state
            _registered = true;
            _windowControl = windowControl;

            // We successfully _registered
            return true;
        }

        public void Unregister()
        {
            // Check that we have _registered
            if (!_registered)
            { throw new NotSupportedException("You cannot unregister a hotkey that is not _registered"); }

            // It's possible that the _control itself has died: in that case, no need to unregister!
            var helper = new WindowInteropHelper(_windowControl);
            _source.RemoveHook(HwndHook);
            _source = null;
            // Clean up after ourselves
            if (UnregisterHotKey(helper.Handle, _id) == 0)
            { throw new Win32Exception(); }

            // Clear the _control reference and register state
            _registered = false;
            _windowControl = null;
        }

        private void Reregister()
        {
            // Only do something if the key is already _registered
            if (!_registered)
            { return; }

            // Save _control reference
            Window windowControl = _windowControl;

            // Unregister and then reregister again
            Unregister();
            Register(windowControl);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    if (wParam.ToInt32() == _id)
                    {
                        OnPressed();
                        handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        public bool PreFilterMessage(ref Message message)
        {
            // Only process WM_HOTKEY messages
            if (message.Msg != WM_HOTKEY)
            { return false; }

            // Check that the ID is our key and we are registerd
            if (_registered && (message.WParam.ToInt32() == _id))
            {
                // Fire the event and pass on the event if our handlers didn't handle it
                return OnPressed();
            }
            else
            { return false; }
        }

        private bool OnPressed()
        {
            // Fire the event if we can
            HandledEventArgs handledEventArgs = new HandledEventArgs(false);
            Pressed?.Invoke(this, handledEventArgs);

            // Return whether we handled the event or not
            return handledEventArgs.Handled;
        }

        public override string ToString()
        {
            // We can be empty
            if (Empty)
            { return "(none)"; }

            // Build key name
            string keyName = Enum.GetName(typeof(Keys), _keyCode);
            switch (_keyCode)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    // Strip the first character
                    keyName = keyName?.Substring(1);
                    break;
                default:
                    // Leave everything alone
                    break;
            }

            // Build modifiers
            string modifiers = "";
            if (_shift)
            { modifiers += "Shift+"; }
            if (_control)
            { modifiers += "Control+"; }
            if (_alt)
            { modifiers += "Alt+"; }
            if (_windows)
            { modifiers += "Windows+"; }

            // Return result
            return modifiers + keyName;
        }

        public bool Empty => _keyCode == Keys.None;

        public bool Registered => _registered;

        public Keys KeyCode
        {
            get { return _keyCode; }
            set
            {
                // Save and reregister
                _keyCode = value;
                Reregister();
            }
        }

        public bool Shift
        {
            get { return _shift; }
            set 
            {
                // Save and reregister
                _shift = value;
                Reregister();
            }
        }

        public bool Control
        {
            get { return _control; }
            set
            { 
                // Save and reregister
                _control = value;
                Reregister();
            }
        }

        public bool Alt
        {
            get { return _alt; }
            set
            { 
                // Save and reregister
                _alt = value;
                Reregister();
            }
        }

        public bool Windows
        {
            get { return _windows; }
            set 
            {
                // Save and reregister
                _windows = value;
                Reregister();
            }
        }
    }
}

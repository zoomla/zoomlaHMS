using CefSharp.Wpf;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZoomlaHms.Common;
using System.Windows.Interop;

namespace ZoomlaHms
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly KeyboardHook keyboardHook;

        public App()
        {
            keyboardHook = new KeyboardHook();
            keyboardHook.OnKeyDownEvent += KeyboardHook_OnKeyDownEvent;
            keyboardHook.OnKeyUpEvent += KeyboardHook_OnKeyUpEvent;

            this.Info("ctor", "CefSharp init.");
            var sett = new CefSettings();
            sett.CefCommandLineArgs.Add("--allow-file-access-from-files");
            sett.CefCommandLineArgs.Add("--disable-web-security");
            Cef.Initialize(sett);

            try
            {
                this.Info("ctor", "Clear temporary file.");
                foreach (var item in System.IO.Directory.GetFileSystemEntries(SystemPath.TempFileDirectory))
                {
                    if (System.IO.File.Exists(item))
                    { System.IO.File.Delete(item); }
                    else if (System.IO.Directory.Exists(item))
                    { System.IO.Directory.Delete(item, true); }
                }
            }
            catch (Exception ex)
            {
                this.Error("ctor", "Cannot clear temporary file.", ex);
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logging.Error(sender, "<unknown>", "Unhandled exception.", e.Exception);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            keyboardHook.UnHook();
            Logging.Info(sender, "<unknown>", "Application shutdown.\n");
            Logging.SyncLogToFile();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Info(sender, "<unknown>", "Application startup.");
            keyboardHook.SetHook();
        }


        private bool
            ctrlL = false,
            ctrlR = false,
            altL = false,
            altR = false,
            shiftL = false,
            shiftR = false,
            winL = false,
            winR = false,
            num0 = false,
            num1 = false,
            num2 = false,
            num3 = false,
            num4 = false,
            num5 = false,
            num6 = false,
            num7 = false,
            num8 = false,
            num9 = false;

        private void KeyboardHook_OnKeyUpEvent(Key key)
        {
            switch (key)
            {
                case Key.LeftCtrl:
                    ctrlL = false;
                    break;
                case Key.RightCtrl:
                    ctrlR = false;
                    break;

                case Key.LeftAlt:
                    altL = false;
                    break;
                case Key.RightAlt:
                    altR = false;
                    break;

                case Key.LeftShift:
                    shiftL = false;
                    break;
                case Key.RightShift:
                    shiftR = false;
                    break;

                case Key.LWin:
                    winL = false;
                    break;
                case Key.RWin:
                    winR = false;
                    break;

                case Key.D1:
                    num1 = false;
                    break;
                case Key.D2:
                    num2 = false;
                    break;
                case Key.D3:
                    num3 = false;
                    break;
                case Key.D4:
                    num4 = false;
                    break;
                case Key.D5:
                    num5 = false;
                    break;
                case Key.D6:
                    num6 = false;
                    break;
                case Key.D7:
                    num7 = false;
                    break;
                case Key.D8:
                    num8 = false;
                    break;
                case Key.D9:
                    num9 = false;
                    break;
                case Key.D0:
                    num0 = false;
                    break;

                default:
                    break;
            }
        }

        private void KeyboardHook_OnKeyDownEvent(Key key)
        {
            switch (key)
            {
                case Key.LeftCtrl:
                    ctrlL = true;
                    break;
                case Key.RightCtrl:
                    ctrlR = true;
                    break;

                case Key.LeftAlt:
                    altL = true;
                    break;
                case Key.RightAlt:
                    altR = true;
                    break;

                case Key.LeftShift:
                    shiftL = true;
                    break;
                case Key.RightShift:
                    shiftR = true;
                    break;

                case Key.LWin:
                    winL = true;
                    break;
                case Key.RWin:
                    winR = true;
                    break;

                case Key.D1:
                    num1 = true;
                    break;
                case Key.D2:
                    num2 = true;
                    break;
                case Key.D3:
                    num3 = true;
                    break;
                case Key.D4:
                    num4 = true;
                    break;
                case Key.D5:
                    num5 = true;
                    break;
                case Key.D6:
                    num6 = true;
                    break;
                case Key.D7:
                    num7 = true;
                    break;
                case Key.D8:
                    num8 = true;
                    break;
                case Key.D9:
                    num9 = true;
                    break;
                case Key.D0:
                    num0 = true;
                    break;

                default:
                    break;
            }

            HandleHotKey();
        }

        private void HandleHotKey()
        {
            //ctrl + alt + 0: 唤起主窗体
            if ((ctrlL || ctrlR) && (altL || altR) && num0)
            {
                if (Application.Current.MainWindow != null)
                { Win32Api.SetWindowToForegroundWithAttachThreadInput(Application.Current.MainWindow); }
                return;
            }

            //ctrl + alt + 1: 主题包同步
            if ((ctrlL || ctrlR) && (altL || altR) && num1)
            {
                new SingleModule("themepack/paksync") { Title = "主题包同步" };
                return;
            }

            //ctrl + alt + 2: 主题包推送
            if ((ctrlL || ctrlR) && (altL || altR) && num2)
            {
                new SingleModule("themepack/syncmobile") { Title = "手机推送工具" };
                return;
            }

            //ctrl + alt + 3: 主题包审计
            if ((ctrlL || ctrlR) && (altL || altR) && num3)
            {
                new SingleModule("extend/hwtviewer") { Title = "主题包审计" };
                return;
            }

            //ctrl + alt + 4: 主题打包
            if ((ctrlL || ctrlR) && (altL || altR) && num4)
            {
                new SingleModule("themepack/packit") { Title = "主题打包" };
                return;
            }

            //ctrl + alt + 5: 取色工具
            if ((ctrlL || ctrlR) && (altL || altR) && num5)
            {
                new SingleModule("extend/takecolor") { Title = "屏幕取色" };
                return;
            }
        }
    }


    public class Win32Api
    {
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        public const int WH_KEYBOARD_LL = 13;

        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static void SetWindowToForegroundWithAttachThreadInput(Window window)
        {
            var interopHelper = new WindowInteropHelper(window);
            var thisWindwThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindow = GetForegroundWindow();
            var currentForegroudWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);

            AttachThreadInput(currentForegroudWindowThreadId, thisWindwThreadId, true);

            window.Show();
            window.Activate();

            AttachThreadInput(currentForegroudWindowThreadId, thisWindwThreadId, false);
            window.Topmost = true;
            window.Topmost = false;
        }
    }

    public class KeyboardHook
    {
        int hHook;
        Win32Api.HookProc KeyboardHookDelegate;

        public delegate void OnKeyDown(Key key);
        public delegate void OnKeyUp(Key key);

        public event OnKeyDown OnKeyDownEvent;
        public event OnKeyDown OnKeyUpEvent;

        public void SetHook()
        {
            KeyboardHookDelegate = new Win32Api.HookProc(KeyboardHookProc);
            ProcessModule cModule = Process.GetCurrentProcess().MainModule;

            var mh = Win32Api.GetModuleHandle(cModule.ModuleName);
            hHook = Win32Api.SetWindowsHookEx(Win32Api.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
        }

        public void UnHook()
        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }

        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                Win32Api.KeyboardHookStruct KeyDataFromHook = (Win32Api.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.KeyboardHookStruct));
                int keyData = KeyDataFromHook.vkCode;

                if (OnKeyDownEvent != null && (wParam == Win32Api.WM_KEYDOWN || wParam == Win32Api.WM_SYSKEYDOWN))
                {
                    OnKeyDownEvent.Invoke(KeyInterop.KeyFromVirtualKey(keyData));
                }

                if (OnKeyUpEvent != null && (wParam == Win32Api.WM_KEYUP || wParam == Win32Api.WM_SYSKEYUP))
                {
                    OnKeyUpEvent.Invoke(KeyInterop.KeyFromVirtualKey(keyData));
                }
            }

            return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
        }
    }
}

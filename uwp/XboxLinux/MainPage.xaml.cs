using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Xbox_Linux
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Dictionary<string, string> keyCodes = new Dictionary<string, string>()
        {
            { "A", "[0x1E]" },
            { "B", "[0x30]" },
            { "C", "[0x2E]" },
            { "D", "[0x20]" },
            { "E", "[0x12]" },
            { "F", "[0x21]" },
            { "G", "[0x22]" },
            { "H", "[0x23]" },
            { "I", "[0x17]" },
            { "J", "[0x24]" },
            { "K", "[0x25]" },
            { "L", "[0x26]" },
            { "M", "[0x32]" },
            { "N", "[0x31]" },
            { "O", "[0x18]" },
            { "P", "[0x19]" },
            { "Q", "[0x10]" },
            { "R", "[0x13]" },
            { "S", "[0x1F]" },
            { "T", "[0x14]" },
            { "U", "[0x16]" },
            { "V", "[0x2F]" },
            { "W", "[0x11]" },
            { "X", "[0x2D]" },
            { "Y", "[0x15]" },
            { "0", "[0x0B]" },
            { "1", "[0x02]" },
            { "2", "[0x03]" },
            { "3", "[0x04]" },
            { "4", "[0x05]" },
            { "5", "[0x06]" },
            { "6", "[0x07]" },
            { "7", "[0x08]" },
            { "8", "[0x09]" },
            { "9", "[0x0A]" },
            { "Enter", "[0x1C]" },
            { "Backspace", "[0x0E]" },
            { "Del", "[0x53]" },
            { "Space", "[0x39]" },
            { "←", "[0x4B]" },
            { "↑", "[0x48]" },
            { "→", "[0x4D]" },
            { "↓", "[0x50]" },
            { "Point", "[0x34]" },
            { "Bar1", "[0x35]" },
            { "Bar2", "[0x2B]" },
            { "Key1", "[0x1A]" },
            { "Key2", "[0x1B]" },
            { "SemiPoint", "[0x27]" },
            { "Apostrophe", "[0x28]" },
            { "Bar3", "[0x0C]" },
            { "Add1", "[0x0D]" },
            { "Accent", "[0x29]" },
            { "Capital", "[0x3A]" },
            { "Escape", "[0x01]" },
            { "TAB", "[0x0F]" },
            { "F1", "[0x3B]" },
            { "F2", "[0x3C]" },
            { "F3", "[0x3D]" },
            { "F4", "[0x3E]" },
            { "F5", "[0x3F]" },
            { "F6", "[0x40]" },
            { "F7", "[0x41]" },
            { "F8", "[0x42]" },
            { "F9", "[0x43]" },
            { "F10", "[0x44]" },
            { "F11", "[0x57]" },
            { "F12", "[0x58]" }
        };

        public MainPage()
        {
            this.InitializeComponent();

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await Terminal.EnsureCoreWebView2Async();

            // Set the mapping from a virtual host name to a folder in the package
            Terminal.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "app.example", // The virtual host name mapped to virtual machines
                               // - leave as app.example for performance!
                "Virtual_Machines", // v86 virtual machine resource
                CoreWebView2HostResourceAccessKind.Allow // Disable CORS
            );

            Terminal.CoreWebView2.Settings.IsScriptEnabled = true;
            Terminal.CoreWebView2.Settings.IsWebMessageEnabled = true;
            Terminal.CoreWebView2.Settings.IsNonClientRegionSupportEnabled = true;
            Terminal.CoreWebView2.Settings.AreHostObjectsAllowed = true;
            Terminal.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            Terminal.CoreWebView2.Settings.IsZoomControlEnabled = false;
            Terminal.CoreWebView2.Settings.AreDevToolsEnabled = false;
            Terminal.Source = new Uri("http://app.example/index.html");
        }

        private void Terminal_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {

        }

        private void Terminal_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {

        }

        private async void AllButtons_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string key)
            {
                if (keyCodes.TryGetValue(key, out string scanCode))
                {
                    string script = $@"
                        var code = {scanCode};
                        console.log('sending keys ' + code);
                        emulator.keyboard_send_scancodes(code);
                    ";
                    await Terminal.CoreWebView2.ExecuteScriptAsync(script);
                }
                else
                {
                    Console.WriteLine("Key not found: " + key);
                }
            }
        }

        private void ButtonK2_Click(object sender, RoutedEventArgs e)
        {
            // Alterna entre Visível e Colapsado
            if (VirtualKeyboard2.Visibility == Visibility.Visible)
            {
                VirtualKeyboard2.Visibility = Visibility.Collapsed;
            }
            else
            {
                VirtualKeyboard2.Visibility = Visibility.Visible;
            }
        }

        private void ButtonK3_Click(object sender, RoutedEventArgs e)
        {
            if (VirtualKeyboard3.Visibility == Visibility.Visible)
            {
                VirtualKeyboard3.Visibility = Visibility.Collapsed;
            }
            else
            {
                VirtualKeyboard3.Visibility = Visibility.Visible;
            }
        }

        private void ButtonK4_Click(object sender, RoutedEventArgs e)
        {
            if (VirtualKeyboard4.Visibility == Visibility.Visible)
            {
                VirtualKeyboard4.Visibility = Visibility.Collapsed;
            }
            else
            {
                VirtualKeyboard4.Visibility = Visibility.Visible;
            }
        }

        private void ButtonK1_Click(object sender, RoutedEventArgs e)
        {
            if (VirtualKeyboard1.Visibility == Visibility.Visible)
            {
                VirtualKeyboard1.Visibility = Visibility.Collapsed;
            }
            else
            {
                VirtualKeyboard1.Visibility = Visibility.Visible;
            }
        }

        private void ButtonK5_Click(object sender, RoutedEventArgs e)
        {
            if (VirtualKeyboard5.Visibility == Visibility.Visible)
            {
                VirtualKeyboard5.Visibility = Visibility.Collapsed;
            }
            else
            {
                VirtualKeyboard5.Visibility = Visibility.Visible;
            }
        }
    }
}

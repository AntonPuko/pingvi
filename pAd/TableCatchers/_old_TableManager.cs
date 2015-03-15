using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Pingvi
{
    public class TableManager
    {
        public string TableWindowClassName { get; set; }

        private AutomationElement _observingTable;

        public event Action<AutomationElement> NewTable; 

        public TableManager() {
            TableWindowClassName = "PokerStarsTableFrameClass";
        }

        public void Start() {
            _observingTable = AutomationElement.RootElement;
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);
        }

        public void Stop() {
            Automation.RemoveAutomationFocusChangedEventHandler(OnFocusChanged);
            _observingTable = null;
        }
       
        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e) {
            try {
                AutomationElement focusedElement = (AutomationElement) sender;
                TreeWalker walker = TreeWalker.RawViewWalker;

                AutomationElement topLevelWindow = GetTopLevelWindow(focusedElement);
                if (topLevelWindow == null) return;
                if (topLevelWindow != _observingTable
                    && (string) topLevelWindow.GetCurrentPropertyValue(AutomationElement.ClassNameProperty)
                    == TableWindowClassName) {
                    _observingTable = topLevelWindow;

                    if (NewTable != null) {
                        NewTable(_observingTable);
                    }
                }
            }
            catch (ElementNotAvailableException ex) {
                MessageBox.Show(ex.InnerException.Message + "In method OnFocusChanged");
            }
        }

        private AutomationElement GetTopLevelWindow(AutomationElement element) {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            
            AutomationElement elementParent;
            AutomationElement node = element;
            try {
                if (node == AutomationElement.RootElement) return node;
                while (true) {
                    elementParent = walker.GetParent(node);
                    if (elementParent == null) return null;
                    if (elementParent == AutomationElement.RootElement) break;
                    node = elementParent;
                }
            }
            catch (ElementNotAvailableException) {
                node = null;
            }
            catch (ArgumentNullException) {
                node = null;
            }
            return node;
        }

    

      
   

      
       
    }
}

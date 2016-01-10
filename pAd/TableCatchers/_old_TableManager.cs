using System;
using System.Windows;
using System.Windows.Automation;

namespace Pingvi
{
    public class TableManager
    {
        private AutomationElement _observingTable;

        public TableManager() {
            TableWindowClassName = "PokerStarsTableFrameClass";
        }

        public string TableWindowClassName { get; set; }

        public event Action<AutomationElement> NewTable;

        public void Start() {
            _observingTable = AutomationElement.RootElement;
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);
        }

        public void Stop() {
            Automation.RemoveAutomationFocusChangedEventHandler(OnFocusChanged);
            _observingTable = null;
        }

        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e) {
            try
            {
                var focusedElement = (AutomationElement) sender;
                var walker = TreeWalker.RawViewWalker;

                var topLevelWindow = GetTopLevelWindow(focusedElement);
                if (topLevelWindow == null) return;
                if (topLevelWindow != _observingTable
                    && (string) topLevelWindow.GetCurrentPropertyValue(AutomationElement.ClassNameProperty)
                    == TableWindowClassName)
                {
                    _observingTable = topLevelWindow;

                    if (NewTable != null)
                    {
                        NewTable(_observingTable);
                    }
                }
            }
            catch (ElementNotAvailableException ex)
            {
                MessageBox.Show(ex.InnerException.Message + "In method OnFocusChanged");
            }
        }

        private AutomationElement GetTopLevelWindow(AutomationElement element) {
            var walker = TreeWalker.ControlViewWalker;

            AutomationElement elementParent;
            var node = element;
            try
            {
                if (node == AutomationElement.RootElement) return node;
                while (true)
                {
                    elementParent = walker.GetParent(node);
                    if (elementParent == null) return null;
                    if (elementParent == AutomationElement.RootElement) break;
                    node = elementParent;
                }
            }
            catch (ElementNotAvailableException)
            {
                node = null;
            }
            catch (ArgumentNullException)
            {
                node = null;
            }
            return node;
        }
    }
}
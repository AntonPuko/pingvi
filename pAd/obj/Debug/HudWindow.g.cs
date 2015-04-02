﻿#pragma checksum "..\..\HudWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "247B4C77F92BC3A76C8927104DED6AFC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Pingvi {
    
    
    /// <summary>
    /// HudWindow
    /// </summary>
    public partial class HudWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border HudBorder;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label StackLabel;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label PotOddsLabel;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run HeroStateRun;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run DecisionRun;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run Stat1DefRun;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run Stat1ValRun;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run Stat2DefRun;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\HudWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run Stat2ValRun;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Pingvi;component/hudwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\HudWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 4 "..\..\HudWindow.xaml"
            ((Pingvi.HudWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 4 "..\..\HudWindow.xaml"
            ((Pingvi.HudWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.HudBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.StackLabel = ((System.Windows.Controls.Label)(target));
            
            #line 15 "..\..\HudWindow.xaml"
            this.StackLabel.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.StackLabel_MouseDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.PotOddsLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.HeroStateRun = ((System.Windows.Documents.Run)(target));
            return;
            case 6:
            this.DecisionRun = ((System.Windows.Documents.Run)(target));
            return;
            case 7:
            this.Stat1DefRun = ((System.Windows.Documents.Run)(target));
            return;
            case 8:
            this.Stat1ValRun = ((System.Windows.Documents.Run)(target));
            return;
            case 9:
            this.Stat2DefRun = ((System.Windows.Documents.Run)(target));
            return;
            case 10:
            this.Stat2ValRun = ((System.Windows.Documents.Run)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}


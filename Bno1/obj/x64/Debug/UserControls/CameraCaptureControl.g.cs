﻿#pragma checksum "D:\projects\LexLabs\MaderJ\hackathon\Bno1\Bno1\UserControls\CameraCaptureControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A5B72503EC7AB92970B692909770B033"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bno1.UserControls
{
    partial class CameraCaptureControl : 
        global::Windows.UI.Xaml.Controls.UserControl, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    global::Windows.UI.Xaml.Controls.UserControl element1 = (global::Windows.UI.Xaml.Controls.UserControl)(target);
                    #line 13 "..\..\..\UserControls\CameraCaptureControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.UserControl)element1).Loaded += this.UserControl_Loaded;
                    #line 14 "..\..\..\UserControls\CameraCaptureControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.UserControl)element1).Unloaded += this.UserControl_Unloaded;
                    #line default
                }
                break;
            case 2:
                {
                    this.capturePreview = (global::Windows.UI.Xaml.Controls.CaptureElement)(target);
                    #line 17 "..\..\..\UserControls\CameraCaptureControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.CaptureElement)this.capturePreview).Tapped += this.capturePreview_Tapped;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace StockD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class App : System.Windows.Application
    {

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;

#line 4 "..\..\App.xaml"
            this.StartupUri = new System.Uri("Login.xaml", System.UriKind.Relative);

#line default
#line hidden
            System.Uri resourceLocater = new System.Uri("/ShubhaRt;component/app.xaml", System.UriKind.Relative);

#line 1 "..\..\App.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }


    }
}

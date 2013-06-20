using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HP.LR.VuGen.ServiceCore.Data.ProjectSystem;
using HP.LR.VuGen.ServiceCore.Data.Protocol;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

//debuggernonusercode
using System.Diagnostics;
namespace SurroundWithTransactionAddin
{
    public class EnterTransactionDetailsContent : UserControl, IComponentConnector
    {
        internal TextBox tName;
        internal ComboBox tStatus;
        private bool _contentLoaded;

        public EnterTransactionDetailsContent()
        {
            this.InitializeComponent();
            this.tName.Focus();
            base.Loaded += new RoutedEventHandler(this.EnterTransactionDetailsContent_Loaded);

        }
        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Uri resourceLocater = new Uri("/SurroundWithTransactionAddin;entertransactiondetailscontent1.xaml", UriKind.Relative);
            Application.LoadComponent(this, resourceLocater);
        }

        private void EnterTransactionDetailsContent_Loaded(object sender, RoutedEventArgs e)
        {
            //int maxLineNumber = ((GotoLineNumberViewModel)base.DataContext).MaxLineNumber;
            //LineNumberValueValidator.MaxLineNumber = maxLineNumber;
            this.tName.Text = "";
            this.tStatus.Items.Add(new object[] { "LR_AUTO", "LR_FAIL", "LR_PASS", "LR_STOP" });
            this.tStatus.SelectedIndex = 0;
        }
    }
}

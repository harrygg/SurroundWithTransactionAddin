/*using System;
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
using System.ComponentModel;*/

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace SurroundWithTransactionAddin
{
    public partial class EnterTransactionDetailsContent : UserControl, IComponentConnector
    {
        internal TextBox tName;
        //internal ComboBox tStatus;
        private bool _contentLoaded;
        /*private String transactionName;
        public String TransactionName
        {
            get { return this.transactionName; }
            set { this.transactionName = value; }
        }*/
        public EnterTransactionDetailsContent()
        {
            this.InitializeComponent();
            //this.DataContext = this;
            this.tName.Focus();
            base.Loaded += new RoutedEventHandler(this.EnterTransactionDetailsContent_Loaded);

        }
        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Uri resourceLocater = new Uri("/SurroundWithTransactionAddin;component/entertransactiondetailscontent.xaml", UriKind.Relative);
            
            //Uri resourceLocater = new Uri("entertransactiondetailscontent.xaml", UriKind.Relative);
            Application.LoadComponent(this, resourceLocater);
        }
        
        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            if (connectionId != 1)
            {
                this._contentLoaded = true;
                return;
            }
            this.tName = (TextBox)target;
        }
        
        private void EnterTransactionDetailsContent_Loaded(object sender, RoutedEventArgs e)
        {
            this.tName.Text = "a";
            //this.tStatus.Items.Add(new object[] { "LR_AUTO", "LR_FAIL", "LR_PASS", "LR_STOP" });
            //this.tStatus.SelectedIndex = 0;
        }
        //public event PropertyChangedEventHandler PropertyChanged;
        private void tName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this.t.Text = this._lastValidText;

            //e.Handled = true;
            //if (this.PropertyChanged != null)
            //{
            //    this.PropertyChanged(this, new PropertyChangedEventArgs("TransactionName"));
            //}
        }
    }
}

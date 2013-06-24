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
        internal ComboBox tStatus;
        private bool contentLoaded;
        
        public EnterTransactionDetailsContent()
        {
            this.InitializeComponent();
            this.tName.Focus();
            base.Loaded += new RoutedEventHandler(this.EnterTransactionDetailsContent_Loaded);
        }

        public void InitializeComponent()
        {
            if (this.contentLoaded)
                return;
            this.contentLoaded = true;
            Uri resourceLocater = new Uri("/SurroundWithTransactionAddin;component/entertransactiondetailscontent.xaml", UriKind.Relative);
            Application.LoadComponent(this, resourceLocater);
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.tName = (TextBox)target;
                    return;
                case 2:
                    this.tStatus = (ComboBox)target;
                    return;
            }
            this.contentLoaded = true;
        }
        
        private void EnterTransactionDetailsContent_Loaded(object sender, RoutedEventArgs e)
        {
            this.tName.Text = "";
            this.tStatus.SelectedIndex = 0;
        }

        [DebuggerNonUserCode]
        internal Delegate _CreateDelegate(Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, this, handler);
        }

    }
}

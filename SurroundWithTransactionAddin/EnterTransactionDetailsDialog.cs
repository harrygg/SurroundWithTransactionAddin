using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HP.Utt.UttDialog;
using HP.LR.VuGen.ServiceCore.Data.ProjectSystem;
using HP.LR.VuGen.ServiceCore.Data.Protocol;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SurroundWithTransactionAddin
{
    public class EnterTransactionDetailsDialog : CustomDialog
    {
        private EnterTransactionDetailsViewModel viewModel;
        public String TransactionName {
            get { return this.viewModel.TransactionName; }
            private set { this.viewModel.TransactionName = value;}
        }
        public String TransactionStatus
        {
            get {
                var ts = (TransactionStatus)this.viewModel.TransactionStatuses.CurrentItem;
                return ts.Status; 
            }
        }

        public EnterTransactionDetailsDialog()
        {
            this.viewModel = new EnterTransactionDetailsViewModel();
            base.Title = "Enter Transaction Details";
            base.ResizeMode = ResizeMode.NoResize;
            this.ShowHelpButton = false;
            EnterTransactionDetailsContent content = new EnterTransactionDetailsContent();
            content.DataContext = viewModel;
            base.Content = content;
            this.AddOkButton(new Action<CustomDialog>(this.OkAction));
            this.AddCancelButton();
        }

        private void OkAction(CustomDialog dialog)
        {
            this.DialogResult = CustomDialogResult.Ok;
            this.Close();
        }
    }
}

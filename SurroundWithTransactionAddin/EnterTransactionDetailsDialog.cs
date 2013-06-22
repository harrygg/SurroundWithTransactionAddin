using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using HP.LR.VuGen.ServiceCore.Interfaces;
using HP.LR.VuGen.BackEnd.StepManager.StepData;
using HP.LR.VuGen.ServiceCore.Data.StepService;
using HP.LR.Vugen.Common;
using HP.LR.VuGen.Snapshots.WebSnapshotBrowserControl.Interfaces;
using HP.LR.VuGen.Snapshots.WebSnapshotBrowserControl.ViewEditors;*/

using HP.Utt.UttDialog;

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
    public class EnterTransactionDetailsDialog : CustomDialog
    {
        private EnterTransactionDetailsViewModel _viewModel;
        //private String transactionName;
        public String TransactionName {
            get { return this._viewModel.TransactionName; }
            private set { this._viewModel.TransactionName = value;}
        }
        //public String tStatus = String.Empty;

        public EnterTransactionDetailsDialog()
        {
            base.Title = "Enter Transaction Details";
            base.ResizeMode = ResizeMode.NoResize;
            this.ShowHelpButton = false;
            EnterTransactionDetailsContent content = new EnterTransactionDetailsContent();
            content.DataContext = _viewModel;
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

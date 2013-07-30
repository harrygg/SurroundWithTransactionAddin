using System;
using HP.Utt.UttCore;
using HP.Utt.CodeEditorUtils;
using ICSharpCode.SharpDevelop.Editor;
using System.Text;
using System.Xml;
using HP.Utt.UttDialog;
using System.Xml.Linq;
//
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using HP.LR.VuGen.ServiceCore.Interfaces;
using HP.LR.VuGen.BackEnd.StepManager.StepData;
using HP.LR.VuGen.ServiceCore.Data.StepService;
using HP.LR.Vugen.Common;
using HP.LR.VuGen.Snapshots.WebSnapshotBrowserControl.Interfaces;
using HP.LR.VuGen.Snapshots.WebSnapshotBrowserControl.ViewEditors;

using HP.LR.VuGen.ServiceCore.Data.ProjectSystem;
using HP.LR.VuGen.ServiceCore.Data.Protocol;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SurroundWithTransactionAddin
{
    class SurroundWithSubTransactionCommand : SurroundWithTransactionCommand
    {
        protected string startStep = "lr_strart_sub_transaction";
        protected string endStep = "lr_end_sub_transaction";

        public override void Run()
        {
            ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
            if (editor == null)
                return;

            var stepService = ServiceManager.Instance.GetService<IStepService>();
            if (stepService == null)
                return;

            IVuGenProjectService projectService = (IVuGenProjectService)ServiceManager.Instance.GetService(typeof(IVuGenProjectService));
            IVuGenScript script = projectService.GetActiveScript();

            ReadOnlyCollection<IStepModel> transactions = stepService.GetScriptStepsByName("lr_start_transaction", script);
            //Find the nearest lr_start_transaction step going up the script
            //String parentTransaction = FindParentTransaction().StepName;

            //Find out which step(s) are selected for surrounding
            if (SetSelectedSteps(stepService, editor) == false)
                MessageService.ShowMessage("Please select steps to surround");

            var parserStatus = stepService.GetParserStatus(lastSelectedStep.FunctionCall.Location.FilePath);
            if (parserStatus == false)
            {
                MessageService.ShowMessage("Cannot add step");
                return;
            }

            using (EnterTransactionDetailsDialog dialog = new EnterTransactionDetailsDialog())
            {
                dialog.ShowDialog();
                if (dialog.DialogResult == CustomDialogResult.Ok)
                {
                    //create the lr_start_sub_transaction step
                    //AddTransaction(startStep, firstSelectedStep, stepService, dialog.TransactionName, parentTransaction.);
                    //create the lr_end_sub_transaction step
                    AddTransaction(endStep, lastSelectedStep, stepService, dialog.TransactionName);
                }
            }
        }
        /*
        IStepModel FindParentTransaction()
        {
            try
            {
                ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
                var stepService = ServiceManager.Instance.GetService<IStepService>();

                //IVuGenProjectService projectService = (IVuGenProjectService)ServiceManager.Instance.GetService(typeof(IVuGenProjectService));
                //IVuGenScript script = projectService.GetActiveScript();

                //get only the web_url, web_submit_data, web_submit_form, web_custom_request, web_link
                //ReadOnlyCollection<IStepModel> steps = stepService.GetScriptSteps(script);
                

                var currentStep = stepService.CurrentStep;
                while (currentStep == null || currentStep.StepName != "lr_start_transaction")
                {
                    editor.Caret.Line--;
                    currentStep = stepService.CurrentStep;
                }
                return currentStep;
            }
            catch (Exception ex)
            {
                MessageService.ShowMessage(ex.Message);
                Log.VuGen.Error(string.Format("Error occurred when searching for selected step, (Type: '{1}', Exception: '{2}')", Owner.GetType(), ex));
                return null;
            }*
        }*/
    }
}

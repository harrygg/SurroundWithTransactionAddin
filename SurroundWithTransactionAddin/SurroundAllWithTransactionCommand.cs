using System;
using HP.Utt.UttCore;
using HP.Utt.CodeEditorUtils;
using ICSharpCode.SharpDevelop.Editor;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
using HP.Utt.ProgressDialog;
using HP.Utt.ProgressDialog.ViewModel;
using HP.Utt.Common.Parallelism;
using System.Security.Policy;
using System.Threading;

//debuggernonusercode
using System.Diagnostics;

namespace SurroundWithTransactionAddin
{
    class SurroundAllWithTransactionsCommand : SurroundWithTransactionCommand
    {
        private int tNumber = 1;

        public override void Run()
        {
            /*UttProgressDialogHelper pdh = null;
            bool showProgress = true;
            Action<UttTask> taskAction = (UttTask t) =>
            {
                SurroundStepsWithTransactions();
                showProgress = false;
                if (pdh != null)
                    pdh.Close();
            };
            UttTask task = new UttTask("SurroundStepsWithTransactions", taskAction, false, false, 1);
            UttTaskManager taskManager = new UttTaskManager(1, 1, false);
            taskManager.Add(task, UttTaskAddOption.ReplaceLastTask);

            UttProgressDialogSettings settings = new UttProgressDialogSettings
            {
                PersistenceUniqueId = "SurroundWithProgress",
                Title = "Surrounding request steps with transactions",
                AllowUserClose = false,
                AutoCloseOnComplete = true,
            };

            try
            {
                if (showProgress) 
                    UttProgressDialogHelper.ShowSimpleDialog(settings, out pdh, "Please wait...");
                taskManager.Start();
                taskManager.Wait();
            }
            finally
            {
                if (pdh != null)
                {
                    showProgress = false;
                    pdh.Close();
                }
            }*/


            //taskManager.Start();
            //if (showProgress)
                //UttProgressDialogHelper.ShowSimpleDialog(settings, out pdh, "Please wait...");


            SurroundStepsWithTransactions();
        }

        private void RunAddin()
        {

        }

        private void SurroundStepsWithTransactions()
        {
            try
            {
                ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
                if (editor == null)
                    return;

                var stepService = ServiceManager.Instance.GetService<IStepService>();
                if (stepService == null)
                    return;

                IVuGenProjectService projectService = (IVuGenProjectService)ServiceManager.Instance.GetService(typeof(IVuGenProjectService));
                IVuGenScript script = projectService.GetActiveScript();
                ReadOnlyCollection<IStepModel> steps = stepService.GetScriptSteps(script);
                String protocol = script.Protocols.ToString();
                List<String> validSteps = new List<String>();

                bool suspend = false;
                String tName = String.Empty;

                //Create a List of all steps that should be surrounded with a transaction
                if (protocol.Contains("web"))
                {
                    validSteps.a{"web_url", "web_submit_data", "web_submit_form", "web_custom_request", "web_link",
                    "web_browser", "web_button", "web_edit_field", "web_element", "web_file", "web_image", "web_image_link", "web_image_submit", "web_text_link", 
                    "web_service_call", "soap_requst",
                };

                String[] invalidSteps = new String[] {"lr_", "", "", ""};

                foreach (var step in steps)
                {
                    if (step.StepName == "web_concurrent_start")
                    {
                        suspend = true;
                        //save the step name so we have it for the web_concurrent_end step
                        tName = step.ComposedName;
                        AddTransaction("lr_start_transaction", step, stepService, tName);
                    }
                    
                    if (step.StepName == "web_concurrent_end")
                    {
                        suspend = false;
                        AddTransaction("lr_end_transaction", step, stepService, tName);
                    }

                    if (validSteps.Any(step.StepName.Equals))
                    {
                        //surround with transactions only if we are not within web_concurrent group
                        if (suspend == false)
                        {
                            AddTransaction("lr_start_transaction", step, stepService);
                            AddTransaction("lr_end_transaction", step, stepService);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.VuGen.Error(string.Format("Error occurred when adding the {2} step. (Type: '{0}', Exception: '{1}')", Owner.GetType(), ex, Name));
                MessageService.ShowMessage(string.Format(ex.StackTrace));
            }
        }
    }
}

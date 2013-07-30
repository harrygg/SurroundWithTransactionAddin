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

//debuggernonusercode
using System.Diagnostics;


namespace SurroundWithTransactionAddin
{
    public class SurroundWithTransactionCommand : UttBaseWpfCommand
    {
        private int tNumber = 1;
        protected IStepModel firstSelectedStep = null;
        protected IStepModel lastSelectedStep = null;

        public override void Run()
        {
            try
            {
                ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
                if (editor == null)
                    return;

                var stepService = ServiceManager.Instance.GetService<IStepService>();
                if (stepService == null)
                    return;

                //Find out which are the first and last steps selected for surrounding
                if (SetSelectedSteps(stepService, editor) == false)
                {
                    MessageService.ShowMessage("Please select steps to surround with a Transaction");
                    return;
                }

                var parserStatus = stepService.GetParserStatus(lastSelectedStep.FunctionCall.Location.FilePath);
                if (parserStatus == false)
                {
                    MessageService.ShowMessage("Cannot add step");
                    return;
                }

                //get the parrent transaction if exists, otherwise Null
                IStepModel parentTransaction = FindParentTransaction();

                using (EnterTransactionDetailsDialog dialog = new EnterTransactionDetailsDialog())
                {
                    dialog.ShowDialog();
                    if (dialog.DialogResult == CustomDialogResult.Ok)
                    {
                        if (parentTransaction == null)
                        {
                            //create the lr_start_transaction step
                            AddTransaction("lr_start_transaction", firstSelectedStep, stepService, dialog.TransactionName);                        
                            //create the lr_end_transaction step
                            AddTransaction("lr_end_transaction", lastSelectedStep, stepService, dialog.TransactionName);
                        }
                        else
                        {
                            //create the lr_start_sub_transaction step
                            AddTransaction("lr_start_sub_transaction", firstSelectedStep, stepService, dialog.TransactionName, parentTransaction.ComposedName.Substring(20));
                            //create the lr_end_sub_transaction step
                            AddTransaction("lr_end_sub_transaction", lastSelectedStep, stepService, dialog.TransactionName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.VuGen.Error(string.Format("Error occurred when adding a transaction step. (Type: '{0}', Exception: '{1}')", Owner.GetType(), ex));
                MessageService.ShowMessage(string.Format(ex.StackTrace));
            }
        }

        IStepModel FindParentTransaction()
        {
            try
            {
                ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
                var stepService = ServiceManager.Instance.GetService<IStepService>();

                IStepModel currentStep;
                //Go up the script and find the first lr_start_transaction if there is no lr_end_transaction
                while (editor.Caret.Line > 1)
                {
                    if (stepService.CurrentStep != null && ( stepService.CurrentStep.StepName == "lr_end_transaction" || stepService.CurrentStep.StepName == "lr_start_transaction"))
                        break;
                    
                    currentStep = stepService.CurrentStep;
                    editor.Caret.Line--;
                    while (editor.Caret.Line > 1 && currentStep == stepService.CurrentStep)
                        editor.Caret.Line--;
                }

                return (stepService.CurrentStep == null || stepService.CurrentStep.StepName == "lr_end_transaction") ? null : stepService.CurrentStep;
            }
            catch (Exception ex)
            {
                MessageService.ShowMessage(ex.Message);
                Log.VuGen.Error(string.Format("Error occurred when trying to find a parent selection, (Type: '{1}', Exception: '{2}')", Owner.GetType(), ex));
                return null;
            }
        }

        public bool SetSelectedSteps(IStepService stepService, ITextEditor editor)
        {
            try
            {
                firstSelectedStep = stepService.CurrentStep;
                lastSelectedStep = stepService.CurrentStep;

                //Check if we have selected more than 1 step, then we'll find the first and last selected steps
                if (editor.SelectionLength > 0)
                {
                    int selectionStart = editor.SelectionStart;
                    int selectionEnd = editor.SelectionStart + editor.SelectionLength;

                    editor.Caret.Offset = selectionEnd;
                    //Get the last selected line
                    int lastSelectedLine = editor.Caret.Line;

                    //Move the cursor to the start of selection to find out the line of the first step
                    editor.Caret.Offset = selectionStart;
                    //Get the first selected line
                    int firstSelectedLine = editor.Caret.Line;

                    //Check if the line contains a step. If it doesn't move the line down the script
                    //until we find a step or we reach the selection end
                    firstSelectedStep = GetLastSelectedStep(lastSelectedLine, false);
                    //Move the cursor to the end of selection to find out the end line
                    editor.Caret.Offset = selectionEnd;
                    //Check if the line contains a step. If it doesn't move the line up the script 
                    //until we reach the selection start
                    lastSelectedStep = GetLastSelectedStep(firstSelectedLine, true);
                }

                if (firstSelectedStep == null && lastSelectedStep == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                MessageService.ShowMessage(ex.Message);
                Log.VuGen.Error(string.Format("Error occurred when searching for selected step, (Type: '{0}', Exception: '{1}')", Owner.GetType(), ex));
                return false;
            }
        }

        /// <summary>
        /// Method to find the nearest selected step. It is needed when the selection starts/ends with an empty line that has no step.
        /// Then depending on the first/last step we'll go down/up the script to search for the last selected step.
        /// </summary>
        /// <param name="line">The line number</param>
        /// <param name="up">The direction of the search</param>
        /// <returns></returns>
        IStepModel GetLastSelectedStep(int limitLine, bool up = false)
        {
            try
            {
                ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
                var stepService = ServiceManager.Instance.GetService<IStepService>();

                var currentStep = stepService.CurrentStep;
                while (currentStep == null)
                {
                    // if we are searching for the first selected step and the line has no step
                    // we'll go down the script, else we'll go up until we find a step
                    if (up)
                    {
                        editor.Caret.Line--;
                        //if we reach the last selected line return null
                        if (editor.Caret.Line < limitLine)
                            return null;
                    }
                    else
                    {
                        editor.Caret.Line++;
                        //if we reach the first selected line return null
                        if (editor.Caret.Line > limitLine)
                            return null;
                    }
                    currentStep = stepService.CurrentStep;
                }
                return currentStep;
            }
            catch (Exception ex)
            {
                MessageService.ShowMessage(ex.Message);
                Log.VuGen.Error(string.Format("Error occurred when searching for selected step, (Type: '{0}', Exception: '{1}')", Owner.GetType(), ex));
                return null;
            }
        }

        public void AddTransaction(string tType, IStepModel step, IStepService stepService, String stepName = "", String parentTransactionName = "")
        {
            try
            {
                var fc = new FunctionCall();
                var fcs = new FunctionCallSignature { Name = tType};

                //check if the step is already predefined. This is in case of surrounding web_concurrent_start and web_concurrent_end
                //String name = (stepName == "") ? tNumber + ". " + step.ComposedName : tNumber + ". " + stepName;

                fcs.Parameters.Add(new FunctionCallParameter(stepName, ParameterType.ArgtypeString));
                //if we are adding lr_start_sub_transaction add the parent transaction name
                if (tType.Contains("start_sub"))
                    fcs.Parameters.Add(new FunctionCallParameter(parentTransactionName, ParameterType.ArgtypeNumber));

                //if we are adding lr_end_transaction add the LR_AUTO parameter
                if (tType.Contains("end"))
                    fcs.Parameters.Add(new FunctionCallParameter("LR_AUTO", ParameterType.ArgtypeNumber));

                fc.Signature = fcs;
                fc.Location = new FunctionCallLocation(step.FunctionCall.Location.FilePath, null, null);
                IStepModel newStep = stepService.GenerateStep(fc);

                RelativeStep relativity = (tType.Contains("start")) ? RelativeStep.Before : RelativeStep.After;

                if (tType.Contains("end"))
                    tNumber++;

                stepService.AddStep(ref newStep, step, relativity, false);
            }
            catch (Exception ex)
            {
                MessageService.ShowMessage(string.Format(ex.StackTrace));
            }
        }
    }
}

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

namespace SurroundWithTransactionAddin
{
    public class SurroundWithTransactionCommand : UttBaseWpfCommand
    {
        /// <summary>
        /// Check if we are not editing a TruClient | Java | VB | COM based script
        /// </summary>
        /// <returns></returns>
        public static bool IsValidProtocol()
        {
            IVuGenProjectService projectService = (IVuGenProjectService)ServiceManager.Instance.GetService(typeof(IVuGenProjectService));
            IVuGenScript script = projectService.GetActiveScript();

            if (script == null 
                || script.Protocols.ToString().Contains("TruClient") 
                || script.Protocols.ToString().Contains("Java")
                || script.Protocols.ToString().Contains("VB")
                || script.Protocols.ToString().Contains("COM")
                )
                return false;
            
            return true;
        }


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

                var firstSelectedStep = stepService.CurrentStep;
                var lastSelectedStep = stepService.CurrentStep;

                //Check if we have selected more than 1 step, then we'll find the first and last selected steps
                if (editor.SelectionLength > 0)
                {
                    //Move the cursor to the start of selection to find out the line of the first step
                    editor.Caret.Offset = editor.SelectionStart;
                    //Check if the line contains a step. If it doesn't move the line down the script until we find a step
                    firstSelectedStep = FindNearestSelectedStep(editor.Caret.Line);
                    //Move the cursor to the end of selection to find out the end line
                    editor.Caret.Offset = editor.Caret.Offset + editor.SelectionLength;
                    //Check if the line contains a step. If it doesn't move the line up the script
                    lastSelectedStep = FindNearestSelectedStep(editor.Caret.Line, false);
                }

                var parserStatus = stepService.GetParserStatus(lastSelectedStep.FunctionCall.Location.FilePath);
                if (parserStatus == false)
                {
                    MessageService.ShowMessage("Cannot add step");
                    return;
                }

                //create the lr_end_transaction step
                var lrEndTransactionFunctionCall = new FunctionCall();
                var lrEndTransactionSignature = new FunctionCallSignature { Name = "lr_end_transaction" };
                lrEndTransactionSignature.Parameters.Add(new FunctionCallParameter("", ParameterType.ArgtypeString));
                lrEndTransactionSignature.Parameters.Add(new FunctionCallParameter("LR_AUTO", ParameterType.ArgtypeNumber));
                lrEndTransactionFunctionCall.Signature = lrEndTransactionSignature;
                lrEndTransactionFunctionCall.Location = new FunctionCallLocation(lastSelectedStep.FunctionCall.Location.FilePath, null, null);
                IStepModel lrEndTransactionStepModel = stepService.GenerateStep(lrEndTransactionFunctionCall);

                //show the lr_end_transaction dialog
                //TODO - Change the title of the dialog box. Now it is "End transaction"
                lrEndTransactionStepModel.ShowArguments(asyncResult =>
                {
                    var result = (ShowArgumentsResult)asyncResult.AsyncState;
                    if (result.IsModified)
                    {
                        //set the last parameter to False, so it will not move cursor to the newly added step
                        stepService.AddStep(ref lrEndTransactionStepModel, lastSelectedStep, RelativeStep.After, false);
                        //create the lr_start_transaction step
                        var lrStartTransactionFunctionCall = new FunctionCall();
                        var lrStartTransactionSignature = new FunctionCallSignature { Name = "lr_start_transaction" };

                        //TODO I didn't find a way YET to get the newly added parameter name. So I use the dirty way substirnging the ComposedName :(
                        lrStartTransactionSignature.Parameters.Add(new FunctionCallParameter(lrEndTransactionStepModel.ComposedName.Substring(18), ParameterType.ArgtypeString));
                        lrStartTransactionFunctionCall.Signature = lrStartTransactionSignature;
                        lrStartTransactionFunctionCall.Location = new FunctionCallLocation(firstSelectedStep.FunctionCall.Location.FilePath, null, null);
                        IStepModel lrStartTransactionStepModel = stepService.GenerateStep(lrStartTransactionFunctionCall);
                        stepService.AddStep(ref lrStartTransactionStepModel, firstSelectedStep, RelativeStep.Before, false);
                    }
                });
            }
            catch (Exception ex)
            {
                Log.VuGen.Error(string.Format("Error occurred when adding the {2} step. (Type: '{0}', Exception: '{1}')", Owner.GetType(), ex, Name));
            }
        }

        protected string Name
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Method to find the nearest selected step. It is needed when the selection starts/ends with an empty line that has no step.
        /// Then depending on the first/last step we'll go down/up the script to search for nearest steps.
        /// </summary>
        /// <param name="line">The line number</param>
        /// <param name="up">The direction of the search</param>
        /// <returns></returns>
        IStepModel FindNearestSelectedStep(int line, bool up = false)
        {
            try
            {
                ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
                var stepService = ServiceManager.Instance.GetService<IStepService>();

                var currentStep = stepService.CurrentStep;
                while (currentStep == null)
                {
                    //if we are validating the first selected line we'll go down the script, else we'll go up until we find a step
                    if (up) editor.Caret.Line--; else editor.Caret.Line++;
                    currentStep = stepService.CurrentStep;
                }
                return currentStep;
            }
            catch (Exception ex)
            {
                MessageService.ShowMessage(ex.Message);
                Log.VuGen.Error(string.Format("Error occurred when searching for selected step near line {0}, (Type: '{1}', Exception: '{2}')", line, Owner.GetType(), ex));
                return null;
            }
        }
    }
}

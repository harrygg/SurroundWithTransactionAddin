using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using HP.LR.VuGen.ServiceCore.Interfaces;
using HP.LR.VuGen.BackEnd.StepManager.StepData;
using HP.LR.VuGen.ServiceCore.Data.StepService;
using HP.LR.Vugen.Common;
//using HP.LR.VuGen.Snapshots.WebSnapshotBrowserControl.Interfaces;
//using HP.LR.VuGen.Snapshots.WebSnapshotBrowserControl.ViewEditors;

using HP.LR.VuGen.ServiceCore.Data.ProjectSystem;
using HP.LR.VuGen.ServiceCore.Data.Protocol;
using ICSharpCode.SharpDevelop.Editor;
using HP.Utt.CodeEditorUtils;

namespace SurroundWithTransactionAddin
{
    public class IsValidCondition : IConditionEvaluator
    {
        public bool IsValid(object owner, ICSharpCode.Core.Condition condition)
        {
            return IsValidProtocol() && IsValidSelection();
        }

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
                || script.Protocols.ToString().Contains("NET")
                )
                return false;
            return true;
        }

        /// <summary>
        /// Check if we right clicked on an empty line that has no step
        /// </summary>
        /// <returns></returns>
        public static bool IsValidSelection()
        {
            ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
            //if we have selected a single line
            if (editor.SelectionLength == 0)
            {
                var stepService = ServiceManager.Instance.GetService<IStepService>();
                if (stepService == null)
                    return false;

                if (stepService.CurrentStep == null)
                    return false;
            }
            return true;
        }
    }
}

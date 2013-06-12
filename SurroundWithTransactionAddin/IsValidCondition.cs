using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Core;

namespace SurroundWithTransactionAddin
{
  public class IsValidCondition:IConditionEvaluator
  {
    public bool IsValid(object owner, Condition condition)
    {
        return SurroundWithTransactionCommand.IsValidProtocol();
    }
  }
}

using System;
using HP.Utt.Common;

namespace SurroundWithTransactionAddin
{
    public class EnterTransactionDetailsViewModel : UttDataHolder
    {
        private String _transactionName;
        public String TransactionName
        {
            get
            {
                return this._transactionName;
            }
            set
            {
                this._transactionName = value;
                this.OnPropertyChanged("TransactionName");
            }
        }
    }
}

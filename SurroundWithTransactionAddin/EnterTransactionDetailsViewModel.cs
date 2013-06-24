using System;
using HP.Utt.Common;
using System.Windows.Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace SurroundWithTransactionAddin
{
    public class EnterTransactionDetailsViewModel : UttDataHolder, INotifyPropertyChanged
    {
        private String transactionName;
        private CollectionView transactionStatuses;
 
        public String TransactionName
        {
            get { return this.transactionName; }
            set 
            {
                this.transactionName = value;
                this.OnPropertyChanged("TransactionName");
            }
        }

        public CollectionView TransactionStatuses
        {
            get { return transactionStatuses; }
        }

        public EnterTransactionDetailsViewModel()
        {
            IList<TransactionStatus> statusList = new List<TransactionStatus>();
            statusList.Add(new TransactionStatus("LR_AUTO"));
            statusList.Add(new TransactionStatus("LR_PASS"));
            statusList.Add(new TransactionStatus("LR_FAIL"));
            statusList.Add(new TransactionStatus("LR_STOP"));
            transactionStatuses = new CollectionView(statusList);
        }
    }
}

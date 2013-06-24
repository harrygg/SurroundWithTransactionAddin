using System;

namespace SurroundWithTransactionAddin
{

    public class TransactionStatus
    {
        public string Status { get; set; }
        public TransactionStatus(string status)
        {
            Status = status;
        }
    }
}

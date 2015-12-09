using System;
using JudoPayDotNet.Models;
using Newtonsoft.Json;

namespace JudoDotNetXamarin
{
    public class JudoReceipt: ITransactionResult
    {
        private ITransactionResult receipt;

        public JudoReceipt ()
        {
        }

        public JudoReceipt (ITransactionResult receipt)
        {
            this.receipt = receipt;
        }

        [JsonIgnoreAttribute]
        public string ReceiptId {
            get { return receipt.ReceiptId; }
            set { receipt.ReceiptId = value; }
        }

        [JsonIgnoreAttribute]
        public string Result {
            get { return receipt.Result; }
            set { receipt.Result = value; }
        }

        [JsonIgnoreAttribute]
        public string Message {
            get { return receipt.Message; }
            set { receipt.Message = value; }
        }

        [JsonIgnoreAttribute]
        public ITransactionResult FullReceipt {
            get { return receipt; }
        }
    }
}


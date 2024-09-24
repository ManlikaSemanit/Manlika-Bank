using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManlikaBank.Models
{
    public class AccountModel
    {
        public string BankNo { get; set; }
        public decimal Balance { get; set; }
        public List<TransferModel> TransferModels { get; set; }
    }

    public class TransferModel
    {
        public int TransactionID { get; set; }
        public string ActionName { get; set; }
        public DateTime? Datetime { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }

    }
}
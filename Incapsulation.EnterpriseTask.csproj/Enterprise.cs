using System;
using System.Linq;

namespace Incapsulation.EnterpriseTask
{
    public class Enterprise
    {
        private readonly Guid guid;

        public Guid Guid { get; }

        private string name;

        public string Name { get; set; }

        private string inn;

        public string Inn
        {
            get => inn;
            set => inn = inn.Length != 10 || !inn.All(z => char.IsDigit(z)) ? throw new ArgumentException() : value;
        }

        private DateTime establishDate;

        public DateTime EstablishDate { get; set; }

        public TimeSpan ActiveTimeSpan => DateTime.Now - establishDate;

        public double GetTotalTransactionsAmount()
        {
            DataBase.OpenConnection();
            var amount = 0.0;
            
            foreach (Transaction t in DataBase.Transactions())
                amount += t.EnterpriseGuid == guid ? t.Amount : 0;
            
            return amount;
        }
    }
}
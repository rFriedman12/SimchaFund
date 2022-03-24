using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFundHW21.Data
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool AlwaysInclude { get; set; }
        public DateTime DateCreated { get; set; }
        public List<Contribution> Contributions { get; set; }
        public List<Deposit> Deposits { get; set; }

        public decimal Balance
        {
            get
            {
                decimal balance = 0;
                foreach(var deposit in Deposits)
                {
                    balance += deposit.Amount;
                }
                foreach(var contribution in Contributions)
                {
                    balance -= contribution.Amount;
                }

                return balance;
            }
        }
    }
}

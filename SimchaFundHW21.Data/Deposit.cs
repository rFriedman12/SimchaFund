using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFundHW21.Data
{
    public class Deposit
    {
        public int ContributorId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}

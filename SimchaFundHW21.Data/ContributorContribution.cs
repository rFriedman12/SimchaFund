using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFundHW21.Data
{
    public class ContributorContribution
    {
        public int ContributorId { get; set; }
        public bool Include { get; set; }
        public decimal Amount { get; set; }
    }
}

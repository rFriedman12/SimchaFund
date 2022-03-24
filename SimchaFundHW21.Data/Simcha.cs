using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFundHW21.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<Contribution> Contributions { get; set; }
        public decimal TotalContributed
        {
            get
            {
                if (Contributions != null)
                {
                    decimal total = 0;
                    foreach (Contribution contribution in Contributions)
                    {
                        total += contribution.Amount;
                    }
                    return total;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}

using SimchaFundHW21.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFundHW21.Web.Models
{
    public class ContributorViewModel
    {
        public Contributor Contributor { get; set; }
        public bool Include { get; set; }
        public decimal Amount { get; set; }
    }
}

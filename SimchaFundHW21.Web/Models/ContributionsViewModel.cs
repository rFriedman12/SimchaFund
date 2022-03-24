using SimchaFundHW21.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFundHW21.Web.Models
{
    public class ContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public List<ContributorViewModel> Contributors { get; set; }
    }
}

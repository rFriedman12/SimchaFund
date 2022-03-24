using Microsoft.AspNetCore.Mvc;
using SimchaFundHW21.Data;
using SimchaFundHW21.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFundHW21.Web.Controllers
{
    public class ContributorsController : Controller
    {
        private string _connString = "Data Source=.\\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;";

        public IActionResult Index()
        {
            var mgr = new SimchaFundDatabaseManager(_connString);
            var model = new ContributorsViewModel
            {
                Contributors = mgr.GetAllContributors()
            };
            return View(model);
        }

        public IActionResult New(Contributor contributor, decimal initialDeposit)
        {
            var mgr = new SimchaFundDatabaseManager(_connString);
            int contributorId = mgr.AddContributor(contributor);
            mgr.AddDeposit(initialDeposit, contributorId);            
            return Redirect("/contributors");
        }
    }
}

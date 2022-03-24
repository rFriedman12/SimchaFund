using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimchaFundHW21.Data;
using SimchaFundHW21.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFundHW21.Web.Controllers
{
    public class SimchasController : Controller
    {
        private string _connString = "Data Source=.\\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;";

        public IActionResult Index()
        {
            var mgr = new SimchaFundDatabaseManager(_connString);
            var model = new SimchasViewModel
            {
                Simchas = mgr.GetAllSimchas(),
                TotalContributors = mgr.GetTotalContributors()
            };
            return View(model);
        }

        public IActionResult New(Simcha simcha)
        {
            var mgr = new SimchaFundDatabaseManager(_connString);
            mgr.AddSimcha(simcha);
            return Redirect("/simchas");
        }

        public IActionResult Contributions(int simchaId)
        {
            var mgr = new SimchaFundDatabaseManager(_connString);
            var model = new ContributionsViewModel
            {
                Simcha = mgr.GetSimchaById(simchaId),
                Contributors = new List<ContributorViewModel>()
            };
            foreach(Contributor contributor in mgr.GetAllContributors())
            {
                List<int> contributors = mgr.GetContributorsForSimcha(simchaId);
                var contributorViewModel = new ContributorViewModel
                {
                    Contributor = contributor,
                    Include = contributors.Contains(contributor.Id)
                };
                //if (contributorViewModel.Include)
                //{
                //    contributorViewModel.Amount = mgr.GetContribution(simchaId, contributor.Id);
                //}
                model.Contributors.Add(contributorViewModel);
            }
            return View(model);
        }

        public IActionResult UpdateContributions(int simchaId, List<ContributorContribution> contributors)
        {
            var mgr = new SimchaFundDatabaseManager(_connString);
            mgr.UpdateContributionsForSimcha(simchaId, contributors);
            return Redirect($"/Simchas/Contributions?id={simchaId}");
        }
    }
}

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PowerBiEmbedded.Models;
using PowerBiEmbedded.Services;

namespace PowerBiEmbedded.Controllers
{
    public class HomeController : Controller
    {
        private PowerBiServiceApi powerBiServiceApi;
        private IConfiguration configuration;

        public HomeController(PowerBiServiceApi powerBiServiceApi, IConfiguration configuration)
        {
            this.powerBiServiceApi = powerBiServiceApi;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Embed()
        {
            // Get workspace and report IDs from configuration
            Guid workspaceId = new Guid(configuration["PowerBi:WorkspaceId"]);
            Guid reportId = new Guid(configuration["PowerBi:ReportId"]);
            
            var viewModel = await powerBiServiceApi.GetReport(workspaceId, reportId);
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SidelinerFunction
{
    public static class QueryInfractionData
    {
        [FunctionName("QueryInfractionData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var year = req.Query["year"];
            if (string.IsNullOrEmpty(year) || int.TryParse(year, out _) == false)
            {
                if (int.Parse(year) < 2020)
                {
                    return new BadRequestObjectResult("This data only goes back to the 2020 season.");
                }
                return new BadRequestObjectResult("Please enter a year as a parameter on the query string.");
            }
            var infractionData = await WebScraper.GetInfractionData(year);

            return new OkObjectResult(infractionData);
        }
    }
}

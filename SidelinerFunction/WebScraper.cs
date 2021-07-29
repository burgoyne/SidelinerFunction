using HtmlAgilityPack;
using SidelinerFunction.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace SidelinerFunction
{
    class WebScraper
    {
        private readonly static HttpClient _httpClient = new HttpClient();

        public static async Task<LeaguePenaltyModel> GetInfractionData(string year)
        {
            var url = "";
            var suspPath = "";
            var finePath = "";
            switch (year)
            {
                case "2020":
                    url = Constants.Url2020;
                    suspPath = Constants.SuspPath2020;
                    finePath = Constants.FinePath2020;
                    break;
                case "2021":
                    url = Constants.Url2021;
                    suspPath = Constants.SuspPath2021;
                    finePath = Constants.FinePath2021;
                    break;
                default:
                    //do something 
                    break;
            }

            var htmlDocument = new HtmlDocument();
            var httpResponseMessage = await _httpClient.GetAsync(url);
            await EnsureSuccessStatusCode(httpResponseMessage);
            var SuspStatsAsHtml = await httpResponseMessage.Content.ReadAsStringAsync();
            htmlDocument.LoadHtml(SuspStatsAsHtml);

            var arr = new string[] { suspPath, finePath };
            var penaltyData = GetData(htmlDocument, arr);

            return penaltyData;
        }

        private static async Task EnsureSuccessStatusCode(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return;
            }

            var statusCode = httpResponseMessage.StatusCode;
            var httpContent = httpResponseMessage.Content.ReadAsStringAsync();
            var reasonPhrase = httpResponseMessage.ReasonPhrase;

            throw new HttpRequestException($"Status Code:({(int)statusCode} - {statusCode}). Reason Phrase: ({reasonPhrase}). Content: ({httpContent})");
        }

        private static LeaguePenaltyModel GetData(HtmlDocument htmlDocument, string[] pathArray)
        {
            var suspensions = new List<SuspensionModel>();
            var fines = new List<FineModel>();

            for (int a = 0; a < pathArray.Length; a++)
            {
                //loop through rows, skipping first row
                var counter = 0;
                foreach (HtmlNode row in htmlDocument.DocumentNode.SelectNodes(pathArray[a]).Skip(1))
                {
                    counter++;
                    //ignore last row (contains totals)
                    if (counter < htmlDocument.DocumentNode.SelectNodes(pathArray[a]).Skip(1).Count())
                    {
                        //loop through each cell in the row and add values to array
                        HtmlNodeCollection cells = row.SelectNodes("td");
                        var arr = new string[cells.Count];
                        for (int i = 0; i < cells.Count; ++i)
                        {
                            var cellContents = i != 3 ? cells[i].FirstChild.InnerText : cells[i].InnerText;
                            if (!string.IsNullOrEmpty(cellContents))
                            {
                                var newStr = CleanString(cellContents);
                                arr[i] = newStr;
                            }
                        }

                        try
                        {
                            if (a == 0)
                            {
                                var susp = new SuspensionModel
                                {
                                    IncidentDate = DateTime.TryParse(arr[0], out DateTime incDate) ? incDate : DateTime.MinValue,
                                    OffenderName = arr[1],
                                    OffenderTeam = arr[2],
                                    OffenseDesc = arr[3],
                                    ActionDate = DateTime.TryParse(arr[4], out DateTime actDate) ? actDate : DateTime.MinValue,
                                    OffenseLength = arr[5],
                                    SalaryLoss = arr[6] == "N/A" ? 0 : double.Parse(arr[6], System.Globalization.NumberStyles.Currency)
                                };

                                suspensions.Add(susp);
                            }
                            else if (a == 1)
                            {
                                var fine = new FineModel
                                {
                                    IncidentDate = DateTime.TryParse(arr[0], out DateTime incDate) ? incDate : DateTime.MinValue,
                                    OffenderName = arr[1],
                                    OffenderTeam = arr[2],
                                    OffenseDesc = arr[3],
                                    ActionDate = DateTime.TryParse(arr[4], out DateTime actDate) ? actDate : DateTime.MinValue,
                                    SalaryLoss = arr[5] == "N/A" ? 0 : double.Parse(arr[5], System.Globalization.NumberStyles.Currency)
                                };

                                fines.Add(fine);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0} Exception caught.", e);
                        }
                        //use array values to populate corresponding models
                        
                    }
                }
            }

            //return suspension and fine data
            var output = new LeaguePenaltyModel
            {
                Suspensions = suspensions,
                Fines = fines
            };
            return output;
        }

        private static string CleanString(string x)
        {
            //clean string by removing formatting characters
            if (x.Contains("&"))
            {
                x = x.Substring(0, x.IndexOf("&"));
            }
            // only take first $ amount if multiples exist
            if (x.Contains("#"))
            {
                x = x.Replace("#", "");
                if (!x.Contains("#"))
                {
                    var s = x.IndexOf('$');
                    s = x.IndexOf('$', s + 1);
                    x = x.Substring(0, s);
                }
            }
            //strip \n from string
            x = x.Replace("\n", "");

            return x;
        }
    }
}

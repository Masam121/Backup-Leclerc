using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Models.Dto;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using NetflixAPI.Models;

namespace ProjectDashboardAPI.Services
{
    public class SapService : ISapService
    {
        public async Task<IEnumerable<ProjectSAP>> GetSapProject()
        {
            IEnumerable<ProjectSAP> projectsSAP = new List<ProjectSAP>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var data = await client.GetAsync(string.Concat("http://api.dev.gbl/v3/", "projects"));
                data.EnsureSuccessStatusCode();
                var stringResult = await data.Content.ReadAsStringAsync();
                projectsSAP = JsonConvert.DeserializeObject<IEnumerable<ProjectSAP>>(stringResult);
            }
            return projectsSAP;
        }
    }
}

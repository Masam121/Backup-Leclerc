using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Models.Dto
{
    public class WorkloadDataDto
    {
        public List<string> MonthCategory { get; set; }
        public List<double> ActualSerie { get; set; }
        public List<double> EstimatedSerie { get; set; }
        public List<double> MonthlyWorkload { get; set; }
    }
}

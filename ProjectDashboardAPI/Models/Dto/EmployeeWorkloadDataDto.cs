using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Models.Dto
{
    public class EmployeeWorkloadDataDto
    {
        public List<String> TimeLine { get; set; }
        public List<EmployeeDataTableRow> Rows { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SidelinerFunction.Models
{
    class FineModel
    {
        public DateTime IncidentDate { get; set; }
        public DateTime ActionDate { get; set; }
        public string OffenderName { get; set; }
        public string OffenderTeam { get; set; }
        public string OffenseDesc { get; set; }
        public double SalaryLoss { get; set; }
    }
}

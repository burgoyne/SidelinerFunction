using System;
using System.Collections.Generic;
using System.Text;

namespace SidelinerFunction.Models
{
    class LeaguePenaltyModel
    {
        public List<SuspensionModel> Suspensions { get; set; }
        public List<FineModel> Fines { get; set; }
    }
}

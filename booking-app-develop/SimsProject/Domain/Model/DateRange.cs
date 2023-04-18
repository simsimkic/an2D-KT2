using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimsProject.Domain.Model
{
    public class DateRange
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateRange() { }
        public DateRange(DateOnly startDate, DateOnly endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
        public override string ToString()
        {
            return StartDate.ToString("dd.MM.yyyy.") + " - " + EndDate.ToString("dd.MM.yyyy.");
        }
    }
}

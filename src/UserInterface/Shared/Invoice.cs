using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    class Invoice
    {
        public Guid Id { get; set; }
        public DateTime? DateOfTreatment { get; set; }
        public string NameOfInstituation { get; set; }
        public decimal? Amount { get; set; }
    }
}

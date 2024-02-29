using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityDataApi.Data
{
    public class SearchFilterPagnationParameters
    {
        public string? Search { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? AddressLine { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortBy { get; set; } = "Id";
        public string? SortDirection { get; set; } = "asc";
        public int Page { get; set; } =1;
        public int PageSize { get; set; } = 10;
    }
}
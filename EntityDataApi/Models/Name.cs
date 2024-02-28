using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityDataApi.Models
{

    public class Name
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }
        public int EntityId { get; set; }
        [JsonIgnore]
        public virtual Entity? Entity { get; set; }
    }
}
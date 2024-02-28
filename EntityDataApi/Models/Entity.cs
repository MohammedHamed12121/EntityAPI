using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDataApi.Models
{
    public class Entity : IEntity
    {
        public int Id { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }

        public virtual List<Name>? Names { get; set; }
        public virtual List<Address>? Addresses { get; set; }
        public virtual List<Date>? Dates { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace EntityDataApi.Models
{
    public interface IEntity
    {
        public int Id { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }

        [JsonIgnore]
        public List<Name>? Names { get; set; }
        [JsonIgnore]
        public List<Address>? Addresses { get; set; }
        [JsonIgnore]
        public List<Date>? Dates { get; set; }
    }
}
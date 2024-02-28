using System.Text.Json.Serialization;

namespace EntityDataApi.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public int EntityId { get; set; }
        [JsonIgnore]
        public virtual Entity? Entity { get; set; }
    }
}
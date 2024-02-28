using System.Text.Json.Serialization;

namespace EntityDataApi.Models
{
    public class Date
    {
        public int Id { get; set; }
        public string? DateType { get; set; }
        public DateTime? DateValue { get; set; }

        public int EntityId { get; set; }
        [JsonIgnore]
        public virtual Entity? Entity { get; set; }
    }
}
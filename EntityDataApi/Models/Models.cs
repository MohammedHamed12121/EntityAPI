using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityDataApi.Models
{
    public interface IEntity
    {
        public string? Id { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }

        public List<Name>? Names { get; set; }
        public List<Address>? Addresses { get; set; }
        public List<Date>? Dates { get; set; }
    }

    public class Entity : IEntity
    {
        public string? Id { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }

        public List<Name>? Names { get; set; }
        public List<Address>? Addresses { get; set; }
        public List<Date>? Dates { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public string? EntityId { get; set; }
        public Entity? Entity { get; set; }
    }

    public class Date
    {
        public int Id { get; set; }
        public string? DateType { get; set; }
        public DateTime? DateValue { get; set; }

        public string? EntityId { get; set; }
        public Entity? Entity { get; set; }
    }

    public class Name
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }
        public string? EntityId { get; set; }
        public Entity? Entity { get; set; }
    }
}
using System;

namespace Northwind.Components
{
    public class Address : IEquatable<Address>
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public int PostalCode { get; private set; }
        public string Country { get; private set; }

        private Address() {}

        public Address(string street, string city, int postalCode, string country)
        {
            Street = street;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }

        public bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Street, Street) && Equals(other.City, City) && other.PostalCode == PostalCode && Equals(other.Country, Country);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Address)) return false;
            return Equals((Address) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Street.GetHashCode();
                result = (result*397) ^ City.GetHashCode();
                result = (result*397) ^ PostalCode;
                result = (result*397) ^ Country.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(Address left, Address right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Address left, Address right)
        {
            return !Equals(left, right);
        }
    }
}
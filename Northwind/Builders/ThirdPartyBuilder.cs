using Northwind.Components;
using Northwind.Entities;

namespace Northwind.Builders
{
    public abstract class ThirdPartyBuilder<TConcreteBuilder> where TConcreteBuilder : ThirdPartyBuilder<TConcreteBuilder>
    {
        protected string _name = "Acme Corporation";
        protected Address _address = new Address("some street", "some city", 12345, "some country");
        protected string _contactName;
        protected string _phone;
        protected string _fax;

        public TConcreteBuilder WithName(string name)
        {
            _name = name;
            return (TConcreteBuilder)this;
        }

        public TConcreteBuilder WithAddress(Address address)
        {
            _address = address;
            return (TConcreteBuilder)this;
        }

        public TConcreteBuilder WithContactName(string contact)
        {
            _contactName = contact;
            return (TConcreteBuilder)this;
        }

        public TConcreteBuilder WithPhone(string phone)
        {
            _phone = phone;
            return (TConcreteBuilder)this;
        }

        public TConcreteBuilder WithFax(string fax)
        {
            _fax = fax;
            return (TConcreteBuilder)this;
        }

        protected void SetInheritedOptionalProperties(ThirdParty thirdParty)
        {
            if (_contactName != null) thirdParty.ContactName = _contactName;
            if (_fax != null) thirdParty.Fax = _fax;
            if (_phone != null) thirdParty.Phone = _phone;
        }
    }
}
using System;
using System.Linq;
using NHibernate;
using Northwind.Components;
using Northwind.Entities;
using Northwind.Enums;
using QuickGenerate;
using QuickGenerate.Primitives;

namespace NHibernateWorkshop
{
    public static class TestData
    {
        public static class Using
        {
            public static DomainGenerator TheseConventions()
            {
                return
                    new DomainGenerator()
                        .With<Entity<int>>(options => options.Ignore(entity => entity.Id))
                        .With<Entity<Guid>>(options => options.Ignore(entity => entity.Id))
                        .Component<Address>()
                        .With<Address>(options => options.Length(address => address.Street, 1, 100))
                        .With<Address>(options => options.Length(address => address.City, 1, 100))
                        .With<Address>(options => options.Length(address => address.Country, 1, 100));
            }
        }

        private static DomainGenerator EmployeeGenerator(ISession session)
        {
            return Using.TheseConventions()
                .With<Employee>(options => options.Ignore(employee => employee.Id))
                .With<Employee>(options => options.Length(employee => employee.FirstName, 1, 50))
                .With<Employee>(options => options.Length(employee => employee.LastName, 1, 75))
                .With<Employee>(options => options.Length(employee => employee.Title, 1, 50))
                .With<Employee>(options => options.Range(employee => employee.Salary, 1700, 3500))
                .With<Employee>(options => options.Length(employee => employee.Phone, 1, 15))
                .ForEach<Employee>(employee => session.Save(employee));
        }

        public static void Create(ISession session)
        {
            var customers = Using.TheseConventions()
                .With<Customer>(options => options.Length(customer => customer.Name, 5, 100))
                .With<Customer>(options => options.Range(customer => customer.DiscountPercentage, 0, 25))
                .ForEach<Customer>(customer => session.Save(customer))
                .Many<Customer>(20, 40)
                .ToArray();

            var managers = EmployeeGenerator(session)
                .Many<Employee>(2);

            var employees = EmployeeGenerator(session)
                .ForEach<Employee>(employee => Maybe.Do(() => managers.PickOne().AddSubordinate(employee)))
                .Many<Employee>(20)
                .ToArray();

            var suppliers = Using.TheseConventions()
                .With<Supplier>(options => options.Length(supplier => supplier.Website, 1, 100))
                .With<Supplier>(options => options.Length(supplier => supplier.Name, 5, 25))
                .ForEach<Supplier>(supplier => session.Save(supplier))
                .Many<Supplier>(20)
                .ToArray();

            var products = Using.TheseConventions()
                .ForEach<ProductSource>(productsource => session.Save(productsource))
                .With<Product>(options => options.Ignore(product => product.Version))
                .With<Product>(options => options.For(
                    product => product.Category,
                    ProductCategory.Beverages,
                    ProductCategory.Condiments,
                    ProductCategory.DairyProducts,
                    ProductCategory.Produce))
                .With<Product>(g => g.Method<double>(0, 5, (product, d) => product.AddSource(suppliers.PickOne(), d)))
                .With<Product>(options => options.Length(product => product.Name, 1, 50))
                .ForEach<Product>(product => session.Save(product))
                .Many<Product>(50)
                .ToArray();

            Using.TheseConventions()
                .With<OrderItem>(options => options.For(item => item.Product, products))
                .OneToMany<Order, OrderItem>(1, 5, (order, item) => order.AddItem(item))
                .With<Order>(options => options.For(order => order.Customer, customers))
                .With<Order>(options => options.For(order => order.Employee, employees))
                .OneToOne<Order, Address>((order, address) => order.DeliveryAddress = address)
                .ForEach<Order>(order => session.Save(order))
                .Many<Order>(50);

            session.Flush();
        }
    }
}

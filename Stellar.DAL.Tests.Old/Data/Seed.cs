using System;
using System.Collections.Generic;
using System.IO;

namespace Stellar.DAL.Tests.Data
{
    public class Seed
    {
        private static List<Person>[] PersonGroups { get; } = new List<Person>[2];
        public static List<Person> Persons0 => PersonGroups[0];
        public static List<Person> Persons1 => PersonGroups[1];

        static Seed()
        {
            LoadPersons(@"Data\persons1.csv");
        }

        private static void LoadPersons(string path)
        {
            PersonGroups[0] = new List<Person>();
            PersonGroups[1] = new List<Person>();

            using var reader = new StreamReader(new FileStream(path, FileMode.Open));

            var i = 0;

            while (!reader.EndOfStream)
            {
                var fields = reader.ReadLine()?.Split(',');

                if (fields == null)
                {
                    break;
                }

                var personModel = new Person
                {
                    FirstName = fields[0],
                    LastName = fields[1],
                    Email = fields[2],
                    Phone = fields[3],
                    Address = new Address
                    {
                        Line1 = fields[4],
                        City = fields[5],
                        Country = 1,
                        StateOrProvince = fields[6],
                        Zip = fields[7]
                    }
                };

                PersonGroups[i++ % 2].Add(personModel);
            }
        }

        #region customer
        public static CustomerWithFields SCustomerWithFields = new()
        {
            FirstName = "Clark",
            LastName = "Kent",
            DateOfBirth = DateTime.Parse("06/18/1938")
        };

        public static Customer Customer1 = new()
        {
            FirstName = "Clark",
            LastName = "Kent",
            DateOfBirth = DateTime.Parse("06/18/1938")
        };

        public static Customer Customer2 = new()
        {
            FirstName = "Bruce",
            LastName = "Wayne",
            DateOfBirth = DateTime.Parse("05/27/1939")
        };

        public static Customer Customer3 = new()
        {
            FirstName = "Peter",
            LastName = "Parker",
            DateOfBirth = DateTime.Parse("08/18/1962")
        };

        public static CustomerModel CustomerWithTraits = new()
        {
            FirstName = Customer1.FirstName,
            LastName = Customer1.LastName,
            DateOfBirth = Customer1.DateOfBirth,
            Traits = new List<string>
            {
                "flight",
                "super-strength",
                "invulnerability",
                "super-speed",
                "vision powers"
            }
        };

        public static IEnumerable<Customer> Customers
        {
            get
            {
                yield return Customer1;
                yield return Customer2;
                yield return Customer3;
            }
        }
        #endregion

        #region Persons
        public static Person Superman = new()
        {
            FirstName = "Clark",
            LastName = "Kent",
            Email = TestHelpers.RandomString(20) + "clark.kent@thedailyplanet.com",
            Phone = TestHelpers.RandomDigits(15),
            Address = new Address
            {
                Line1 = "1938 Sullivan Place",
                City = "Metropolis",
                Country = 1,
                StateOrProvince = "NY"
            },
            Traits = CustomerWithTraits.Traits
        };

        public static Person Batman = new()
        {
            FirstName = "Bruce",
            LastName = "Wayne",
            Email = TestHelpers.RandomString(20) + "not-bruce@wayne.com",
            Phone = TestHelpers.RandomDigits(15),
            Address = new Address
            {
                Line1 = "1007 Mountain Drive",
                City = "Gotham",
                Country = 1,
                StateOrProvince = "NY"
            }
        };

        public static Person Spiderman = new()
        {
            FirstName = "Peter",
            LastName = "Parker",
            Email = TestHelpers.RandomString(20) + "peter.parker@friendlyneighborhood.com",
            Phone = TestHelpers.RandomDigits(15),
            Address = new Address
            {
                Line1 = "2001 Ingram Street",
                City = "Queens",
                Country = 1,
                StateOrProvince = "NY"
            }
        };

        public static IEnumerable<Person> Persons
        {
            get
            {
                yield return Superman;
                yield return Batman;
                yield return Spiderman;
            }
        }
        #endregion
    }
}
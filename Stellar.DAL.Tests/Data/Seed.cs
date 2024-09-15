using System.Collections;

namespace Stellar.DAL.Tests.Data;

public class Seed
{
    private static List<Person>[] PersonGroups { get; } = new List<Person>[2];

    static Seed()
    {
        LoadPersonGroups(@"Data\persons1.csv");
    }

    private static void LoadPersonGroups(string path)
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
    public static CustomerWithFields CustomerWithFields = new()
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

    public static CustomerWithTraits CustomerWithTraits = new()
    {
        FirstName = Customer1.FirstName,
        LastName = Customer1.LastName,
        DateOfBirth = Customer1.DateOfBirth,
        Traits = new List<string>
        {
            "flight",
            "super strength",
            "invulnerability",
            "super speed",
            "laser eyes"
        }
    };

    public static IEnumerable<Customer> Customers {
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

    public class Persons : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { Superman };
            yield return new object[] { Batman };
            yield return new object[] { Spiderman };
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Persons0 : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var enumerator = PersonGroups[0].GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return new object[]{ enumerator.Current };
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Persons1 : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var enumerator = PersonGroups[1].GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return new object[]{ enumerator.Current };
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
#endregion

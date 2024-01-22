using System.Security.Cryptography;
using System.Text;
using Stellar.DAL.Model;

namespace Stellar.DAL.Tests.Data
{
    #region Customers
    /// <summary>
    /// A customer DTO defined with C# fields.
    /// </summary>
    public class CustomerWithFields
    {
        public long? CustomerId;
        public string? FirstName;
        public string? LastName;
        public DateTime? DateOfBirth;
    }

    /// <summary>
    /// A customer entity defined with properties.
    /// </summary>
    public class Customer : Entity
    {
        public long? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    /// <summary>
    /// A customer model.
    /// </summary>
    public class CustomerWithTraits : Customer
    {
        public IEnumerable<string>? Traits { get; set; }

        //public void ApplyBusinessLogic() { }
    }

    /// <summary>
    /// A customer without any data, typical of DDD.
    /// </summary>
    public class CustomerWithoutFields
    {
    }
    #endregion

    #region Address
    /// <summary>
    /// Address entity.
    /// </summary>
    public class Address : Entity
    {
        public long? AddressId { get; set; }
        public bool IsPoBox { get; set; }
        public string Line1 { get; set; } = string.Empty;
        public string Line2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Country { get; set; }
        public string StateOrProvince { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
    #endregion

    #region Person
    /// <summary>
    /// Person entity.
    /// </summary>
    [Entity("", "Person")]
    public class Person : Entity
    {
        public long? PersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public byte[] PasswordSalt { get; }
        public byte[] PasswordHash { get; }
        
        [Ignore]
        public Address? Address { get; set; }
        
        [Ignore]
        public IEnumerable<string>? Traits { get; set; }

        public Person()
        {
            using var hmac = new HMACSHA512();

            PasswordSalt = hmac.Key;
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Password01#"));
        }

        // The root of composite models references all children to delete cascade
        // and are saved from the bottom up to facilitate updating the model.
    }
    #endregion
}
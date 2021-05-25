using System.Collections.Generic;
using NUnit.Framework;

namespace Stellar.DAL.Tests
{
    [TestFixture]
    public class DynamicDictionaryTests
    {
        [Test]
        public void NonexistentProperty()
        {
            dynamic obj = new DynamicDictionary();

            var firstName = obj.FirstName;

            Assert.Null(firstName);
        }

        [Test]
        public void Assignment()
        {
            dynamic obj = new DynamicDictionary();

            obj.FirstName = "Clark";

            Assert.That(obj.FirstName == "Clark");
        }

        [Test]
        public void Casting()
        {
            dynamic obj = new DynamicDictionary();

            var dictionary = (IDictionary<string, object>)obj;

            Assert.NotNull(dictionary);
        }

        [Test]
        public void AccessAfterCasting1()
        {
            dynamic obj = new DynamicDictionary();

            obj.FirstName = "Clark";

            var dictionary = (IDictionary<string, object>)obj;

            Assert.That(dictionary.ContainsKey("FirstName"));
            Assert.That(dictionary["FirstName"].ToString() == "Clark");
        }

        [Test]
        public void AccessAfterCasting2()
        {
            dynamic obj = new DynamicDictionary();

            var dictionary = (IDictionary<string, object>)obj;

            dictionary.Add("FirstName", "Clark");

            Assert.That(obj.FirstName == "Clark");
        }

        [Test]
        public void CaseSensitivity()
        {
            dynamic obj = new DynamicDictionary();

            obj.FirstName = "Clark";

            Assert.That(obj.FIRSTNAME == "Clark");
            Assert.That(obj.firstname == "Clark");
            Assert.That(obj.fIrStNaMe == "Clark");
        }

        [Test]
        public void AccessAnonymousValue()
        {
            dynamic obj = new DynamicDictionary();

            obj.Customer = new { FirstName = "Clark", LastName = "Kent" };

            Assert.That(obj.Customer.FirstName == "Clark");
        }
    }}

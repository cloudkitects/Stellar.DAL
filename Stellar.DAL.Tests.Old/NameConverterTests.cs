using NUnit.Framework;

namespace Stellar.DAL.Tests
{
    [TestFixture]
    internal class NameConverterTests
    {
        [TestCase("PersonId,Name", ExpectedResult = "person_id,name")]
        [TestCase("FirstName,DateOfBirth,Not_ImplementedToday", ExpectedResult = "first_name,date_of_birth,not_implemented_today")]
        [TestCase("ABSTract,OrderedDictionary,TOTally", ExpectedResult = "abstract,ordered_dictionary,totally")]
        public string PascalToLowerSnake(string source)
        {
            return NameConverter.Convert(NamingConvention.Pascal, NamingConvention.LowerSnake, source);
        }
        [TestCase("[person_id],[household_segment_fk],[sub_segment_fk],[primary_contact_mechanism_fk],[platform_user_name],[full_name],[date_0f_birth],[platform_phone_number],[support_email]",
            ExpectedResult = "[PersonId],[HouseholdSegmentFk],[SubSegmentFk],[PrimaryContactMechanismFk],[PlatformUserName],[FullName],[Date0fBirth],[PlatformPhoneNumber],[SupportEmail]")]
        public string LowerSnakeToPascal(string source)
        {
            return NameConverter.Convert(NamingConvention.LowerSnake, NamingConvention.Pascal, source);
        }
    }
}

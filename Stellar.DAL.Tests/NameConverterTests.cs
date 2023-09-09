namespace Stellar.DAL.Tests
{
    public class NameConverterTests
    {
        [Theory]
        [InlineData("PersonId,Name", NamingConvention.Pascal, NamingConvention.LowerSnake, "person_id,name")]
        [InlineData("FirstName,DateOfBirth,Not_ImplementedToday", NamingConvention.Pascal, NamingConvention.LowerSnake, "first_name,date_of_birth,not_implemented_today")]
        [InlineData("ABSTract,OrderedDictionary,TOTally", NamingConvention.Pascal, NamingConvention.LowerSnake, "abstract,ordered_dictionary,totally")]
        [InlineData("[person_id],[household_segment_fk],[sub_segment_fk],[primary_contact_mechanism_fk],[platform_user_name],[full_name],[date_0f_birth],[platform_phone_number],[support_email]",
            NamingConvention.LowerSnake,
            NamingConvention.Pascal,
            "[PersonId],[HouseholdSegmentFk],[SubSegmentFk],[PrimaryContactMechanismFk],[PlatformUserName],[FullName],[Date0fBirth],[PlatformPhoneNumber],[SupportEmail]")]
        public void Converts(string source, NamingConvention sourceConvention, NamingConvention targetConvention, string target)
        {
            Assert.Equal(target, NameConverter.Convert(sourceConvention, targetConvention, source));
        }
    }
}

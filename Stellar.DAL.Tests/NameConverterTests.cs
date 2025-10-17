namespace Stellar.DAL.Tests;

public class NameConverterTests
{
    public static TheoryData<string, NamingConvention, NamingConvention, string> Data => new()
    {
        { "PersonId,Name",                                      NamingConvention.Pascal,     NamingConvention.LowerSnake, "person_id,name" },
        { "FirstName,DateOfBirth,Not_ImplementedToday",         NamingConvention.Pascal,     NamingConvention.UpperSnake, "FIRST_NAME,DATE_OF_BIRTH,NOT_IMPLEMENTED_TODAY" },
        { "ABSTract,OrderedDictionary,TOTally",                 NamingConvention.Pascal,     NamingConvention.Camel,      "aBSTract,orderedDictionary,tOTally" },
        
        { "[household_segment_fk],[full_name],[date_0f_birth]", NamingConvention.LowerSnake, NamingConvention.Pascal,     "[HouseholdSegmentFk],[FullName],[Date0fBirth]" },
        { "[household_segment_fk],[full_name],[date_0f_birth]", NamingConvention.LowerSnake, NamingConvention.Camel,      "[householdSegmentFk],[fullName],[date0fBirth]" },
        { "[household_segment_fk],[full_name],[date_0f_birth]", NamingConvention.LowerSnake, NamingConvention.UpperSnake, "[HOUSEHOLD_SEGMENT_FK],[FULL_NAME],[DATE_0F_BIRTH]" },
        
        { "segmentFk,address,KPI",                              NamingConvention.Camel,      NamingConvention.UpperSnake, "SEGMENT_FK,ADDRESS,KPI" },
        { "segmentFk,address,KPI",                              NamingConvention.Camel,      NamingConvention.LowerSnake, "segment_fk,address,kpi" },
        { "abstract,minutesToMidnight,lineOfWork",              NamingConvention.Camel,      NamingConvention.Pascal,     "Abstract,MinutesToMidnight,LineOfWork" },

        { "`HELL_NO`,`BASED_ON`,`GET_PUSHED`,`MAYBE_3_PARTS`",  NamingConvention.UpperSnake, NamingConvention.LowerSnake, "`hell_no`,`based_on`,`get_pushed`,`maybe_3_parts`" },
        { "`HELL_NO`,`BASED_ON`,`GET_PUSHED`,`MAYBE_3_PARTS`",  NamingConvention.UpperSnake, NamingConvention.Camel,      "`hellNo`,`basedOn`,`getPushed`,`maybe3Parts`" },
        { "`HELL_NO`,`BASED_ON`,`GET_PUSHED`,`MAYBE_3_PARTS`",  NamingConvention.UpperSnake, NamingConvention.Pascal,     "`HellNo`,`BasedOn`,`GetPushed`,`Maybe3Parts`" }
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void Converts(string source, NamingConvention sourceConvention, NamingConvention targetConvention, string target)
    {
        Assert.Equal(target, NameConverter.Convert(sourceConvention, targetConvention, source));
    }

    [Fact]
    public void AddsConversionOrThrows()
    {
        static string conversion(string s) { return $"*{s}*"; }

        NameConverter.AddConversion(NamingConvention.Pascal, NamingConvention.Camel, conversion);

        Assert.Throws<ArgumentException>(() => NameConverter.AddConversion(NamingConvention.Pascal, NamingConvention.Pascal, null!));
        Assert.Throws<ArgumentNullException>(() => NameConverter.AddConversion(NamingConvention.Pascal, NamingConvention.Camel, null!));
    }
}

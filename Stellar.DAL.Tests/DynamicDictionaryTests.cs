namespace Stellar.DAL.Tests;

public class DynamicDictionaryTests
{
    [Fact]
    public void NonexistentPropertyIsNull()
    {
        dynamic obj = new DynamicDictionary();

        var firstName = obj.FirstName;

        Assert.Null(firstName);
    }

    [Fact]
    public void CreatesProperty()
    {
        dynamic obj = new DynamicDictionary();

        obj.FirstName = "Clark";

        Assert.Equal("Clark", obj.FirstName);
    }

    [Fact]
    public void CanBeCastedDown()
    {
        dynamic obj = new DynamicDictionary();

        var dictionary = (IDictionary<string, object>)obj;

        Assert.NotNull(dictionary);
    }

    [Fact]
    public void DwoncastingWorks()
    {
        dynamic obj = new DynamicDictionary();

        obj.FirstName = "Clark";

        var dictionary = (IDictionary<string, object>)obj;

        Assert.True(dictionary.ContainsKey("FirstName"));
        Assert.Equal("Clark", dictionary["FirstName"].ToString());
        Assert.Equal(obj.FirstName, $"{dictionary["FirstName"]}");
    }

    [Fact]
    public void ReferenceUpdatesWork()
    {
        dynamic obj = new DynamicDictionary();

        var dictionary = (IDictionary<string, object>)obj;

        dictionary.Add("FirstName", "Clark");

        Assert.Equal("Clark", obj.FirstName);
    }

    [Fact]
    public void PropertiesAreCaseInsensitive()
    {
        dynamic obj = new DynamicDictionary();

        obj.FirstName = "Clark";

        Assert.Equal("Clark", obj.FIRSTNAME);
        Assert.Equal("Clark", obj.firstname);
        Assert.Equal("Clark", obj.fIrStNaMe);
    }

    [Fact]
    public void CreatesComposites()
    {
        dynamic obj = new DynamicDictionary();

        obj.Customer = new { FirstName = "Clark", LastName = "Kent" };

        Assert.Equal("Clark", obj.Customer.FirstName);
    }
}

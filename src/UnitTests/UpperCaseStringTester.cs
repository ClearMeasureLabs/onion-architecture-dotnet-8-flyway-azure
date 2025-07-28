using Newtonsoft.Json;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;
using Shouldly;

namespace ProgrammingWithPalermo.ChurchBulletin.UnitTests;

[TestFixture]
public class UpperCaseStringTester
{
    [Test]
    public void ShouldBeAssignableFromString()
    {
        UpperCaseString? s = "abc";
        string? s2 = s;

        var result = s2;
        var result2 = s.ToString();

        result2.ShouldBe(result);
    }

    [Test]
    public void CanSerialize()
    {
        UpperCaseString s = "abc";
        var serialized = JsonConvert.SerializeObject(s);
        Console.WriteLine(serialized);
        var deserializeObject = JsonConvert.DeserializeObject<UpperCaseString>(serialized)!;
        deserializeObject.ToString().ShouldBe("ABC");
    }
}
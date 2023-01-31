namespace ECE461_CLI.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var actual = "Bazim";
        Assert.That(actual, Is.EqualTo("Bazim"));
    }
}
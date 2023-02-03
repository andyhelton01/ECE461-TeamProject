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

    [Test]
    public void Test2()
    {
        var actual = "Alec";
        Assert.That(actual, Is.EqualTo("Alec"));
    }

    [Test]
    public void Test3()
    {
        var actual = "Andy";
        Assert.That(actual, Is.EqualTo("Andy"));
    }

    [Test]
    public void Test4()
    {
        var actual = "Alec";
        Assert.That(actual, Is.EqualTo("Alec"));
    }
}
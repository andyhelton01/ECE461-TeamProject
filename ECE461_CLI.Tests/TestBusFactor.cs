namespace ECE461_CLI.Tests;

public class BusFactorTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task TestBusFactorHighScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/pytorch/pytorch");
        Metric m = new BusFactor(lib);
        await m.Calculate();
        Assert.That(m.score, Is.GreaterThan(0.7));
    }

    [Test]
    public async Task TestBusFactorLowScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/skejserjensen/Hangman");
        Metric m = new BusFactor(lib);
        await m.Calculate();
        Assert.That(m.score, Is.LessThan(0.5));
    }

}
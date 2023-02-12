namespace ECE461_CLI.Tests;

public class RampUpTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task TestRampUpHighScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/nodejs/node");
        Metric m = new RampUp(lib);
        await m.Calculate();
        Assert.That(m.score, Is.GreaterThan(0.7));
    }

    [Test]
    public async Task TestRampUpLowScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/andyhelton01/ECE461-TeamProject");
        Metric m = new RampUp(lib);
        await m.Calculate();
        Assert.That(m.score, Is.LessThan(0.5));
    }
}
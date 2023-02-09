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
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/andyhelton01/ECE461-TeamProject");
        Metric m = new RampUp(lib);
        await m.Calculate();
        Assert.That(m.score, Is.GreaterThan(0.7));
    }
}
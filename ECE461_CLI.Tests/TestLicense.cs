namespace ECE461_CLI.Tests;

public class LicenseTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task TestLicenseHighScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/pytorch/pytorch");
        Metric m = new LicenseMetric(lib);
        await m.Calculate();
        Assert.That(m.score, Is.GreaterThan(0.99));
    }

    [Test]
    public async Task TestLicenseLowScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/skejserjensen/Hangman");
        Metric m = new LicenseMetric(lib);
        await m.Calculate();
        Assert.That(m.score, Is.LessThan(0.02));
    }

}
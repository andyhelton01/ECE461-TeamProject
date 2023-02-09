namespace ECE461_CLI.Tests;

public class ResponsiveMaintainerTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task TestResponsiveMaintainerHighScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/pytorch/pytorch");
        Metric m = new ResponsiveMaintainer(lib);
        await m.Calculate();
        Assert.That(m.score, Is.GreaterThan(0.7));
    }

    [Test]
    public async Task TestResponsiveMaintainerLowScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/skejserjensen/Hangman");
        Metric m = new ResponsiveMaintainer(lib);
        await m.Calculate();
        Assert.That(m.score, Is.LessThan(0.5));
    }
}
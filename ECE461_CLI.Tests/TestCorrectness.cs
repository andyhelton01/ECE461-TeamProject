namespace ECE461_CLI.Tests;

public class CorrectnessTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task TestCorrectnessHighScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/pytorch/pytorch");
        Metric m = new Correctness(lib);
        await m.Calculate();
        Assert.That(m.score, Is.GreaterThan(0.7));
    }

    [Test]
    public async Task TestCorrectnessLowScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/skejserjensen/Hangman");
        Metric m = new Correctness(lib);
        await m.Calculate();
        Assert.That(m.score, Is.LessThan(0.5));
    }

}
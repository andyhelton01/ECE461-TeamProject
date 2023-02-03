namespace ECE461_CLI.Tests;

public class ResponsiveMaintainerTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestResponsiveMaintainer()
    {
        Library lib = new GitUrlLibrary("https://github.com/andyhelton01/ECE461-TeamProject");
        lib.GetScore();

        foreach (Metric m in lib.metrics)
        {
            if (m.name == "ResponsiveMaintainer")
                Assert.That(m.score, Is.GreaterThan(0.7));
        }
    }
}
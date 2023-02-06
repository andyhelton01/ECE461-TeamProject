﻿namespace ECE461_CLI.Tests;

public class ResponsiveMaintainerTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestHighScore()
    {
        GitUrlLibrary lib = new GitUrlLibrary("https://github.com/andyhelton01/ECE461-TeamProject");
        Metric m = new ResponsiveMaintainer(lib);
        m.Calculate();
        Assert.That(m.score, Is.GreaterThan(0.7));
    }
}
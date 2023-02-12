using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ECE461_CLI.Tests;

public class LibraryTests
{
	[SetUp]
	public void Setup()
	{

	}

	[Test]
	public void TestGitUrlLibraryHighScore()
	{
		Library lib = new GitUrlLibrary("https://github.com/pytorch/pytorch");
		lib.GetScore();
		Assert.That(lib.score, Is.GreaterThan(0.7));
	}

	[Test]
	public void TestGitUrlLibraryLowScore()
	{
		Library lib = new GitUrlLibrary("https://github.com/skejserjensen/Hangman");
		lib.GetScore();
		Assert.That(lib.score, Is.LessThan(0.5));
	}

	[Test]
	public void TestNPMGitLibraryHighScore()
	{
		Library lib = UrlLibrary.GetFromNpmUrl("https://www.npmjs.com/package/express");
		lib.GetScore();
		Assert.That(lib.score, Is.LessThan(0.5));
	}

	[Test]
	public void TestNPMLibrary()
	{
		Library lib = UrlLibrary.GetFromNpmUrl("https://www.npmjs.com/package/@kre-form/abc");
		lib.GetScore();
		lib.ToJson();
	}

	[Test]
	public void TestIncorrectUrl()
	{
		Library lib = UrlLibrary.GetFromNpmUrl("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
		lib.GetScore();
	}

    [Test]
    public void TestLibraryToOutput()
    {
		Library lib = new Library("Bazim");
        lib.ToOutput();
    }

}
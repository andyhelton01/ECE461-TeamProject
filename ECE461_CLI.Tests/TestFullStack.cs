using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Diagnostics;

namespace ECE461_CLI.Tests;

public class FullStackTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestFullStackValidInput()
    {
        string[] args = { "../../../../sample_url_file.txt" };
        Program prog = new Program();
        prog.runUrlFile(args[0]);
    }

    [Test]
    public void TestFullStackInvalidInput()
    {
        string[] args = { "blahblahblah" };
        Program prog = new Program();
        prog.runUrlFile(args[0]);
    }

}
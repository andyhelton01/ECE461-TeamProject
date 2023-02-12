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
    public void TestValidInput()
    {
        string[] args = { "../../../../sample_url_file.txt" };
        Program prog = new Program();
        prog.runUrlFile(args[0]);
    }

    [Test]
    public void TestIncorrectFileName()
    {
        string[] args = { "blahblahblah" };
        Program prog = new Program();
        prog.runUrlFile(args[0]);
    }

    [Test]
    public void TestIncorrectFilePath()
    {
        string[] args = { "blahblahblah/sample_url_file.txt" };
        Program prog = new Program();
        prog.runUrlFile(args[0]);
    }


    [Test]
    public void TestInvalidFormat()
    {
        string[] args = { "../../../../edge_case_url_file.txt" };
        Program prog = new Program();
        prog.runUrlFile(args[0]);
    }

    [Test]
    public void TestMain()
    {
        string[] args = { "../../../../sample_url_file.txt" };
        Program.Main(args);
    }

}
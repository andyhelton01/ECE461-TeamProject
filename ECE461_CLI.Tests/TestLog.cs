namespace ECE461_CLI.Tests;

public class LogTests
{
	[SetUp]
	public void Setup()
	{

	}

	[Test]
	public void TestLogError()
	{
		Program.log.Clear();
		string error = "some error";
		Program.LogError(error);
		String.Equals(Program.log.ToString(), "[ERROR]" + error);
	}

    [Test]
    public void TestLogWarning()
    {
        Program.log.Clear();
        string warning = "some warning";
        Program.LogError(warning);
        String.Equals(Program.log.ToString(), "[WARNING]" + warning);
    }

    [Test]
    public void TestLogInfo()
    {
        Program.log.Clear();
        string info = "some info";
        Program.LogError(info);
        String.Equals(Program.log.ToString(), "[INFO]" + info);
    }

    [Test]
    public void TestLogDebug()
    {
        Program.log.Clear();
        string debug = "some debug";
        Program.LogError(debug);
        String.Equals(Program.log.ToString(), "[DEBUG]" + debug);
    }

}
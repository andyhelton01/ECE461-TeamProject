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
        Program.LOG_LEVEL = 3;
		string error = "some error";
		Program.LogError(error);
		String.Equals(Program.log.ToString(), "[ERROR]" + error);
	}

    [Test]
    public void TestLogWarning()
    {
        Program.log.Clear();
        Program.LOG_LEVEL = 3;
        string warning = "some warning";
        Program.LogError(warning);
        String.Equals(Program.log.ToString(), "[WARNING]" + warning);
    }

    [Test]
    public void TestLogInfo()
    {
        Program.log.Clear();
        Program.LOG_LEVEL = 3;
        string info = "some info";
        Program.LogError(info);
        String.Equals(Program.log.ToString(), "[INFO]" + info);
    }

    [Test]
    public void TestLogDebug()
    {
        Program.log.Clear();
        Program.LOG_LEVEL = 3;
        string debug = "some debug";
        Program.LogError(debug);
        String.Equals(Program.log.ToString(), "[DEBUG]" + debug);
    }

}
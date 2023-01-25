using System;

namespace ECE461_CLI
{
	class CLI
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			// TODO interpret command line args

			Library testLib = new GitUrlLibrary("https://github.com/andyhelton01/ECE461-TeamProject");
			Console.WriteLine(testLib);

		}
	}
}

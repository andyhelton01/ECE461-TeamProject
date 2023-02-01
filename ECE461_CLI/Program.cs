using System;

namespace ECE461_CLI
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!\n");

			// TODO get url file from command line

			// TODO create list of libraries by parsing url file
			// and splitting up between npm and git libs
			Library testLib = new GitUrlLibrary("https://github.com/andyhelton01/ECE461-TeamProject");
			Console.WriteLine(testLib);

			Library npmTestLib = UrlLibrary.GetFromNpmUrl("https://www.npmjs.com/package/winston");
			Console.WriteLine(testLib);
		}
	}
}

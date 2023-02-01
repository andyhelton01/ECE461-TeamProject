using System;
using System.Threading.Tasks;

namespace ECE461_CLI
{
	
	
	class Program
	{	
		static void testRun() {
			GitUrlLibrary testLib = new GitUrlLibrary("https://github.com/andyhelton01/ECE461-TeamProject");
			testLib.GetScore();
			Console.WriteLine(testLib);

			
			Task task = UrlLibrary.GetFromNpmUrl("https://www.npmjs.com/package/winston");
			task.Wait();
		}
		
		static void Main(string[] args)
		{
			foreach (string s in args) {
					Console.WriteLine(s);
				}
			if (args.Length == 0 || args[0].Length != 0) {
				Console.WriteLine(args.Length);
				testRun();
			}else{
				Console.WriteLine(args.Length);
				
				

				// TODO get url file from command line

				// TODO create list of libraries by parsing url file
				// and splitting up between npm and git libs
			}
			
			

			
		}
	}
}

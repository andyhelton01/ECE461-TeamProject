using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ECE461_CLI
{
	
	
	class Program
	{	
		static void testRun() {
			GitUrlLibrary testLib = new GitUrlLibrary("https://github.com/andyhelton01/ECE461-TeamProject");
			testLib.GetScore();
			Console.WriteLine(testLib);

			
			Library lib = UrlLibrary.GetFromNpmUrl("https://www.npmjs.com/package/winston");
			Console.WriteLine(lib);
		}
		
		static void Main(string[] args)
		{
			foreach (string s in args) {
					Console.WriteLine(s);
				}
			if (args.Length == 0 || args[0].Length == 0) {
				Console.WriteLine(args.Length);
				testRun();
			}else{

				// TODO sanitize inputs with error messages and stuff

				

				// create list of libraries by parsing url file
				// and splitting up between npm and git libs
				List<Library> libraries = new List<Library>();
  
				foreach (string line in System.IO.File.ReadLines(args[0]))
				{  
					System.Console.WriteLine(line);
					if (line.Contains("npmjs")) {
						libraries.Add(UrlLibrary.GetFromNpmUrl(line));
					}else{
						libraries.Add(new GitUrlLibrary(line));
					}
					
				}


				// Test block to give randoms scores to libraries
				// Random rand = new Random();
				// // calculate scores
				// foreach(Library lib in libraries) {
				// 	lib.GetScore();
				// 	lib.score = (float) rand.NextDouble();
				// }

				// sort libraries
				libraries.Sort(new Library.LibraryComparer());

				// output
				
				foreach(Library lib in libraries) {
					Console.WriteLine(lib);
				}


			}
			
			

			
		}
	}
}

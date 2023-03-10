using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace ECE461_CLI
{


	public class Program
	{
		// parameters
		public const double REQUEST_TIMEOUT_TIME = 10;



		// members
		public static short LOG_LEVEL = 0; // 0 is silent, 1 means informational messages, 2 means debug messages
		public static string LOG_FILE = "bin/log_file.txt";
		public static short ProgramStatus = 0;
		public static StringBuilder log = new StringBuilder();

		public Program()
		{
			string log_level_env_var = Environment.GetEnvironmentVariable("LOG_LEVEL");
			if (!(log_level_env_var is null) && log_level_env_var.Length > 0)
			{
				try
				{
					LOG_LEVEL = short.Parse(log_level_env_var);
				}
				catch (Exception)
				{
					LogError("invalid value for LOG_LEVEL environment variable: " + log_level_env_var + "   Should be an integer [0,2]");
				}
			}

			string log_file_env_var = Environment.GetEnvironmentVariable("LOG_FILE");
			if (!(log_file_env_var is null) && log_file_env_var.Length > 0)
			{
				LOG_FILE = log_file_env_var;
			}
		}

		public void testRun()
		{
			LOG_LEVEL = 5;

			// runUrlFile("/home/shay/a/afpannun/class_files/461/ECE461-TeamProject/edge_case_url_file.txt");
			// runUrlFile("/home/shay/a/afpannun/class_files/461/ECE461-TeamProject/sample_url_file.txt");
			//runUrlFile("C:\\Users\\afpannun\\source\\repos\\ECE461-TeamProject\\sample_url_file.txt");
			runUrlFile("/mnt/c/Users/apun1/Documents/GitHub/ECE 461 code/ECE461-TeamProject/sample_url_file.txt");

		}

		public void runUrlFile(string filename)
		{
			// create list of libraries by parsing url file
			// and splitting up between npm and git libs
			List<Library> libraries = new List<Library>();

			LogDebug("starting line parsing");

			IEnumerable<string> lines;



			try
			{
				lines = System.IO.File.ReadLines(filename);
			}
			catch (FileNotFoundException)
			{
				LogError("Url file not found: " + filename);
				return;
			}
			catch (DirectoryNotFoundException)
			{
				LogError("Url directory not found: " + filename);
				return;
			}


			foreach (string line in lines)
			{

				// TODO sanitize inputs with error messages and stuff		

				LogDebug("Parsing url: " + line);
				// System.Console.WriteLine(line);
				if (line.Length == 0)
				{
					LogWarning("url file contained and empty line. Skipping");
					continue;
				}

				if (line.Contains("npmjs"))
				{
					Library newLib = UrlLibrary.GetFromNpmUrl(line);
					if (newLib != null)
					{
						libraries.Add(newLib);
					}
					else
					{
						LogError("Invalid library Url: " + line);
					}
				}
				else
				{
					Library newLib = new GitUrlLibrary(line);
					if (newLib != null)
					{
						libraries.Add(newLib);
					}
					else
					{
						LogError("Invalid library Url: " + line);
					}
				}
			}

			if (libraries.Count == 0)
			{
				LogWarning("No valid urls were found in the url file");
			}


			Random rand = new Random();
			// calculate scores (this is not necessary, as asking the lib to print itself will calculate score automatically. we do this to make sure all error messages are seperated from output)
			foreach (Library lib in libraries)
			{
				lib.GetScore();
				//lib.score = (float) rand.NextDouble();
			}

			// sort libraries
			libraries.Sort(new Library.LibraryComparer());

			// output

			foreach (Library lib in libraries)
			{
				Console.WriteLine(lib.ToOutput());
			}

		}

		public static int Main(string[] args)
		{
			Program prog = new Program();
			try
			{
				File.Delete(LOG_FILE);
			}
			catch (DirectoryNotFoundException) { }

			if (args.Length == 0 || args[0].Length == 0)
			{
				LogDebug("Received " + args.Length + " cli arguments");
				prog.testRun();
			}
			else
			{

				prog.runUrlFile(args[0]);

			}
			try
			{
				File.AppendAllText(LOG_FILE, log.ToString());
			}
			catch (DirectoryNotFoundException)
			{
				LogWarning("Log file directory not found. Check your LOG_FILE environment variable");
			}

			return ProgramStatus;


		}


		public static void LogError(string msg)
		{
			ProgramStatus = 1; // set that we had an error so we return EXIT_FAILURE

			string outmsg = "[ERROR] " + msg;

			if (LOG_LEVEL >= 1)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(outmsg);
				Console.ForegroundColor = ConsoleColor.White;

				log.AppendLine(outmsg);
			}


		}
		public static void LogWarning(string msg)
		{

			string outmsg = "[WARNING] " + msg;

			if (LOG_LEVEL >= 1)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(outmsg);
				Console.ForegroundColor = ConsoleColor.White;

				log.AppendLine(outmsg);
			}
		}

		public static void LogInfo(string msg)
		{
			string outmsg = "[INFO] " + msg;

			if (LOG_LEVEL >= 1)
			{
				Console.WriteLine(outmsg);

				log.AppendLine(outmsg);
			}
		}

		public static void LogDebug(string msg)
		{
			string outmsg = "[DEBUG] " + msg;

			if (LOG_LEVEL >= 3)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine(outmsg);
				Console.ForegroundColor = ConsoleColor.White;
			}
			if (LOG_LEVEL >= 1)
			{
				log.AppendLine(outmsg);
			}
		}

	}
}

using System;

namespace FlippoIO
{
	class Program
	{
		static void Main(string[] args)
		{
			Interpreter.Init();
			Interpreter.DoFile(args.Length > 0 ? String.Join(" ", args) : "default.txt");

			Scheduler.WaitForCompletion();
			Debug.WriteLine("Created " + AsyncThread.ThreadCount + " AsyncThreads.");
			Console.WriteLine("Press ENTER to exit.");
			Console.ReadLine();
			Scheduler.Stop();
		}
	}
}

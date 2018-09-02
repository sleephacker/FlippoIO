using System;
using System.Collections.Generic;
using System.Threading;

namespace FlippoIO
{
	public class AsyncThread
	{
		public delegate void AsyncFunction();

		public static int ThreadCount => GetThreadCount();

		private static List<AsyncThread> threads = new List<AsyncThread>();
		private static List<AsyncThread> queue = new List<AsyncThread>();
		private static object mainLock = new object();
		private static int id_counter = 1;

		private EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.AutoReset);
		private Thread thread;
		private AsyncFunction function;
		private int id = id_counter++;
		private int runs = 0;

		public static AsyncThread Borrow(AsyncFunction function)
		{
			AsyncThread thread = null;
			lock(mainLock)
			{
				if(queue.Count > 0)
				{
					thread = queue[0];
					queue.RemoveAt(0);
				}
			}
			if(thread == null)
				thread = new AsyncThread();
			thread.function = function;
			thread.wait.Set();
			return thread;
		}

		public static void Return(AsyncThread thread, bool terminatedGracefully)
		{
			if(!terminatedGracefully)
				thread.Reset();
			else
			{
				thread.function = null;
				thread.wait.Reset(); // TODO: this somehow fixes a bug...
			}
			lock(mainLock)
			{
				queue.Add(thread);
			}
		}

		public static void EmptyQueue()
		{
			List<AsyncThread> oldQueue;
			lock(mainLock)
			{
				oldQueue = queue;
				queue = new List<AsyncThread>();
				foreach(AsyncThread t in oldQueue)
				{
					t.Stop();
					threads.Remove(t);
				}
			}

		}

		public static void KillAll()
		{
			List<AsyncThread> oldThreads;
			lock(mainLock)
			{
				if(threads.Count > queue.Count)
					Debug.WriteLine("WARNING: Attempted to kill all AsyncThreads while some were still running.");
				oldThreads = threads;
				threads = new List<AsyncThread>();
				queue = new List<AsyncThread>();
			}
			foreach(AsyncThread t in oldThreads) t.Stop();
		}

		private static int GetThreadCount()
		{
			lock(mainLock)
				return threads.Count;
		}

		private AsyncThread()
		{
			(thread = new Thread(new ThreadStart(MainLoop))).Start();
			lock(mainLock)
				threads.Add(this);
		}

		private void Reset()
		{
			wait.Reset();
			thread.Abort();
			(thread = new Thread(new ThreadStart(MainLoop))).Start();
		}

		private void Stop()
		{
			wait.Reset();
			thread.Abort();
		}

		private void MainLoop()
		{
			while(true)
			{
				try
				{
					wait.WaitOne();
					function();
					runs++;
				}
				catch(ThreadAbortException e) { break; }
				catch(Exception e)
				{
					if(function != null)
						Debug.WriteLine("Exception occured in AsyncThread " + ToString() + ", while executing " + function.Method + ": " + e.Message + Environment.NewLine + e.StackTrace);
					else
						Debug.WriteLine("Exception occured in AsyncThread " + ToString() + ": " + e.Message + Environment.NewLine + e.StackTrace);
				}
			}
		}

		public override string ToString()
		{
			return id + "-" + runs;
		}
	}
}

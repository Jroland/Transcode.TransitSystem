using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Transcode.TransitSystem
{
	class Program
	{
		private static FileSystemWatcher _watcher = new FileSystemWatcher();
		private static ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
		private static Task _worker;
		private static readonly object _lock = new object();
		private static bool _interrupted = false;
		
		static void Main(string[] args)
		{
			var t = new Transcode.TransitSystem.Code.Transcoder();
			var test = t.ConvertToWidescreen(@"C:\Source\TestFiles\MVI_0121.AVI", @"C:\Source\TestFiles\MVI_0121.MP4", 480, 320);
			Console.WriteLine(test);
			Console.ReadLine();

			_watcher.EnableRaisingEvents = true;
			_watcher.Filter = "*.avi";
			_watcher.IncludeSubdirectories = true;
			_watcher.Path = @"C:\Down\Test";
			_watcher.Changed += _watcher_Changed;
			
			_worker = Task.Factory.StartNew(() =>
			                                {
				while (_interrupted == false)
				{
					string file;
					if (_queue.TryDequeue(out file) == true)
					{
						ProcessModifiedFile(file);
					}
					else
					{
						lock (_lock)
						{
							Monitor.Wait(_lock, 20000);
						}
					}
				}
			});
			
			//scan the directory and check for any untranscoded avi files and add them to the queue
			
			Console.WriteLine("Press any key to exit...");
			Console.ReadLine();
			_interrupted = true;
		}
		
		private static void ProcessModifiedFile(string file)
		{
			//make sure file has not been run already
			//determine wide screen or TV
			//execute transcoding
			//move file to desired location
			//delete the original file if set to do so
		}
		
		private static void _watcher_Changed(object sender, FileSystemEventArgs e)
		{
			try
			{
				_watcher.EnableRaisingEvents = false;
				lock (_lock)
				{
					_queue.Enqueue(e.FullPath);
					Monitor.Pulse(_lock);
				}
			}
			finally
			{
				_watcher.EnableRaisingEvents = true;
			}
		}
	}
}

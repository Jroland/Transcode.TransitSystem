using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcode.TransitSystem.Code
{
	public class Transcoder
	{
		private const string WIDESCREEN_PARAMS = "-i {0} -s {2}x{3} -aspect {2}:{3} -r 30000/1001 -vcodec libx264 -pass 1 -b 360k -bt 416k -vpre iphone -f mp4 -an {1} && ffmpeg -y -i {0} -s {2}x{3} -aspect {2}:{3} -r 30000/1001 -vcodec libx264 -pass 2 -b 360k -bt 416k -vpre iphone -f mp4 -acodec libmp3lame -ac 2 -ar 44100 -ab 64000 {1}";
		
		public string ConvertToWidescreen(string sourceFile, string destinationFile, int width, int height)
		{
			ProcessStartInfo psi = null;
			Process transcodeProcess = null; 
			var t = new TaskCompletionSource<bool>();
			StringBuilder destFile = new StringBuilder(destinationFile);
			
			var extension = Path.GetExtension(destinationFile);
			if (extension.Length > 0)
			{
				destFile.Length -= extension.Length + 1;
				destFile.Append("-ws.mp4");
			}
			
			var ffmpegPath = string.Format(@"C:\bin\ffmpeg\bin\{0}", "ffmpeg.exe");
			psi = new ProcessStartInfo(ffmpegPath, string.Format(WIDESCREEN_PARAMS, sourceFile, destFile, width, height));
			
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;

			transcodeProcess = new Process();
			transcodeProcess.EnableRaisingEvents = true;
			transcodeProcess.OutputDataReceived += HandleOutputDataReceived;
			transcodeProcess.ErrorDataReceived += HandleErrorDataReceived;
			transcodeProcess.Exited += HandleExited;
			transcodeProcess.StartInfo = psi;
			transcodeProcess.Start();
			
			//TODO : catch error messages and catch the outputs
			
			while (transcodeProcess.HasExited == false)
				transcodeProcess.WaitForExit();

			if (transcodeProcess.ExitCode > 0)
				throw new Exception(string.Format("ConvertToWidescreen Exception (ExitCode > 0)! source file: {0}. Destination file: {1}.",
				                                  sourceFile, destFile));
			
			return destFile.ToString();
		}

		void HandleExited (object sender, EventArgs e)
		{
			
		}

		void HandleErrorDataReceived (object sender, DataReceivedEventArgs e)
		{

		}

		void HandleOutputDataReceived (object sender, DataReceivedEventArgs e)
		{
		
		}
	}
}

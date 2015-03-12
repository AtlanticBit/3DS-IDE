#region Using Directives

using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;

#endregion Using Directives


namespace ScintillaPad
{
	internal static class Program
	{
		#region Methods

		private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Terminate(e.ExceptionObject);
		}


		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Terminate(e.Exception);
		}


		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			Application.Run(new MainForm(args));
		}


		private static void Terminate(object exception)
		{
			string errorFilePath = typeof(Program).Assembly.Location + ".log";

			try
			{
				string message = Util.Format(
					"An unexpected error has occurred and the application has been forced to close.{0}" +
					"Additional information may be available in the file {1}.{0}" +
					"We are sorry for the inconvenience.",
					Environment.NewLine,
					Path.GetFileName(errorFilePath));

				MessageBox.Show(message, Util.AssemblyTitle);
			}
			finally
			{
				try
				{
					// Try to append the exception to a log file
					using (StreamWriter sw = new StreamWriter(errorFilePath, true))
					{
						sw.WriteLine("Timestamp: {0}", DateTime.Now);
						sw.WriteLine("OS Version: {0}", Environment.OSVersion);
						sw.WriteLine("Framework Version: {0}", Environment.Version);
						sw.WriteLine(exception);
						sw.WriteLine();
					}
				}
				catch { }

				// Exit the application
				Application.Exit();
			}
		}

		#endregion Methods
	}
}

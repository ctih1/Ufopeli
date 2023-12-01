using Jypeli;
using System;
namespace ufopeli
{
	/// @author onni.nevala
	/// @version 24.11.2023
	/// <summary>
	/// 
	/// </summary>
	/// 
	// TODO
	public class Logger
	{
		private String script;
		private String message;
		public void AddInfo(String script)
		{
			this.script = script;
		}
		public String Log(String type, String message)
		{
			// Add 'log.txt' later...
			message = String.Format("[{0}]: ({1}) - {2}", this.script, type.ToUpper(), message);
			Console.WriteLine(message);
			return message;
		}
	}
}

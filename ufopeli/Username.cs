using Jypeli;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ufopeli
{
	public class Username
	{
		private RegistryKey key;
		private Object value;
		public InputBox Input()
		{
			InputBox input = new InputBox();
			input.Title = "Username";
			input.BorderColor = Color.White;
			input.TextColor = Color.White;
			input.Color = new Color(0, 0, 0, 50);
			input.Position = new Vector(0, -250);
			input.Text = "Username";
			input.Width = 300;
			input.Height = 80;
			return input;
		}

		public PushButton AcceptButton()
		{
			PushButton button = new PushButton("Accept");
			button.Text = "Accept";
			button.TextColor = Color.Black;
			button.BorderColor = Color.White;
			button.Color = new Color(15,255,15,10);
			button.Position = new Vector(0, -330);
			button.Width = 90;
			button.Height = 40;
			return button;
		}

		public void SaveUsername(string username)
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
			key.CreateSubKey("Ufopeli");
			key = key.OpenSubKey("Ufopeli", true);
			key.SetValue("username", username);
		}

		public String GetUsername()
		{
			key = Registry.CurrentUser.OpenSubKey("Software", true);
			key = key.OpenSubKey("Ufopeli");
			value = key.GetValue("username",null);
			if (value == null)
			{
				return null;
			}
			else
			{
				return value.ToString();
			}
		}
	}


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace ufopeli
{
	public class Leaderboard
	{
		private Dictionary<String, Object> data = new Dictionary<String, Object>
		{
			{"score", 0},
			{"time", null},
			{"username", null},
			{"version", 1}
		};

		private Boolean SaveData(Dictionary<String, Object> data)
		{
			JsonConvert.SerializeObject(data);
			return true;
		} 

	}
}

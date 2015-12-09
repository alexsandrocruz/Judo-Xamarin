namespace JudoDotNetXamarin
{
	public class ClientDetails
	{
		public string OS { get; set; }

		public string DeviceModel { get; set; }

		public string DeviceId { get; set; }

		public string CultureLocale { get; set; }

		public string Serial { get; set; }

		public bool SslPinningEnabled { get; set; }
		// couldn't find the direct API support for following things there are some work around with native APIs
		public bool IsRoaming { get; set; }

		public string NetworkName { get; set; }

		public bool Rooted {get;set;}
	}
}


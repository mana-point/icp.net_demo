using System;

public class Helpers
{
	public static DateTime JavaTimeStampToDateTime(double javaTimeStamp)
	{
		// Java timestamp is milliseconds past epoch
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
		return dateTime;
	}


	public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
	{
		// Unix timestamp is seconds past epoch
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
		return dateTime;
	}

	public static int timeRemaining(DateTime dateTime)
	{
		return (int)(Math.Max(0, dateTime.Subtract(DateTime.Now).TotalSeconds));
	}

	public static string getTimeString(int timeLeft)
	{
		if (timeLeft > 0)
		{
			int seconds = (int)(timeLeft % 60);
			int minutes = (int)((timeLeft / 60) % 60);
			int hours = (int)((timeLeft / (60 * 60)) % 24);
			int days = (int)((timeLeft / (60 * 60 * 24)));
			string time = "";

			if (days > 0)
				time = days.ToString() + "d ";
			if (hours > 0 || days > 0)
				time += hours.ToString() + "h ";
			if (minutes > 0 || hours > 0 || days > 0)
				time += minutes.ToString() + "m ";

			time += seconds.ToString() + "s";

			return time;
		}

		return "";
	}
}
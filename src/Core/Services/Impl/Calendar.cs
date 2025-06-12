using System;

namespace Core.Services.Impl
{
	public class Calendar : ICalendar
	{
		public DateTime GetCurrentTime()
		{
			return DateTime.Now;
		}
	}
}
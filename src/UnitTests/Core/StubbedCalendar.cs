using System;
using Core.Services;

namespace UnitTests.Core
{
	public class StubbedCalendar : ICalendar
	{
		private DateTime _currentTime;

		public StubbedCalendar(DateTime currentTime)
		{
			_currentTime = currentTime;
		}

		public DateTime GetCurrentTime()
		{
			return _currentTime;	
		}
	}
}
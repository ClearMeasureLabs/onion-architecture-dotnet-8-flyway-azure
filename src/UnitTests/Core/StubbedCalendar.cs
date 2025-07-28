using Core.Services;

namespace UnitTests.Core;

public class StubbedCalendar : ICalendar
{
    private readonly DateTime _currentTime;

    public StubbedCalendar(DateTime currentTime)
    {
        _currentTime = currentTime;
    }

    public DateTime GetCurrentTime()
    {
        return _currentTime;
    }
}
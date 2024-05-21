using System;

namespace Cheetah.WebApi.Core.Models;

public class DateAndValue<T>
{
    public DateTimeOffset Date { get; set; }
    public T Value { get; set; }
}

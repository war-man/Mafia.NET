﻿using Mafia.NET.Matches.Options.DayTypes;

namespace Mafia.NET.Matches.Options
{
    public interface ISettings
    {
        IDayType DayType { get; }
    }
}
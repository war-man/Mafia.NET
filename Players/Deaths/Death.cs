﻿using Mafia.NET.Players.Roles.Abilities;
using System;

namespace Mafia.NET.Players.Deaths
{
#nullable enable
    public class Death : IDeath
    {
        public int Day { get; set; }
        public IPlayer Victim { get; set; }
        public string VictimName { get; set; }
        public string? VictimRole { get; set; }
        public DeathCause Cause { get; set; }
        public IPlayer? Killer { get; set; }
        public string LastWill { get; set; }
        public string? DeathNote { get; set; }
        public string Description { get; set; }

        public Death(int day, IPlayer victim, DeathCause cause, string description, IPlayer? killer = null)
        {
            Day = day;
            Victim = victim;
            VictimName = victim.Name;
            VictimRole = victim.Role.Name;
            Cause = cause;
            Killer = killer;
            LastWill = victim.LastWill;
            DeathNote = killer?.DeathNote;
            Description = description;
        }

        public Death(IAbility ability, IPlayer victim) : this(victim.Match.Phase.Day, victim, DeathCause.MURDER, ability.MurderDescriptions.Get(), ability.User)
        {
        }
    }
}

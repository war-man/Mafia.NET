﻿using Mafia.NET.Matches.Chats;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Phases
{
    public class PhaseChangeEventArgs : EventArgs
    {
        public IPhase Before { get; set; }
        public IPhase After { get; set; }
        public IList<INotification> Notifications{ get; set; }

        public PhaseChangeEventArgs(IPhase before, IPhase after, IList<INotification> notifications)
        {
            Before = before;
            After = after;
            Notifications = notifications;
        }

    }
}
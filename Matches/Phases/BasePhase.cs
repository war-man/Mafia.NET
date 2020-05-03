﻿using System;

#nullable enable

namespace Mafia.NET.Matches.Phases
{
    public abstract class BasePhase : IPhase
    {
        public IMatch Match { get; }
        public string Name { get; }
        public int Duration { get; }
        public IPhase? PreviousPhase { get; }
        public IPhase? NextPhase { get; }
        public bool Skippable { get; }
        public IPhase? Supersedes { get; set; }
        public IPhase? SupersededBy { get; set; }

        public BasePhase(IMatch match, string name, int duration = -1, IPhase? nextPhase = null, bool skippable = false)
        {
            Match = match;
            Name = name;
            Duration = duration;
            NextPhase = nextPhase;
            Skippable = skippable;
        }

        public virtual void Start()
        {
        }

        public virtual IPhase End()
        {
            if (NextPhase == null)
            {
                throw new NullReferenceException("Base End() method called with no next phase");
            }

            return NextPhase;
        }
    }
}

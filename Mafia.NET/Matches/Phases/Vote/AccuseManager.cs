﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class Accuser
    {
        public Accuser(IPlayer player, bool anonymousVote)
        {
            Player = player;
            Target = null;
            Name = Player.Name;
            AnonymousVote = anonymousVote;
            Power = 1;
            Active = true;
        }

        public IPlayer Player { get; set; }
        [CanBeNull] protected IPlayer Target { get; set; }
        public string Name { get; set; }
        public bool AnonymousVote { get; set; }
        public int Power { get; set; }
        public bool Active { get; set; }

        [CanBeNull]
        public IPlayer Accusing()
        {
            return Target;
        }

        public bool Accuse(IPlayer target, [CanBeNull, NotNullWhen(true)] out Entry notification)
        {
            notification = default;
            if (!Active || Target == target) return false;

            var change = Target != null;
            Target = target;

            if (change)
                notification = Entry.Chat(AnonymousVote ? DayKey.AnonymousTryChange : DayKey.TryChange, Player, target);
            else
                notification = Entry.Chat(AnonymousVote ? DayKey.AnonymousTryAdd : DayKey.TryAdd, Player, target);

            return notification != default;
        }

        public bool Unaccuse([CanBeNull, NotNullWhen(true)] out Entry notification)
        {
            notification = default;
            if (!Active || Target == null) return false;

            Target = null;
            notification = Entry.Chat(AnonymousVote ? DayKey.AnonymousTryRemove : DayKey.TryRemove, Player);

            return notification != default;
        }
    }

    public class AccuseManager
    {
        public AccuseManager(IMatch match, Action<IPlayer> enoughVotes)
        {
            Match = match;
            Accusers = new Dictionary<IPlayer, Accuser>();
            EnoughVotes = enoughVotes;
            Active = true;

            foreach (var player in Match.LivingPlayers)
                Accusers[player] = new Accuser(player, match.Setup.AnonymousVoting);
        }

        public IMatch Match { get; set; }
        protected IDictionary<IPlayer, Accuser> Accusers { get; set; }
        protected Action<IPlayer> EnoughVotes { get; }
        private bool _active { get; set; }

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                foreach (var accuser in Accusers.Values) accuser.Active = true;
            }
        }

        public void Accuse(IPlayer accuser, IPlayer target)
        {
            if (!Active) return;

            var accused = Accusers[accuser].Accuse(target, out var notification);

            if (accused)
            {
                foreach (var player in Match.AllPlayers) player.OnNotification(notification);

                if (VotesAgainst(target) >= RequiredVotes())
                {
                    Active = false;
                    EnoughVotes(target);
                }
            }
        }

        public Accuser Get(IPlayer accuser)
        {
            return Accusers[accuser];
        }

        public void Unaccuse(IPlayer accuser)
        {
            if (!Active) return;

            var unaccused = Accusers[accuser].Unaccuse(out var notification);

            if (unaccused)
                foreach (var player in Match.AllPlayers)
                    player.OnNotification(notification);
        }

        public IList<Accuser> GetAccusers(IPlayer player)
        {
            return Accusers.Values.Where(accuser => accuser.Accusing() == player).ToList();
        }

        public int VotesAgainst(IPlayer player)
        {
            return GetAccusers(player).Count;
        }

        public int TotalVotes()
        {
            return Accusers.Count;
        }

        public int RequiredVotes()
        {
            return TotalVotes() / 2 + 1;
        }
    }
}
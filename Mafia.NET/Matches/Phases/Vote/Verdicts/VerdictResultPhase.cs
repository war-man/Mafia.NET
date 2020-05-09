﻿using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts
{
    public class VerdictResultPhase : BasePhase
    {
        private VerdictManager Verdicts { get; }

        public VerdictResultPhase(IMatch match, VerdictManager verdicts, uint duration = 10) : base(match, "Vote Recount", duration)
        {
            Verdicts = verdicts;
        }

        public override IPhase NextPhase()
        {
            return Verdicts.Innocent() ? Supersedes : new LastWordsPhase(Match, Verdicts.Player);
        }

        public override void Start()
        {
            var trialOver = Notification.Popup("The trial is over and the votes have been counted.");
            var decision = Verdicts.Decision();
            var messages = Verdicts.Votes();

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(trialOver);
                player.OnNotification(decision);
                player.OnNotification(messages);
            }

            base.Start();
        }
    }
}
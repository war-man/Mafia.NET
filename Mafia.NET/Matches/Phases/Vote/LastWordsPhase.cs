﻿using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class LastWordsPhase : BasePhase
    {
        public LastWordsPhase(IMatch match, IPlayer player, uint duration = 10) : base(match, "Last Words", duration)
        {
            Player = player;
        }

        public IPlayer Player { get; }

        public override IPhase NextPhase()
        {
            return new ExecutionPhase(Match, Player) {Supersedes = Supersedes};
        }

        public override void Start()
        {
            ChatManager.Main().Mute();
            ChatManager.Main().Mute(Player, false);
            var entry = Notification.Popup(DayKey.AnyLastWords);

            foreach (var player in Match.AllPlayers) player.OnNotification(entry);

            base.Start();
        }
    }
}
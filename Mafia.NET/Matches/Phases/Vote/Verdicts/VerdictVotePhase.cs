﻿using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles.Abilities.Mafia;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts
{
    public class VerdictVotePhase : BasePhase
    {
        public VerdictManager Verdicts { get; }

        public VerdictVotePhase(IMatch match, IPlayer player, uint duration = 15) : base(match, "Vote", duration)
        {
            Verdicts = new VerdictManager(match, player);
        }

        public override IPhase NextPhase() => new VerdictResultPhase(Match, Verdicts);

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers.Values);

            if (Verdicts.Player.Blackmailed && !Match.Abilities.Setup<BlackmailerSetup>().BlackmailedTalkDuringTrial)
            {
                ChatManager.Main().Participants[Verdicts.Player].Muted = true;
            }

            Notification notification = Notification.Popup($"The town may now vote on the fate of {Verdicts.Player.Name}.");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            base.Start();
        }

        public override void End()
        {
            base.End();
            Verdicts.End();
        }
    }
}
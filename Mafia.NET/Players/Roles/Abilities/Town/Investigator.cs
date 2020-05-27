﻿using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum InvestigatorKey
    {
        ExactDetect,
        Detect,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Investigator", typeof(InvestigatorSetup))]
    public class Investigator : TownAbility<InvestigatorSetup>
    {
        public override void Detect()
        {
            if (!TargetManager.Try(out var target)) return;

            User.Crimes.Add(CrimeKey.Trespassing);

            // TODO: Target switch
            var message = Setup.DetectsExactRole
                ? Notification.Chat(InvestigatorKey.ExactDetect, target, target.Crimes.RoleName())
                : target.Crimes.Crime(InvestigatorKey.Detect);

            User.OnNotification(message);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User),
                TargetNotification.Enum<InvestigatorKey>());
        }
    }

    public class InvestigatorSetup : ITownSetup
    {
        public bool DetectsExactRole = false;
    }
}
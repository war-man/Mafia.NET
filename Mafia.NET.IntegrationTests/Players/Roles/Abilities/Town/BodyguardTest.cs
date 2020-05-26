﻿using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Bodyguard))]
    public class BodyguardTest : BaseMatchTest
    {
        [TestCase("Bodyguard,Citizen,Mafioso", true, false)]
        [TestCase("Bodyguard,Citizen,Mafioso", false, false)]
        [TestCase("Bodyguard,Citizen,Godfather", true, false)]
        [TestCase("Bodyguard,Citizen,Godfather", false, true)]
        public void Piercing(string namesString, bool piercing, bool enemyAlive)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new BodyguardSetup {IgnoresInvulnerability = piercing});
            match.Start();

            Deaths(match, 0);

            match.Skip<NightPhase>();

            var bodyguard = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var godfather = match.AllPlayers[2];

            bodyguard.Role.Ability.TargetManager.Set(citizen);
            godfather.Role.Ability.TargetManager.Set(citizen);

            match.Skip<DeathsPhase>();

            Assert.That(bodyguard.Alive, Is.False);
            Assert.That(citizen.Alive, Is.True);
            Assert.That(godfather.Alive, Is.EqualTo(enemyAlive));
        }

        [TestCase("Bodyguard,Citizen,Doctor,Godfather", true)]
        [TestCase("Bodyguard,Citizen,Doctor,Godfather", false)]
        public void Healing(string namesString, bool healing)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new BodyguardSetup {CanBeHealed = healing});
            match.Start();

            Deaths(match, 0);

            match.Skip<NightPhase>();

            var bodyguard = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var doctor = match.AllPlayers[2];
            var godfather = match.AllPlayers[3];

            bodyguard.Role.Ability.TargetManager.Set(citizen);
            doctor.Role.Ability.TargetManager.Set(bodyguard);
            godfather.Role.Ability.TargetManager.Set(citizen);

            match.Skip<DeathsPhase>();

            Assert.That(bodyguard.Alive, Is.EqualTo(healing));
            Assert.That(citizen.Alive, Is.True);
            Assert.That(doctor.Alive, Is.True);
            Assert.That(godfather.Alive, Is.False);
        }
    }
}
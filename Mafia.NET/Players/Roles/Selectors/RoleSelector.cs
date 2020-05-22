﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;

namespace Mafia.NET.Players.Roles.Selectors
{
    public interface IRoleSelector : IColorizable, ILocalizable
    {
        string Id { get; }
        Key Name { get; }
        Key Summary { get; }
        Key Goal { get; }
        Key Abilities { get; }
        IImmutableList<RoleEntry> Possible { get; }
        HashSet<RoleEntry> Excludes { get; }
        bool Random => Possible.Count > 1;

        bool TryResolve(Random random, out RoleEntry entry);
    }

    public class RoleSelector : IRoleSelector
    {
        public RoleSelector(
            string id,
            Key name,
            Key summary,
            Key goal,
            Key abilities,
            List<RoleEntry> possible,
            Color color)
        {
            Id = id;
            Name = name;
            Summary = summary;
            Goal = goal;
            Abilities = abilities;
            Possible = possible.ToImmutableList();
            Excludes = new HashSet<RoleEntry>();
            Color = color;
        }

        public RoleSelector(RoleRegistry registry, ICategory category) : this(
            category.Id,
            category.Name,
            category.Description,
            Key.Empty,
            Key.Empty,
            category.Possible(registry),
            category.Team.Color)
        {
        }
        
        public RoleSelector(RoleEntry role) : this(
            role.Id,
            role.Name,
            role.Summary,
            role.Goal,
            role.Abilities,
            new List<RoleEntry>() {role},
            role.Color)
        {
        }

        public RoleSelector(RoleRegistry registry, ITeam team) : this(
            team.Id,
            team.Name,
            Key.Empty, // TODO
            Key.Empty,
            Key.Empty,
            registry.Team(team),
            team.Color)
        {
        }

        public RoleSelector(RoleRegistry registry) : this(
            "Any Random",
            SelectorKey.AnyRandom,
            SelectorKey.AnyRandomDescription,
            Key.Empty,
            Key.Empty,
            registry.Get(),
            ColorTranslator.FromHtml("#00CCFF")
            )
        {
        }

        public string Id { get; }
        public Key Name { get; }
        public Key Summary { get; }
        public Key Goal { get; }
        public Key Abilities { get; }
        public IImmutableList<RoleEntry> Possible { get; }
        public HashSet<RoleEntry> Excludes { get; }
        public Color Color { get; }

        public bool TryResolve(Random random, [CanBeNull, NotNullWhen(true)] out RoleEntry entry)
        {
            entry = default;
            var possible = new HashSet<RoleEntry>(Possible);
            
            foreach (var excluded in Excludes)
            {
                possible.Remove(excluded);
            }

            if (possible.Count > 0)
                entry = possible.ElementAt(random.Next(possible.Count()));

            return entry != default;
        }
        
        public string Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Localize();
        }
    }
}
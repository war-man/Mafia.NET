﻿using Mafia.NET.Extension;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Resources;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class AbilityEntry
    {
        public string Name { get; }
        public Type Ability { get; }
        public Type Setup { get; }
        public MessageRandomizer MurderDescriptions { get; }

        public AbilityEntry(string name, Type ability, Type setup, MessageRandomizer murderDescriptions)
        {
            Name = name;
            Ability = ability;
            Setup = setup;
            MurderDescriptions = murderDescriptions;
        }
    }

    public class AbilityRegistry
    {
        private static readonly MessageRandomizer DefaultMurderDescriptions = new MessageRandomizer("They died in mysterious ways");
        private static readonly Lazy<AbilityRegistry> Lazy = new Lazy<AbilityRegistry>(() => new AbilityRegistry());
        public static AbilityRegistry Default { get => Lazy.Value; }
        public IImmutableDictionary<string, AbilityEntry> Names { get; }
        public IImmutableDictionary<Type, AbilityEntry> Types { get; }

        public AbilityRegistry()
        {
            var attributes = new Dictionary<string, (Type, RegisterAbilityAttribute)>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attribute = (RegisterAbilityAttribute)type.GetCustomAttributes(typeof(RegisterAbilityAttribute), true).FirstOrDefault();

                    if (attribute == null) continue;

                    attributes[attribute.Name] = (type, attribute);
                }
            }

            var names = new Dictionary<string, AbilityEntry>();
            var types = new Dictionary<Type, AbilityEntry>();
            var roles = Resource.FromDirectory("Roles", "*.yml");
            foreach (YamlMappingNode yaml in roles)
            {
                var name = yaml["name"].AsString();
                if (!attributes.ContainsKey(name)) continue; // TODO

                var type = attributes[name].Item1;
                var ability = type;
                var setup = attributes[name].Item2.Setup;
                MessageRandomizer murderDescriptions = DefaultMurderDescriptions;
                if (((YamlMappingNode)yaml["ability"]).Contains("murder_descriptions"))
                {
                    murderDescriptions = new MessageRandomizer(yaml["ability"]["murder_descriptions"][0].AsStringList());
                }

                var entry = new AbilityEntry(name, ability, setup, murderDescriptions);

                names[name] = entry;
                types[type] = entry;
            }

            Names = names.ToImmutableDictionary();
            Types = types.ToImmutableDictionary();
        }

        public AbilityEntry Entry<T>() where T : IAbility
        {
            var type = typeof(T);

            if (!Types.ContainsKey(type))
            {
                throw new InvalidCastException("No ability found with type " + type);
            }

            return Types[type];
        }

        public T Ability<T>(IMatch match, IPlayer user) where T : IAbility, new()
        {
            var entry = Entry<T>();
            var ability = new T()
            {
                Match = match,
                User = user,
                Name = entry.Name,
                MurderDescriptions = entry.MurderDescriptions,
                TargetManager = new TargetManager(match, user)
            };

            ability.Initialize();

            return ability;
        }

        public T Setup<T>() where T : IAbilitySetup, new() => new T();
    }
}
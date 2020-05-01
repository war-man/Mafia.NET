﻿using Mafia.NET.Players.Roles.Categories;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Mafia.NET.Players.Roles
{
    public class RoleSetup
    {
        private static Random Random = new Random();
        public IReadOnlyList<IRole> AllRoles { get; }
        public IReadOnlyList<IRole> MandatoryRoles { get; }
        public IReadOnlyList<ICategory> MandatoryCategories { get; }

        public RoleSetup(
            IEnumerable<IRole> allRoles,
            IEnumerable<IRole>? mandatoryRoles = null,
            IEnumerable<ICategory>? mandatoryCategories = null)
        {
            AllRoles = new List<IRole>(allRoles);
            MandatoryRoles = mandatoryRoles != null ? new List<IRole>(mandatoryRoles) : new List<IRole>();
            MandatoryCategories = mandatoryCategories != null ? new List<ICategory>(mandatoryCategories) : new List<ICategory>();
        }

        public RoleSetup(IEnumerable<IRole>? mandatoryRoles = null, IEnumerable<ICategory>? mandatoryCategories = null) : this(Role.Roles.Values, mandatoryRoles, mandatoryCategories)
        {
        }

        public int GetPlayers()
        {
            return MandatoryRoles.Count + MandatoryCategories.Count;
        }

        public List<IRole> GetRoles()
        {
            List<IRole> roles = new List<IRole>(MandatoryRoles);
            List<IRole> possibleRoles = new List<IRole>(AllRoles);
            possibleRoles.RemoveAll(role => !role.Categories.Any(category => MandatoryCategories.Contains(category)));

            foreach (var category in MandatoryCategories)
            {
                var categoryRoles = possibleRoles.Where(role => role.Categories.Contains(category));
                var role = categoryRoles.ElementAt(Random.Next(categoryRoles.Count()));
                roles.Add(role);
            }

            return roles;
        }
    }
}

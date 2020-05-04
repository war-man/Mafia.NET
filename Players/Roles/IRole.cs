﻿using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;
using System.Collections.Generic;
using System.Drawing;

namespace Mafia.NET.Players.Roles
{
    public interface IRole
    {
        string Name { get; }
        ITeam Affiliation { get; }
        IReadOnlyList<ICategory> Categories { get; }
        Color Tint { get; }
    }
}

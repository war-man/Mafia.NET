﻿using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Web.Models
{
    public class GameSettingsModel
    {
        public int DayLength { get; set; }
        public string DayType { get; set; }
        public int NightLength { get; set; }
        public bool LastWillAllowed { get; set; }
        public int DiscussionTime { get; set; }
        public string StartGameAt { get; set; }
        public string NightType { get; set; }
        public bool PmAllowed { get; set; }
        public bool Discussion { get; set; }
        public bool TrialPausesDay { get; set; }
        public bool TrialDefense { get; set; }
        public bool ChooseNames { get; set; }
        public int TrialLength { get; set; }
        public string[] Roles { get; set; }

        public List<IRoleSelector> RoleEntries()
        {
            return Roles
                .Select(role => RoleRegistry.Default.Selector(role))
                .ToList();
        }
    }
}
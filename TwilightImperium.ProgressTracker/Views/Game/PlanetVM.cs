﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilightImperium.ProgressTracker.Game;

namespace TwilightImperium.ProgressTracker.Views.Game
{
    public class PlanetVM:ChildViewModel<UserVM>
    {
        private bool _isExhausted;
        private bool _isSelected;
        public PlanetCard Model { get; }
        public PlanetVM(UserVM parent, PlanetCard model, bool exhausted) : base(parent)
        {
            Model = model;
            _isExhausted = exhausted;
        }

        public bool IsExhausted
        {
            get => _isExhausted;
            set
            {
                _isExhausted = value;
                ThisPropChanged();
                Parent.PropChanged(nameof(UserVM.InfluenceString), nameof(UserVM.ResourceString), nameof(UserVM.SelectedString));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                ThisPropChanged();
                Parent?.PropChanged(nameof(UserVM.CanExhaustPlanet), nameof(UserVM.ResourceString),nameof(UserVM.InfluenceString), nameof(UserVM.SelectedString));
            }
        }
    }

    public class PlanetVMComparer : IComparer<PlanetVM>
    {
        public int Compare(PlanetVM x, PlanetVM y)
        {
            return StringComparer.CurrentCultureIgnoreCase.Compare(x.Model.Name, y.Model.Name);
        }
    }
}

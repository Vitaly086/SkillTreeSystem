using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class SkillModel
    {
        public IReadOnlyReactiveProperty<SkillState> State => _state;

        public List<int> NeighbourIndex => _neighbourIndex;

        public bool IsBaseSkill => _isBaseSkill;

        public int Cost => _cost;

        public Vector2 Position => _position;

        [SerializeField] private Vector2 _position;

        [SerializeField] private bool _isBaseSkill;

        [SerializeField] private int _cost;

        [SerializeField] private List<int> _neighbourIndex;

        private readonly ReactiveProperty<SkillState> _state = new ReactiveProperty<SkillState>();

        public void ChangeState(SkillState state)
        {
            _state.Value = state;
        }
    }
}
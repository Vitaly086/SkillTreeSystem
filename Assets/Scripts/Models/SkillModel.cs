using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class SkillModel
    {
        [field: SerializeField]
        public Sprite SkillIcon { get; private set; }

        [field: SerializeField]
        public string Name { get; private set; }

        public IReadOnlyReactiveProperty<SkillState> State => _state;

        public List<int> NeighbourIndex => _neighbourIndex;

        public bool IsBaseSkill => _isBaseSkill;

        public int Cost => _cost;

        public Vector2 Position => _position;


        [SerializeField]
        private Vector2 _position;

        [SerializeField]
        private bool _isBaseSkill;

        [SerializeField]
        private int _cost;

        [SerializeField]
        private List<int> _neighbourIndex;

        private readonly ReactiveProperty<SkillState> _state = new();

        public void ChangeState(SkillState state)
        {
            _state.Value = state;
        }
    }
}
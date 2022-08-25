using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class SkillModel
    {
        public List<int> NeighbourIndex => _neighbourIndex;

        public bool IsBaseSkill => _isBaseSkill;
        public int Cost => _cost;
        public Vector2 Position => _position;
        
        [SerializeField] private Vector2 _position;
        [SerializeField] private bool _isBaseSkill;
        [SerializeField] private int _cost;
        [SerializeField] private List<int> _neighbourIndex;
    }
}
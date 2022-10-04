using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "SkillTreeModel", menuName = "ScriptableObjects/SkillTreeModel")]
    public class SkillTreeModel : ScriptableObject
    {
        public IReadOnlyList<SkillModel> SkillModels => _skillModels;
        [field: SerializeField] public float SkillOffsetZ { get; private set; }

        [SerializeField] private List<SkillModel> _skillModels;
    }
}
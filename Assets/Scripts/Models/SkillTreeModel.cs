using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "SkillTreeModel", menuName = "ScriptableObjects/SkillTreeModel")]
    public class SkillTreeModel : ScriptableObject
    {
        public IReadOnlyList<SkillModel> SkillModels => _skillModels;
        
        [SerializeField] private List<SkillModel> _skillModels; 
    }
}
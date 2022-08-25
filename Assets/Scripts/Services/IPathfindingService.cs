using System.Collections.Generic;
using Presenters;

namespace Services
{
    public interface IPathfindingService : IService
    {
        public bool IsAllNeighboursConnectBaseSkills(SkillPresenter soldSkill, List<SkillPresenter> destinations);
    }
}
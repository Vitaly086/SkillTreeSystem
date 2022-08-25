using System.Collections.Generic;
using Presenters;


namespace Services
{
    public class PathfindingService : IPathfindingService
    {
        public bool IsAllNeighboursConnectBaseSkills(SkillPresenter soldSkill, List<SkillPresenter> destinations)
        {
            return true;
        }
    }
}
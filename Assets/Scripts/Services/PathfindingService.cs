using System.Collections.Generic;
using Models;
using Presenters;


namespace Services
{
    public class PathfindingService : IPathfindingService
    {
        private readonly HashSet<SkillPresenter> _visitedNodes = new();

        public bool IsAllNeighboursConnectBaseSkills(SkillPresenter soldSkill, List<SkillPresenter> destinations)
        {
            _visitedNodes.Clear();
            _visitedNodes.Add(soldSkill); 

            foreach (var neighbour in soldSkill.GetNeighbours())
            {
                if (neighbour.State.Value != SkillState.Bought || _visitedNodes.Contains(neighbour))
                {
                    continue;
                }
                
                _visitedNodes.Clear(); 
                _visitedNodes.Add(soldSkill); 

                if (!HasPathToNodes(neighbour, destinations))
                {
                    return false;
                }
            }

            return true;
        }

        private bool HasPathToNodes(SkillPresenter currentSkill, List<SkillPresenter> destinations)
        {
            if (destinations.Contains(currentSkill))
            {
                return true;
            }

            _visitedNodes.Add(currentSkill);

            foreach (var neighbour in currentSkill.GetNeighbours())
            {
                if (_visitedNodes.Contains(neighbour) || neighbour.State.Value != SkillState.Bought)
                {
                    continue;
                }

                if (HasPathToNodes(neighbour, destinations))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
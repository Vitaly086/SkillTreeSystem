using System.Collections.Generic;
using Models;
using Presenters;


namespace Services
{
    public class PathfindingService : IPathfindingService
    {
        private readonly HashSet<SkillPresenter> _usedNodes = new HashSet<SkillPresenter>();
        private IReadOnlyList<SkillPresenter> _neighbours;

        public bool IsAllNeighboursConnectBaseSkills(SkillPresenter soldSkill, List<SkillPresenter> destinations)
        {
            _usedNodes.Clear();
            _neighbours = soldSkill.GetNeighbours();
            var allNeighboursHasPath = false;

            foreach (var currentSkill in _neighbours)
            {
                if (currentSkill.State.Value == SkillState.Bought)
                {
                    _usedNodes.Add(soldSkill);
                    if (HasPathToNodes(currentSkill, destinations))
                    {
                        allNeighboursHasPath = true;
                    }
                    else
                    {
                        allNeighboursHasPath = false;
                        break;
                    }
                }
            }

            return allNeighboursHasPath;
        }

        private bool HasPathToNodes(SkillPresenter currentSkill, List<SkillPresenter> destinations)
        {
            if (!destinations.Contains(currentSkill))
            {
                _usedNodes.Add(currentSkill);
            }

            if (destinations.Contains(currentSkill))
            {
                return true;
            }

            foreach (var neighbour in currentSkill.GetNeighbours())
            {
                if (_usedNodes.Contains(neighbour) || neighbour.State.Value != SkillState.Bought)
                {
                    continue;
                }

                if (HasPathToNodes(neighbour, destinations))
                {
                    _usedNodes.Clear();
                    return true;
                }
            }

            return false;
        }
    }
}
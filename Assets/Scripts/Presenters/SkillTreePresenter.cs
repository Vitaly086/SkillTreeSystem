using System.Collections.Generic;
using System.Linq;
using Events;
using Models;
using Services;
using UniRx;
using UnityEngine;

namespace Presenters
{
    public class SkillTreePresenter : MonoBehaviour
    {
        [SerializeField] private LineRenderer _egdePrefab;
        [SerializeField] private SkillTreeModel _skillTreeModel;
        [SerializeField] private SkillPresenter _skillPrefab;

        private readonly List<SkillPresenter> _skillPresenters = new List<SkillPresenter>();
        private readonly List<SkillPresenter> _basePresenters = new List<SkillPresenter>();

        public void Initialize(IMoneyService moneyService, IPathfindingService pathfinding)
        {
            CreateSkillTree(moneyService, pathfinding);

            MessageBroker.Default.Receive<SellAllSkillsEvent>().Subscribe(_ => SellAllSkills());
        }

        private void SellAllSkills()
        {
            for (var i = _skillPresenters.Count - 1; i >= 0; i--)
            {
                _skillPresenters[i].Sell();
            }
        }

        private void CreateSkillTree(IMoneyService moneyService, IPathfindingService pathfinding)
        {
            CreateSkillPresenter(moneyService, pathfinding);

            for (var index = 0; index < _skillPresenters.Count; index++)
            {
                var currentPresenter = _skillPresenters[index];
                var skillModel = _skillTreeModel.SkillModels[index];

                currentPresenter.SetBasePresenters(_basePresenters);

                foreach (var childPresenter in skillModel.NeighbourIndex
                             .Select(neighbourIndex =>
                                 _skillPresenters[neighbourIndex]))
                {
                    currentPresenter.AddNeighbours(childPresenter);
                    CreateEdges(currentPresenter, childPresenter);
                }
            }
        }

        private void CreateSkillPresenter(IMoneyService moneyService, IPathfindingService pathfinding)
        {
            for (int i = 0; i < _skillTreeModel.SkillModels.Count; i++)
            {
                var skillModel = _skillTreeModel.SkillModels[i];

                var position = new Vector3(skillModel.Position.x, skillModel.Position.y, _skillTreeModel.SkillOffsetZ);
                var currentPresenter = Instantiate(_skillPrefab, position, Quaternion.identity,
                    transform);
                currentPresenter.name = "Skill " + i;
                currentPresenter.Initialize(skillModel, moneyService, pathfinding);

                _skillPresenters.Add(currentPresenter);

                if (skillModel.IsBaseSkill)
                {
                    _basePresenters.Add(currentPresenter);
                }
            }
        }

        private void CreateEdges(SkillPresenter currentSkill, SkillPresenter neighbourSkill)
        {
            var edge = Instantiate(_egdePrefab, transform);
            edge.positionCount = 2;
            edge.SetPosition(0, currentSkill.transform.position);
            edge.SetPosition(1, neighbourSkill.transform.position);
        }
    }
}
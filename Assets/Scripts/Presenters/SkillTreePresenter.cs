using System.Collections.Generic;
using System.Linq;
using Events;
using Models;
using Services;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Views;

namespace Presenters
{
    public class SkillTreePresenter : MonoBehaviour
    {
        [FormerlySerializedAs("_egdePrefab")]
        [SerializeField]
        private LineRenderer _edgePrefab;
        [SerializeField]
        private SkillTreeModel _skillTreeModel;
        [SerializeField]
        private SkillView _skillPrefab;

        private readonly List<SkillPresenter> _skillPresenters = new();

        public void Initialize(IMoneyService moneyService, IPathfindingService pathfinding)
        {
            BuildSkillTree(moneyService, pathfinding);

            MessageBroker.Default.Receive<SellAllSkillsEvent>().Subscribe(_ => SellAllSkills());
        }

        private void SellAllSkills()
        {
            foreach (var presenter in _skillPresenters)
            {
                presenter.SellSkill();
            }
        }

        private void BuildSkillTree(IMoneyService moneyService, IPathfindingService pathfinding)
        {
            CreatePresenters(moneyService, pathfinding);
            ConnectPresenters();
        }

        private void CreatePresenters(IMoneyService moneyService, IPathfindingService pathfinding)
        {
            var baseSkills = new List<SkillPresenter>();

            for (int i = 0; i < _skillTreeModel.SkillModels.Count; i++)
            {
                var skillModel = _skillTreeModel.SkillModels[i];
                var skillPresenter = InstantiateSkillPresenter(skillModel, i, moneyService, pathfinding);

                _skillPresenters.Add(skillPresenter);
                if (skillModel.IsBaseSkill)
                {
                    baseSkills.Add(skillPresenter);
                }
            }
            
            foreach (var skillPresenter in _skillPresenters)
            {
                skillPresenter.SetBaseSkills(baseSkills);
            }
        }
        
        private SkillPresenter InstantiateSkillPresenter(SkillModel model, int index, IMoneyService moneyService,
            IPathfindingService pathfinding)
        {
            var position = new Vector2(model.Position.x, model.Position.y);
            var skillView = Instantiate(_skillPrefab, position, Quaternion.identity, transform);
            skillView.name = $"Skill {index}";
            var presenter = new SkillPresenter(model, moneyService, pathfinding, skillView);
            return presenter;
        }

        private void ConnectPresenters()
        {
            for (int i = 0; i < _skillPresenters.Count; i++)
            {
                var currentPresenter = _skillPresenters[i];
                var currentModel = _skillTreeModel.SkillModels[i];

                var neighbourPresenters = currentModel.NeighbourIndex
                    .Select(index => _skillPresenters[index])
                    .ToList();

                foreach (var neighbourPresenter in neighbourPresenters)
                {
                    currentPresenter.AddNeighbour(neighbourPresenter);
                    CreateEdgeBetween(currentPresenter, neighbourPresenter);
                }
            }
        }

        private void CreateEdgeBetween(SkillPresenter from, SkillPresenter to)
        {
            var edge = Instantiate(_edgePrefab, transform);
            edge.positionCount = 2;
            edge.SetPositions(new[]
            {
                from.ViewPosition, to.ViewPosition
            });
        }

        private void OnDestroy()
        {
            foreach (var skillPresenter in _skillPresenters)
            {
                skillPresenter.Dispose();
            }
        }
    }
}
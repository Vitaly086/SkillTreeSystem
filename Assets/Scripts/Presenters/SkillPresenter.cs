using System.Collections.Generic;
using System.Linq;
using Events;
using Models;
using Services;
using UniRx;
using UnityEngine;
using Views;

namespace Presenters
{
    public class SkillPresenter : MonoBehaviour

    {
        public int Cost => _model.Cost;
        public IReadOnlyReactiveProperty<SkillState> State => _model.State;

        [SerializeField] private SkillView _view;

        private readonly List<SkillPresenter> _neighbours = new List<SkillPresenter>();
        private List<SkillPresenter> _baseSkills;
        private IPathfindingService _pathfinding;
        private SkillModel _model;
        private IMoneyService _moneyService;

        public void Initialize(SkillModel model, IMoneyService moneyService, IPathfindingService pathfinding)
        {
            _model = model;
            _moneyService = moneyService;
            _pathfinding = pathfinding;

            SetDefaultData(model);
        }

        private void Start()
        {
            SubscribeOnStateUpdate();
            SubscribeOnMoneyUpdate();
            SubscribeOnClick();
        }

        public void AddNeighbours(SkillPresenter neighbour)
        {
            _neighbours.Add(neighbour);
        }

        public void SetBasePresenters(List<SkillPresenter> basePresenters)
        {
            _baseSkills = basePresenters;
        }

        public IReadOnlyList<SkillPresenter> GetNeighbours()
        {
            return _neighbours;
        }

        public void Buy()
        {
            if (!_model.IsBaseSkill && IsAvailable())
            {
                _model.ChangeState(SkillState.Bought);
                _moneyService.SubtractMoney(_model.Cost);
            }

            RefreshSelectedSkill();
        }

        public void Sell()
        {
            if (!_model.IsBaseSkill && WasBought())
            {
                _model.ChangeState(SkillState.Available);
                _moneyService.AddMoney(_model.Cost);
            }

            RefreshSelectedSkill();
        }

        private void SetDefaultData(SkillModel model)
        {
            if (model.IsBaseSkill)
            {
                _model.ChangeState(SkillState.Bought);
                _view.Text.text = $"{name} - base skill";
            }
            else
            {
                _model.ChangeState(SkillState.Unavailable);
                _view.Text.text = $"{name} cost {model.Cost} money";
            }
        }

        private void SubscribeOnClick()
        {
            _view.Button.OnClickAsObservable()
                .Subscribe(_ => CurrentSkillSelected())
                .AddTo(this);
        }

        private void CurrentSkillSelected()
        {
            MessageBroker.Default
                .Publish(new CurrentSkillSelectedEvent(this, CanBuy(), CanSell()));
        }

        private void SubscribeOnMoneyUpdate()
        {
            _moneyService.Money.Subscribe(_ => UpdateState()).AddTo(this);
        }

        private void SubscribeOnStateUpdate()
        {
            _model.State.Subscribe(state =>
            {
                _view.UpdateButton(state);
                _neighbours.ForEach(neighbour => neighbour.UpdateState());
            }).AddTo(this);
        }

        private void UpdateState()
        {
            if (WasBought())
            {
                return;
            }

            if (EnoughMoney() && IsAnyNeighbourBought())
            {
                _model.ChangeState(SkillState.Available);
                return;
            }

            _model.ChangeState(SkillState.Unavailable);
        }

        private bool CanBuy()
        {
            return !_model.IsBaseSkill && IsAvailable();
        }

        private bool CanSell()
        {
            return !_model.IsBaseSkill && WasBought() && IsAllNeighboursConnectBaseSkills();
        }

        private bool IsAllNeighboursConnectBaseSkills()
        {
            return _pathfinding
                .IsAllNeighboursConnectBaseSkills(this, _baseSkills);
        }

        private bool IsAvailable()
        {
            return _model.State.Value == SkillState.Available;
        }

        private bool EnoughMoney()
        {
            return _moneyService.Money.Value >= _model.Cost;
        }

        private bool WasBought()
        {
            return _model.State.Value == SkillState.Bought;
        }

        private bool IsAnyNeighbourBought()
        {
            return _neighbours.Any(presenter => presenter.State.Value == SkillState.Bought);
        }

        private void RefreshSelectedSkill()
        {
            MessageBroker.Default.Publish(new CurrentSkillSelectedEvent(null));
        }
    }
}
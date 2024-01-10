using System;
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
    public class SkillPresenter : IDisposable
    {
        public int Cost => _model.Cost;
        public IReadOnlyReactiveProperty<SkillState> State => _model.State;
        public Vector3 ViewPosition => _view.transform.position;
        public string SkillName => _model.Name;

        private readonly SkillView _view;
        private readonly List<SkillPresenter> _neighbours = new();
        private List<SkillPresenter> _baseSkills;
        private readonly IPathfindingService _pathfinding;
        private readonly SkillModel _model;
        private readonly IMoneyService _moneyService;
        private readonly CompositeDisposable _disposables = new();


        public SkillPresenter(SkillModel model, IMoneyService moneyService, IPathfindingService pathfinding,
            SkillView view)
        {
            _model = model;
            _moneyService = moneyService;
            _pathfinding = pathfinding;
            _view = view;

            Initialize();
        }

        private void Initialize()
        {
            _view.Initialize(_model.SkillIcon);
            SetDefaultData();
            SubscribeEvents();
        }

        public void AddNeighbour(SkillPresenter neighbour) => _neighbours.Add(neighbour);
        public void SetBaseSkills(List<SkillPresenter> basePresenters) => _baseSkills = basePresenters;
        public IReadOnlyList<SkillPresenter> GetNeighbours() => _neighbours;


        public void BuySkill()
        {
            if (!_model.IsBaseSkill && IsAvailable())
            {
                _model.ChangeState(SkillState.Bought);
                _moneyService.SubtractMoney(_model.Cost);
            }

            RefreshSelectedSkill();
        }

        public void SellSkill()
        {
            if (!_model.IsBaseSkill && IsBought())
            {
                _model.ChangeState(SkillState.Available);
                _moneyService.AddMoney(_model.Cost);
            }

            RefreshSelectedSkill();
        }

        private void SubscribeEvents()
        {
            _view.Button.OnClickAsObservable()
                .Subscribe(_ => CurrentSkillSelected())
                .AddTo(_disposables);

            _moneyService.Money.Subscribe(_ => UpdateState()).AddTo(_disposables);

            _model.State.Subscribe(state =>
            {
                _view.UpdateButton(state);
                _neighbours.ForEach(neighbour => neighbour.UpdateState());
            }).AddTo(_disposables);
        }

        private void SetDefaultData()
        {
            if (_model.IsBaseSkill)
            {
                _model.ChangeState(SkillState.Bought);
                _view.SetPrice("");
            }
            else
            {
                _model.ChangeState(SkillState.Unavailable);
                _view.SetPrice($"{_model.Cost}$");
            }
        }

        private void CurrentSkillSelected()
        {
            MessageBroker.Default
                .Publish(new CurrentSkillSelectedEvent(this, CanBuy(), CanSell()));
        }

        private void UpdateState()
        {
            if (IsBought()) return;
            _model.ChangeState(EnoughMoney() && IsAnyNeighbourBought() ? SkillState.Available : SkillState.Unavailable);
        }

        private bool CanBuy() => !_model.IsBaseSkill && IsAvailable();

        private bool CanSell() => !_model.IsBaseSkill && IsBought() &&
                                  _pathfinding.IsAllNeighboursConnectBaseSkills(this, _baseSkills);

        private bool IsAvailable() => _model.State.Value == SkillState.Available;

        private bool IsBought() => _model.State.Value == SkillState.Bought;
        private bool EnoughMoney() => _moneyService.Money.Value >= _model.Cost;

        private bool IsAnyNeighbourBought() => _neighbours.Any(presenter => presenter.State.Value == SkillState.Bought);

        private void RefreshSelectedSkill() => MessageBroker.Default.Publish(new CurrentSkillSelectedEvent(null));
        public void Dispose() => _disposables?.Dispose();
    }
}
﻿using System.Collections.Generic;
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
        public IReadOnlyReactiveProperty<SkillState> State => _state;

        [SerializeField] private SkillView _view;

        private readonly List<SkillPresenter> _neighbours = new List<SkillPresenter>();
        private readonly ReactiveProperty<SkillState> _state = new ReactiveProperty<SkillState>();
        private List<SkillPresenter> _basePresenters;
        private IPathfindingService _pathfinding;
        private SkillModel _model;
        private IMoneyService _moneyService;

        public void Initialize(SkillModel model, IMoneyService moneyService, IPathfindingService pathfinding)
        {
            _model = model;
            _moneyService = moneyService;
            _pathfinding = pathfinding;

            if (model.IsBaseSkill)
            {
                _state.Value = SkillState.Bought;
                _view.Text.text = $"{name} - base skill";
            }
            else
            {
                _state.Value = SkillState.Unavailable;
                _view.Text.text = $"{name} cost {model.Cost} money";
            }
        }

        private void Start()
        {
            SubscribeStateUpdate();
            SubscribeMoneyUpdate();
            SubscribeSelectedSkill();
        }

        public void AddNeighbours(SkillPresenter neighbour)
        {
            _neighbours.Add(neighbour);
        }

        public void SetBasePresenters(List<SkillPresenter> basePresenters)
        {
            _basePresenters = basePresenters;
        }

        public IReadOnlyList<SkillPresenter> GetNeighbours()
        {
            return _neighbours;
        }

        public void Buy()
        {
            if (!_model.IsBaseSkill && IsAvailable())
            {
                _state.Value = SkillState.Bought;
                _moneyService.SubtractMoney(_model.Cost);
            }

            RefreshSelectedSkill();
        }

        public void Sell()
        {
            if (!_model.IsBaseSkill && WasBought())
            {
                _state.Value = SkillState.Available;
                _moneyService.AddMoney(_model.Cost);
            }

            RefreshSelectedSkill();
        }

        private void SubscribeSelectedSkill()
        {
            _view.Button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (IsCanSell())
                    {
                        MessageBroker.Default
                            .Publish(new SelectCurrentPresenterEvent(currentPresenter: this, false, true));
                        return;
                    }

                    if (IsCanBuy())
                    {
                        MessageBroker.Default
                            .Publish(new SelectCurrentPresenterEvent(currentPresenter: this, true, false));
                        return;
                    }

                    MessageBroker.Default
                        .Publish(new SelectCurrentPresenterEvent(currentPresenter: this, false, false));
                })
                .AddTo(this);
        }

        private void SubscribeMoneyUpdate()
        {
            _moneyService.Money.Subscribe(_ => UpdateState()).AddTo(this);
        }

        private void SubscribeStateUpdate()
        {
            _state.Subscribe(state =>
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

            if (!WasBought() && EnoughMoney() &&
                IsAnyNeighbourBought(this))
            {
                _state.Value = SkillState.Available;
                return;
            }

            _state.Value = SkillState.Unavailable;
        }

        private bool IsCanBuy()
        {
            return !_model.IsBaseSkill && IsAvailable();
        }

        private bool IsCanSell()
        {
            return !_model.IsBaseSkill && WasBought() && IsAllNeighboursConnectBaseSkills();
        }

        private bool IsAllNeighboursConnectBaseSkills()
        {
            return _pathfinding
                .IsAllNeighboursConnectBaseSkills(this, _basePresenters);
        }

        private bool IsAvailable()
        {
            return _state.Value == SkillState.Available;
        }

        private bool EnoughMoney()
        {
            return _moneyService.Money.Value >= _model.Cost;
        }

        private bool WasBought()
        {
            return _state.Value == SkillState.Bought;
        }

        private bool IsAnyNeighbourBought(SkillPresenter currentPresenter)
        {
            return currentPresenter._neighbours.Any(presenter => presenter.State.Value == SkillState.Bought);
        }

        private void RefreshSelectedSkill()
        {
            MessageBroker.Default.Publish(new SelectCurrentPresenterEvent(null));
        }
    }
}
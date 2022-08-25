using Presenters;
using Services;
using UnityEngine;

namespace GameCore
{
    public class SkillTreeInitializer : MonoBehaviour
    {
        [SerializeField] private SkillTreePresenter[] _skillTreePresenters;
        
        private MoneyService _moneyService;
        private PathfindingService _pathfindingService;

        private void Awake()
        {
            var serviceLocator = ServiceLocator.Instance;
            RegisterServices(serviceLocator);

            foreach (var skillTreePresenter in _skillTreePresenters)
            {
                skillTreePresenter.Initialize(_moneyService, _pathfindingService);
            }
        }

        private void RegisterServices(ServiceLocator serviceLocator)
        {
            _moneyService = new MoneyService();
            serviceLocator.RegisterSingle<IMoneyService>(_moneyService);
            _pathfindingService = new PathfindingService();
            serviceLocator.RegisterSingle<IPathfindingService>(_pathfindingService);
        }
    }
}
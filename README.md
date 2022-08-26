## Skill  Tree 
В данном проекте разработана система Skill Tree с использованием ассета UniRx.
Архитектура проекта простороена на паттерне MV(R)P. Также используется паттерн ServiceLocator, Singletone, алгоритм поиска пути в глубину (DFS).

Тестовое задание:
[Кефир СПб — Тестовое задание.pdf](https://github.com/Vitaly086/Skill_Tree_Test/files/9423957/default.pdf)

## Задачи:

1) Индикатор количества денег. :heavy_check_mark:

2) Кнопка Заработать. При нажатии на нее игрок получает денеги. :heavy_check_mark:

3) Умения с номером и состоянием: изучено/не изучено. :heavy_check_mark:

4) Выделение только одного умения. Все взаимодействия производятся с выделенным умением. :heavy_check_mark:

5) Индикатор стоимости выделенного умения. :heavy_check_mark:

6) Кнопка Изучить. Активна только если выделенную способность можно изучить. :heavy_check_mark:

7) Кнопка Забыть. Активна только если выделенную способность можно забыть. :heavy_check_mark:

8) Кнопка Забыть все. Активна всегда, продает все умения. :heavy_check_mark:


## Дополнительно сделано:
1) Добавление множества деревье скиллов. :heavy_check_mark:

2) Создание цикличных связей скилов. :heavy_check_mark: 

3) Возможность выбора нескольких базовых скиллов. :heavy_check_mark: 

4) Умения отображают свою цену. :heavy_check_mark: 

5) Новые умения открываются при условии покупки предыдущего и достаточного количества денег. :heavy_check_mark: 

# Игра

https://user-images.githubusercontent.com/93872632/186951903-b7c72be2-eaef-4d3a-afb4-a8149f005141.mp4


# Игровая сцена

![изображение](https://user-images.githubusercontent.com/93872632/186952270-5166b61a-97d7-439d-b5c1-0ef8ae803396.png)

# Настройка и запуск игры

Дерево скиллов это ScriptableObject в котором задается модель дерева и [Skill Model](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Models/SkillModel.cs) каждого скилда,
в дальнейшем возможно переделать модель по JSON и получать информацию с сервера:

![изображение](https://user-images.githubusercontent.com/93872632/186952385-815b0afe-012c-4810-9158-5f829034904d.png)

За старт игры отвечает класс [Skill Tree Initializer](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/GameCore/SkillTreeInitializer.cs),
он содержит ссылки на все Skill Tree Presenter:

![изображение](https://user-images.githubusercontent.com/93872632/186649138-b6090277-c457-48c4-8959-f031db26dbca.png)

Каждое дерево имеет свой [Skill Tree Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillTreePresenter.cs),
который создает Skill Presenter, а также продает их всех:

![изображение](https://user-images.githubusercontent.com/93872632/186656785-d801d2c3-7414-4567-8220-1d2b659d912c.png)

[Skill Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillPresenter.cs)
это класс который объеденяет [Skill Model](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Models/SkillModel.cs)
и [Skill View](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Views/SkillView.cs).
В нем происходит основная логика взаимодействия с скиллами.
Данный класс подписывается на реактивное свойство Money в [Money Service](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Services/MoneyService.cs)
и на реактивное свойство [Skill State](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Models/SkillState.cs).
При изменении состояний денег или стейта у каждого скилла происходит обновление стейта, так работает логика включения и отключения интерактивности у скиллов.

```C#
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
```
При клике на скилл отправляется сообщение о выбранном текущем Скилле, на которое подписаные различные презентеры, 
например [Skill Price Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillPricePresenter.cs)

Проверка возможности продажи происходит в [Pathfinding Service](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Services/PathfindingService.cs),
в данном классе реализован алгоритм поиска пути в глубину (DFS).

```C#
private bool DFS(SkillPresenter currentPresenter, List<SkillPresenter> destinations)
        {
            if (!destinations.Contains(currentPresenter))
            {
                _usedNodes.Add(currentPresenter);
            }

            if (destinations.Contains(currentPresenter))
            {
                return true;
            }

            foreach (var neighbour in currentPresenter.GetNeighbours())
            {
                if (_usedNodes.Contains(neighbour) || neighbour.State.Value != SkillState.Bought)
                {
                    continue;
                }

                if (DFS(neighbour, destinations))
                {
                    _usedNodes.Clear();
                    return true;
                }
            }

            return false;
        }
```

Для отображения денег в UI [User Money Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/UserMoneyPresenter.cs) через [Service Locator](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Services/ServiceLocator.cs) получает ссылку на [IMoney Service](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Services/IMoneyService.cs) и подписывается на реактивное свойство Money. Класс [Service Locator](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Services/ServiceLocator.cs) использует паттерн [Singletone](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Services/Singleton.cs).


# Итог
Я реализовал все задачи ТЗ и расширил свою реализацию исходя из своего опыта в играх, где у персонажа может быть несколько веток скиллов для разных специализаций, которые можно покупать параллельно.
Например World of warcraft 

![изображение](https://user-images.githubusercontent.com/93872632/186959814-2f3eaf2d-2bd0-4238-9432-0881aa328c1a.png)


Моя реализация позволяет делать множество веток скиллов на одной сцене переиспользуя готовые классы.
Также в одной ветке можно сделать несколько базовых классов, что добавляет нам еще больший функционал в возможности постороения дерева скиллов.


В дальнейшем можно добавить функционал:
1) уровень каждого изучченого скилла;
2) получения Skill Model с сервера в форма JSON;


В моих знаниях, возможно, сейчас есть пробелы, но я готов много учитьтся и работать, чтобы исправить это.
Буду благодарен за обратную связь по выполнения ТЗ.







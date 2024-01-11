## Skill  Tree 
В данном проекте разработана система Skill Tree с использованием ассета UniRx.

Архитектура проекта построена на паттерне MVP.

В основе лежит использование:
1) RX
2) MessageBroker
3) ServiceLocator (можно заменить на Zenject)


## Сделано:

1) Индикатор количества денег. :heavy_check_mark:

2) Кнопка Заработать. При нажатии на нее игрок получает денеги. :heavy_check_mark:

3) Умения с номером и состоянием: изучено/не изучено. :heavy_check_mark:

4) Выделение только одного умения. Все взаимодействия производятся с выделенным умением. :heavy_check_mark:

5) Индикатор стоимости выделенного умения. :heavy_check_mark:

6) Кнопка Изучить. Активна только если выделенную способность можно изучить. :heavy_check_mark:

7) Кнопка Забыть. Активна только если выделенную способность можно забыть. :heavy_check_mark:

8) Кнопка Забыть все. Активна всегда, продает все умения. :heavy_check_mark:

9) Добавление множества деревьев скиллов. :heavy_check_mark:

10) Создание цикличных связей скиллов. :heavy_check_mark: 

11) Возможность выбора нескольких базовых скиллов. :heavy_check_mark: 

12) Умения отображают свою цену. :heavy_check_mark: 

13) Новые умения открываются при условии покупки предыдущего и достаточного количества денег. :heavy_check_mark: 

# Видео

https://github.com/Vitaly086/SkillTreeSystem/assets/93872632/1bb0f0a6-92f6-4d79-93b1-f9971c188178


# Настройка и запуск игры

Дерево скиллов это ScriptableObject в котором задается модель дерева и [Skill Model](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Models/SkillModel.cs) каждого скилла, в дальнейшем возможно переделать модель по JSON и получать информацию с сервера:

<img width="428" alt="Снимок экрана 2024-01-11 в 00 07 51" src="https://github.com/Vitaly086/SkillTreeSystem/assets/93872632/9db7ca72-a49e-455a-a17d-cb4e48b30f5d">


За старт игры отвечает класс [Skill Tree Initializer](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/GameCore/SkillTreeInitializer.cs), он содержит ссылки на все Skill Tree Presenter:

<img width="420" alt="Снимок экрана 2024-01-11 в 00 06 30" src="https://github.com/Vitaly086/SkillTreeSystem/assets/93872632/d3894380-e0ce-4eed-a843-a23614110f41">



На одном экране может быть несколько деревьев со скиллами, как в WOW:


![изображение](https://github.com/Vitaly086/SkillTreeSystem/assets/93872632/0b419982-2a30-421e-a2d0-c8c702b40518)



Каждое дерево имеет свой [Skill Tree Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillTreePresenter.cs), класс который отвечает за логику работы одного дерева скиллов:

<img width="432" alt="Снимок экрана 2024-01-11 в 00 07 24" src="https://github.com/Vitaly086/SkillTreeSystem/assets/93872632/b8c90665-a723-4739-8caf-701a45e26d74">


# Как происходит логика обновления состояния скилла?

При изменении денег или состояния соседних скиллов - [Skill Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillPresenter.cs) пересчитывает свое состояние.


```C#
private void UpdateState()
{
    if (IsBought()) return;
    
    if (EnoughMoney() && IsAnyNeighbourBought())
    {
        _model.ChangeState(SkillState.Available);
    }
    else
    {
        _model.ChangeState(SkillState.Unavailable);
    }
}
```

# Как происходит обновление UI при выделении скилла?
При клике на скилл, [Skill Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillPresenter.cs)
кидает в общую шину сообщение о текущем выбранном скилле, а UI подписывается на это сообщение, например [Skill Price Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillPricePresenter.cs).


```C#
private void CurrentSkillSelected()
{
    MessageBroker.Default
                .Publish(new CurrentSkillSelectedEvent(this, CanBuy(), CanSell()));
}
```

# Как происходит обновление UI при выделении скилла?
Для продажи скилла - необходимо надо знать, что все его купленные соседи соединены с базовым скиллом (без продаваемого скилла в пути).
Для этого используется алгоритм поиска пути в глубину, [Pathfinding Service](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Services/PathfindingService.cs) проверяет имеет ли каждый купленный сосед путь до базового скилла.

```C#
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
```



# Итог

Моя реализация системы скиллов позволяет делать множество веток скиллов на одной сцене переиспользуя готовые классы.
Также в одной ветке можно сделать несколько базовых классов, что добавляет нам еще больший функционал в возможности построения дерева скиллов.


В дальнейшем можно добавить функционал:
1) Уровень каждого изученного  скилла;
2) Получение Skill Model с сервера в форма JSON;
3) Встроить Zenject









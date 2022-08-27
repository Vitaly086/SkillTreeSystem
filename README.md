## Skill  Tree 
В данном проекте разработана система Skill Tree с использованием ассета UniRx.

Архитектура проекта построена на паттерне MV(R)P.

В основе лежит использование:
1) ServiceLocator (можно заменить на Zenject)
2) MessageBroker
3) RX

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


https://user-images.githubusercontent.com/93872632/187025485-f8fa7a2c-68d6-4a9c-9016-9f6697f0a4a9.mp4


# Игровая сцена


![изображение](https://user-images.githubusercontent.com/93872632/187030023-3f25ef27-fafd-4adf-a2c9-d27027f5f479.png)


# Настройка и запуск игры

Дерево скиллов это ScriptableObject в котором задается модель дерева и [Skill Model](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Models/SkillModel.cs) каждого скилла,
в дальнейшем возможно переделать модель по JSON и получать информацию с сервера:

![изображение](https://user-images.githubusercontent.com/93872632/186952385-815b0afe-012c-4810-9158-5f829034904d.png)

За старт игры отвечает класс [Skill Tree Initializer](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/GameCore/SkillTreeInitializer.cs), он содержит ссылки на все Skill Tree Presenter (на одном экране может быть несколько деревьев со скиллами, как в WOW):

![изображение](https://user-images.githubusercontent.com/93872632/186649138-b6090277-c457-48c4-8959-f031db26dbca.png)

Каждое дерево имеет свой [Skill Tree Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillTreePresenter.cs), класс который отвечает за логику работы одного дерева скиллов:

![изображение](https://user-images.githubusercontent.com/93872632/186656785-d801d2c3-7414-4567-8220-1d2b659d912c.png)

# Как происходит логика обновления состояния скилла?

При изменении денег или состояния соседних скиллов - [Skill Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillPresenter.cs) пересчитывает свое состояние.


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

# Как происходит обновление UI при выделении скилла?
При клике на скилл, [Skill Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillPresenter.cs)
кидает в общую шину сообщение о текущем выбранном скилле, а UI подписывается на это сообщение, например [Skill Price Presenter](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Presenters/SkillPricePresenter.cs).


```C#
private void CurrentSkillSelected()
{
    if (CanSell())
    {
        MessageBroker.Default
            .Publish(new SelectCurrentPresenterEvent(currentPresenter: this, isCanBuy: false,
                isCanSell: true));
        return;
    }

    if (CanBuy())
    {
        MessageBroker.Default
            .Publish(new SelectCurrentPresenterEvent(currentPresenter: this, isCanBuy: true,
                isCanSell: false));
        return;
    }

    MessageBroker.Default
        .Publish(new SelectCurrentPresenterEvent(currentPresenter: this, isCanBuy: false,
            isCanSell: false));
}
```

# Как происходит обновление UI при выделении скилла?
Для продажи скилла - необходимо надо знать, что все его купленные соседи соединены с базовым скиллом (без продаваемого скилла в пути).
Для этого используется алгоритм поиска пути в глубину, [Pathfinding Service](https://github.com/Vitaly086/Skill_Tree_Testgame/blob/master/Assets/Scripts/Services/PathfindingService.cs) проверяет имеет ли каждый купленный сосед путь до базового скилла.

```C#
private bool HasPathToNodes(SkillPresenter currentPresenter, List<SkillPresenter> destinations)
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

        if (HasPathToNodes(neighbour, destinations))
        {
            _usedNodes.Clear();
            return true;
        }
    }
{
```



# Итог
Я реализовал все задачи ТЗ и расширил свою реализацию исходя из своего опыта в играх, где у персонажа может быть несколько веток скиллов для разных специализаций, которые можно покупать параллельно.

Например World of warcraft 

![изображение](https://user-images.githubusercontent.com/93872632/186959814-2f3eaf2d-2bd0-4238-9432-0881aa328c1a.png)


Моя реализация позволяет делать множество веток скиллов на одной сцене переиспользуя готовые классы.
Также в одной ветке можно сделать несколько базовых классов, что добавляет нам еще больший функционал в возможности построения дерева скиллов.


В дальнейшем можно добавить функционал:
1) Уровень каждого изученного  скилла;
2) Получение Skill Model с сервера в форма JSON;
3) Встроить Zenject


В моих знаниях, возможно, сейчас есть пробелы, но я готов много учиться и работать, чтобы исправить это.
Буду благодарен за обратную связь по выполнению ТЗ.







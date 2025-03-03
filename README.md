# Block Tower
## 🎮 Геймплей

- Перетаскивайте блоки для построения башни
- Соблюдайте правила размещения блоков
- Удаляйте блоки, перетаскивая их в корзину
- Прогресс автоматически сохраняется

## 🛠 Технические особенности

- **Архитектура**: Проект построен с использованием Zenject (Dependency Injection) для обеспечения слабой связанности компонентов
- **Паттерны**: 
  - Factory Pattern для создания блоков
  - Observer Pattern для системы уведомлений
  - Strategy Pattern для правил размещения блоков
- **Сохранение**: Автоматическое сохранение состояния башни
- **UI**: Адаптивный пользовательский интерфейс с поддержкой различных разрешений экрана

## 📦 Зависимости

- DOTween
- Zenject (Extenject)
- UniRx

## 🔄 Система правил размещения

Проект поддерживает гибкую систему правил размещения блоков через интерфейс `IPlacementRule`:

```csharp
public interface IPlacementRule
{
    bool CanPlace(GameObject droppedCube, GameObject targetCube);
    string GetFailureMessage();
}
```

## 🎨 Кастомизация

Вы можете легко добавить новые типы блоков и правила размещения:

1. Создайте новую конфигурацию блоков `GameConfig` с помощью ScriptableObject
2. Реализуйте новое правило размещения, наследуясь от `IPlacementRule`
3. Добавьте правило в `Tower` через метод `AddPlacementRule`

 

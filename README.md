# Turret Rush

A small Unity prototype built as a test assignment: the player controls a turret mounted on a car that drives forward automatically along a generated road. Stickman enemies stand idle along the way; once the car gets close, an enemy starts running toward it and deals damage on contact. The player aims the turret by touch to shoot enemies before they reach the car. Survive with HP left by the time you reach the finish line to win.

## Core gameplay loop

1. The level starts with the camera behind the car, car idle.
2. Tap the screen to start — the car begins moving forward, the turret can be aimed by touch/drag.
3. Idle enemies activate and start chasing the car once it enters their detection range.
4. The turret auto-fires while the car is moving; bullets damage anything implementing `IDamageable`.
5. Reach the finish line with HP remaining → **You Win**. Run out of HP → **You Lose**.
6. Tap again after a win/lose to restart the level (fade transition, level regenerated, car and enemies reset).

## Demo
Video:
https://drive.google.com/file/d/1Tp8SkfU6UbWGf3M10KyuIoFDs1rd75af/view?usp=drivesdk

## Tech stack

- **Unity 6000.3.15f1**, Universal Render Pipeline (with a dedicated `Mobile` quality tier).
- **Zenject** for dependency injection (`GameInstaller`, `SceneContext`).
- **UniTask** for async/await level loading instead of coroutines.
- **New Input System**, touch-based (tap to start/restart, screen-position drag to aim the turret).
- **Unity's built-in `ObjectPool<T>`** for bullets.

## Project structure

```
Assets/Scripts/
├── Core/          Game state, level generation/loading, DI installer
├── Gameplay/
│   ├── Combat/    Health, damage interface, health bar UI
│   ├── Enemy/     Enemy interface/base class + concrete enemy logic
│   ├── Player/    Car movement, turret target detection
│   └── Turret/    Turret aiming, shooting, pooled bullets
├── Services/Input/ Generated Input System wrapper
└── UI/            Fader (scene transitions), overlay UI (win/lose/start)
```

Namespaces mirror the folder layout (`Core`, `Gameplay.Combat`, `Gameplay.Enemy`, `Gameplay.Player`, `Gameplay.Turret`, `Services.Input`, `UI`), which keeps responsibilities easy to locate and keeps assembly boundaries clean if the project grows.

## What's worth highlighting

- **Enemy system built for extension** — an `IEnemy` interface + `EnemyBase` abstract class. New archetypes just inherit `EnemyBase` and implement `Activate`/`ResetEnemy`; the rest of the game only talks to enemies through the interface.
- **Animator-driven UI transitions.** `Fader` and `GameStateScreens` are thin bridges between `GameManager`/`LevelLoader` events and `Animator` triggers — fade look, timing and easing live in the Animator Controller, not in code.
- **Event-driven game state.** `GameManager` exposes a `GameState` enum and `Action` events; every other system reacts to them instead of polling or referencing each other directly.
- **UniTask instead of coroutines**, used throughout (level transitions, camera shake, hit reactions) for cancellable, linear async flow.
- **Object pooling for bullets** via Unity's `ObjectPool<Bullet>`, since the turret fires continuously.
- **Zenject DI** for cross-cutting services (`GameManager`, `MainInputSystem`, `LevelLoader`, `LevelGenerator`) instead of singletons/`Find`.
- **A dedicated mobile quality tier** in `QualitySettings`, matching the touch-first input scheme.

## Running the project

1. Open the project in Unity 6000.3.15f1 with the Universal Render Pipeline.
2. Open `Assets/Scenes/Game.unity`.
3. Press Play. In the Editor, enable touch simulation from mouse (Window → Analysis → Input Debugger → Options → Simulate Touch Input From Mouse) since the input scheme is touch-only.
4. Tap/click to start the run, drag across the screen to aim the turret.

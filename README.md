# Turret Rush

A small Unity prototype built as a test assignment: the player controls a turret mounted on a car that drives forward automatically along a generated road. Stickman enemies stand idle along the way; once the car gets close, an enemy starts running toward it and deals damage on contact. The player aims the turret by touch to shoot enemies before they reach the car. Survive with HP left by the time you reach the finish line to win.

This README gives a general overview of the project. For a deep-dive into the architecture and a comparison against the original assignment, see [`Аналіз архітектури.md`](./Аналіз%20архітектури.md). For a list of possible improvements, see [`Варіанти покращення.md`](./Варіанти%20покращення.md).

## Core gameplay loop

1. The level starts with the camera behind the car, car idle.
2. Tap the screen to start — the car begins moving forward, the turret can be aimed by touch/drag.
3. Idle enemies activate and start chasing the car once it enters their detection range.
4. The turret auto-fires while the car is moving; bullets damage anything implementing `IDamageable`.
5. Reach the finish line with HP remaining → **You Win**. Run out of HP → **You Lose**.
6. Tap again after a win/lose to restart the level (fade transition, level regenerated, car and enemies reset).

## Tech stack

- **Unity 6000.x**, Universal Render Pipeline (with a dedicated `Mobile` quality tier).
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

- **Enemy system built for extension.** Enemies are defined through an `IEnemy` interface plus an `EnemyBase` abstract class (`Assets/Scripts/Gameplay/Enemy/`). Any new enemy archetype (a ranged enemy, a tank, a fast runner) just needs to inherit `EnemyBase` and implement `Activate`/`ResetEnemy` — the rest of the game (detection, spawning, damage) already talks to enemies purely through the interface, via `TryGetComponent<IEnemy>`/`IDamageable`. No other system needs to change to add a new enemy type.
- **A single, reusable "universal" Fader.** `UI/Fader.cs` is a thin bridge between game events (`LevelLoader.OnLevelLoadStart/Complete`) and an `Animator` (`FadeIn`/`FadeOut` triggers). The fade look, timing and easing all live in the Animator Controller, not in code — swapping the transition animation (a wipe, a different color, a logo splash) is a matter of editing the controller's clips, with zero script changes. The same pattern (`Animator` + trigger events driven by `GameManager`) is reused for `UIManager`, so all UI state transitions (`HideStart`, `ShowWin`, `ShowLose`, `Reset`) follow one consistent, designer-friendly convention.
- **Clean event-driven game state.** `GameManager` exposes a small `GameState` enum and `Action` events (`OnGameStarted`, `OnGameWon`, `OnGameLost`, `OnGameRestart`). Every other system (player, turret, UI, fader) only reacts to these events instead of polling state or referencing each other directly — this keeps systems decoupled and easy to test/replace independently.
- **Async level flow instead of coroutines.** `LevelLoader` uses `UniTask` to sequence "fade in → regenerate level → fade out" with proper `CancellationToken` support, which reads linearly instead of being spread across coroutine `yield` steps and callbacks.
- **Object pooling for bullets.** `TurretShooter` uses Unity's `ObjectPool<Bullet>` instead of `Instantiate`/`Destroy` per shot, which matters given the turret fires continuously during a run.
- **Dependency injection via Zenject.** Cross-cutting services (`GameManager`, `MainInputSystem`, `LevelLoader`, `LevelGenerator`) are injected into the classes that need them (`PlayerController`, `TurretController`, `TurretShooter`, `Fader`, `UIManager`) rather than looked up through singletons or `Find`, which keeps dependencies explicit and swappable.
- **A dedicated mobile quality tier** is configured in `QualitySettings` (reduced shadow distance/resolution, no MSAA, no soft particles), showing the project was built with mobile performance in mind from the start, matching the touch-first input scheme.

## Running the project

1. Open the project in Unity 6000.x with the Universal Render Pipeline.
2. Open `Assets/Scenes/Game.unity`.
3. Press Play. In the Editor, enable touch simulation from mouse (Window → Analysis → Input Debugger → Options → Simulate Touch Input From Mouse) since the input scheme is touch-only.
4. Tap/click to start the run, drag across the screen to aim the turret.

# StateMachineKit

StateMachineKit is a flexible, plug-and-play finite state machine (FSM) toolkit for .NET / game development.
It provides a lightweight abstraction for defining states, transitioning between them, and integrating with engines
(such as Godot via the companion `StateMachineKit.Godot` package).

## Packages

| Package | Version | Description |
|---------|---------|-------------|
| StateMachineKit | 2.0.0 | Core abstractions (interfaces, discovery attribute, helpers) supporting .NET 8 and .NET Standard 2.1. |
| StateMachineKit.Godot | 0.0.1 | Godot (C#) integration layer depending on the core package. |

## Key Features
- Generic, owner-driven state machine: `IStateMachine<TContext>` / `IState<TContext>`
- Attribute-based automatic state discovery (`[DiscoverableState]`)
- Extension methods for ticking (Update / FixedUpdate)
- Pluggable design; you keep your concrete state definitions decoupled
- Godot integration package for engine-specific hooks

## Quick Start (Core)
```csharp
public sealed class IdleState : IState<MyActor> { /* ... */ }
public sealed class MoveState : IState<MyActor> { /* ... */ }

var actor = new MyActor("Player");
var fsm = FiniteStateMachine.Create(actor);
fsm.Initialize<IdleState>();

// In your update loop
fsm.Tick(deltaTime);
```

## Godot Integration
Install the `StateMachineKit.Godot` NuGet package and reference it from your Godot C# project. The Godot layer
adds engine-friendly owner definitions and can be extended to tie into lifecycle callbacks (`_Process`, `_PhysicsProcess`).

## Versioning
- Core at 2.0.0 introduces improved initialization semantics and packaging metadata
- Godot integration starts at 0.0.1 (early preview)

## Roadmap
- Source generator for compile-time state registration
- Transition validation & visualization hooks
- Async lifecycle (optional)

## Contributing
Issues and PRs are welcome. Please include tests for behavioral changes.

## License
MIT – see `license.md`.

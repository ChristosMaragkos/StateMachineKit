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

// Assuming you've already defined classes
// that implement IStateMachine and IStateOwner.

var actor = new MyActor("Player");
var fsm = FiniteStateMachine.Create(actor);
fsm.Initialize<IdleState>();

// In your update loop
fsm.Tick(deltaTime);
```

## Godot Integration
Install the `StateMachineKit.Godot` NuGet package and reference it from your Godot C# project. The Godot layer
adds engine-friendly owner definitions and can be extended to tie into lifecycle callbacks (`_Process`, `_PhysicsProcess`).

For now, the only useful class within the Godot module is GodotState, which implements the `IState<TContext>` interface and contains virtual methods.
The state owner and state machine implementations proved redundant, as Godot does not load `GlobalClass`es from external Assemblies.

With that in mind:

## Quick Start (Godot)
```csharp
// First, define a global class extending the Godot node you want to manage states for, and implementing IStateOwner.
[GlobalClass]
public partial class Player : CharacterBody2D, IStateOwner{ /* ... */ }

// Then, in another file:

[GlobalClass]
public partial class PlayerStateMachine : Node, IStateMachine<Player> {
  // You're expected to write some logic of your own, depending on how you would like to manage States.
  // The current recommended way is to use Reflection to help State Machines discover State classes that meet
  // the criteria you set. This is the code snippet I provide as a reference:
  private readonly Dictionary<Type, IState<Player>> _states = new();
  private void FindAllStates()
        {
            _states.Clear();
            var stateType = typeof(IState<StateOwner>);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes()
                         .Where(t => stateType.IsAssignableFrom(t) 
                                     && t is { IsInterface: false, IsAbstract: false })
                         .Where(t => Attribute.IsDefined(t, typeof(DiscoverableStateAttribute))))
                if (Activator.CreateInstance(type) is IState<StateOwner> instance)
                    _states[type] = instance;
        }
  // . . .
}

// And then, you can declare your state classes by implementing IState<Player>, or extending GodotState<Player>.
```
StateMachineKit also provides a `DiscoverableStateAttribute` that you can use to separate classes you actually want to register in state machines.
It is recommended that you cache each set of states after discovery for optimization purposes.

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


using Godot;
using StateMachineKit.Core.Attributes;
using StateMachineKit.Core.Interfaces;

namespace StateMachineKit.Godot.Implementation;

public abstract partial class AbstractStateMachine<TContext> : Node, IStateMachine<TContext>
    where TContext : class, IStateOwner
{
    private readonly Dictionary<Type, IState<TContext>> _states = new();
    private bool _isInitialized;

    public TContext? Context { get; private set; }
    public IState<TContext>? CurrentState { get; private set; }

    public void AttachOwner(TContext owner = null!)
    {
        Context = FindOwner();
    }
    
    private TContext FindOwner()
    {
        return GetParent<TContext>() ?? throw new InvalidOperationException(
            $"The FSM's owner of type {typeof(TContext).Name} could not be found in the node hierarchy.");
    }

    public void Initialize<TState>() where TState : class, IState<TContext>
    {
        if (_isInitialized)
        {
            GD.PrintErr($"[{Context!.StateOwnerName}] The state machine has already been initialized.");
            return;
        }
        AttachOwner();
        FindAllStates();
        ChangeState<TState>();
        _isInitialized = true;
    }
    
    protected virtual void FindAllStates()
    {
        _states.Clear();
        
        var stateType = typeof(IState<TContext>);
        var assembly = GetType().Assembly;
        foreach (var type in assembly.GetTypes()
                     .Where(t => stateType.IsAssignableFrom(t)
                                 && t is { IsInterface: false, IsAbstract: false })
                     .Where(t => Attribute.IsDefined(t, typeof(DiscoverableStateAttribute))))
            if (Activator.CreateInstance(type) is IState<TContext> state)
                _states[type] = state;
    }

    public void ChangeState<TState>() where TState : class, IState<TContext>
    {
        if (!TryChangeState<TState>())
        {
            throw new InvalidOperationException(
                $"State of type {typeof(TState).Name} is not registered in the state machine.");
        }
    }

    public bool TryChangeState<TState>() where TState : class, IState<TContext>
    {
        if (!_states.TryGetValue(typeof(TState), out var newState))
        {
            GD.PrintErr($"[{Context?.StateOwnerName}] State of type {typeof(TState).Name} does not exist.");
            return false;
        }

        var oldState = CurrentState;
        oldState?.OnExit(Context ?? throw new NullReferenceException(
            $"[{nameof(Context)}] is null. Ensure the state machine has an owner assigned."));
        
        CurrentState = newState;
        newState.OnEnter(Context!, oldState);
        
        return true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        this.Tick((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        this.FixedTick((float)delta);
    }
}

/// <summary>
/// A base implementation of a state for Godot.
/// Makes it easier to create states by providing 
/// default (empty) implementations of the interface methods.
/// </summary>
/// <typeparam name="TContext">The context surrounding the state. Typically, the type of its owner.</typeparam>
public abstract class GodotState<TContext> : IState<TContext>
    where TContext : class, IStateOwner
{
    public virtual void OnEnter(TContext ctx,
        IState<TContext>? from = null){}

    public virtual void OnExit(TContext ctx){}

    public virtual void OnUpdate(TContext ctx,
        IStateMachine<TContext> stateMachine,
        float deltaTime = 0){}

    public virtual void OnFixedUpdate(TContext ctx,
        IStateMachine<TContext> stateMachine,
        float deltaTime = 0){}
}
namespace StateMachineKit.Core.Interfaces
{
    /// <summary>
    /// Abstraction for a state in a state machine.
    /// Defines the contract for entering, exiting, and updating states.
    /// </summary>
    /// <typeparam name="TContext">The context provided to this State, usually its owner.</typeparam>
    public interface IState<TContext> 
        where TContext : class, IStateOwner
    {
        /// <summary>
        /// Called when the state is entered.
        /// The logic placed here will be executed once when the state is activated.
        /// </summary>
        /// <param name="ctx">The context surrounding this State. Usually its Owner.</param>
        /// <param name="from">The previous State. Optional.</param>
        void OnEnter(TContext ctx, IState<TContext>? from = null);

        /// <summary>
        /// Called when the state is exited.
        /// Use to perform cleanup or commit results before switching to another state.
        /// </summary>
        /// <param name="ctx">The context (owner) that hosts this state.</param>
        void OnExit(TContext ctx);

        /// <summary>
        /// Called every update tick.
        /// </summary>
        /// <param name="ctx">The context (owner) that hosts this state.</param>
        /// <param name="stateMachine">The state machine managing this state.</param>
        /// <param name="deltaTime">Elapsed time since last update (optional).</param>
        void OnUpdate(TContext ctx, IStateMachine<TContext> stateMachine, float deltaTime = 0);

        /// <summary>
        /// Called on fixed-timestep updates (e.g. physics).
        /// </summary>
        /// <param name="ctx">The context (owner) that hosts this state.</param>
        /// <param name="stateMachine">The state machine managing this state.</param>
        /// <param name="deltaTime">Elapsed fixed timestep (optional).</param>
        void OnFixedUpdate(TContext ctx, IStateMachine<TContext> stateMachine, float deltaTime = 0);
    }

    /// <summary>
    /// Contract for a state machine that manages transitions between states for a given owner.
    /// </summary>
    /// <typeparam name="TContext">Type of the owning context.</typeparam>
    public interface IStateMachine<TContext> where TContext : class, IStateOwner
    {
        
        /// <summary>
        /// Gets the context (owner) associated with this state machine.
        /// </summary>
        TContext Context { get; }
        /// <summary>
        /// The state currently active.
        /// </summary>
        IState<TContext> CurrentState { get; }

        /// <summary>
        /// Attaches the owning context. Must be called before initialization
        /// if not provided via constructor.
        /// </summary>
        /// <param name="owner">The owner instance.</param>
        void AttachOwner(TContext owner);
        
        /// <summary>
        /// Initializes the state machine to the specified starting state.
        /// Must be called before using the state machine.
        /// </summary>
        /// <typeparam name="TState">The type of the state to initialize the
        /// state machine with.</typeparam>
        void Initialize<TState>() where TState : class, IState<TContext>;

        /// <summary>
        /// Forces a transition to the specified state type.
        /// </summary>
        /// <typeparam name="TState">Target state type.</typeparam>
        void ChangeState<TState>() where TState : class, IState<TContext>;

        /// <summary>
        /// Attempts a transition to the specified state type, returning
        /// false if it cannot occur.
        /// </summary>
        /// <typeparam name="TState">Target state type.</typeparam>
        /// <returns>True if the transition occurred.</returns>
        bool TryChangeState<TState>() where TState : class, IState<TContext>;
    }

    /// <summary>
    /// Represents an entity that owns and drives a state machine.
    /// </summary>
    public interface IStateOwner
    {
        /// <summary>
        /// Friendly name for debugging/logging.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Called to set up the owner before use.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called to clean up resources when the owner is no longer needed.
        /// </summary>
        void Destroy();
    }

    /// <summary>
    /// Helper extensions for state machine operations.
    /// </summary>
    public static class StateMachineExtensions
    {
        /// <summary>
        /// Ticks the current state, calling its update logic.
        /// Should be called every frame or update cycle.
        /// </summary>
        /// <param name="sm">The state machine instance this is called on.</param>
        /// <param name="deltaTime">The time that has elapsed since the last frame.
        /// Defaults to zero.</param>
        /// <typeparam name="TContext">The context associated with this State Machine.</typeparam>
        public static void Tick<TContext>(this IStateMachine<TContext> sm, float deltaTime = 0)
            where TContext : class, IStateOwner
        {
            var ctx = sm.Context;
            sm.CurrentState.OnUpdate(ctx, sm, deltaTime);
        }
        
        /// <summary>
        /// Ticks the current state for fixed-timestep updates.
        /// Should be called at consistent intervals, e.g. for physics updates.
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="deltaTime"></param>
        /// <typeparam name="TContext"></typeparam>
        public static void FixedTick<TContext>(this IStateMachine<TContext> sm, float deltaTime)
            where TContext : class, IStateOwner
        {
            var ctx = sm.Context;
            sm.CurrentState.OnFixedUpdate(ctx, sm, deltaTime);
        }
    }
}
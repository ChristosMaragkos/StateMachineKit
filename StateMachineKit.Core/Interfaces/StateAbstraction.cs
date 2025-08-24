using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachineKit.Core.Interfaces
{
    /// <summary>
    /// Interface for a state that can be managed by a state machine.
    /// The state is owned by a class that implements the IStateOwner interface.
    /// The state machine is responsible for managing the current state of the owner,
    /// changing states, and calling the appropriate methods on the current state.
    /// The state should implement the Enter, Exit, Update, and PhysicsUpdate methods
    /// to handle its own logic and behavior.
    /// </summary>
    /// <typeparam name="TStateOwner"></typeparam>
    public interface IState<TStateOwner> where TStateOwner : class, IStateOwner
    {
        /// <summary>
        /// Called when the state is entered.
        /// You can optionally pass the previous State to this method.
        /// This is useful for state transitions where you need to know the previous state.
        /// </summary>
        /// <param name="previousState">The previous State. Optional.</param>
        void Enter(IState<TStateOwner>? previousState = null);
        
        /// <summary>
        /// Automatically called when the state is exited.
        /// This method should contain any cleanup logic that needs to
        /// be performed when the state is exited (e.g. unsubscribing from events).
        /// Has no functionality by default, but can be overridden in derived classes.
        /// </summary>
        void Exit(){}
        
        /// <summary>
        /// Intended to be called automatically by the state machine once per processing step.
        /// </summary>
        void Update();
        
        /// <summary>
        /// Intended to be called automatically by the state machine once per physics step.
        /// This is useful for physics-related updates that need to
        /// be processed after all other updates.
        /// </summary>
        /// <param name="deltaTime">The time that has elapsed since the last physics step.</param>
        void PhysicsUpdate(float deltaTime = 0f);
        
        /// <summary>
        /// The state machine that owns this state.
        /// This is used to access the state machine's functionality, such as changing states.
        /// </summary>
        IStateMachine<TStateOwner> StateMachine { get; set; }

        /// <summary>
        /// The owner of the state. This is used to access the owner's functionality, such as
        /// getting the owner's name or calling the owner's methods.
        /// </summary>
        TStateOwner Owner { get; set; }
    }
    
    /// <summary>
    /// Interface for a state machine that manages states for a specific owner.
    /// The owner is a class that implements the IStateOwner interface.
    /// The state machine is responsible for managing the current state of the owner,
    /// changing states, and calling the appropriate methods on the current state.
    /// The state machine should be initialized with an initial state before it can be used.
    /// The state machine automatically calls the Enter method of the initial state
    /// when it is initialized, and the Exit method of the current state
    /// when the state is changed to a new state.
    /// The state machine also automatically calls the Update and PhysicsUpdate methods
    /// of the current state when the Update and PhysicsUpdate methods of the state machine are called.
    /// This allows the state machine to manage the current state of the owner
    /// and ensure that the appropriate methods are called on the current state
    /// at the appropriate times.
    /// </summary>
    /// <typeparam name="TStateOwner"></typeparam>
    public interface IStateMachine<TStateOwner> where TStateOwner : class, IStateOwner
    {
        /// <summary>
        /// Type-safe dictionary of all states that are managed by the state machine.
        /// The key is the type of the state, and the value is the state itself.
        /// </summary>
        Dictionary<Type, IState<TStateOwner>> States { get; }
        
        /// <summary>
        /// The owner of the state machine.
        /// Necessary to set the owner of each state when the state machine is initialized.
        /// </summary>
        public TStateOwner Owner { get; set; }
        
        /// <summary>
        /// Abstract method to find the owner of the state machine.
        /// </summary>
        /// <returns></returns>
        TStateOwner FindOwner();
        
        /// <summary>
        /// The current state of the state machine.
        /// This property is automatically updated when the state machine changes states.
        /// It is used to access the current state's functionality,
        /// such as calling its Update or PhysicsUpdate methods.
        /// </summary>
        public IState<TStateOwner> CurrentState { get; set; }

        /// <summary>
        /// Method to initialize the state machine with an initial state.
        /// This method should be called once before the state machine is used.
        /// The initial state will be set as the current state of the state machine.
        /// The state machine will automatically call the Enter method of the initial state.
        /// The initial state should not be null.
        /// </summary>
        /// <param name="initialState">The state to initialize the State Machine with.</param>
        void Initialize(IState<TStateOwner> initialState)
        {
            if (initialState == null) throw new ArgumentNullException(nameof(initialState));
            Owner ??= FindOwner();

            States.Clear();
            
            // Collect explicitly provided states from override plus reflection discovery.
            var explicitStates = GetAllStates() ?? new List<IState<TStateOwner>>();
            var discoveredTypes = DiscoverStateTypes();
            foreach (var t in discoveredTypes)
            {
                if (States.ContainsKey(t) || explicitStates.Any(s => s.GetType() == t) || t == initialState.GetType()) continue;
                if (Activator.CreateInstance(t) is IState<TStateOwner> inst) explicitStates.Add(inst);
            }
            
            var all = new List<IState<TStateOwner>> { initialState };
            all.AddRange(explicitStates.OfType<IState<TStateOwner>>()
                .Where(s => !ReferenceEquals(s,
                    initialState))
                .Where(s => s.GetType() != initialState.GetType()));

            foreach (var state in all)
            {
                Attach(state);
                States[state.GetType()] = state;
            }

            CurrentState = initialState;
            CurrentState.Enter();
        }
        
        /// <summary>
        /// Generic, type-safe retrieval.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        TState GetState<TState>() where TState : class, IState<TStateOwner>
        {
            return (TState)States[typeof(TState)];
        }
        
        /// <summary>
        /// Tries to get the state of the specified type.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        bool TryGetState<TState>(out TState? state) where TState : class, IState<TStateOwner>
        {
            if (States.TryGetValue(typeof(TState), out var value))
            {
                state = (TState)value;
                return true;
            }
            state = null;
            return false;
        }

        /// <summary>
        /// Method to change the current state of the state machine.
        /// Automatically calls the Exit method of the current state
        /// and the Enter method of the new state.
        /// The new state should not be null.
        /// </summary>
        /// <param name="newStateType"></param>
        void ChangeState(Type newStateType)
        {
            if (newStateType == null) throw new ArgumentNullException(nameof(newStateType));
            if (CurrentState == null) throw new InvalidOperationException("StateMachine not initialized – CurrentState is null.");
            if (!States.TryGetValue(newStateType, out var next)) throw new KeyNotFoundException($"State of type {newStateType.Name} not registered.");
            if (ReferenceEquals(CurrentState, next)) return; // no-op transition
            var previousState = CurrentState;
            previousState.Exit();
            CurrentState = next;
            CurrentState.Enter(previousState);
        }
        
        /// <summary>
        /// Generic convenience.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        void ChangeState<TState>() where TState : class, IState<TStateOwner>
        {
            ChangeState(typeof(TState));
        }
        
        /// <summary>
        /// Tries to change the state to the specified type.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        bool TryChangeState<TState>() where TState : class, IState<TStateOwner>
        {
            if (!States.ContainsKey(typeof(TState))) return false;
            ChangeState(typeof(TState));
            return true;
        }

        /// <summary>
        /// New: enforce previous state type at compile-time.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        bool TryTransition<TFrom, TTo>()
            where TFrom : class, IState<TStateOwner>
            where TTo : class, IState<TStateOwner>
        {
            if (CurrentState is TFrom && States.ContainsKey(typeof(TTo)))
            {
                ChangeState(typeof(TTo));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Automatically calls the Update method of the current state.
        /// This method should be called once per processing step,
        /// such as in _Process in Godot.
        /// </summary>
        void ExecuteStateUpdate(){ CurrentState?.Update(); }
        
        /// <summary>
        /// Automatically calls the PhysicsUpdate method of the current state.
        /// This method should be called once per physics step,
        /// such as in _PhysicsProcess in Godot.
        /// The deltaTime parameter is the time that has elapsed since the last physics step.
        /// This is useful for physics-related updates
        /// that need to be processed after all other updates.
        /// </summary>
        void ExecuteStatePhysics(float deltaTime = 0f){ CurrentState?.PhysicsUpdate(deltaTime); }
        
        /// <summary>
        /// Attaches a state to the state machine.
        /// This method is used to set the owner and state machine properties
        /// of the state when it is created or added to the state machine.
        /// </summary>
        /// <param name="state">The state to attach.</param>
        void Attach(IState<TStateOwner> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            state.Owner = Owner;
            state.StateMachine = this;
        }
        
        /// <summary>
        /// Default: user can override to manually list instances (preferred for DI / custom ctor states).
        /// </summary>
        /// <returns></returns>
        List<IState<TStateOwner>> GetAllStates() => new List<IState<TStateOwner>>();
        
        /// <summary>
        /// Reflection-based type discovery (parameterless ctor only) – override to customize or disable.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> DiscoverStateTypes()
        {
            var stateInterface = typeof(IState<TStateOwner>);
            var assemblies = new[] { Owner?.GetType().Assembly, typeof(TStateOwner).Assembly }.Where(a => a != null).Distinct();
            foreach (var asm in assemblies)
            {
                foreach (var t in asm!.GetTypes())
                {
                    if (t.IsAbstract) continue;
                    if (!stateInterface.IsAssignableFrom(t)) continue;
                    if (t.GetConstructor(Type.EmptyTypes) == null) continue;
                    yield return t;
                }
            }
        }
    }

    /// <summary>
    /// Interface for a state owner.
    /// The owner is a class that implements this interface and
    /// is responsible for managing its own state.
    /// The owner should implement the Initialize and Destroy
    /// methods to handle its own initialization and cleanup.
    /// The Name property is used for debugging purposes
    /// to identify the owner in logs or error messages.
    /// The owner can be used to access and alter its own state via the state machine.
    /// </summary>
    public interface IStateOwner
    {
        /// <summary>
        /// The name of the owner, used for debugging purposes.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Called when the owner is initialized.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called when the owner is destroyed.
        /// </summary>
        void Destroy();
    }
    
    public static class StateMachineExtensions
    {
        public static void Initialize<TStateOwner>(this IStateMachine<TStateOwner> stateMachine, IState<TStateOwner> initialState) where TStateOwner : class, IStateOwner
        {
            stateMachine.Initialize(initialState);
        }
    }
}
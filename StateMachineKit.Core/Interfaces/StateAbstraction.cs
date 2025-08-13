namespace StateMachineKit.Core.Interfaces
{
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
        IStateMachine<TStateOwner> StateMachine { get; protected set; }

        /// <summary>
        /// The owner of the state. This is used to access the owner's functionality, such as
        /// getting the owner's name or calling the owner's methods.
        /// </summary>
        TStateOwner Owner { get; protected set; }
    }
    
    public interface IStateMachine<TStateOwner> where TStateOwner : class, IStateOwner
    {
        /// <summary>
        /// Method to initialize the state machine with an initial state.
        /// This method should be called once before the state machine is used.
        /// The initial state will be set as the current state of the state machine.
        /// The state machine will automatically call the Enter method of the initial state.
        /// The initial state should not be null.
        /// </summary>
        /// <param name="initialState">The state to initialize the State Machine with.</param>
        void Initialize(IState<TStateOwner> initialState);
        
        /// <summary>
        /// The owner of the state machine.
        /// Necessary to set the owner of each state when the state machine is initialized.
        /// </summary>
        IStateOwner Owner { get;}
        
        /// <summary>
        /// The current state of the state machine.
        /// This property is automatically updated when the state machine changes states.
        /// It is used to access the current state's functionality,
        /// such as calling its Update or PhysicsUpdate methods.
        /// </summary>
        protected IState<TStateOwner> CurrentState { get; set; }

        /// <summary>
        /// Method to change the current state of the state machine.
        /// Automatically calls the Exit method of the current state
        /// and the Enter method of the new state.
        /// The new state should not be null.
        /// </summary>
        /// <param name="newState"></param>
        void ChangeState(IState<TStateOwner> newState)
        {
            CurrentState.Exit();
            var previousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter(previousState);
        }

        /// <summary>
        /// Automatically calls the Update method of the current state.
        /// This method should be called once per processing step,
        /// such as in _Process in Godot.
        /// </summary>
        void Update(){CurrentState.Update();}
        
        /// <summary>
        /// Automatically calls the PhysicsUpdate method of the current state.
        /// This method should be called once per physics step,
        /// such as in _PhysicsProcess in Godot.
        /// The deltaTime parameter is the time that has elapsed since the last physics step.
        /// This is useful for physics-related updates
        /// that need to be processed after all other updates.
        /// </summary>
        void PhysicsUpdate(){CurrentState.PhysicsUpdate(deltaTime:0f);}
    }

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
}
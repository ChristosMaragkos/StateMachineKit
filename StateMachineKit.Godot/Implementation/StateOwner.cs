using Godot;
using Godot.Globalizer.Attributes;
using StateMachineKit.Core.Interfaces;

namespace StateMachineKit.Godot.Implementation;

// -------------------------------------------------------------------------------
// StateOwner Implementations
// -------------------------------------------------------------------------------
// These are example implementations of IStateOwner for different Godot node types.
// You can create your own implementations based on your project's requirements.
// -------------------------------------------------------------------------------
// Each implementation includes a StateOwnerName property that combines the node's name
// with a unique identifier if MakeNameUnique is set to true. This helps ensure that
// each state owner can be uniquely identified, which is useful in complex scenes.
// -------------------------------------------------------------------------------
// If you wish to create your own StateOwnerType, define a global class extending
// any Node type and implement the IStateOwner interface.
// -------------------------------------------------------------------------------

[GlobalizerWrap("StateMachineOwner2D")]
public partial class StateOwner2D : CharacterBody2D, IStateOwner
{
    public string StateOwnerName => Name + (MakeNameUnique ? $"_{GetInstanceId()}" : "");

    [Export] public bool MakeNameUnique = true;

    public virtual void Initialize(){}

    public virtual void Destroy()
    {
        QueueFree();
    }
}

[GlobalizerWrap("StateMachineOwner3D")]
public partial class StateOwner3D : CharacterBody3D, IStateOwner
{
    public string StateOwnerName => Name + (MakeNameUnique ? $"_{GetInstanceId()}" : "");

    [Export] public bool MakeNameUnique = true;

    public virtual void Initialize(){}

    public virtual void Destroy()
    {
        QueueFree();
    }
}

[GlobalizerWrap("StateMachineOwner")]
public partial class StateOwner : Node, IStateOwner
{
    public string StateOwnerName => Name + (MakeNameUnique ? $"_{GetInstanceId()}" : "");
    
    public virtual void Initialize(){}
    
    [Export] public bool MakeNameUnique = true;

    public virtual void Destroy()
    {
        QueueFree();
    }
}
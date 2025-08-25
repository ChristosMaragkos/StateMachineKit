using Godot;
using StateMachineKit.Core.Interfaces;

namespace StateMachineKit.Godot.Implementation;

[GlobalClass]
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

[GlobalClass]
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

[GlobalClass]
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
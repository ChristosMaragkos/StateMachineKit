using Godot;
using Godot.NativeInterop;

namespace StateMachineKit.Godot.Implementation {

partial class StateOwner3D
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.SaveGodotObjectData(info);
        info.AddProperty(PropertyName.@MakeNameUnique, global::Godot.Variant.From<bool>(this.@MakeNameUnique));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreGodotObjectData(global::Godot.Bridge.GodotSerializationInfo info)
    {
        base.RestoreGodotObjectData(info);
        if (info.TryGetProperty(PropertyName.@MakeNameUnique, out var _value_MakeNameUnique))
            this.@MakeNameUnique = _value_MakeNameUnique.As<bool>();
    }
}

}

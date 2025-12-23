using Robust.Shared.GameStates;

namespace Content.Shared._Cathedral.EntityBound;

/// <summary>
/// Used to ensure that an entity cannot leave the range of another entity or a specific coordinate.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EntityBoundComponent : Component
{
    [AutoNetworkedField, DataField]
    public EntityUid? BoundTo;

    [AutoNetworkedField, DataField]
    public float Range = 10f;

    [AutoNetworkedField, DataField]
    public string? JointId;
}

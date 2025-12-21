using Robust.Shared.GameStates;

namespace Content.Shared._Cathedral.EntityBound;


/// <summary>
/// Designates that this entity is connected to a <see cref="EntityBoundComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EntityAnchorComponent : Component
{
    [AutoNetworkedField, DataField]
    public EntityUid? BoundEntity;

    /// <summary>
    /// Refers to the invisible entity that holds the joint down.
    /// </summary>
    [AutoNetworkedField, DataField]
    public EntityUid? FollowingEnt;
}

using System.Diagnostics;
using System.Numerics;
using Content.Shared.Actions.Components;
using Content.Shared.Follower;
using Content.Shared.Hands;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared._Cathedral.EntityBound;

/// <summary>
///     Event raised directed at items or clothing when they are equipped or held. In order for an item to grant actions some
///     system can subscribe to this event and add actions to the <see cref="Actions"/> list.
/// </summary>
/// <remarks>
///     Note that a system could also just manually add actions as a result of a <see cref="GotEquippedEvent"/> or <see
///     cref="GotEquippedHandEvent"/>. This exists mostly as a convenience event, while also helping to keep
///     action-granting logic separate from general equipment behavior.
/// </remarks>
public abstract class SharedEntityBoundSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _xForm = default!;
    [Dependency] private readonly SharedJointSystem _joints = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly FollowerSystem _follow = default!;
    [Dependency] private readonly INetManager _net = default!;
    public override void Initialize()
    {
        base.Initialize();

        // UpdatesAfter.Add(typeof(SharedPhysicsSystem));
        // UpdatesOutsidePrediction = true;
        // SubscribeLocalEvent<EntityAnchorComponent, MoveEvent>(OnAnchorMove);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateAnchorMovement(frameTime);
    }

    private void UpdateAnchorMovement(float frameTime)
    {
        var anchorQuery = GetEntityQuery<EntityAnchorComponent>();
        var query = EntityQueryEnumerator<EntityAnchorComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (!anchorQuery.TryGetComponent(uid, out var anchor) || anchor.FollowingEnt == null)
            {
                continue;
            }

            if (!TryComp<PhysicsComponent>(uid, out var physics))
                return;
            // SetCoordinates makes sure that it also teleports with the user and helps thwart overzealous prediction.
            _physics.SetLinearVelocity(anchor.FollowingEnt.Value, physics.LinearVelocity);
            if (!_xForm.InRange(anchor.FollowingEnt.Value, uid, 2.5f))
                _xForm.SetCoordinates(anchor.FollowingEnt.Value, Transform(uid).Coordinates);
        }
    }

    public bool BindToEntity(EntityUid bound, EntityUid target, float? range = null)
    {
        if (TerminatingOrDeleted(bound))
            return false;

        EnsureComp<EntityBoundComponent>(bound, out var boundComp);
        EnsureComp<EntityAnchorComponent>(target, out var anchorComp);

        // Indicates that this is already bound to something.
        if (boundComp.JointId != null)
            return false;

        boundComp.BoundTo = target;

        var spawnedAnchor = Spawn("BoundAnchorEntity", Transform(target).Coordinates);
        anchorComp.FollowingEnt = spawnedAnchor;
        _physics.WakeBody(spawnedAnchor);

        if (anchorComp.FollowingEnt == null)
            return false;

        var bindingJoint = _joints.CreateDistanceJoint(bound, spawnedAnchor, id: "bindingJoint");
        bindingJoint.MaxLength = range ?? boundComp.Range;
        bindingJoint.MinLength = 0f;
        bindingJoint.Stiffness = 0f;
        boundComp.JointId = bindingJoint.ID;

        Dirty(bound, boundComp);
        Dirty(target, anchorComp);
        return true;
    }

    public abstract void SetJointStatus(EntityUid ent, bool status, EntityBoundComponent? bound = null);
}

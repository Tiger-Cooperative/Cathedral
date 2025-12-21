using Content.Shared.Follower;
using Robust.Shared.Containers;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared._Cathedral.EntityBound;

/// <summary>
/// This system allows the game to 'bind' certain entities to others.
/// The entity that is attached TO cannot be pulled on, but can pull on the 'bound' entity.
/// </summary>
public sealed class EntityBoundSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _xForm = default!;
    [Dependency] private readonly SharedJointSystem _joints = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly IGameTiming _timer = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateAnchorMovement();
    }

    private void UpdateAnchorMovement()
    {
        var query = EntityQueryEnumerator<EntityAnchorComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.FollowingEnt == null)
            {
                continue;
            }

            if (!TryComp<PhysicsComponent>(uid, out var physics))
                return;
            _physics.SetLinearVelocity(comp.FollowingEnt.Value, physics.LinearVelocity);
            // SetCoordinates makes sure that it also teleports with the user and helps thwart overzealous prediction.
            // Uses a range query to not lag out the client constantly.
            if (!_xForm.InRange(comp.FollowingEnt.Value, uid, 1f))
                _xForm.SetCoordinates(comp.FollowingEnt.Value, Transform(uid).Coordinates);
        }
    }

    public void SetJointStatus(EntityUid ent, bool status, EntityBoundComponent? bound = null)
    {
        if (!Resolve(ent, ref bound, false)
            || !TryComp<JointComponent>(ent, out var joint)
            || bound.JointId == null
            || !joint.GetJoints.TryGetValue(bound.JointId, out var bindJoint))
            return;
        _joints.SetEnabled(bindJoint, status);
    }

    public void BindToEntity(EntityUid bound, EntityUid target, float? range = null)
    {
        if (TerminatingOrDeleted(bound) || !_timer.IsFirstTimePredicted)
            return;

        EnsureComp<EntityBoundComponent>(bound, out var boundComp);
        EnsureComp<EntityAnchorComponent>(target, out var anchorComp);

        // Indicates that this is already bound to something.
        if (boundComp.JointId != null)
            return;

        boundComp.BoundTo = target;

        var spawnedAnchor = Spawn("BoundAnchorEntity", Transform(target).Coordinates);
        anchorComp.FollowingEnt = spawnedAnchor;
        _physics.WakeBody(spawnedAnchor);

        if (anchorComp.FollowingEnt == null)
            return;

        if (!TryComp<PhysicsComponent>(bound, out var boundPhysics) ||
            !TryComp<PhysicsComponent>(spawnedAnchor, out var anchorPhysics))
            return;
        if (!_timer.ApplyingState)
        {
            var bindingJoint = _joints.CreateDistanceJoint(bound, spawnedAnchor, boundPhysics.LocalCenter, anchorPhysics.LocalCenter, id: "bindingJoint");
            bindingJoint.MaxLength = range ?? 4f;
            bindingJoint.MinLength = 0f;
            bindingJoint.Stiffness = 0f;
            boundComp.JointId = bindingJoint.ID;
        }

        Dirty(bound, boundComp);
        Dirty(target, anchorComp);
    }
}

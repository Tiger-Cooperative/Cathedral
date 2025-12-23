using Content.Shared._Cathedral.EntityBound;
using Robust.Server.GameObjects;
using Robust.Server.Physics;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Server._Cathedral.EntityBound;

/// <summary>
/// This system allows the game to 'bind' certain entities to others.
/// The entity that is attached TO cannot be pulled on, but can pull on the 'bound' entity.
/// </summary>
public sealed class EntityBoundSystem : EntitySystem
{
    [Dependency] private readonly TransformSystem _xForm = default!;
    [Dependency] private readonly JointSystem _joints = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly IGameTiming _timer = default!;
    [Dependency] private readonly INetManager _net = default!;

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
        if (TerminatingOrDeleted(bound))
            return;

        EnsureComp<EntityBoundComponent>(bound, out var boundComp);
        EnsureComp<EntityAnchorComponent>(target, out var anchorComp);

        // Indicates that this is already bound to something.
        if (boundComp.JointId != null)
            return;

        boundComp.BoundTo = target;

        if (anchorComp.FollowingEnt == null)
        {
            var spawnedAnchor = Spawn("BoundAnchorEntity", Transform(target).Coordinates);
            anchorComp.FollowingEnt = spawnedAnchor;
            _physics.WakeBody(spawnedAnchor);
        }

        if (!TryComp<PhysicsComponent>(bound, out var boundPhysics) ||
            !TryComp<PhysicsComponent>(anchorComp.FollowingEnt, out var anchorPhysics))
            return;

        boundComp.JointId = $"bind-joint-{GetNetEntity(bound)}";
        EnsureComp<JointComponent>(anchorComp.FollowingEnt.Value, out var joints);


        var bindingJoint = _joints.CreateDistanceJoint(anchorComp.FollowingEnt.Value,
                bound,
                anchorPhysics.LocalCenter,
                boundPhysics.LocalCenter,
                boundComp.JointId);
        bindingJoint.MaxLength = range ?? 4f;
        bindingJoint.MinLength = 0f;
        bindingJoint.Stiffness = 0f;
    }
}

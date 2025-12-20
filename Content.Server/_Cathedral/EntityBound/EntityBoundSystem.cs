using Content.Shared._Cathedral.EntityBound;
using Robust.Server.Physics;
using Robust.Shared.Physics;

namespace Content.Server._Cathedral.EntityBound;

public sealed class EntityBoundSystem : SharedEntityBoundSystem
{
    [Dependency] private readonly JointSystem _joints = default!;

    // public override void Update(float frameTime)
    // {
    //     base.Update(frameTime);
    //     UpdateAnchorMovement(frameTime);
    // }
    //
    // private void UpdateAnchorMovement(float frameTime)
    // {
    //     var anchorQuery = GetEntityQuery<EntityAnchorComponent>();
    //     var query = EntityQueryEnumerator<EntityAnchorComponent>();
    //     while (query.MoveNext(out var uid, out var comp))
    //     {
    //         if (!anchorQuery.TryGetComponent(uid, out var anchor) || anchor.FollowingEnt == null)
    //         {
    //             continue;
    //         }
    //
    //         if (!TryComp<PhysicsComponent>(uid, out var physics))
    //             return;
    //         _xForm.SetCoordinates(anchor.FollowingEnt.Value, Transform(uid).Coordinates);
    //         _physics.SetLinearVelocity(anchor.FollowingEnt.Value, physics.LinearVelocity);
    //     }
    // }
    public override void SetJointStatus(EntityUid ent, bool status, EntityBoundComponent? bound = null)
    {
        if ((bound == null && !TryComp(ent, out bound))
            || !TryComp<JointComponent>(ent, out var joint)
            || bound.JointId == null
            || !joint.GetJoints.TryGetValue(bound.JointId, out var bindJoint))
            return;
        _joints.SetEnabled(bindJoint, status);
        Dirty(ent, bound);
    }
}

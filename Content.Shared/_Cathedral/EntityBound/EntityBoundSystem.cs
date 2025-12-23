using Content.Shared.Movement.Pulling.Systems;
using Robust.Shared.Audio.Systems;
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
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly PullingSystem _pull = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EntityBoundComponent, JointRemovedEvent>(OnJointRemoved);
        SubscribeLocalEvent<EntityAnchorComponent, ComponentShutdown>(OnAnchorShutdown);
    }

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

            if (!TryComp<PhysicsComponent>(uid, out var physics) || !HasComp<PhysicsComponent>(comp.FollowingEnt))
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

    public bool BindToEntity(EntityUid bound, EntityUid target, float? range = null)
    {
        if (TerminatingOrDeleted(bound))
            return false;

        EnsureComp<EntityBoundComponent>(bound, out var boundComp);
        EnsureComp<EntityAnchorComponent>(target, out var anchorComp);

        if (boundComp.JointId != null)
            return boundComp.BoundTo == target;

        if (anchorComp.FollowingEnt == null)
        {
            var spawnedAnchor = PredictedSpawnAtPosition("BoundAnchorEntity", Transform(target).Coordinates);
            anchorComp.FollowingEnt = spawnedAnchor;
        }

        if (anchorComp.FollowingEnt == null)
            return false;

        if (!TryComp<PhysicsComponent>(bound, out var boundPhysics) ||
            !TryComp<PhysicsComponent>(anchorComp.FollowingEnt, out var anchorPhysics))
            return false;

        boundComp.JointId = $"bind-joint-{GetNetEntity(bound)}";

        boundComp.BoundTo = target;
        anchorComp.BoundEntities.Add(bound);

        if (!_timer.ApplyingState)
        {
            var bindingJoint = _joints.CreateDistanceJoint(anchorComp.FollowingEnt.Value, bound,anchorPhysics.LocalCenter, boundPhysics.LocalCenter,  boundComp.JointId);
            bindingJoint.MaxLength = range ?? 10f;
            bindingJoint.MinLength = 0f;
            bindingJoint.Stiffness = 0f;
        }

        Dirty(bound, boundComp);
        Dirty(target, anchorComp);
        return true;
    }

    private void OnJointRemoved(Entity<EntityBoundComponent> ent, ref JointRemovedEvent args)
    {
        if (ent.Comp.BoundTo == null
            || TerminatingOrDeleted(ent.Comp.BoundTo)
            ||!TryComp<EntityAnchorComponent>(ent.Comp.BoundTo, out var anchor)
            || !anchor.BoundEntities.Contains(ent))
        {
            ent.Comp.BoundTo = null;
            RemComp<EntityBoundComponent>(ent);
            return;
        }
        anchor.BoundEntities.Remove(ent);
        if (anchor.BoundEntities.Count == 0)
        {
            PredictedQueueDel(anchor.FollowingEnt);
            RemComp<EntityAnchorComponent>(ent.Comp.BoundTo.Value);
        }
    }

    private void OnAnchorShutdown(Entity<EntityAnchorComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.FollowingEnt == null)
            return;
        foreach (var bound in ent.Comp.BoundEntities)
        {
            if (!TryComp<EntityBoundComponent>(bound, out var boundComp) ||
                !TryComp<JointComponent>(bound, out var jointComp)
                || boundComp.JointId == null
                || !jointComp.GetJoints.TryGetValue(boundComp.JointId, out var bindJoint))
                continue;
            _joints.RemoveJoint(bindJoint);
            RemComp<EntityBoundComponent>(bound);
        }
        PredictedQueueDel(ent.Comp.FollowingEnt);
    }
}

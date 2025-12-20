using Content.Shared._Cathedral.EntityBound;
using Robust.Client.Audio;
using Robust.Client.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Physics;

namespace Content.Client._Cathedral.EntityBound;

public sealed class EntityBoundSystem : SharedEntityBoundSystem
{
    // [Dependency] private readonly TransformSystem _xForm = default!;
    // [Dependency] private readonly AudioSystem _audio = default!;
    //
    // public override void Initialize()
    // {
    //     base.Initialize();
    //     SubscribeLocalEvent<EntityBoundComponent, MoveEvent>(OnMove);
    // }
    //
    // private void OnMove(Entity<EntityBoundComponent> ent, ref MoveEvent args)
    // {
    //     _audio.PlayPvs(new SoundPathSpecifier("/Audio/Items/bikehorn.ogg"), ent);
    //     var target = ent.Comp.BoundTarget;
    //     if (target != null)
    //         if ((_xForm.GetWorldPosition(target.Value) - _xForm.GetWorldPosition(ent.Owner)).Length() < ent.Comp.Range)
    //         {
    //             _xForm.SetCoordinates(ent.Owner, args.OldPosition);
    //
    //         }
    //
    // }
    public override void SetJointStatus(EntityUid ent, bool status, EntityBoundComponent? bound = null)
    {
        
    }
}

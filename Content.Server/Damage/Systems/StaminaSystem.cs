using Content.Server.Effects;
using Content.Shared.Damage.Systems;
using Robust.Shared.Player;

namespace Content.Server.Damage.Systems;

public sealed partial class StaminaSystem : SharedStaminaSystem
{

    [Dependency] private readonly ColorFlashEffectSystem _colorFlash = default!;
    protected override void DoColorFlash(EntityUid ent, EntityUid? source)
    {
        _colorFlash.RaiseEffect(Color.Aqua, new List<EntityUid> { ent }, Filter.Pvs(ent, entityManager: EntityManager).RemoveWhereAttachedEntity(o => o == source));
    }
}

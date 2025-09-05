using Content.Client.Effects;
using Content.Shared.Damage.Systems;
using Robust.Shared.Player;

namespace Content.Client.Damage.Systems;

public sealed partial class StaminaSystem : SharedStaminaSystem
{
    [Dependency] private readonly ColorFlashEffectSystem _colorFlash = default!;

    protected override void DoColorFlash(EntityUid ent, EntityUid? source)
    {
        _colorFlash.RaiseEffect(Color.Aqua, new List<EntityUid> { ent }, Filter.Local());
    }
}

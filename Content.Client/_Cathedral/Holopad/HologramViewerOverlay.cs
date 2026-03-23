using Content.Shared.Holopad;
using Content.Shared.Movement.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;

namespace Content.Client._Cathedral.Holopad;
/// <summary>
/// Secondary overlay that allows an entity to see where a projected hologram would be if they can't see it.
/// Currently complementary to <see cref="HolopadOverlay"/>.
/// </summary>
public sealed class HologramViewerOverlay : Overlay
{
    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    private IEntityManager _entManager;
    private IPlayerManager _playerManager;

    public HologramViewerOverlay(IEntityManager entManager, IPlayerManager playerManager)
    {
        _entManager = entManager;
        _playerManager = playerManager;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var relayMoverQuery = _entManager.EntityQueryEnumerator<RelayInputMoverComponent>();
        var xFormQuery = _entManager.GetEntityQuery<TransformComponent>();
        var xFormSystem = _entManager.System<TransformSystem>();
        var holoQuery = _entManager.GetEntityQuery<HolopadHologramComponent>();
        var spriteQuery = _entManager.GetEntityQuery<SpriteComponent>();

        while (relayMoverQuery.MoveNext(out var relayEnt, out var relayComp))
        {
            var linked = relayComp.RelayEntity;

            if (!xFormQuery.TryGetComponent(linked, out var xForm) || !holoQuery.HasComponent(linked)
                || !spriteQuery.TryGetComponent(linked, out var sprite) || _playerManager.LocalEntity != relayEnt)
                continue;

            if (!sprite.Visible)
                args.WorldHandle.DrawCircle(xFormSystem.ToWorldPosition(xForm.Coordinates), 0.1f, Color.Aqua.WithAlpha(0.3f));
        }
    }
}

using Content.Shared.Holopad;
using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Client._Cathedral.Holopad;

/// <summary>
/// This system allows the game to 'bind' certain entities to others.
/// The entity that is attached TO cannot be pulled on, but can pull on the 'bound' entity.
/// </summary>
public sealed class HolopadOverlay : Overlay
{
    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowFOV;

    private IEntityManager _entManager;

    public HolopadOverlay(IEntityManager entManager)
    {
        _entManager = entManager;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var holoQuery = _entManager.EntityQueryEnumerator<HolopadHologramComponent>();
        var xFormQuery = _entManager.GetEntityQuery<TransformComponent>();
        var xFormSystem = _entManager.System<SharedTransformSystem>();

        while (holoQuery.MoveNext(out var holoEnt, out var holoComp))
        {
            var source = holoComp.LinkedSource;

            if (!xFormQuery.TryGetComponent(holoEnt, out var holoXForm)
                || !xFormQuery.TryGetComponent(source, out var sourceXForm))
                continue;

            if (holoXForm.MapID != sourceXForm.MapID)
                continue;

            var holoWorldPos = xFormSystem.GetWorldPosition(holoXForm, xFormQuery);
            var sourceWorldPos = xFormSystem.GetWorldPosition(sourceXForm, xFormQuery);
            var diff = holoWorldPos - sourceWorldPos;
            var angle = diff.ToWorldAngle();
            var length = diff.Length() / 2f;
            var midPoint = sourceWorldPos + diff / 2;
            const float Width = 0.05f;

            var box = new Box2(-Width, -length, Width, length);
            var rotated = new Box2Rotated(box.Translated(midPoint), angle, midPoint);

            var color = holoComp.Color1;

            args.WorldHandle.DrawRect(rotated, color.WithAlpha(holoComp.Alpha));
        }
    }
}

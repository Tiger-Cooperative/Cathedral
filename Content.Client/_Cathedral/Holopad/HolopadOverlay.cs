using Content.Shared.Holopad;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Client._Cathedral.Holopad;
/// <summary>
/// This whole thing is basically a stand-in for a proper shader that would make only parts of the hologram that are in line of sight of the holopad are visible.
/// Light can't pass through walls, after all. That being said, I have no fucking clue how shaders work and don't feel like it's worth working on visuals right now.
/// Contact Tirochora on Github if you are either willing and able to help or give advice.
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
        var occludeSystem = _entManager.System<OccluderSystem>();
        var spriteQuery = _entManager.GetEntityQuery<SpriteComponent>();
        var spriteSystem = _entManager.System<SpriteSystem>();

        while (holoQuery.MoveNext(out var holoEnt, out var holoComp))
        {
            var source = holoComp.LinkedSource;

            if (!xFormQuery.TryGetComponent(holoEnt, out var holoXForm)
                || !xFormQuery.TryGetComponent(source, out var sourceXForm)
                || !spriteQuery.TryGetComponent(holoEnt, out var holoSprite))
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

            if (holoSprite.Visible)
                args.WorldHandle.DrawRect(rotated, color.WithAlpha(0.5f));

            var inSight = occludeSystem.InRangeUnoccluded(xFormSystem.ToMapCoordinates(holoXForm.Coordinates),
                xFormSystem.ToMapCoordinates(sourceXForm.Coordinates),
                999f,
                true);

            if (holoSprite.Visible != inSight)
                spriteSystem.SetVisible((holoEnt, holoSprite), inSight);
        }
    }
}

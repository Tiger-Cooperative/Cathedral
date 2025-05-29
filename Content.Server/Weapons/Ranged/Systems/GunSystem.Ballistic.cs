using Content.Shared.Audio;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    protected override void Cycle(EntityUid uid, BallisticAmmoProviderComponent component, MapCoordinates coordinates, EntityUid? user = null)
    {
        EntityUid? ent = null;

        // TODO: Combine with TakeAmmo
        if (component.Entities.Count > 0)
        {
            var existing = component.Entities[^1];
            component.Entities.RemoveAt(component.Entities.Count - 1);
            DirtyField(uid, component, nameof(BallisticAmmoProviderComponent.Entities));

            Containers.Remove(existing, component.Container);
            // Pretty large check, but makes sure sounds play when they are supposed to and not when they aren't.
            if ((user == null || !_hands.TryGetEmptyHand(user.Value, out var hand) || !_hands.TryPickupAnyHand(user.Value, existing)) && TryComp<CartridgeAmmoComponent>(existing, out var cartridge))
                Audio.PlayPvs(cartridge.EjectSound, existing, AudioParams.Default.WithVariation(SharedContentAudioSystem.DefaultVariation).WithVolume(-1f));
            EnsureShootable(existing);
        }
        else if (component.UnspawnedCount > 0)
        {
            component.UnspawnedCount--;
            DirtyField(uid, component, nameof(BallisticAmmoProviderComponent.UnspawnedCount));
            ent = Spawn(component.Proto, coordinates);
            EnsureShootable(ent.Value);
        }

        if (ent != null)
        {
            if ((user == null || !_hands.TryGetEmptyHand(user.Value, out var hand) || !_hands.TryPickupAnyHand(user.Value, ent.Value)) && TryComp<CartridgeAmmoComponent>(ent.Value, out var cartridge))
                EjectCartridge(ent.Value);
        }

        var cycledEvent = new GunCycledEvent();
        RaiseLocalEvent(uid, ref cycledEvent);
    }
}

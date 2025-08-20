using Content.Shared.Actions;
using Content.Shared.Mindshield.Components;

namespace Content.Shared.Mindshield.FakeMindShield;

public sealed class FakeMindShieldSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FakeMindShieldComponent, FakeMindShieldToggleEvent>(OnToggleMindshield);
    }

    private void OnToggleMindshield(EntityUid uid, FakeMindShieldComponent comp, FakeMindShieldToggleEvent args)
    {
        comp.IsEnabled = !comp.IsEnabled;
        args.Toggle = true;
        args.Handled = true;
        Dirty(uid, comp);
    }
}

public sealed partial class FakeMindShieldToggleEvent : InstantActionEvent;

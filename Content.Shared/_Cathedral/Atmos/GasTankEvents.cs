using Content.Shared.Atmos;
using Content.Shared.Body.Components;

namespace Content.Shared._Cathedral.Atmos;

/// <summary>
/// Raised on the gas tank when something with <see cref="InternalsComponent"/> breathes from it.
/// </summary>
public sealed class GasTankInhaledFromEvent : EntityEventArgs
{
    public EntityUid Inhaler;

    public bool TankLowPressure;
}

/// <summary>
/// Raised on the gas tank when it releases gas without being inhaled from.
/// </summary>
public sealed class GasTankGasReleasedEvent : EntityEventArgs
{

}

using Robust.Shared.GameStates;

namespace Content.Shared._Cathedral.ChemVaporizer;

/// <summary>
///     This component allows a gas tank to dissolve chemicals in its slot into the gas supply.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ChemVaporizerComponent : Component
{
    [DataField]
    public string SolutionSlotName = "beakerSlot";

    [DataField]
    // This is the rate at which chems get inhaled. Each breath in will put this many units into the inhaler's body.
    public int VaporizeRate = 1;

    [DataField]
    public int[] TransferValues = { 1, 5, 10, 15 };
}

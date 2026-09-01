using Content.Server.Body.Systems;
using Content.Server.Fluids.EntitySystems;
using Content.Shared._Cathedral.Atmos;
using Content.Shared._Cathedral.ChemVaporizer;
using Content.Shared.Body.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server._Cathedral.ChemVaporizer;

public sealed class ChemVaporizerSystem : SharedChemVaporizerSystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstream = default!;
    [Dependency] private readonly SmokeSystem _smoke = default!;
    [Dependency] private readonly TransformSystem _xform = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChemVaporizerComponent, GasTankInhaledFromEvent>(OnGasTankInhaledFrom);
        SubscribeLocalEvent<ChemVaporizerComponent, GasTankGasReleasedEvent>(OnGasTankGasReleased);
    }

    private void OnGasTankInhaledFrom(Entity<ChemVaporizerComponent> ent, ref GasTankInhaledFromEvent args)
    {
        if (!TryComp<ItemSlotsComponent>(ent.Owner, out var itemSlots) || args.TankLowPressure)
            return;
        var container = _itemSlots.GetItemOrNull(ent.Owner, ent.Comp.SolutionSlotName, itemSlots);
        if (container != null && TryComp<SolutionContainerManagerComponent>(container, out var solutionContainerManager)
                              && TryComp<BloodstreamComponent>(args.Inhaler, out var bloodstream)
                              && TryComp<FitsInDispenserComponent>(container, out var dispenserComp)
                              && _solution.TryGetFitsInDispenser((container.Value, dispenserComp, solutionContainerManager), out var solution, out _))
        {
            var splitSoln = _solution.SplitSolution(solution.Value, ent.Comp.VaporizeRate);
            _bloodstream.TryAddToChemicals((args.Inhaler, bloodstream), splitSoln);
        }
    }

    private void OnGasTankGasReleased(Entity<ChemVaporizerComponent> ent, ref GasTankGasReleasedEvent args)
    {
        // TODO: decide a ratio for this to split the chemical
        if (!TryComp<ItemSlotsComponent>(ent.Owner, out var itemSlots))
            return;
        var chemHolder = _itemSlots.GetItemOrNull(ent.Owner, ent.Comp.SolutionSlotName, itemSlots);
        if (chemHolder == null || !TryComp<FitsInDispenserComponent>(chemHolder, out var dispenser) ||
            !TryComp<SolutionContainerManagerComponent>(chemHolder, out var solutionContainer) ||
            !_solution.TryGetFitsInDispenser((chemHolder.Value, dispenser, solutionContainer), out var solution, out _))
            return;
        var splitSoln = _solution.SplitSolution(solution.Value, ent.Comp.VaporizeRate);
        if (splitSoln.Volume != 0)
        {
            var foam = Spawn(new EntProtoId("TearGasSmokeWhite"), _xform.GetMapCoordinates(ent.Owner));
            _smoke.StartSmoke(foam, splitSoln, 3f, 1);
        }
    }
}

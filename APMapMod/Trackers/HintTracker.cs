using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Packets;
using ItemChanger;

namespace APMapMod.Trackers;

public class HintTracker
{
    private static ArchipelagoSession _session;
    
    public static void Hook()
    {
        _session = Archipelago.HollowKnight.Archipelago.Instance.session;
        _session.Socket.PacketReceived += OnHint;
    }

    private static void OnHint(ArchipelagoPacketBase packet)
    {
        // ignore all non hint json packets
        if (packet is not HintPrintJsonPacket)
            return;
        
        var hint =  (HintPrintJsonPacket)packet;
        APMapMod.Instance.LogDebug($"hint packet found. item: {hint.Item.Item} location: {hint.Item.Location} player: {hint.Item.Player}");

        // ignore items in other players worlds.
        if (hint.Item.Player != _session.ConnectionInfo.Slot)
            return;
        
        var location = _session.Locations.GetLocationNameFromId(hint.Item.Location);

        if (ItemChanger.Internal.Ref.Settings.Placements.ContainsKey(location))
        {
            var placement = ItemChanger.Internal.Ref.Settings.Placements[location];
            placement.AddVisitFlag(VisitState.Previewed);
        }
    }
}
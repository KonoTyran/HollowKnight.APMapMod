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
        
        if (location.StartsWith(LocationNames.Leg_Eater))
        {
            location = LocationNames.Leg_Eater;
        }
        else if (location.StartsWith(LocationNames.Seer))
        {
            location = LocationNames.Seer;
        }
        else if (location.StartsWith(LocationNames.Iselda))
        {
            location = LocationNames.Iselda;
        }
        else if (location.StartsWith(LocationNames.Grubfather))
        {
            location = LocationNames.Grubfather;
        }
        else if (location.StartsWith(LocationNames.Sly_Key))
        {
            location = LocationNames.Sly_Key;
        }
        else if (location.StartsWith(LocationNames.Sly))
        {
            location = LocationNames.Sly;
        }
        else if (location.StartsWith(LocationNames.Salubra))
        {
            location = LocationNames.Salubra;
        }
        else if (location.StartsWith(LocationNames.Egg_Shop))
        {
            location = LocationNames.Egg_Shop;
        }

        var loc = Finder.GetLocation(location);
        if (loc != null)
        {
            var placement = ItemChanger.Internal.Ref.Settings.Placements[location];
            placement.GetTag<Archipelago.HollowKnight.IC.ArchipelagoPlacementTag>().Hinted = true;
            placement.AddVisitFlag(VisitState.Previewed);
        }
    }
}
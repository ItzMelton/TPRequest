using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TPRequest
{
    [ApiVersion(2, 1)]
    public class TPRequest : TerrariaPlugin
    {
        public override string Name => "TPRequest";
        public override Version Version => new Version(1, 0, 0);
        public override string Author => "Melton";
        public override string Description => "Request, accept/reject req, and teleport to a player";

        public TPRequest(Main game) : base(game)
        { }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tpa.request", RequestCommand, "tpa", "tprequest"));
            Commands.ChatCommands.Add(new Command("tpa.accept", AcceptCommand, "tpaccept", "tpaa"));
            Commands.ChatCommands.Add(new Command("tpa.reject", RejectCommand, "tpreject", "tpar"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commands.ChatCommands.Remove(new Command("tpa.request", RequestCommand, "tpa", "tprequest"));
                Commands.ChatCommands.Remove(new Command("tpa.accept", AcceptCommand, "tpaccept", "tpaa"));
                Commands.ChatCommands.Remove(new Command("tpa.reject", RejectCommand, "tpreject", "tpar"));
            }
            base.Dispose(disposing);
        }
   
        private static Dictionary<TSPlayer, TSPlayer> teleportRequests = new Dictionary<TSPlayer, TSPlayer>();
        
        private async void RequestCommand(CommandArgs args)
        {
            var player = args.Player;
            if (player is null || !player.Active || !player.RealPlayer) return;
            if (args.Parameters.Count == 0)
            {
                player.SendErrorMessage("Invalid syntax! Exp usage: /tpa {playerName}");
                return;
            }
            var targetPlayer = TSPlayer.FindByNameOrID(args.Parameters[0]);
            if (targetPlayer.Count != 1)
            {
                player.SendErrorMessage(targetPlayer.Count == 0 ? "Invalid player!" : $"More than one ({targetPlayer.Count}) players matched, Try again with a more detailed name.");
                return;
            }
            teleportRequests[player] = targetPlayer[0];

            if (targetPlayer != null && targetPlayer[0] == player)
            {
                player.SendErrorMessage("You can't request yourself!");
                return;
            }
            
            player.SendSuccessMessage($"You're now requesting to teleport to {targetPlayer[0].Name}.");
            targetPlayer[0].SendInfoMessage($"{player.Name} is requesting you to teleport to your current location. Enter /tpaccept to accept and enter /tpreject to reject the request.");

            CancellationTokenSource cts = new CancellationTokenSource();
            Task timeoutTask = Task.Delay(15000, cts.Token); 
            await timeoutTask;
            if (teleportRequests.ContainsKey(player))
            {          
                player.SendErrorMessage($"{targetPlayer[0].Name} ignored your request.");
                targetPlayer[0].SendInfoMessage($"You did not accept {player.Name} teleport request on time.");
                 // Request timed out
        
                teleportRequests.Remove(player);
                /// Remove the teleport request
            }
        }

        private void AcceptCommand(CommandArgs args)
        {
            var player = args.Player;
            if (player is null || !player.Active || !player.RealPlayer) return;

            TSPlayer requestingPlayer = null;
            /// Ensure the player is the target of a teleport request

            foreach (var request in teleportRequests)
            {
                if (request.Value == player)
                {
                    requestingPlayer = request.Key;
                    break;
                }
            }

            if (requestingPlayer == null)
            {
                player.SendErrorMessage("You have no pending teleport requests yet.");
                return;
            }
            
            requestingPlayer.SendSuccessMessage($"{player.Name} has accepted your teleport request. You have been teleported to their location.");
            player.SendSuccessMessage($"You have accepted {requestingPlayer.Name} teleport request.");
            requestingPlayer.Teleport(player.X, player.Y);
            
            teleportRequests.Remove(requestingPlayer);
        }

        private void RejectCommand(CommandArgs args)
        {
            var player = args.Player;
            if (player is null || !player.Active || !player.RealPlayer) return;

            TSPlayer requestingPlayer = null;
            /// Ensure the player is the target of a teleport request

            foreach (var request in teleportRequests)
            {
                if (request.Value == player)
                {
                    requestingPlayer = request.Key;
                    break;
                }
            }

            if (requestingPlayer == null)
            {
                player.SendErrorMessage("You have no pending teleport requests yet.");
                return;
            }

            requestingPlayer.SendErrorMessage($"{player.Name} has rejected your teleport request.");
            player.SendInfoMessage($"You have rejected {requestingPlayer.Name} teleport request.");
            teleportRequests.Remove(requestingPlayer);
        }
    }
}

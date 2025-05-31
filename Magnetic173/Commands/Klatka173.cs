using System;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using RemoteAdmin;
using UnityEngine;

namespace Magnetic173.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Klatka173 : ICommand
    {
        public string Command => MagneticCage173.Instance.Config.CageCommand;
        public string[] Aliases => null;
        public string Description => MagneticCage173.Instance.Translation.Description;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = MagneticCage173.Instance.Translation.ConsoleUse;
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            if (player == null)
            {
                response = MagneticCage173.Instance.Translation.CantDetectPlayer;
                return false;
            }

            if (!MagneticCage173.Instance.Config.IsEnabled)
            {
                response = MagneticCage173.Instance.Translation.FeatureDisabled;
                return false;
            }

            if (!MagneticCage173.Instance.Config.AllowedRoles.Contains(player.Role.Type))
            {
                response = MagneticCage173.Instance.Translation.NotAllowedRole;
                return false;
            }

            if (MagneticCage173.Instance.ActiveCountdowns.ContainsKey(player) || Magnetic173.MagneticCage173.Instance.IsPlayerCurrentlyCaging(player)) 
            {
                response = MagneticCage173.Instance.Translation.AlreadyCaging;
                return false;
            }

            if (Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, 2f))
            {
                Player target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>()); 

                if (target != null && target.Role.Type == RoleTypeId.Scp173)
                {
                    if (Magnetic173.MagneticCage173.Instance.ActiveCages.ContainsKey(target))
                    {
                        response = MagneticCage173.Instance.Translation.Scp173AlreadCaged;
                        return false;
                    }

                    Magnetic173.MagneticCage173.Instance.InitiateCagingProcess(player, target); 
                    response = MagneticCage173.Instance.Translation.Initiating;
                    return true;
                }
                else
                {
                    response = MagneticCage173.Instance.Translation.TooFar;
                    return false;
                }
            }
            else 
            {
                response = MagneticCage173.Instance.Translation.TooFar;
                return false;
            }
        }
    }
}
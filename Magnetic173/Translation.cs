using System.ComponentModel;
using Exiled.API.Interfaces;
using UnityEngine;

namespace Magnetic173
{
    public class Translation : ITranslation
    {
        [Description("Command Description")]
        public string Description { get; set; } = "Initiating the magnetic cage creation procedure on SCP-173.";

        [Description("Response when command is used by console")]
        public string ConsoleUse { get; set; } = "This command can only be used by a player.";

        [Description("No Player detection message")]
        public string CantDetectPlayer { get; set; } = "Player could not be identified..";

        [Description("Response when the feature is disabled")]
        public string FeatureDisabled { get; set; } = "The magnetic cage function is currently disabled.";

        [Description("Response when the player is not allowed to use the command")]
        public string NotAllowedRole { get; set; } = "Your role does not permit the use of this command.";

        [Description("Response when the player is already caging or has an active cage")]
        public string AlreadyCaging { get; set; } = "You are already in the process of caging or have an active cage.";

        [Description("Response when the SCP-173 is already caged")]
        public string Scp173AlreadCaged { get; set; } = "This SCP-173 is already in a cage.";

        [Description("Response when initiating the cage procedure")]
        public string Initiating { get; set; } = "Initiating magnetic cage creation procedure...";

        [Description("Response when too far from SCP-173 or not looking directly at it")]
        public string TooFar { get; set; } = "You must look directly at SCP-173 from a close distance.";

        [Description("Hint when died during cage creation")]
        public string Death { get; set; } = "<color=grey><b>Cage creation procedure canceled – you died.</color></b>.";

        [Description("Hint when changing role during cage creation")]
        public string RoleChange { get; set; } =
            "<color=grey><b>Cage creation procedure canceled - change role.</color></b>";

        [Description("Hint when SCP-173 tries to attack while in a cage")]
        public string CantAttackInCage { get; set; } = "<color=red><b>You can’t attack while in a magnetic cage!</color></b>";

        [Description("Hint for SCp-173 when his cage has been destroyed")]
        public string CageDestroyed173 { get; set; } = "<color=grey><b>Your cage has been destroyed!</color></b>";

        [Description("Hint for caging player when cage is destroyed")]
        public string CageDestroyedCaging { get; set; } = "<color=grey><b>Your cage has been destroyed!</color></b>";

        [Description("Hint for caging player when cage is created")]
        public string CageCreated { get; set; } = "<color=green><b>Magnetic cage created successfully!</color></b>";

        [Description("Hint for player when SCP-173 is already in the cage")]
        public string StartCageProcess { get; set; } =
            "<color=grey><b>This SCP-173 is already in the cage.</color></b>";

        [Description("Hint for player when somebody is already creating cage or cage is active")]
        public string CageIsInProgress { get; set; } =
            "<color=grey><b>Somebody is already creating cage or cage is already active.</color></b>";

        [Description("Hint for player when cage is alredy being created")]
        public string CageIsInProgress173 { get; set; } =
            "<color=grey><b>You have already initiated the cage creation procedure.</color></b>";

        [Description("Hint for player when cage procedure is cancelled")]
        public string CageCancelled { get; set; } = "<color=grey><b>Cage creation procedure canceled.</color></b>";

        [Description("Hint for player when moved too far away during cage creation")]
        public string MovedTooAway { get; set; } =
            "<color=grey><b>Cage creation procedure canceled</color></b>\\n<color=white><b>You moved too far away...</color></b>";

        [Description("Hint for player when player looked away during cage creation")]
        public string LookedAway { get; set; } =
            "<color=grey><b>Cage creation procedure canceled</color></b>\\n<color=white><b>You looked away...</color></b>";

        [Description("Hint for player when cage started creating")]
        public string CageInProgress { get; set; } =
            "<b><color=grey>Deploying magnetic cage…</color></b>\\n<color=white><b>Remaining:</b></color> <color=grey>{time}</color>\\n<color=grey><b>Don’t move and don’t look around!</color></b>";
        
        [Description("Hint for player when cage failed to create")]
        public string FailedToCreateCage { get; set; } = "<color=grey><b>Failed to create the cage.</color></b>";

        [Description("Hint for player detected schematic error")]
        public string SchematicError { get; set; } = "<color=red><b>Error:</color></b> <color=grey><b>Failed to create the cage (schematic error).</color></b>";
        
        [Description("Hint for player when cage is being created but schematic is not found")]
        public string SchematicNotLoaded { get; set; } = "<color=red><b>Error:</color></b> <color=grey><b>Failed to create the cage (schematic not loaded).</color></b>";
            
        [Description("Hint for player when cage has been activated")]
        public string CageActivePlayer { get; set; } = "<color=white><b>Magnetic cage active!</color></b>";
        
        [Description("Hint for SCP-173 when caught in a magnetic cage")]
        public string CageActiveScp173 { get; set; } = "<color=grey><b>You have been caught in a magnetic cage!</color></b>";
        
        [Description("Hint for SCP-173 when released from the cage")]
        public string Released { get; set; } = "<color=grey><b>You have been released from the cage.</color></b>";
        
        
            



    }
}
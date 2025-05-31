using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using MEC;
using ProjectMER.Features.Objects;

namespace Magnetic173
{
    public class MagneticCage173 : Plugin<Config, Translation>
    {
        public override string Name => "MagneticCage173";
        public override string Author => "Feniks Studio - Tymek";
        public override Version Version => new Version(1, 0, 2); 
        public override Version RequiredExiledVersion => new Version(9, 6, 0);

        public static MagneticCage173 Instance { get; private set; }

        private EventHandlers _handlers;

        public Dictionary<Player, CageInfo> ActiveCages { get; } = new Dictionary<Player, CageInfo>();
        public Dictionary<Player, CoroutineHandle> ActiveCountdowns { get; } = new Dictionary<Player, CoroutineHandle>();

        public class CageInfo
        {
            public Player CagingPlayer { get; }
            public Player CagedScp173 { get; }
            public SchematicObject CageSchematic { get; set; }
            public CoroutineHandle UpdateCoroutine { get; }
            public float CurrentHealth { get; set; } 

            public CageInfo(Player cagingPlayer, Player cagedScp173, CoroutineHandle updateCoroutine)
            {
                CagingPlayer = cagingPlayer;
                CagedScp173 = cagedScp173;
                CageSchematic = null; 
                UpdateCoroutine = updateCoroutine;
                CurrentHealth = 0; 
            }
        }

        public override void OnEnabled()
        {
            Instance = this;
            _handlers = new EventHandlers();

            Exiled.Events.Handlers.Server.RoundStarted += _handlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers += _handlers.OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Verified += _handlers.OnPlayerVerified;
            Exiled.Events.Handlers.Player.Destroying += _handlers.OnPlayerDestroying;
            Exiled.Events.Handlers.Player.Died += _handlers.OnPlayerDied;
            Exiled.Events.Handlers.Player.Hurting += _handlers.OnPlayerHurting;
            Exiled.Events.Handlers.Player.ChangingRole += _handlers.OnChangingRole;
            Exiled.Events.Handlers.Player.Shooting += _handlers.OnPlayerShooting;
            Exiled.Events.Handlers.Map.ExplodingGrenade += _handlers.OnExplodingGrenade;

            if (!IsMerLoaded())
            {
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= _handlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= _handlers.OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Verified -= _handlers.OnPlayerVerified;
            Exiled.Events.Handlers.Player.Destroying -= _handlers.OnPlayerDestroying;
            Exiled.Events.Handlers.Player.Died -= _handlers.OnPlayerDied;
            Exiled.Events.Handlers.Player.Hurting -= _handlers.OnPlayerHurting;
            Exiled.Events.Handlers.Player.ChangingRole -= _handlers.OnChangingRole;
            Exiled.Events.Handlers.Player.Shooting -= _handlers.OnPlayerShooting;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= _handlers.OnExplodingGrenade;

            _handlers.CleanupAllCages();

            _handlers = null;
            Instance = null;
            base.OnDisabled();
        }

        public void InitiateCagingProcess(Player cagingPlayer, Player targetScp173)
        {
            _handlers?.StartCagingProcess(cagingPlayer, targetScp173);
        }

        public bool IsPlayerCurrentlyCaging(Player player)
        {
            return _handlers?.IsAnyPlayerCaging(player) ?? false;
        }

        private static bool IsMerLoaded()
        {
            try
            {
                var typeCheck = typeof(ProjectMER.Features.MapUtils);
                return true;
            }
            catch (TypeLoadException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
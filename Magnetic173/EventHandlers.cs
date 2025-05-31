using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using UnityEngine;
using Player = Exiled.API.Features.Player;

namespace Magnetic173
{
    public class EventHandlers
    {
        private readonly MagneticCage173 _plugin = MagneticCage173.Instance;

        public void OnRoundStarted() => CleanupAllCages();
        public void OnWaitingForPlayers() => CleanupAllCages();

        public void OnPlayerDestroying(DestroyingEventArgs ev)
        {
            if (_plugin.ActiveCountdowns.TryGetValue(ev.Player, out var countdownCoroutine))
            {
                Timing.KillCoroutines(countdownCoroutine);
                _plugin.ActiveCountdowns.Remove(ev.Player);
            }

            if (TryGetCageByCagingPlayer(ev.Player, out var cageInfo))
            {
                CleanupCage(cageInfo.CagedScp173);
            }
            else if (_plugin.ActiveCages.TryGetValue(ev.Player, out cageInfo)) 
            {
                CleanupCage(ev.Player); 
            }
        }
        
        
        
        //Smierc
        
        
        
        public void OnPlayerDied(DiedEventArgs ev)
        {
            if (_plugin.ActiveCountdowns.TryGetValue(ev.Player, out var countdownCoroutine))
            {
                Timing.KillCoroutines(countdownCoroutine);
                _plugin.ActiveCountdowns.Remove(ev.Player);
                ev.Player.ShowHint(MagneticCage173.Instance.Translation.Death, 5);
            }


            if (TryGetCageByCagingPlayer(ev.Player, out var cageInfo))
            {
                CleanupCage(cageInfo.CagedScp173);
            }
            else if (_plugin.ActiveCages.TryGetValue(ev.Player, out cageInfo)) 
            {
                CleanupCage(ev.Player);
            }
        }
        
        
        
        //Zmiana roli
        
        
        
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (_plugin.ActiveCountdowns.TryGetValue(ev.Player, out var countdownCoroutine) && !_plugin.Config.AllowedRoles.Contains(ev.NewRole))
            {
                Timing.KillCoroutines(countdownCoroutine);
                _plugin.ActiveCountdowns.Remove(ev.Player);
                ev.Player.ShowHint(MagneticCage173.Instance.Translation.RoleChange, 5);
            }

            if (TryGetCageByCagingPlayer(ev.Player, out var cageInfo) && !_plugin.Config.AllowedRoles.Contains(ev.NewRole))
            {
                CleanupCage(cageInfo.CagedScp173);
            }
            else if (_plugin.ActiveCages.TryGetValue(ev.Player, out cageInfo))
            {
                CleanupCage(ev.Player);
            }
        }
        
        
        
        //BLOKADA ATAKU
        
        
        
        public void OnPlayerHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null
                && ev.Attacker.Role.Type == RoleTypeId.Scp173
                && _plugin.ActiveCages.ContainsKey(ev.Attacker))
            {
                ev.IsAllowed = false;
                ev.Attacker.ShowHint(MagneticCage173.Instance.Translation.CantAttackInCage);
            }
        }
        
        
        
        //STRZELANIE
        
        
        
        public void OnPlayerShooting(ShootingEventArgs ev)
        {
            if (ev.Player == null || !(ev.Item is Exiled.API.Features.Items.Firearm exiledFirearm))
                return;

            var baseFirearm = exiledFirearm.Base;
            if (baseFirearm == null)
            {
                return; 
            }


            float rayDistance = 100f; 

            if (Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit hit, rayDistance))
            {
                foreach (var cageInfo in _plugin.ActiveCages.Values.ToList())
                {
                    if (cageInfo.CageSchematic != null && cageInfo.CageSchematic.gameObject != null)
                    {
                        if (hit.collider.transform.IsChildOf(cageInfo.CageSchematic.transform))
                        {
                            float damage = 0f;
                            try
                            {
                                damage = GetFirearmDamage(baseFirearm.ItemTypeId);

                                if (damage <= 0)
                                {
                                    damage = 10f; 
                                }
                            }
                            catch (Exception)
                            {
                                damage = 10f;
                            }

                            cageInfo.CurrentHealth -= damage;

                            if (cageInfo.CurrentHealth <= 0)
                            {
                                cageInfo.CagedScp173?.ShowHint(MagneticCage173.Instance.Translation.CageDestroyed173, 5);
                                cageInfo.CagingPlayer?.ShowHint(MagneticCage173.Instance.Translation.CageDestroyedCaging, 5);
                                CleanupCage(cageInfo.CagedScp173);
                                break;
                            }
                            break;
                        }
                    }
                }
            }
        }
        
        
        
        //Wybuch Granatu
        
        
        
        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {

            if (ev.Projectile.Type != ItemType.GrenadeHE)
            {
                return;
            }


            float destructionRadius = 3f; 

       
            foreach (var cageInfo in _plugin.ActiveCages.Values.ToList())
            {
                if (cageInfo.CageSchematic != null && cageInfo.CageSchematic.gameObject != null)
                {
                    float distanceToCage = Vector3.Distance(ev.Position, cageInfo.CageSchematic.transform.position);

                    if (distanceToCage <= destructionRadius)
                    {
                        cageInfo.CagedScp173?.ShowHint(MagneticCage173.Instance.Translation.CageDestroyed173, 5);
                        cageInfo.CagingPlayer?.ShowHint(MagneticCage173.Instance.Translation.CageDestroyedCaging, 5);
                        
                        CleanupCage(cageInfo.CagedScp173);
                    }
                }
            }
        }
        
        //TAKIE COŚ BEZ TAKIEGO CZEGOŚ Z TAKIM CZYMŚ, NIE INTERESUJ SIĘ.
        // BO CO? ZABRONISZ MI SIĘ INTERESOWAĆ?
        
        
        private float GetFirearmDamage(ItemType firearmType)
        {
            switch (firearmType)
            {
                case ItemType.GunCOM15: return 15f;
                case ItemType.GunCOM18: return 15f;
                case ItemType.GunFSP9: return 25f;
                case ItemType.GunCrossvec: return 25f;
                case ItemType.GunE11SR: return 35f;
                case ItemType.GunAK: return 45f;
                case ItemType.GunLogicer: return 55f;
                case ItemType.GunRevolver: return 15f;
                case ItemType.GunShotgun: return 12f * 8;
                case ItemType.ParticleDisruptor: return 1000f; 
                case ItemType.GunFRMG0: return 125f;
                case ItemType.GunA7: return 32f; 
                default:
                    return 0f;
            }
        }
        
        
        
        //START
        
        
        
        public void StartCagingProcess(Player cagingPlayer, Player targetScp173)
        {
            if (_plugin.ActiveCages.ContainsKey(targetScp173))
            {
                cagingPlayer.ShowHint(MagneticCage173.Instance.Translation.Scp173AlreadCaged, 5);
                return;
            }
            if (IsAnyPlayerCaging(cagingPlayer))
            {
                cagingPlayer.ShowHint(MagneticCage173.Instance.Translation.AlreadyCaging, 5);
                return;
            }
            if (_plugin.ActiveCountdowns.ContainsKey(cagingPlayer))
            {
                cagingPlayer.ShowHint(MagneticCage173.Instance.Translation.CageIsInProgress173, 5);
                return;
            }


            Vector3 startPosition = cagingPlayer.Position;

            CoroutineHandle countdownCoroutine = Timing.RunCoroutine(CountdownCoroutine(cagingPlayer, targetScp173, startPosition));
            _plugin.ActiveCountdowns.Add(cagingPlayer, countdownCoroutine);
        }

        
        
        private IEnumerator<float> CountdownCoroutine(Player cagingPlayer, Player targetScp173, Vector3 startPosition)
        {
            float remainingTime = _plugin.Config.CountdownDuration;
            while (remainingTime > 0)
            {
                if (cagingPlayer == null || !cagingPlayer.IsConnected || targetScp173 == null || !targetScp173.IsConnected || targetScp173.Role.Type != RoleTypeId.Scp173 || !_plugin.Config.AllowedRoles.Contains(cagingPlayer.Role.Type))
                {
                    cagingPlayer?.ShowHint(MagneticCage173.Instance.Translation.CageCancelled);
                    if (cagingPlayer != null) _plugin.ActiveCountdowns.Remove(cagingPlayer);
                    yield break; 
                }

                if (Vector3.Distance(cagingPlayer.Position, startPosition) > 1.0f)
                {
                    cagingPlayer.ShowHint(MagneticCage173.Instance.Translation.MovedTooAway, 5);
                    _plugin.ActiveCountdowns.Remove(cagingPlayer);
                    yield break;
                }


                cagingPlayer.ShowHint(_plugin.Translation.CageInProgress.Replace("{time}",$"{Mathf.CeilToInt(remainingTime)}"), 1.1f); // Dodano info o nieoddalaniu się
                yield return Timing.WaitForSeconds(1f);
                remainingTime -= 1f;
            }

            if (cagingPlayer == null || !cagingPlayer.IsConnected || targetScp173 == null || !targetScp173.IsConnected || targetScp173.Role.Type != RoleTypeId.Scp173 || !_plugin.Config.AllowedRoles.Contains(cagingPlayer.Role.Type))
            {
                cagingPlayer?.ShowHint(MagneticCage173.Instance.Translation.FailedToCreateCage, 5);
                if (cagingPlayer != null) _plugin.ActiveCountdowns.Remove(cagingPlayer);
                yield break;
            }

            _plugin.ActiveCountdowns.Remove(cagingPlayer);

            Vector3 spawnPosition = cagingPlayer.Position + cagingPlayer.Transform.forward * 3f;
            Quaternion spawnRotation = cagingPlayer.Transform.rotation;

            SchematicObject cageInstance = null;

            try
            {
                cageInstance = ObjectSpawner.SpawnSchematic(
                    _plugin.Config.SchematicName,
                    spawnPosition,
                    spawnRotation.eulerAngles
                );
            }
            catch (Exception)
            {
                cagingPlayer.ShowHint(MagneticCage173.Instance.Translation.SchematicError, 10);
                yield break;
            }

            if (cageInstance == null)
            {
                cagingPlayer.ShowHint(MagneticCage173.Instance.Translation.SchematicNotLoaded, 10);
                yield break;
            }

            cagingPlayer.ShowHint(MagneticCage173.Instance.Translation.CageActivePlayer, 5);
            targetScp173.ShowHint(MagneticCage173.Instance.Translation.CageActiveScp173, 5);

            CoroutineHandle updateCoroutine = Timing.RunCoroutine(UpdateCageCoroutine(cagingPlayer, targetScp173, cageInstance));

            MagneticCage173.CageInfo newCageInfo = new MagneticCage173.CageInfo(cagingPlayer, targetScp173, updateCoroutine);
            newCageInfo.CageSchematic = cageInstance;
            newCageInfo.CurrentHealth = 200f;

            _plugin.ActiveCages.Add(targetScp173, newCageInfo); 
        }


        
        private IEnumerator<float> UpdateCageCoroutine(Player cagingPlayer, Player cagedScp173, SchematicObject cageSchematic)
        {
            if (cageSchematic == null)
            { 
                CleanupCage(cagedScp173); yield break; 
            }
            while (true)
            {
                if (cageSchematic?.transform == null)
                {  
                    CleanupCage(cagedScp173); yield break; 
                }
                if (cagingPlayer == null || !cagingPlayer.IsConnected || cagedScp173 == null || !cagedScp173.IsConnected ||
                    cagedScp173.Role.Type != RoleTypeId.Scp173 || !_plugin.Config.AllowedRoles.Contains(cagingPlayer.Role.Type))
                { 
                    CleanupCage(cagedScp173); yield break; 
                }
                try
                {
                    Vector3 targetCagePosition = cagingPlayer.Position + cagingPlayer.Transform.forward * 3f;
                    Quaternion targetCageRotation = cagingPlayer.Transform.rotation;
                    cageSchematic.transform.position = targetCagePosition;
                    cageSchematic.transform.rotation = targetCageRotation;
                    Vector3 targetScpPosition = targetCagePosition + Vector3.up * 1.0f;
                    cagedScp173.Position = targetScpPosition;
                }
                catch (Exception)
                {
                    CleanupCage(cagedScp173); yield break;
                }
                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        
        
        public void CleanupCage(Player cagedScp173)
        {
            if (cagedScp173 == null) return;
            if (_plugin.ActiveCages.TryGetValue(cagedScp173, out MagneticCage173.CageInfo cageInfo))
            {
                Timing.KillCoroutines(cageInfo.UpdateCoroutine);
                if (cageInfo.CageSchematic?.transform != null)
                {
                    try
                    {
                        UnityEngine.Object.Destroy(cageInfo.CageSchematic.transform.gameObject);
                    }
                    catch (Exception)
                    {
                        // Noi co? Znaleźliście coś ciekawego?
                        // Tak, znalazłem
                    }
                }
                _plugin.ActiveCages.Remove(cagedScp173);
                cagedScp173?.ShowHint(MagneticCage173.Instance.Translation.Released, 3);
            }
        }

        public void CleanupAllCages()
        {
            List<Player> countingPlayers = _plugin.ActiveCountdowns.Keys.ToList();
            foreach (var p in countingPlayers) { if (_plugin.ActiveCountdowns.TryGetValue(p, out var c)) Timing.KillCoroutines(c); }
            _plugin.ActiveCountdowns.Clear();
            List<Player> cagedScps = _plugin.ActiveCages.Keys.ToList();
            foreach (var scp in cagedScps) { CleanupCage(scp); }
            _plugin.ActiveCages.Clear();
        }

        internal bool IsAnyPlayerCaging(Player potentialCager) => _plugin.ActiveCages.Values.Any(ci => ci.CagingPlayer == potentialCager);
        private bool TryGetCageByCagingPlayer(Player cagingPlayer, out MagneticCage173.CageInfo cageInfo)
        {
            foreach (var kvp in _plugin.ActiveCages) { if (kvp.Value.CagingPlayer == cagingPlayer) { cageInfo = kvp.Value; return true; } }
            cageInfo = null; return false;
        }

        public void OnPlayerVerified(VerifiedEventArgs ev) { }
    }
}
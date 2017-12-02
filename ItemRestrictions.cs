using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace LeeIzaZombie.ItemRestrictions
{
    public class ItemRestrictions : RocketPlugin<ItemRestrictionsConfiguration>
    {
        public static ItemRestrictions Instance;
        public string version = "3.0";

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "item_notPermitted", "Item not permitted: {0}" },
                    { "translation_version_dont_edit", "2" }
                };
            }
        }

        protected override void Load()
        {
            Instance = this;
            Logger.LogWarning("Setting up Item Restrictions by LeeIzaZombie, improved by Extra. v" + version);
            Logger.LogWarning("--");
            int count = 0; foreach (ushort item in this.Configuration.Instance.Items) { count++; }
            int group_count = 0; foreach (UnrestrictGroup g in this.Configuration.Instance.UnrestrictGroups) { group_count++; }
            Logger.LogWarning("Black listed items found: " + count);
            Logger.LogWarning("Groups found: " + group_count);
            Logger.LogWarning("--");
            Logger.LogWarning("Item Restrictions is ready!");
        }

        private void CheckInventory(UnturnedPlayer player)
        {
            bool willRestrict;
            for(int i = 0; i < this.Configuration.Instance.Items.Count; i++)
            {
                willRestrict = true;
                ushort item = this.Configuration.Instance.Items[i];
                foreach (UnrestrictGroup group in this.Configuration.Instance.UnrestrictGroups)
                {
                    if (player.HasPermission(group.permission) && willRestrict)
                    {
                        foreach (ushort id in group.Items)
                        {
                            if (item == id)
                            {
                                willRestrict = false;
                                break;
                            }
                        }
                    }
                }
                if (willRestrict)
                {
                    try
                    {
                        for (byte page = 0; page < PlayerInventory.PAGES; page++)
                        {
                            byte itemCount = player.Player.inventory.getItemCount(page);
                            for (byte index = 0; index < itemCount; index++)
                            {
                                if (player.Player.inventory.getItem(page, index).item.id == item)
                                {
                                    UnturnedChat.Say(player, Translate("item_notPermitted", UnturnedItems.GetItemAssetById(item).itemName), Color.red);
                                    player.Player.inventory.removeItem(page, index);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        protected override void Unload() {

        }

        public List<UnturnedPlayer> Players()
        {
            List<UnturnedPlayer> list = new List<UnturnedPlayer>();

            for(int i = 0; i < Provider.clients.Count; i++)
            {
                SteamPlayer sp = Provider.clients[i];
                UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(sp);
                list.Add(p);
            }
            return list;
        }

        public void FixedUpdate()
        {
            if (Level.isLoaded && Provider.clients.Count > 0)
                IntegrityCheck();
        }

        DateTime Second = DateTime.Now;
        private void IntegrityCheck()
        {
            if ((DateTime.Now - this.Second).TotalSeconds > Configuration.Instance.CheckInterval)
            {
                if (Provider.clients.Count > 0)
                {
                    for(int i = 0; i < Players().Count; i++)
                    {
                        UnturnedPlayer player = Players()[i];
                        if (!R.Permissions.HasPermission(player, "ir.safe"))
                            CheckInventory(player);
                        else if (player.IsAdmin && !Configuration.Instance.ignoreAdmin && !R.Permissions.HasPermission(player, "ir.safe"))
                            CheckInventory(player);
                    }
                }
                Second = DateTime.Now;
            }
        }
    }
}

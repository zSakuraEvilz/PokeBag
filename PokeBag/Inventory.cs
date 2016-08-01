using PokemonGo.RocketAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POGOProtos.Data;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Settings.Master;
using System;

namespace PokeBag
{
    class Inventory
    {
        private readonly Client _client;
        public Inventory(Client client)
        {
            _client = client;

        }

        public async Task<IEnumerable<PokemonData>> GetPokemons()
        {
            try
            {
                var inventory = await _client.Inventory.GetInventory();
                return inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData).Where(p => p != null && p?.PokemonId > 0);
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        public async Task<int[]> GetEvolveRequiredCandy(List<PokemonSettings> pokemonSettings, Candy[] pokemonFamilies, PokemonId pokemon)
        {
            try
            {
                var settings = pokemonSettings.Single(x => x.PokemonId == pokemon);
                var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);
                return new[] { familyCandy.Candy_, settings.CandyToEvolve };
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        public async Task<int[]> GetUpgradeRequiredCandy(List<PokemonSettings> pokemonSettings, Candy[] pokemonFamilies, PokemonId pokemon)
        {
            try
            {
                var settings = pokemonSettings.Single(x => x.PokemonId == pokemon);
                var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);
                return new[] { familyCandy.Candy_, settings.EvolutionPips };
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public async Task<IEnumerable<Candy>> GetPokemonFamilies()
        {
            try
            {
                var inventory = await _client.Inventory.GetInventory();
                return
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Candy)
                        .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            try
            {
                var templates = await _client.Download.GetItemTemplates();
                return
                    templates.ItemTemplates.Select(i => i.PokemonSettings)
                        .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

    }
}

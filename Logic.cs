using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using System.Threading.Tasks;
using System;
using POGOProtos.Data;
using System.Collections.Generic;
using POGOProtos.Networking.Responses;

namespace PokeBag
{
    class Logic
    {
        public readonly Client _client;
        public readonly ISettings _clientSettings;
        public readonly Inventory _inventory;

        public bool isLogin = false;
        public Random rand = new Random();

        public Logic(ISettings clientSettings)
        {

            _clientSettings = clientSettings;
            _client = new Client(_clientSettings);
            _inventory = new Inventory(_client);

        }

        public async Task<bool> Login()
        {
            try
            {
                if (_clientSettings.AuthType == AuthType.Ptc)
                    await _client.Login.DoPtcLogin(_clientSettings.PtcUsername, _clientSettings.PtcPassword);
                else if (_clientSettings.AuthType == AuthType.Google)
                {
                    await _client.Login.DoGoogleLogin(_clientSettings.GoogleUsername, _clientSettings.GooglePassword);
                }
                isLogin = true;
                return true;
            }
            catch (Exception ex)
            {
                isLogin = false;
                return false;
            }
           
        }

        public async Task<bool> TransferPokemon(ulong pokemonId)
        {
            try
            {
                await _client.Inventory.TransferPokemon(pokemonId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public async Task<PokemonData> EvolvePokemon(ulong pokemonId)
        {
            try
            {
                var inventory = await _client.Inventory.EvolvePokemon(pokemonId);
                return inventory.EvolvedPokemonData;
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        public async Task<string> GetPlayerName()
        {
            try
            {
                var player = await _client.Player.GetPlayer();
                return player.PlayerData.Username;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

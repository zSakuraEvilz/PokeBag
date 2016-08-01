using System;
using System.Threading;
using MahApps.Metro.Controls;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI;
using POGOProtos.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PokeBag
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Logic logic;
        public MainWindow()
        {
            InitializeComponent();
            var settings = GlobalSettings.Load();
            if (settings == null || (settings.AuthType == AuthType.Google 
                && settings.GoogleUsername == null && settings.GooglePassword == null) ||
                 (settings.AuthType == AuthType.Ptc
                && settings.PtcUsername == null && settings.PtcPassword == null))
            {
                Thread.Sleep(2000);
                Environment.Exit(0);
            }

            ISettings clientSettings = new ClientSettings(settings);
            logic = new Logic(clientSettings);
            MenuRefresh_Click(refresh, null);
        }

        private async void MenuRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!logic.isLogin)
            {
                statusBar.Text = "Is Login Account...";
                await logic.Login();
                playerName.Text = string.Format("Player: {0}", await logic.GetPlayerName());
                statusBar.Text = "Login Success!";
            }
            statusBar.Text = "Refresh Bag...";
            var pokemons = await GetPokemonInBag();
            while (pokemons == null)
            {
                pokemons = await GetPokemonInBag();
            }
            pokeBag.ItemsSource = await GetPokemonInBag();
            statusBar.Text = "Refresh Bag Success!";
         
        }

        private async Task<List<Pokemon>> GetPokemonInBag()
        {
            try
            {
                var pokemonDatas = await logic._inventory.GetPokemons();

                var myPokemonSettings = await logic._inventory.GetPokemonSettings();
                var pokemonSettings = myPokemonSettings.ToList();

                var myPokemonFamilies = await logic._inventory.GetPokemonFamilies();
                var pokemonFamilies = myPokemonFamilies.ToArray();

                List<Pokemon> pokemons = new List<Pokemon>();
                var pokemon = new Pokemon();
                foreach (var pokemonData in pokemonDatas)
                {
                    pokemon = new Pokemon();
                    pokemon.Image = string.Format("Resources/{0}.png", pokemonData.PokemonId);
                    pokemon.Id = pokemonData.Id;
                    pokemon.PokemonId = pokemonData.PokemonId;
                    pokemon.Cp = pokemonData.Cp;
                    pokemon.Iv = (int)PokemonInfo.CalculatePokemonPerfection(pokemonData);
                    pokemon.Stamina = pokemonData.Stamina;
                    pokemon.IndividualStamina = pokemonData.IndividualStamina;
                    pokemon.IndividualAttack = pokemonData.IndividualAttack;
                    pokemon.IndividualDefense = pokemonData.IndividualDefense;
                    pokemon.Toal = pokemonData.IndividualStamina + pokemonData.IndividualAttack + pokemonData.IndividualDefense;
                    pokemon.Move1 = pokemonData.Move1;
                    pokemon.Move2 = pokemonData.Move2;
                    int[] candy = await logic._inventory.GetEvolveRequiredCandy(pokemonSettings, pokemonFamilies, pokemon.PokemonId);
                    if (candy[1] == 0) // fully evolved
                        pokemon.EvolveRequiredCandy = string.Format("0", candy[0], candy[1]);
                    else
                        pokemon.EvolveRequiredCandy = string.Format("{0}/{1}", candy[0], candy[1]);
                    //candy = await logic._inventory.GetUpgradeRequiredCandy(pokemonSettings, pokemonFamilies, pokemon.PokemonId);
                    //if (candy[1] == 0) // fully evolved
                    //    pokemon.UpgradeRequiredCandy = string.Format("0", candy[0], candy[1]);
                    //else
                    //    pokemon.UpgradeRequiredCandy = string.Format("{0}/{1}", candy[0], candy[1]);
                    
                    pokemon.Tag = pokemonData;
                    pokemons.Add(pokemon);
                }
                return pokemons;
            }catch(Exception ex)
            {
                return null;
            }
        }

        private async void Evolve_Click(object sender, RoutedEventArgs e)
        {
            if (!logic.isLogin)
            {
                statusBar.Text = "Is Login Account...";
                await logic.Login();
                playerName.Text = string.Format("Player: {0}", await logic.GetPlayerName());
                statusBar.Text = "Login Success!";
            }
            PokemonData pokemon = (PokemonData)((Button)sender).Tag;
            statusBar.Text = "Evolve Pokemon " + pokemon.PokemonId + " CP: " + pokemon.Cp + " IV: " + (int)PokemonInfo.CalculatePokemonPerfection(pokemon) + " Move1: " + pokemon.Move1 + " Move2 " + pokemon.Move2 +" ...";
            var new_pokemon = await logic.EvolvePokemon(pokemon.Id);
            if(new_pokemon != null)
            {
               // pokeBag.ItemsSource = await GetPokemonInBag();
                statusBar.Text = "Evolve Pokemon " + pokemon.PokemonId + " CP: " + pokemon.Cp + " IV: " + (int)PokemonInfo.CalculatePokemonPerfection(pokemon) +
                    " Move1: " + pokemon.Move1 + " Move2 " + pokemon.Move2 + " Success! New Pokemon " + new_pokemon.PokemonId +" CP: " + new_pokemon.Cp + "  IV: " + PokemonInfo.CalculatePokemonPerfection(new_pokemon) +
                    " Move1: " + new_pokemon.Move1 + " Move2 " + new_pokemon.Move2 ;
                ((Button)sender).IsEnabled = false;
            }
            else
            {
                statusBar.Text = "Evolve Failed!";
            }

        }

        private async void Transfer_Click(object sender, RoutedEventArgs e)
        {
            if (!logic.isLogin)
            {
                statusBar.Text = "Is Login Account...";
                await logic.Login();
                playerName.Text = string.Format("Player: {0}", await logic.GetPlayerName());
                statusBar.Text = "Login Success!";
            }

            PokemonData pokemon = (PokemonData)((Button)sender).Tag;
            statusBar.Text = "Transfer Pokemon " + pokemon.PokemonId + " CP: " + pokemon.Cp + " IV: " + (int)PokemonInfo.CalculatePokemonPerfection(pokemon) + " Move1: " + pokemon.Move1 + " Move2 " + pokemon.Move2 +" ...";
            var new_pokemon = await logic.TransferPokemon(pokemon.Id);
            if (new_pokemon == true)
            {
                //pokeBag.ItemsSource = await GetPokemonInBag();
              
                statusBar.Text = "Transfer Pokemon " + pokemon.PokemonId + " CP: " + pokemon.Cp + " IV: " + (int)PokemonInfo.CalculatePokemonPerfection(pokemon) +
                    " Move1: " + pokemon.Move1 + " Move2 " + pokemon.Move2 + " Success!";
                ((Button)sender).IsEnabled = false;
            }
            else
            {
                statusBar.Text = "Transfer Failed!";
            }
        }

        private void pokeBag_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var a = sender;
        }

        private async void Upgrade_Click(object sender, RoutedEventArgs e)
        {
            if (!logic.isLogin)
            {
                statusBar.Text = "Is Login Account...";
                await logic.Login();
                playerName.Text = string.Format("Player: {0}", await logic.GetPlayerName());
                statusBar.Text = "Login Success!";
            }
            PokemonData pokemon = (PokemonData)((Button)sender).Tag;
            statusBar.Text = "Upgrade Pokemon " + pokemon.PokemonId + " CP: " + pokemon.Cp + " IV: " + (int)PokemonInfo.CalculatePokemonPerfection(pokemon) + " Move1: " + pokemon.Move1 + " Move2 " + pokemon.Move2 + " ...";
            var new_pokemon = await logic.EvolvePokemon(pokemon.Id);
            if (new_pokemon != null)
            {
                // pokeBag.ItemsSource = await GetPokemonInBag();
                statusBar.Text = "Upgrade Pokemon " + pokemon.PokemonId + " CP: " + pokemon.Cp + " IV: " + (int)PokemonInfo.CalculatePokemonPerfection(pokemon) +
                    " Move1: " + pokemon.Move1 + " Move2 " + pokemon.Move2 + " Success! New Pokemon " + new_pokemon.PokemonId + " CP: " + new_pokemon.Cp + "  IV: " + PokemonInfo.CalculatePokemonPerfection(new_pokemon) +
                    " Move1: " + new_pokemon.Move1 + " Move2 " + new_pokemon.Move2;
                ((Button)sender).IsEnabled = false;
            }
            else
            {
                statusBar.Text = "Upgrade Failed!";
            }
        }
    }
}

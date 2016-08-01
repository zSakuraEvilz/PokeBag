using System;
using POGOProtos.Data;
using POGOProtos.Enums;
namespace PokeBag
{
    public class Pokemon
    {
        public Pokemon() { }
        public ulong Id { get; set; }
        public PokemonId PokemonId { get; set; }
        public int Cp { get; set; }
        public double Iv { get; set; }
        public int Stamina { get; set; }
        public int IndividualStamina { get; set; }
        public int IndividualAttack { get; set; }
        public int IndividualDefense { get; set; }
        public int Toal { get; set; }
        public PokemonMove Move1 { get; set; }
        public PokemonMove Move2 { get; set; }
        public string Candies { get; set; }
        public PokemonData Tag { get; set; }
    }
}

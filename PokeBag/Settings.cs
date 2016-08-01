using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;


namespace PokeBag
{

    public class GlobalSettings
    {
        [JsonIgnore]
        public string ProfileConfigPath;
        [JsonIgnore]
        public string ConfigFile;

        public AuthType AuthType;
        public string PtcUsername;
        public string PtcPassword;
        public string GoogleUsername;
        public string GooglePassword;
        public string GoogleRefreshToken;
        public double DefaultAltitude = 10;
        public double DefaultLatitude = 40.785091;
        public double DefaultLongitude = -73.968285;
        [JsonIgnore]
        public int MaxSpawnLocationOffset = 10;

        public static GlobalSettings Default => new GlobalSettings();

        public static GlobalSettings Load()
        {
            GlobalSettings settings;
            var profileConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");

            if (File.Exists(configFile))
            {
                try
                {
                    //if the file exists, load the settings
                    var input = File.ReadAllText(configFile);

                    var jsonSettings = new JsonSerializerSettings();
                    jsonSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                    jsonSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    jsonSettings.DefaultValueHandling = DefaultValueHandling.Populate;

                    settings = JsonConvert.DeserializeObject<GlobalSettings>(input, jsonSettings);
                }
                catch (Newtonsoft.Json.JsonReaderException exception)
                {
                    //Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
                    return null;
                }
            }
            else
            {
                settings = new GlobalSettings();
            }

            settings.ProfileConfigPath = profileConfigPath;
            settings.ConfigFile = configFile;
            settings.Save(configFile);
            return settings;
        }

        public void Save(string fullPath)
        {
            var output = JsonConvert.SerializeObject(this, Formatting.Indented,
                new StringEnumConverter { CamelCaseText = true });

            var folder = Path.GetDirectoryName(fullPath);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(fullPath, output);
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(ConfigFile))
            {
                Save(ConfigFile);
            }
        }
    }

    public class ClientSettings : ISettings
    {
        private readonly GlobalSettings _settings;

        public ClientSettings(GlobalSettings settings)
        {
            _settings = settings;
        }


        public string GoogleUsername => _settings.GoogleUsername;
        public string GooglePassword => _settings.GooglePassword;
        public string GoogleRefreshToken
        {
            get { return _settings.GoogleRefreshToken; }
            set
            {
                _settings.GoogleRefreshToken = value;
                _settings.Save();
            }
        }


        AuthType ISettings.AuthType
        {
            get
            {
                return _settings.AuthType;
            }

            set
            {
                _settings.AuthType = value;
            }
        }

        // Never spawn at the same position.
        private readonly Random _rand = new Random();

        double ISettings.DefaultLatitude
        {
            get
            {
                return _settings.DefaultLatitude + _rand.NextDouble() * (_settings.MaxSpawnLocationOffset / 111111);
            }

            set
            {
                _settings.DefaultLatitude = value;
            }
        }

        double ISettings.DefaultLongitude
        {
            get
            {
                return _settings.DefaultLongitude + _rand.NextDouble() * (_settings.MaxSpawnLocationOffset / 111111 / Math.Cos(_settings.DefaultLatitude));
            }

            set
            {
                _settings.DefaultLongitude = value;
            }
        }

        double ISettings.DefaultAltitude
        {
            get
            {
                return _settings.DefaultAltitude;
            }

            set
            {
                _settings.DefaultAltitude = value;
            }
        }

        string ISettings.PtcPassword
        {
            get
            {
                return _settings.PtcPassword;
            }

            set
            {
                _settings.PtcPassword = value;
            }
        }

        string ISettings.PtcUsername
        {
            get
            {
                return _settings.PtcUsername;
            }

            set
            {
                _settings.PtcUsername = value;
            }
        }

        string ISettings.GoogleUsername
        {
            get
            {
                return _settings.GoogleUsername;
            }

            set
            {
                _settings.GoogleUsername = value;
            }
        }
        string ISettings.GooglePassword
        {
            get
            {
                return _settings.GooglePassword;
            }

            set
            {
                _settings.GooglePassword = value;
            }
        }
    }

}

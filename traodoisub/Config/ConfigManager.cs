using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace traodoisub.Config
{
    public class ConfigManager
    {
        private string configFilePath = "config.json";

        public void SaveConfig(List<ConfigADO> config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.None);
            File.WriteAllText(configFilePath, json);
        }

        public List<ConfigADO> LoadConfig()
        {
            if (File.Exists(configFilePath))
            {
                string json = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<List<ConfigADO>>(json);
            }
            return null; // Nếu không tìm thấy file
        }
    }
}

using System.Configuration;

namespace free_ps2_pub_gen.Properties {
    internal sealed partial class Settings {        
        public Settings() { }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LastIsoPath {
            get { return (string)this["LastIsoPath"]; }
            set { this["LastIsoPath"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LastOutPath {
            get { return (string)this["LastOutPath"]; }
            set { this["LastOutPath"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string MakeFSELF {
            get { return (string)this["MakeFSELF"]; }
            set { this["MakeFSELF"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string DB {
            get { return (string)this["DB"]; }
            set { this["DB"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string PubCmd {
            get { return (string)this["PubCmd"]; }
            set { this["PubCmd"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public bool ClearIso {
            get { return (bool)this["ClearIso"]; }
            set { this["ClearIso"] = value; }
        }
    }
}

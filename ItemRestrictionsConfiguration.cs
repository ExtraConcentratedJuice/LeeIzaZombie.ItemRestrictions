using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LeeIzaZombie.ItemRestrictions
{
    public class ItemRestrictionsConfiguration : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "Item")]
        public List<ushort> Items;
        [XmlArrayItem(ElementName = "Group")]
        public List<UnrestrictGroup> UnrestrictGroups;

        public bool ignoreAdmin;
        public float CheckInterval = 1.0F;

        public void LoadDefaults()
        {
            this.ignoreAdmin = true;
            this.Items = new List<ushort>()
            {
                519,
                1050
            };
            this.UnrestrictGroups = new List<UnrestrictGroup>()
            {
                new UnrestrictGroup { Items = new List<ushort>() {69, 420}, permission = "restriction.group.dank" },
                new UnrestrictGroup { Items = new List<ushort>() {1, 2}, permission = "restriction.group.notVIP" }
            };
        }
    }
    public class UnrestrictGroup
    {
        public UnrestrictGroup()
        {
        }

        [XmlArrayItem(ElementName = "Item")]
        public List<ushort> Items;
        public string permission;
    }
}

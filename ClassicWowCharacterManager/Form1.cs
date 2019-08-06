using ClassicWowCharacterManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassicWowCharacterManager
{
    public partial class Form1 : Form
    {
        private List<FactionRaceClass> FacRaceClassList;
        private Character CurrentCharacter;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FacRaceClassList = GetFactionRaceClassList();
            FillFactionDrop(FacRaceClassList);
        }

        private void FillFactionDrop(List<FactionRaceClass> list)
        {
            var Factions = list.Select(s => s.Faction).Distinct().ToArray();
            Array.Sort(Factions);
            cbFaction.Items.Clear();
            cbFaction.Items.AddRange(Factions);
            cbFaction.SelectedIndex = 0;
        }

        private void FillRaceDrop(List<FactionRaceClass> list)
        {
            var Races = list.Select(s => s.Race).Distinct().ToArray();
            Array.Sort(Races);
            cbRace.Items.Clear();
            cbRace.Items.AddRange(Races);
            cbRace.SelectedIndex = 0;
        }

        private void FillClassDrop(List<FactionRaceClass> list)
        {
            var Classes = list.Select(s => s.Class).ToArray();
            Array.Sort(Classes);
            cbClass.Items.Clear();
            cbClass.Items.AddRange(Classes);
            cbClass.SelectedIndex = 0;
        }

        private void cbFaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            var list = FacRaceClassList.Where(w => w.Faction == cbFaction.SelectedItem.ToString()).Select(s => s).ToList();
            FillRaceDrop(list);
        }

        private void cbRace_SelectedIndexChanged(object sender, EventArgs e)
        {
            var list = FacRaceClassList.Where(w => w.Faction == cbFaction.SelectedItem.ToString() && w.Race == cbRace.SelectedItem.ToString()).Select(s => s).ToList();
            FillClassDrop(list);
        }

        private List<FactionRaceClass> GetFactionRaceClassList()
        {
            var list = new List<FactionRaceClass>();
            var x = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\0023806\source\repos\ClassicWowCharacterManager\ClassicWowCharacterManager\WoWServer.mdf;Integrated Security=True";
            var cmdText = "SELECT * FROM FactionRaceClass";
            using (SqlConnection conn = new SqlConnection(x))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(cmdText, conn))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new FactionRaceClass()
                        {
                            Faction = reader["Faction"].ToString().Trim(),
                            Race = reader["Race"].ToString().Trim(),
                            Class = reader["Class"].ToString().Trim()
                        });
                    }
                }
            }
            return list;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchForm searchForm = new SearchForm(FacRaceClassList);
            var result = searchForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                CurrentCharacter = searchForm.Character;
                LoadCharacter(CurrentCharacter);
                btnUpdate.Enabled = true;
            }
                
        }
        private void LoadCharacter(Character character)
        {
            txtName.Text = character.Name;
            numLvl.Value = character.Level;
            cbFaction.SelectedItem = character.Characteristics.Faction;
            cbRace.SelectedItem = character.Characteristics.Race;
            cbClass.SelectedItem = character.Characteristics.Class;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var level = (int)numLvl.Value;
            var name = txtName.Text;
            var faction = cbFaction.SelectedItem.ToString();
            var race = cbRace.SelectedItem.ToString();
            var cls = cbClass.SelectedItem.ToString();
            var x = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\0023806\source\repos\ClassicWowCharacterManager\ClassicWowCharacterManager\WoWServer.mdf;Integrated Security=True";
            var cmdText = string.Format("INSERT INTO Character(Name, Faction, Race, Class, Level) VALUES ('{0}','{1}','{2}','{3}',{4})", name, faction, race, cls, level);
            using (SqlConnection conn = new SqlConnection(x))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand(cmdText, conn))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Successfully Added!");
                        ResetValues();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error occured adding character: " + ex.Message);
                    }
                }
            }

        }
        private void ResetValues()
        {
            txtName.Text = "";
            numLvl.Value = 1;
            btnUpdate.Enabled = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var level = (int)numLvl.Value;
            var name = txtName.Text;
            var faction = cbFaction.SelectedItem.ToString();
            var race = cbRace.SelectedItem.ToString();
            var cls = cbClass.SelectedItem.ToString();
            var x = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\0023806\source\repos\ClassicWowCharacterManager\ClassicWowCharacterManager\WoWServer.mdf;Integrated Security=True";
            var cmdText = string.Format("UPDATE Character SET Name = '{0}', Faction = '{1}', Race = '{2}', Class = '{3}', Level = {4}) WHERE Name = '{5}", name, faction, race, cls, level, CurrentCharacter.Name);
            using (SqlConnection conn = new SqlConnection(x))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand(cmdText, conn))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Successfully Updated!");
                        ResetValues();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error occured adding character: " + ex.Message);
                    }
                }
            }
        }

        //private void btnSearch_Click(object sender, EventArgs e)
        //{
        //    var x = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\0023806\source\repos\ClassicWowCharacterManager\ClassicWowCharacterManager\WoWServer.mdf;Integrated Security=True";
        //    var list = GetFactionRaceClassList2();

        //    using(SqlConnection conn = new SqlConnection(x))
        //    {
        //        conn.Open();
        //        foreach (var item in list)
        //        {
        //            var cmdText = string.Format("INSERT INTO FactionRaceClass (Faction, Race, Class) VALUES('{0}', '{1}', '{2}')", item.Faction, item.Race, item.Class);
        //            using (SqlCommand command = new SqlCommand(cmdText, conn))
        //            {
        //                try
        //                {
        //                    command.ExecuteNonQuery();
        //                }
        //                catch
        //                {

        //                }
        //            }
        //        }
        //    }
        //}

        //private List<FactionRaceClass> GetFactionRaceClassList2()
        //{
        //    List<FactionRaceClass> list = new List<FactionRaceClass>();
        //    var x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Human",
        //        Class = "Paladin"
        //    };
        //    list.Add(x);
        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Orc",
        //        Class = "Warrior"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Human",
        //        Class = "Mage"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Human",
        //        Class = "Priest"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Human",
        //        Class = "Warrior"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Human",
        //        Class = "Rogue"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Human",
        //        Class = "Warlock"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Dwarf",
        //        Class = "Warrior"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Dwarf",
        //        Class = "Rogue"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Dwarf",
        //        Class = "Hunter"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Dwarf",
        //        Class = "Paladin"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Dwarf",
        //        Class = "Priest"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Gnome",
        //        Class = "Mage"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Gnome",
        //        Class = "Rogue"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Gnome",
        //        Class = "Warlock"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Gnome",
        //        Class = "Warrior"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Night Elf",
        //        Class = "Hunter"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Night Elf",
        //        Class = "Druid"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Night Elf",
        //        Class = "Priest"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Night Elf",
        //        Class = "Rogue"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Alliance",
        //        Race = "Night Elf",
        //        Class = "Warrior"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Orc",
        //        Class = "Hunter"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Orc",
        //        Class = "Rogue"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Orc",
        //        Class = "Shaman"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Orc",
        //        Class = "Warlock"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Tauren",
        //        Class = "Hunter"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Tauren",
        //        Class = "Druid"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Tauren",
        //        Class = "Shaman"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Tauren",
        //        Class = "Warrior"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Troll",
        //        Class = "Hunter"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Troll",
        //        Class = "Warrior"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Troll",
        //        Class = "Mage"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Troll",
        //        Class = "Shaman"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Troll",
        //        Class = "Priest"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Troll",
        //        Class = "Rogue"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Undead",
        //        Class = "Warrior"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Undead",
        //        Class = "Mage"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Undead",
        //        Class = "Warlock"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Undead",
        //        Class = "Rogue"
        //    };
        //    list.Add(x);

        //    x = new FactionRaceClass()
        //    {
        //        Faction = "Horde",
        //        Race = "Undead",
        //        Class = "Priest"
        //    };
        //    list.Add(x);
        //    return list;
        //}
    }
}

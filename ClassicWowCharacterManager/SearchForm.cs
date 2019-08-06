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
    public partial class SearchForm : Form
    {
        List<FactionRaceClass> FacRaceClassList;
        public Character Character;
        public SearchForm(List<FactionRaceClass> list)
        {
            InitializeComponent();
            Character = null;
            FacRaceClassList = list;
            FillFactionDrop(FacRaceClassList);
        }

        private void FillFactionDrop(List<FactionRaceClass> list)
        {
            var Factions = list.Select(s => s.Faction).Distinct().ToArray();
            Array.Sort(Factions);
            cbFaction.Items.Clear();
            cbFaction.Items.Add("All");
            cbFaction.Items.AddRange(Factions);
            cbFaction.SelectedIndex = 0;
        }

        private void FillRaceDrop(List<FactionRaceClass> list)
        {
            var Races = list.Select(s => s.Race).Distinct().ToArray();
            Array.Sort(Races);
            cbRace.Items.Clear();
            cbRace.Items.Add("All");
            cbRace.Items.AddRange(Races);
            cbRace.SelectedIndex = 0;
        }

        private void FillClassDrop(List<FactionRaceClass> list)
        {
            var Classes = list.Select(s => s.Class).Distinct().ToArray();
            Array.Sort(Classes);
            cbClass.Items.Clear();
            cbClass.Items.Add("All");
            cbClass.Items.AddRange(Classes);
            cbClass.SelectedIndex = 0;
        }

        private void cbFaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = cbFaction.SelectedItem.ToString();

            List<FactionRaceClass> list;
            if (selected == "All")
                list = FacRaceClassList;
            else
                list = FacRaceClassList.Where(w => w.Faction == selected).Select(s => s).ToList();
            FillRaceDrop(list);
        }

        private void cbRace_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedRace = cbRace.SelectedItem.ToString();
            var selectedFaction = cbFaction.SelectedItem.ToString();
            List<FactionRaceClass> list;
            if (selectedFaction == "All")
                list = FacRaceClassList;
            else
                list = FacRaceClassList.Where(w => w.Faction == selectedFaction).Select(s => s).ToList();

            if (selectedRace != "All")
                list = list.Where(w => w.Race == selectedRace).Select(s => s).ToList();
            FillClassDrop(list);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(dgCharacters.SelectedRows.Count > 0)
            {
                Character = (Character)dgCharacters.SelectedRows[0].Cells[5].Value;
                this.DialogResult = DialogResult.OK;
            }
            
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgCharacters.Rows.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(" SELECT * FROM Character WHERE Name LIKE '%{0}%' ", txtName.Text));
            if (cbFaction.SelectedItem.ToString() != "All")
                sb.Append(string.Format(" AND Faction = '{0}' ", cbFaction.SelectedItem.ToString()));
            if (cbRace.SelectedItem.ToString() != "All")
                sb.Append(string.Format(" AND Race = '{0}' ", cbRace.SelectedItem.ToString()));
            if (cbClass.SelectedItem.ToString() != "All")
                sb.Append(string.Format(" AND Class = '{0}' ", cbClass.SelectedItem.ToString()));
            if (numLow.Value != 1 || numHigh.Value != 60)
            {
                sb.Append(string.Format(" AND Level >= {0} AND Level <= {1} ", numLow.Value, numHigh.Value));
            }

            var list = GetSearchList(sb.ToString());
            var dgList = new List<DataGridViewRow>();
            foreach (var item in list)
            {
                var dgRow = new DataGridViewRow();
                dgRow.CreateCells(dgCharacters);
                dgRow.Cells[0].Value = item.Name;
                dgRow.Cells[1].Value = item.Level;
                dgRow.Cells[2].Value = item.Characteristics.Faction;
                dgRow.Cells[3].Value = item.Characteristics.Race;
                dgRow.Cells[4].Value = item.Characteristics.Class;
                dgRow.Cells[5].Value = item;
                dgList.Add(dgRow);
            }
            dgCharacters.Rows.AddRange(dgList.ToArray());
        }

        private List<Character> GetSearchList(string searchString)
        {
            var list = new List<Character>();
            StringBuilder sb = new StringBuilder();
            var x = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\0023806\source\repos\ClassicWowCharacterManager\ClassicWowCharacterManager\WoWServer.mdf;Integrated Security=True";
            sb.Append(string.Format(" SELECT * FROM Character WHERE Name LIKE '%{0}%' ", txtName.Text));
            if(cbFaction.SelectedItem.ToString() != "All")
                sb.Append(string.Format(" AND Faction = '{0}' ", cbFaction.SelectedItem.ToString()));
            if (cbRace.SelectedItem.ToString() != "All")
                sb.Append(string.Format(" AND Race = '{0}' ", cbRace.SelectedItem.ToString()));
            if (cbClass.SelectedItem.ToString() != "All")
                sb.Append(string.Format(" AND Class = '{0}' ", cbClass.SelectedItem.ToString()));
            if(numLow.Value != 1 || numHigh.Value != 60)
            {
                sb.Append(string.Format(" AND Level >= {0} AND Level <= {1} ", numLow.Value, numHigh.Value));
            }

            using (SqlConnection conn = new SqlConnection(x))
            {
                conn.Open();


                using (SqlCommand command = new SqlCommand(searchString, conn))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Character()
                        {
                            Name = reader["Name"].ToString().Trim(),
                            Level = int.Parse(reader["Level"].ToString()),
                            Characteristics = new FactionRaceClass()
                            {
                                Faction = reader["Faction"].ToString().Trim(),
                                Race = reader["Race"].ToString().Trim(),
                                Class = reader["Class"].ToString().Trim()
                            }
                        });
                    }
                }
            }

            return list;
        }
    }
}

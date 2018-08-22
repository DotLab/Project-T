using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextyClient
{
    public partial class CharacterInfo : Form
    {
        public CharacterInfo()
        {
            InitializeComponent();
        }

        public void SetName(string name)
        {
            nameTbx.Text = name;
        }

        public void SetDescription(string description)
        {
            descriptionTbx.Text = description;
        }

        public void SetRefreshPoint(int point)
        {
            refreshPointLbl.Text = point.ToString();
        }

        public void SetFatePoint(int point)
        {
            fatePointLbl.Text = point.ToString();
        }

        public void SetAspects(object list)
        {
            aspectListBox.DataSource = list;
        }

        public void SetSkills(object list)
        {
            skillListBox.DataSource = list;
        }

        public void SetStunts(object list)
        {
            stuntListBox.DataSource = list;
        }

        public void SetExtras(object list)
        {
            extraListBox.DataSource = list;
        }

        public void SetConsequences(object list)
        {
            consequenceListBox.DataSource = list;
        }

        public void SetPhysicsStress(int val, int maxVal)
        {
            physicsStressLbl.Text = val + " / " + maxVal;
        }

        public void SetMentalStress(int val, int maxVal)
        {
            mentalStressLbl.Text = val + "/" + maxVal;
        }
    }
}

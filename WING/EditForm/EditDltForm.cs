using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WING
{
    public partial class EditDltForm : Form
    {
        ListBox _DltListBox;
        List<double> _Dlt;
        public EditDltForm()
        {
            InitializeComponent();
        }

        public EditDltForm(ref ListBox DltListBox, ref List<double> Dlt)
        {
            InitializeComponent();
            _DltListBox = DltListBox;
            _Dlt = Dlt;
            editTextBox.Text = Convert.ToString(Dlt[DltListBox.SelectedIndex]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            okEditButton(ref _DltListBox, ref _Dlt);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void okEditButton(ref ListBox DltListBox, ref List<double> Dlt)
        {
            int tmp = DltListBox.SelectedIndex;
            if (editTextBox.Text.Contains("."))
                editTextBox.Text = editTextBox.Text.Replace(".", ",");//учет и запятых и точек
            Dlt[DltListBox.SelectedIndex] = (double)Convert.ToDouble(editTextBox.Text);
            DltListBox.Items.Insert(DltListBox.SelectedIndex, (DltListBox.SelectedIndex + 1) + ". " + Dlt[DltListBox.SelectedIndex]);
            DltListBox.Items.Remove(DltListBox.SelectedItem);
            DltListBox.SelectedIndex = tmp;
            this.Close();
        }
    }
}

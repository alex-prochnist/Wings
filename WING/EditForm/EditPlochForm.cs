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
    public partial class EditPlochForm : Form
    {
        ListBox _koorOrPlochListBox;
        List<double> _XYF;
        public EditPlochForm()
        {
            InitializeComponent();
        }

        public EditPlochForm(ref ListBox koorOrPlochListBox, ref List<double> XYF)
        {
            InitializeComponent();
            _koorOrPlochListBox = koorOrPlochListBox;
            _XYF = XYF;
            editTextBox.Text = Convert.ToString(XYF[koorOrPlochListBox.SelectedIndex]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            okEditButton(ref _koorOrPlochListBox, ref _XYF);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void okEditButton(ref ListBox koorOrPlochListBox, ref List<double> XYF)
        {
            int tmp = koorOrPlochListBox.SelectedIndex;
            if (editTextBox.Text.Contains("."))
                editTextBox.Text = editTextBox.Text.Replace(".", ",");//учет и запятых и точек
            XYF[koorOrPlochListBox.SelectedIndex] = (double)Convert.ToDouble(editTextBox.Text);
            koorOrPlochListBox.Items.Insert(koorOrPlochListBox.SelectedIndex, (koorOrPlochListBox.SelectedIndex + 1) + ". " + XYF[koorOrPlochListBox.SelectedIndex]);
            koorOrPlochListBox.Items.Remove(koorOrPlochListBox.SelectedItem);
            koorOrPlochListBox.SelectedIndex = tmp;
            this.Close();
        }
    }
}

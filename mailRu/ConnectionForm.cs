using System;
using System.Windows.Forms;

namespace mailRu
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();

            FormClosing += ConnectionForm_FormClosing;
        }

        private void ConnectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);    
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if(connectionStringTextBox.Text.Length > 0)
            {
                Form1.mainDbConnectionString = connectionStringTextBox.Text.Length > 0 ? connectionStringTextBox.Text : null;
            }
            if(baseNameTextBox.Text.Length > 0)
            {
                Form1.mainDb = baseNameTextBox.Text;
            }
            if (textBox1.Text.Length > 0)
            {
                Form1.configPath = textBox1.Text;
            }

            Hide();
            new Form1().Show();
        }
    }
}

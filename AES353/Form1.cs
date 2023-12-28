using System;
using System.Drawing;
using System.Windows.Forms;

namespace AES353
{
    public partial class Form1 : Form
    {
        private CommandParser processor;

        public Form1()
        {
            InitializeComponent();
            processor = new CommandParser(codeTextBox, displayArea);
            displayArea.Paint += new PaintEventHandler(displayArea_Paint);
            commandTextBox.KeyUp += new KeyEventHandler(commandTextBox_KeyUp);
            runButton.Click += new EventHandler(click_Run);
            syntaxButton.Click += new EventHandler(click_Syntax);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // This method is called when the form loads.
        }

        private void click_Run(object sender, EventArgs e)
        {
            try
            {
                processor.ExecuteProgram();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error executing the program: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void click_Syntax(object sender, EventArgs e)
        {
            try
            {
                processor.CheckSyntax();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Syntax error: " + ex.Message, "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void commandTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var commandText = commandTextBox.Text;
                try
                {
                    processor.ExecuteCommand(commandText);
                    commandTextBox.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error executing command: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void displayArea_Paint(object sender, PaintEventArgs e)
        {
            processor.SetupGraphics(e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            processor.Cleanup();
        }
    }
}

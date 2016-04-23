using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sddc.SampleDataFlowComponent
{
    public partial class SetWebsiteAddressDialog : Form
    {
        public string WebsiteAddress
        {
            get { return websiteAddressTextBox.Text; }
            set { websiteAddressTextBox.Text = value;  }
        }

        public SetWebsiteAddressDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}

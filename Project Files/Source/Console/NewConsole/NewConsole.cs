using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thetis {
public partial class NewConsole : Form {
    string m_data_path;
    public NewConsole(string[] args, string datapath) {
        m_data_path = datapath;
        InitializeComponent();
    }
}
}

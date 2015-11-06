using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Management.Automation.Runspaces;

namespace DHCPViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var psData = new PowerShellDhcpData();

            if (psData.TestForDhcpModule())
            {
                var f1 = new Form1();
                f1.data = psData;
                Application.Run(f1);
            }
            else
                Application.Run(new Alert());
        }
    }
}

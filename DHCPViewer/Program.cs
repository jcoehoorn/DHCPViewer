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
            if (CheckPrerequisites())
                Application.Run(new Form1());
            else
                Application.Run(new Alert());
        }

        public static bool CheckPrerequisites()
        {
            try
            {
                using (var rs = RunspaceFactory.CreateRunspace())
                {
                    rs.Open();
                    using (var pipeline = rs.CreatePipeline())
                    {
                        pipeline.Commands.Add("Get-DhcpServerv4Scope");
                        pipeline.Invoke();
                    }
                }
            }
            //this WILL fail (missing required parameter).
            // this trick is in which error is in the message
            catch(Exception Ex)
            {
                // TODO: This will fail in other locales, need to get HRESULT value to check
                //      also, this check is slow (requires starting up unnecessary runspace)
                if (Ex.Message.StartsWith("The term 'Get-DhcpServerv4Scope' is not recognized"))
                    return false;
                return true;
            }
            return true; //fallback ... if it somehow succeeds (can happen if run directly on dhcp server), the correct powershell tools obviously do exist
        }
    }
}

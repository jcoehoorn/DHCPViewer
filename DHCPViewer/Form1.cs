using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DHCPViewer
{
    public partial class Form1 : Form
    {
        //this is set from Program.cs
        public PowerShellDhcpData data;

        public Form1()
        {
            InitializeComponent();
        }

        private Dictionary<string, DchpScopeDetails> _scopeDetails;

        private void ChangeServer()
        {
            //todo: want BackgroundWorker for most of this
            Cursor tmp = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            lblError.Text = "";
            tmrServerKeys.Enabled = false;
            SaveLastServer();     
            ClearLeaseGrid();
            SetScopes();
            Cursor.Current = tmp;
        }

        private void txtServer_Leave(object sender, EventArgs e)
        {
            if (SanitizeServerText(txtServer.Text))
                ChangeServer();
        }

        private bool SanitizeServerText(string text)
        {         
            bool result = true;

            //TODO:...
            // Not 100% sure this is necessary any more.
            // The powershell pipeline uses a parameter mechanism that is at least superficially similar to sql parameters
            //    so this may already be quarantined.

            if (!result) // if sanitize is bad
            {
                lblError.Text = "Invalid server text";
            }
            Server = text;
            return result;
        }

        void SetScopes()
        {
            ClearScopesList();
            ClearLeaseGrid();
            try
            {
                var scopes = data.GetDhcpScopeListDetails(Server);
                _scopeDetails = new Dictionary<string, DchpScopeDetails>();
                foreach (var scope in scopes)
                {
                    _scopeDetails.Add(scope.ScopeId, scope);
                    lstScopes.Nodes[0].Nodes.Add(scope.ScopeId, scope.ToString());
                }
                lstScopes.ExpandAll();
            }
            catch(Exception Ex)
            {
                lblError.Text = Ex.Message;
            }
        }

        private void SaveLastServer()
        {
            Properties.Settings.Default.Server = Server;
            Properties.Settings.Default.Save();
        }

        private void ClearLeaseGrid()
        {
            dgvLeases.Rows.Clear();
        }

        private void ClearScopesList()
        {
            lstScopes.Nodes[0].Nodes.Clear();
            txtSummary.Text = "";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            tmrServerKeys.Enabled = false;
            if (txtServer.Text != Server)
            {
                if (SanitizeServerText(txtServer.Text))
                    ChangeServer();
            }
            else {
                UpdateLeaseGrid();
            }
        }

        string Server = null;
        TreeNode SelectedScope = null;

        private void UpdateLeaseGrid()
        {
            if (SelectedScope == null) return;

            Cursor tmp = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            lblError.Text = "";
            ClearLeaseGrid();
            try
            {
                foreach (var row in data.GetDhcpScopeLeases(Server, SelectedScope.Text.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0]))
                {
                    dgvLeases.Rows.Add(row);
                }

            }
            catch(Exception Ex)
            {
                lblError.Text = Ex.Message;
            }

            if (dgvLeases.Rows.Count == 0)
            {
                lblRecords.Text = "";
            }
            else if (dgvLeases.Rows.Count == 1)
            {
                lblRecords.Text = "1 record ";
            }
            else
            {
                lblRecords.Text = string.Format("{0} records ", dgvLeases.Rows.Count);
            }
            Cursor.Current = tmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblError.Width = this.Width - (lblRecords.Width + 50);
            txtServer.Text = Properties.Settings.Default.Server;
            SanitizeServerText(txtServer.Text);
        }

        private void tmrServerKeys_Tick(object sender, EventArgs e)
        {
            if (SanitizeServerText(txtServer.Text))
                ChangeServer();
        }

        private void txtServer_TextChanged(object sender, EventArgs e)
        {
            //need to reset time on each change... this seems to work
            tmrServerKeys.Enabled = false;
            tmrServerKeys.Enabled = true;
        }

        private void SetActiveScopeNode()
        {
            if (lstScopes.SelectedNode != lstScopes.Nodes[0])
            {
                SelectedScope = lstScopes.SelectedNode;
                var details = _scopeDetails[SelectedScope.Text.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0]];
                if (details != null)
                {
                    txtSummary.Text = details.GetSummary();
                }
                UpdateLeaseGrid();
            }
        }

        private void lstScopes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SetActiveScopeNode();        
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            lblError.Width = this.Width - (lblRecords.Width + 50);
        }

        private void dgvLeases_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            string val1 = e.CellValue1?.ToString() ?? "";
            string val2 = e.CellValue2?.ToString() ?? "";

            //TODO: This is inefficient, requires potentially processing the same IPAddress or Date values over and over again
            //       It doesn't seem to cause noticable delays, so low priority, but I'd still like to refactor
            //       this to re-use parsed values.
            int result = 0;
            if (val1 == "" && val2 != "")
            {
                result = -1;
            }
            else if (val1 != "" && val2 == "")
            {
                result = 1;
            }
            else if (val1 == "" && val2 == "")
            {
                result = 0;
            }
            //neither value is empty. Now we can process special columns
            else if (e.Column.HeaderText == "IP Address")
            {
                uint v1 = (uint)System.Net.IPAddress.NetworkToHostOrder((int)System.Net.IPAddress.Parse(val1).Address);
                uint v2 = (uint)System.Net.IPAddress.NetworkToHostOrder((int)System.Net.IPAddress.Parse(val2).Address);
                result = v1.CompareTo(v2);
            }
            else if (e.Column.HeaderText == "Expires")
            {
                DateTime v1 = DateTime.Parse(val1);
                DateTime v2 = DateTime.Parse(val2);
                result = v1.CompareTo(v2);
            }
            //default
            else
            {
                result = val1.CompareTo(val2);
            }
            e.SortResult = result;
            e.Handled = true;
        }

        private void txtServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' &&  SanitizeServerText(txtServer.Text))
            {
                ChangeServer();
            }
        }
    }
}

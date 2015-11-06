using System;
using System.Management.Automation.Runspaces;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;

namespace DHCPViewer
{
    public class PowerShellDhcpData
    {
        private Runspace _runspace;

        public PowerShellDhcpData()
        {
            _runspace = RunspaceFactory.CreateRunspace();
            _runspace.Open();
        }

        public bool TestForDhcpModule()
        {
            try
            {
                using (var pipeline = _runspace.CreatePipeline())
                {
                    pipeline.Commands.Add("Get-DhcpServerv4Scope");
                    pipeline.Invoke();
                }
            }
            //this WILL fail (missing required parameter).
            // this trick is in which error is in the message
            catch (Exception Ex)
            {
                // TODO: This will fail in other locales, need to get HRESULT value to check
                //      also, this check is slow (requires starting up unnecessary runspace)
                if (Ex.Message.StartsWith("The term 'Get-DhcpServerv4Scope' is not recognized"))
                    return false;
                return true;
            }
            return true; //fallback ... if it somehow succeeds (can happen if run directly on dhcp server), the correct powershell tools obviously do exist
        }

        public IEnumerable<string> GetDhcpScopeList(string Server)
        {
            using (var pipeline = _runspace.CreatePipeline())
            {
                pipeline.Commands.Add("Get-DhcpServerv4Scope");
                pipeline.Commands[0].Parameters.Add("ComputerName", Server);

                return pipeline.Invoke().Select(o => string.Format("{0} - [{1}]", o.Properties["ScopeId"].Value.ToString(), o.Properties["Name"].Value.ToString()));
            }
        }

        public IEnumerable<DchpScopeDetails> GetDhcpScopeListDetails(string Server)
        {
            PSObject[] scopes;
            using (var pipeline = _runspace.CreatePipeline())
            {
                pipeline.Commands.Add("Get-DhcpServerv4Scope");
                pipeline.Commands[0].Parameters.Add("ComputerName", Server);
                scopes = pipeline.Invoke().ToArray();
            }

            //todo: queue all command into one pipeline
            foreach (var scope in scopes)
            {
                using (var pipeline = _runspace.CreatePipeline())
                {
                    pipeline.Commands.Add("Get-DhcpServerv4ScopeStatistics");
                    pipeline.Commands[0].Parameters.Add("ComputerName", Server);
                    pipeline.Commands[0].Parameters.Add("ScopeId", scope.Properties["ScopeId"].Value.ToString());
                    yield return new DchpScopeDetails(scope, pipeline.Invoke()[0]);
                }
            }
        }

        public IEnumerable<string[]> GetDhcpScopeLeases(string Server, string Scope)
        {
            using (var pipeline = _runspace.CreatePipeline())
            {
                pipeline.Commands.Add("Get-DhcpServerv4Lease");
                pipeline.Commands[0].Parameters.Add("ComputerName", Server);
                pipeline.Commands[0].Parameters.Add("ScopeId", Scope);

                return pipeline.Invoke().Select(data => new string[] {
                    data.Properties["IPAddress"].Value?.ToString(),
                    data.Properties["ClientId"].Value?.ToString(),
                    data.Properties["HostName"].Value?.ToString(),
                    data.Properties["LeaseExpiryTime"].Value?.ToString(),
                    data.Properties["AddressState"].Value?.ToString()
                });
            }
        }
    }
}
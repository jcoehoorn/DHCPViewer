using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

public class PowerShellDhcpData
{
    private Runspace _runspace;

	public PowerShellDhcpData()
	{
         _runspace = RunspaceFactory.CreateRunspace();
        _runspace.Open();
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

    private IEnumerable<string[]> GetDhcpScopeLeases(string Server, string Scope)
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

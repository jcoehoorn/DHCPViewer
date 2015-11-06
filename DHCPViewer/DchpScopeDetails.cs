using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Net;

namespace DHCPViewer
{
    public class DchpScopeDetails
    {
        public DchpScopeDetails() { }

        internal DchpScopeDetails(PSObject data, PSObject stats)
        { //data.Properties["IPAddress"].Value?.ToString()
            Name = data.Properties["Name"].Value?.ToString() ?? "";
            State = data.Properties["State"].Value?.ToString() ?? "";
            ScopeId = data.Properties["ScopeId"].Value?.ToString() ?? "";
            SubnetMask = data.Properties["SubnetMask"].Value?.ToString() ?? "";
            StartRange = data.Properties["StartRange"].Value?.ToString() ?? "";
            EndRange = data.Properties["EndRange"].Value?.ToString() ?? "";

            // TODO: :/ No idea how well this will parse.
            LeaseDuration = TimeSpan.Parse(data.Properties["LeaseDuration"].Value?.ToString() ?? "");

            Free = int.Parse(stats.Properties["AddressesFree"].Value?.ToString());
            InUse = int.Parse(stats.Properties["AddressesInUse"].Value?.ToString());
            Reserved = int.Parse(stats.Properties["ReservedAddress"].Value?.ToString());
            Pending = int.Parse(stats.Properties["PendingOffers"].Value?.ToString());
            PercentageInUse = double.Parse(stats.Properties["PercentageInUse"].Value?.ToString());
            SuperscopeName = stats.Properties["SuperscopeName"].Value?.ToString();
        }

        public String Name { get; set; }

        public String State { get; set; }

        public TimeSpan LeaseDuration { get; set; }

        //SCOPE
        public String ScopeId
        {
            get { return _scopeId; }
            set
            {
                _scopeAddress = IPAddress.Parse(value);
                _scopeId = value;
            }
        }
        private string _scopeId;

        public IPAddress ScopeAddress { get { return _scopeAddress; } }
        private IPAddress _scopeAddress;

        //SUBNET
        public String SubnetMask
        {
            get { return _subnetMask; }
            set
            {
                _subnet = IPAddress.Parse(value);
                _subnetMask = value;
            }
        }
        private string _subnetMask;

        public IPAddress Subnet { get { return _subnet; } }
        private IPAddress _subnet;

        //RANGE START
        public String StartRange
        {
            get { return _startRange; }
            set
            {
                _startAddress = IPAddress.Parse(value);
                _startRange = value;
            }
        }
        private string _startRange;

        public IPAddress StartAddress { get { return _startAddress; } }
        private IPAddress _startAddress;

        //RANGE END
        public String EndRange
        {
            get { return _endRange; }
            set
            {
                _endAddress = IPAddress.Parse(value);
                _endRange = value;
            }
        }
        private string _endRange;

        public IPAddress EndAddress { get { return _endAddress; } }
        private IPAddress _endAddress;

        public int Free { get; set; }
        public int InUse { get; set; }
        public int Reserved { get; set; }
        public int Pending { get; set; }
        public string SuperscopeName { get; set; }
        public double PercentageInUse { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - [{1}]", ScopeId, Name);
        }

        public string GetSummary()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Name: {0}", Name);sb.AppendLine();
            sb.AppendFormat("Range: {0} - \t{1}\n", StartRange, EndRange); 
            sb.AppendLine();
            sb.AppendFormat("Subnet: {0}\n", SubnetMask); sb.AppendLine();
            sb.AppendFormat("State: {0}\n", State); sb.AppendLine();
            sb.AppendFormat("Duration: {0}\n", LeaseDuration.ToString()); sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("InUse Free Percent"); 
            sb.AppendLine("----- ---- -------"); 
            sb.AppendFormat("{0,-5} {1,4} {2,7:N2}\n", InUse + Pending + Reserved, Free, PercentageInUse); sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("Superscope: {0}", SuperscopeName);
            return sb.ToString();
        }
    }
}

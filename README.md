# DHCPViewer

The initial Windows 10 release of Remote Server Administration Tools left out the DHCP snap-in, 
leaving PowerShell or RDP as the only mechanism to get information from your Windows DHCP server remotely.

I don't mind using these tools to make the changes (changes are rare), but my day to day work sometimes requires 
quick access to dhcp lease information, where I may want to do things like sort by lease expiration, IP address, or 
MAC address. Powershell and RDP just don't appeal to me for this. RDP to servers should only happen when absolutely necessary, and the command line just isn't as nice of a place to manipulate data.

But am I a programmer or what? So I wrote my own Windows Forms tool to wrap around the Powershell commandlets I need
most, and to provide the sorting and visual display features I'm looking for.

## Requires:

* .Net 4.5
* Remote Server Administration Tools
* Windows 8 or newer (Windows 10 recommended)

Built with Visual Studio 2015 Community

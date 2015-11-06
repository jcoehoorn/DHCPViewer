# DHCPViewer

The Windows 10 release of Remote Server Administration Tools left out the DHCP snap-in, 
leaving PowerShell or RDP as the only mechanism to get information from your Windows DHCP server remotely.

I don't mind using these tools to make the changes (changes are rare), but my day to day work sometimes requires 
quick access to dhcp lease information, where I may want to do things like sort by lease expiration, IP address, or 
MAC address. The cli tools just don't appeal to me for this.

But I am a programmer or what? So I wrote my own Windows Forms tool to wrap around the Powershell commandlets I need
most, and to provide the sorting the visual display features I'm looking for.

##Requires:

* .Net 4.5
* Remote Server Administration Tools
* Windows 8 or newer (Windows 10 recommended)

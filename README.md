# SharpGmailC2
Our Friendly Gmail will act as Server and implant will exfiltrate data via smtp and will read commands from C2 (Gmail) via imap protocol

### Used:
1. `EAGetMail` library from Nuget Package Manager.
2. `Costura` and `Costura Fody` from Nuget Package Manager, in order to bundle up all the dlls altogether.

### C2 In-Action:

https://user-images.githubusercontent.com/61424547/201413790-aa4c9948-d909-45d0-853e-2737e55ae4ef.mp4

### Quick Scan:

1. Using [@matterpreter](https://twitter.com/matterpreter)'s [DefenderCheck](https://github.com/matterpreter/DefenderCheck):

![DefenderCheck](https://github.com/reveng007/SharpGmailC2/blob/main/img/DefenderCheck.PNG)

2. Using [Antiscan.me](https://antiscan.me/):

![AntiScan.me](https://github.com/reveng007/SharpGmailC2/blob/main/img/AntiScan.me.PNG)

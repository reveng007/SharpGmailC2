# SharpGmailC2

Our Friendly Gmail will act as Server and implant will exfiltrate data via smtp and will read commands from C2 (Gmail) via imap protocol

---
> :no_entry_sign: [Disclaimer]: Use of this project is for **Educational/ Testing purposes only**. Using it on **unauthorised machines** is **strictly forbidden**. If somebody is found to use it for **illegal/ malicious intent**, author of the repo will **not** be held responsible.
---

### Used:

1. `EAGetMail` library from Nuget Package Manager.
2. `Costura` and `Costura Fody` from Nuget Package Manager, in order to bundle up all the dlls altogether. This actually bulked up my implant, but for this case, I don't think that will matter much as this implant is FUD till now :).

### Precausions to be taken by Operator before Using Gmail as C2:

1. Make sure the Command sent via Gmail, is in `Unread` Mode (if not, mark as Unread) as the implant scans the `Last/latest Unread` mail and checks whether it starts with "`in:`" or not. If it does start with "`in:`", it understands that, that particular textbody is a legit command, and marks that particular mail as `Read` and this continues till the end.

Here is the snippet:

![latest_unreadMail](https://github.com/reveng007/SharpGmailC2/blob/main/img/latest_unreadMail.PNG)

### C2 In-Action:

https://user-images.githubusercontent.com/61424547/201413790-aa4c9948-d909-45d0-853e-2737e55ae4ef.mp4

### Quick Scan:

1. Using [@matterpreter](https://twitter.com/matterpreter)'s [DefenderCheck](https://github.com/matterpreter/DefenderCheck):

![DefenderCheck](https://github.com/reveng007/SharpGmailC2/blob/main/img/DefenderCheck.PNG)

2. Using [Antiscan.me](https://antiscan.me/):

![AntiScan.me](https://github.com/reveng007/SharpGmailC2/blob/main/img/AntiScan.me.PNG)

3. [Capa](https://github.com/mandiant/capa) Scan:

![capa_scan](https://github.com/reveng007/SharpGmailC2/blob/main/img/capa_scan.PNG)

It seems like **capa** is not able to detect the capabilties of my Client implant at all. But definitely creates suspicion, forcing the Malware Analyst to give the binary a second look.

4. WireShark Packet Capture:

![smtp_capture](https://github.com/reveng007/SharpGmailC2/blob/main/img/smtp_capture.PNG)

We can see that the sent commands via Operator via Gmail and the informations that are exfiltrated/ sent out are all encrypted by Gmail's TLS encryption. On top of that, the ip address (marked) isn't suspicious at all, or in other words are OPSEC safe.

![ip_lookup](https://github.com/reveng007/SharpGmailC2/blob/main/img/ip_lookup.PNG)

### <ins>Credits</ins>:

1. Inspired by [NamedPipes](https://github.com/malcomvetter/NamedPipes) from [malcomvetter](https://www.linkedin.com/in/malcomvetter/).
2. Much much much thanks to [@SoumyadeepBas12](https://twitter.com/SoumyadeepBas12) for helping me out with the proper code structure of this project! :smiley:



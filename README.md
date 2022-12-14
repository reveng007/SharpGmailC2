# SharpGmailC2

Our Friendly Gmail will act as Server and implant will exfiltrate data via smtp and will read commands from C2 (Gmail) via imap protocol

### DISCLAIMER:
> This Project doesn't work against Windows Defender after _29th of November, 2022_. This tool is now signatured by MS Windows as `virtool:msil/ "shgmailz." a!mtb`. However, I do have plan to upgrade this project in near future, to a newer version named, ***SharpGmailC2V2***

---
> :no_entry_sign: [Disclaimer]: Use of this project is for **Educational/ Testing purposes only**. Using it on **unauthorised machines** is **strictly forbidden**. If somebody is found to use it for **illegal/ malicious intent**, author of the repo will **not** be held responsible.
---

### Setup

When setting up the intermediate sender and recipient gmail account(s), enable the `POP Download` and `IMAP Access` by following the steps in this (link)[https://support.cloudhq.net/how-to-check-if-imap-is-enabled-in-gmail-or-google-apps-account/]

Once IMAP and POP are enabled, generate an App Password by following the step in this article [here](https://support.google.com/accounts/answer/185833?hl=en). If `App Password` setting is not visible in `Security`, enable 2FA verification for the Gmail account first.

When compiling the code, update the lines that set `emailToAddress`, `password` and `emailToAddress`. Value for `password` should be set to the `App Password` generated in previous step. Also, note that values for `emailToAddress`, and `emailToAddress` can be the same.

### Used:

1. `EAGetMail` library from Nuget Package Manager.
2. `Costura` and `Costura Fody` from Nuget Package Manager, in order to bundle up all the dlls altogether. This actually bulked up my implant, but for this case, I don't think that will matter much as this implant is FUD till now :).

### Precautions to be taken by Operator before Using Gmail as C2:

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

### Threat Detection

SharpGmailC2 can generate following generic behaviour which can assist defenders to detect `SharpGmailC2` or other processes that leverage Gmail mail protocols for Command and Control:

* Anamlous increase in DNS calls to `imap.google.com` and network connections to other Google domains e.g. `1e100.net.`
```
# Monitor high network connections from a particular processID
Channel=Microsoft-Windows-Sysmon
(EventID=3 OR EventID=22)  (3=Network Connection, 22=DNS)
(DestinationHostname=*.1e100.net OR QueryName=*.gmail.com)
```

* Invocation of `powershell` process from a binary process (`.dll` or `.exe`)
```
Channel=Microsoft-Windows-Sysmon
EventID=1
CommandLine=powershell.exe
(ParentImage=*.exe OR ParentImage=*.dll)
```

### Honourable Mentions:
- Got enlisted in the Golden Source of the C2 Matrix (just underneath SharpC2 by [@_RastaMouse](https://twitter.com/_RastaMouse) and [@_xpn_](https://twitter.com/_xpn_)): [google_Sheet](https://docs.google.com/spreadsheets/d/1b4mUxa6cDQuTV2BPC6aA-GR4zGZi0ooPYtBe4IgPsSc/edit#gid=0).

### <ins>Credits</ins>:

1. Inspired by [NamedPipes](https://github.com/malcomvetter/NamedPipes) from [malcomvetter](https://www.linkedin.com/in/malcomvetter/).
2. Much much much thanks to [@SoumyadeepBas12](https://twitter.com/SoumyadeepBas12) for helping me out with the proper code structure of this project! :smiley:



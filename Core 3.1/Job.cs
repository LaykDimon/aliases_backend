using System;
using System.Globalization;
using System.IO;
using System.Linq;
using EAGetMail;
using EASendMail;

namespace Core_3._1
{
    public class Job
    {
        const string AdminEmail = "layk.taviskaron3@gmail.com";
        const string AdminPwd = "moWTfplRf8x0";
        const string ServerAddress = "imap.gmail.com";
        const string RealAddress = "layk.taviskaron@gmail.com";

        // Generate an unqiue email file name based on date time
        static string _generateFileName(int sequence)
        {
            DateTime currentDateTime = DateTime.Now;
            return string.Format("{0}-{1:000}-{2:000}.eml",
                currentDateTime.ToString("yyyyMMddHHmmss", new CultureInfo("en-US")),
                currentDateTime.Millisecond,
                sequence);
        }


        public static void StartJob(string inputEmail, string inputPassword, string receiverEmail)
        {
            try
            {
                // Create a folder named "inbox" under current directory
                // to save the email retrieved.
                string localInbox = string.Format("{0}\\inbox", Directory.GetCurrentDirectory());
                // If the folder is not existed, create it.
                if (!Directory.Exists(localInbox))
                {
                    Directory.CreateDirectory(localInbox);
                }

                // Gmail IMAP4 server is "imap.gmail.com"
                MailServer oServer = new MailServer("aliases.online",
                                inputEmail ?? AdminEmail,
                                inputPassword ?? AdminPwd,
                                EAGetMail.ServerProtocol.Imap4);

                // Enable SSL connection.
                oServer.SSLConnection = true;

                // Set 993 SSL port
                oServer.Port = 993;

                MailClient oClient = new MailClient("TryIt");
                oClient.Connect(oServer);

                var sentDate = DateTime.Now.AddDays(-40);
                while (true)
                {
                    MailInfo[] infos = oClient.GetMailInfos()
                        .Where(x => !x.Read)
                        .ToArray();

                    Console.WriteLine("Total {0} email(s)\r\n", infos.Length);

                    for (int i = 0; i < infos.Length; i++)
                    {
                        MailInfo info = infos[i];
                        Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
                            info.Index, info.Size, info.UIDL);

                        // Receive email from IMAP4 server
                        Mail oMail = oClient.GetMail(info);
                        if (oMail.SentDate <= sentDate)
                        {
                            continue; // this email was already synchronized
                        }

                        sentDate = oMail.SentDate;

                        SendEmail(oMail, receiverEmail);

                        Console.WriteLine("From: {0}", oMail.From.ToString());
                        Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

                        // Generate an unqiue email file name based on date time.
                        string fileName = _generateFileName(i + 1);
                        string fullPath = string.Format("{0}\\{1}", localInbox, fileName);

                        // Save email to local disk
                        oMail.SaveAs(fullPath, true);

                        // Mark email as deleted from IMAP4 server.
                        oClient.Delete(info);
                    }

                }

                // Quit and expunge emails marked as deleted from IMAP4 server.
                oClient.Quit();
                Console.WriteLine("Completed!");
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }
        }

        private static void SendEmail(Mail inputMail, string receiverEmail)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");

                oMail.From = new EASendMail.MailAddress(inputMail.From.Address)
                {
                    Additional = inputMail.From.Additional,
                    Name = inputMail.From.Name
                };

                // Set recipient email address, please change it to yours
                oMail.To = receiverEmail ?? RealAddress;

                // Set email subject
                oMail.Subject = inputMail.Subject;
                // Set email body
                oMail.TextBody = inputMail.TextBody;

                // SMTP server address
                SmtpServer oServer = new SmtpServer(ServerAddress);

                // User and password for ESMTP authentication
                oServer.User = AdminEmail;
                oServer.Password = AdminPwd;

                // Most mordern SMTP servers require SSL/TLS connection now.
                // ConnectTryTLS means if server supports SSL/TLS, SSL/TLS will be used automatically.
                oServer.ConnectType = SmtpConnectType.ConnectTryTLS;

                // If your SMTP server uses 587 port
                oServer.Port = 587;

                // If your SMTP server requires SSL/TLS connection on 25/587/465 port
                // oServer.Port = 25; // 25 or 587 or 465
                // oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                Console.WriteLine("start to send email ...");

                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);

                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }

    }

}

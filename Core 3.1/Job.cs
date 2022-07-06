using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;

namespace Core_3._1
{
    public class Job
    {
        const string AdminEmail = "admin@aliases.website";
        const string AdminPwd = "admin";
        const string ServerAddress = "mail.aliases.website";
        const string RealAddress = "laykdimon278@gmail.com";

        public static void StartJob(string inputEmail, string inputPassword, string receiverEmail)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    // Note: depending on your server, you might need to connect
                    // on port 993 using SecureSocketOptions.SslOnConnect
                    client.Connect(ServerAddress, 143, SecureSocketOptions.StartTls);

                    // Note: use your real username/password here...
                    client.Authenticate(inputEmail, inputPassword);

                    // open the Inbox folder...
                    client.Inbox.Open(FolderAccess.ReadWrite);

                    // search the folder for new messages (aka recently
                    // delivered messages that have not been read yet)
                    /*var uids = client.Inbox.Search(SearchQuery.New);

                    Console.WriteLine("You have {0} new message(s).", uids.Count);*/

                    // ...but maybe you mean unread messages? if so, use this query

                    var uids = client.Inbox.Search(SearchQuery.NotSeen);

                    foreach (var uid in uids)
                    {
                        var message = client.Inbox.GetMessage(uid);
                        Console.WriteLine(message);
                    }

                    Console.WriteLine("You have {0} unread message(s).", uids.Count);

                    client.Inbox.AddFlags(uids, MessageFlags.Seen, true);

                    Console.WriteLine("Now you have {0} unread message(s).", uids.Count);

                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Connect(ServerAddress, 465, true);
                        smtpClient.Authenticate(inputEmail, inputPassword);

                        var sender = new MailboxAddress(inputEmail, inputEmail);
                        var recipients = new[] { new MailboxAddress(receiverEmail, receiverEmail) };

                        // This version of the Send() method uses the supplied sender and
                        // recipients rather than getting them from the message's headers.

                        foreach (var uid in uids)
                        {
                            var message = client.Inbox.GetMessage(uid);
                            smtpClient.Send(message, sender, recipients);
                        }

                        smtpClient.Disconnect(true);
                    }

                    client.Disconnect(true);

                }


            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }

        }

    }

}

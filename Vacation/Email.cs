using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Vacation
{
    class Email
    {
        private readonly ReadFile rf;
        public string testEmail = "";
        public string emailAPI = "";

        public Email(){
            rf = new ReadFile();
            testEmail = rf.ReadFileCreds("testRecEmail");
            emailAPI = "kjakjsjs";//rf.ReadFileCreds("email-api-key");
        }

        //Console.WriteLine("email: " + testEmail);
        public async Task<bool> SendEmail(string addy, string vacationSpot)
        {
            var apiKey = emailAPI;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("suhcha84@gmail.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress($"{testEmail}", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
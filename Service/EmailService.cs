
using Amazon;
using System;
using System.Collections.Generic;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.Lambda.Core;
using System.Net.Mail;
using EmailService.Model;

namespace EmailService.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailInputs input, ILambdaContext context);
    }

    public class EmailService : IEmailService
    {
        public EmailService()
        {
        }

        /// Send Emails
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> SendEmail(EmailInputs input, ILambdaContext context)
        {
            try
            {
                using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast2))
                {
                    var sendRequest = new SendEmailRequest
                    {
                        Source = input.FromAddress,
                        Destination = new Destination
                        {
                            ToAddresses =
                            new List<string> { input.ToAddress }
                        },
                        Message = new Message
                        {
                            Subject = new Content(input.Subject),
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Data = input.Body
                                },
                                Text = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = input.Body
                                }
                            }
                        },
                        // If you are not using a configuration set, comment
                        // or remove the following line 
                        //ConfigurationSetName = configSet
                    };
                    try
                    {
                        context.Logger.LogLine("Sending email using Amazon SES...");
                        var response = await client.SendEmailAsync(sendRequest);
                        context.Logger.LogLine("The email was sent successfully.");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        context.Logger.LogLine("The email was not sent.");
                        context.Logger.LogLine("Error message: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"There is error in sending SES {ex.Message}");
            }
            return false;
        }

    }
}
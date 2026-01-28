using HAWK.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace HAWK.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CareerController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Apply([FromForm] CareerApplicationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var message = new MimeMessage();

                // The sender must match your mailbox
                message.From.Add(new MailboxAddress("HAWK Careers", "info@hawkalahlia.com"));

                // The receiver is your HR inbox
                message.To.Add(new MailboxAddress("HR Department", "info@hawkalahlia.com"));

                // Subject
                message.Subject = $"New Career Application - {dto.Subject}";

                // Body with applicant info
                var builder = new BodyBuilder
                {
                    TextBody =
        $@"New career application received

Applicant Email: {dto.Email}

Message:
{dto.Body}"
                };

                // Attach CV
                using (var ms = new MemoryStream())
                {
                    await dto.CV.CopyToAsync(ms);
                    builder.Attachments.Add(
                        dto.CV.FileName,
                        ms.ToArray(),
                        ContentType.Parse(dto.CV.ContentType)
                    );
                }

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();

                // Connect using your mail server (SSL/TLS, port 465)
                await client.ConnectAsync("mail.hawkalahlia.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

                // Authenticate using your full email and password
                await client.AuthenticateAsync("info@hawkalahlia.com", "Hawk90001448");

                // Send the email
                await client.SendAsync(message);

                // Disconnect safely
                await client.DisconnectAsync(true);

                return Ok(new { message = "Application sent successfully!" });
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return StatusCode(500, new { message = "Error sending application", error = ex.Message });
            }
        }

    }
}

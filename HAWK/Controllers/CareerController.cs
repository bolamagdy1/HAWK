using HAWK.DTOs;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace HAWK.Controllers
{
    public class CareerController : ControllerBase
    {
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromForm] CareerApplicationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HAWK Website", dto.Email));
            message.To.Add(new MailboxAddress("HR Department", "bolamagdy085@gmail.com"));
            message.Subject = dto.Subject;

            var builder = new BodyBuilder
            {
                TextBody = $"\n{dto.Body}"
            };

            using (var ms = new MemoryStream())
            {
                await dto.CV.CopyToAsync(ms);
                ms.Position = 0;
                builder.Attachments.Add(dto.CV.FileName, ms.ToArray(), ContentType.Parse(dto.CV.ContentType));
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("bolamagdy085@gmail.com", "wqqe owdp awpt jotf");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return Ok(new { message = "Application sent successfully!" });
        }
    }
}

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using RamQuim.Models;

namespace RamQuim.Services
{
    public class EmailService
    {
        public async Task EnviarCorreoAsync(ContactoDto dto)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("mrcookiesoftware@gmail.com"));
            email.To.Add(MailboxAddress.Parse("mrcookiesoftware@gmail.com"));
            email.Subject = $"Nuevo mensaje: {dto.Asunto}";

            email.Body = new TextPart("plain")
            {
                Text = $@"
Nombre: {dto.Nombre}
Empresa: {dto.Empresa}
Correo: {dto.Correo}
Teléfono: {dto.Telefono}

Mensaje:
{dto.Mensaje}
"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("mrcookiesoftware@gmail.com", "ltpbgcwchapmbipu");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
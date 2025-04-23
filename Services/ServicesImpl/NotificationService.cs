using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Net.Mail;

namespace Services.ServicesImpl
{
    public class NotificationService : INotificationService
    {
        private readonly IMapper _autoMapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITokenService _tokenService;
        //private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public NotificationService(IUnitOfWorkFactory unitOfWorkFactory,
             IMapper autoMapper,
             IConfiguration configuration,
             ITokenService tokenService
           )
        {
            _autoMapper = autoMapper;
            _unitOfWorkFactory = unitOfWorkFactory;
            _configuration = configuration;
            _tokenService = tokenService;
        }


        public async void SendEmail()
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Admin",
            "admin@example.com");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("Hyder Ali",
            "ToAddress@gmail.com");
            message.To.Add(to);

            message.Subject = "New Booking";
            List<string> servicesList = new List<string>();//booking.BookingService.Select(x => x.Service?.Name).ToList();
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody =$"<h4>User:</h4><br></br>" +
                 $"<h4>Time:</h4><br></br>" +
                $"<h4>Services:</h4>{string.Join(",", servicesList)}<br></br>";
            //bodyBuilder.TextBody = "Hello World!";

            SmtpClient client = new SmtpClient();
            //client.Connect("smtp.gmail.com", 465, true);
            //client.Authenticate("FromAddress@gmail.com", "abcd1234");

            //message.Body = bodyBuilder.ToMessageBody();

            //client.Send(message);
            //client.Disconnect(true);
            client.Dispose();
        }
    }
}

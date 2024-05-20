using MiHairCareApp.Domain.Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IEmailServices
    {
        Task<string> SendEmailAsync(string link, string email, string id);

        Task SendMailAsync(MailRequest mailRequest); 
    }
}

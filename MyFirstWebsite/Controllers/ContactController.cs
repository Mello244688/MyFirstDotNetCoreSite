using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyFirstWebsite.Models;
using MyFirstWebsite.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFirstWebsite.Controllers
{
    public class ContactController : Controller
    {
        private IEmailService emailService;

        public ContactController(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                Task.Run(() => 
                {
                    var message = "Hello " + contact.Name + ",\n\n" + "I have received your message. I will respond shortly. \n\nBest regards,\n\nScott Mello";

                    emailService.SendEmail("scott.mello24@gmail.com", "From: " + contact.Name, "From: " + contact.Name + "\nEmail: " + contact.Email + "\n\n" + contact.Message);
                    emailService.SendEmail(contact.Email, "Message Confirmation", message);
                });

                return View("MessageComplete", contact);
            }

            return View(contact);
            
        }
    }
}

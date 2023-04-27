﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MissionApp.DataAccess.GenericRepository.Interface;
using MissionApp.Entities.Data;
using MissionApp.Entities.Models;
using MissionApp.Entities.ViewModels;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace MissionApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //---------------------------------------- Login -------------------------------------------------//
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM loginVM)
        {
            var userStatus = _unitOfWork.User.GetFirstOrDefault(u => u.Email == loginVM.Email && u.Password == loginVM.Password);
            if (userStatus == null)
            {
                ModelState.AddModelError("Password", "User is not Found... You have to Registration First");
            }
            else
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, userStatus.Email) }, CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, userStatus.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, userStatus.LastName));
                identity.AddClaim(new Claim(ClaimTypes.Email, userStatus.Email));

                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetString("Email", userStatus.Email);

                TempData["Success"] = "Login is Successful...";
                return RedirectToAction("PlatformLandingPage", "Mission");
            }
            return View(loginVM);
        }
        //-------------------------------------------- Log Out --------------------------------------------//

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("PlatformLandingPage", "Mission");
        }

        //--------------------------------------- Registration --------------------------------------------//
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(RegistrationVM registrationVM)
        {

            var user = new User
            {
                FirstName = registrationVM.FirstName,
                LastName = registrationVM.LastName,
                Email = registrationVM.Email,
                PhoneNumber = registrationVM.PhoneNumber,
                Password = registrationVM.Password,
                CityId = 18,
                CountryId = 7
            };
            var EmailFinder = _unitOfWork.User.GetFirstOrDefault(u => u.Email == registrationVM.Email);
            if (EmailFinder == null)
            {
                _unitOfWork.User.Add(user);
                _unitOfWork.Save();
                TempData["Success"] = "Registration is Completed...";
                return RedirectToAction(nameof(Login));
            }
            else
            {
                ModelState.AddModelError("Email", "Email Already Exist...");
                return View(registrationVM);
            }
        }
        //------------------------------------- ForgotPassword -------------------------------------------//
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            var emailChecker = _unitOfWork.User.GetFirstOrDefault(u => u.Email == forgotPasswordVM.Email);

            if (emailChecker == null)
            {
                ModelState.AddModelError("Email", "Email Not found... You have to Registration First");
                return View(forgotPasswordVM);
            }
            else
            {
                // var oldEmail = _context.PasswordResets.FirstOrDefault(u => u.Email == forgotPasswordVM.Email);


                //if(oldEmail != null)
                //{
                //    var oldToken = _context.PasswordResets.FirstOrDefault(u => u.Token == oldEmail.Token);
                //    var oldPasswordReset = new PasswordReset
                //    {
                //        Email = oldEmail.ToString()
                //    }; 
                //    _context.PasswordResets.Remove(oldPasswordReset);
                //    _context.SaveChanges();
                //}
                var token = Guid.NewGuid().ToString();

                var newPasswordReset = new PasswordReset
                {
                    Email = forgotPasswordVM.Email,
                    Token = token,
                };
                _unitOfWork.PasswordReset.Add(newPasswordReset);
                _unitOfWork.Save();


                var resetLink = Url.Action("ResetPassword", "Auth", new { email = forgotPasswordVM.Email, token }, Request.Scheme);
                var fromAddress = new MailAddress("roanrush158@gmail.com", "Mission App");
                var toAddress = new MailAddress(forgotPasswordVM.Email);
                var subject = "Password Reset Request";
                var body = $"Hi,<br /><br />Please click on the following link to reset your password:<br /><br /><a href='{resetLink}'>{resetLink}</a>";

                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                var smtpclient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("roanrush158@gmail.com", "hcsuogfokidvirmz"),
                    EnableSsl = true
                };
                smtpclient.Send(message);
                TempData["Success"] = "Reset Password Link has been sent to Your registered Email Address...";

                return RedirectToAction(nameof(Login));
            }
        }
        //----------------------------------------- ResetPassword ---------------------------------------//
        public IActionResult ResetPassword(string email, string token)
        {
            //var checkData = _context.PasswordResets.FirstOrDefault(u => u.Email == email && u.Token == token);
            //if(checkData == null)
            //{
            //    return RedirectToAction(nameof(Privacy));
            //}
            var resetPassword = new ResetPasswordVM
            {
                Email = email,
                Token = token
            };
            return View(resetPassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Resetpassword(ResetPasswordVM resetPasswordVM)
        {
            var userEmail = _unitOfWork.User.GetFirstOrDefault(u => u.Email == resetPasswordVM.Email);
            //if (userEmail == null)
            //{
            //    RedirectToAction(nameof(ForgotPassword));
            //    TempData["Error"] = "Email is not registered...";
            //}
            userEmail.Password = resetPasswordVM.Password;
            _unitOfWork.User.Update(userEmail);
            _unitOfWork.Save();

            var oldPasswordReset = _unitOfWork.PasswordReset.GetFirstOrDefault(u => u.Token == resetPasswordVM.Token && u.Email == resetPasswordVM.Email);
            if (oldPasswordReset != null)
            {
                _unitOfWork.PasswordReset.Remove(oldPasswordReset);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Login));
        }
        //--------------------------------------- Useless Views ----------------------------------------//
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
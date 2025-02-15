﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using School_Register.Data.Models;
using School_Register.Data.Repositories;
using School_Register.Services.AccountServices;
using School_Register.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace School_Register.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Account> accRepo;
        private readonly IAccountService accService;

        public HomeController(
            IRepository<Account> accRepo,
            IAccountService accService
            )
        {
            this.accRepo = accRepo;
            this.accService = accService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            var model = new AccountViewModel();
            return this.View(model);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (this.accService.CheckIfAccountExists(username))
            {
                if(this.accService.isPasswordCorrect(username, password))
                {
                    this.HttpContext.Session.SetString("username", username);
                    return this.RedirectToAction("Index", "Home");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Грешно потребителското име или парола.");

                    return this.View();
                }
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "Грешно потребителското име или парола.");

                return this.View();
            }
        }

        public IActionResult Logout()
        {
            this.HttpContext.Session.Remove("username");
            return this.RedirectToAction("Login", "Home");
        }

        public IActionResult Register()
        {
            var model = new AccountViewModel();
            return this.View(model);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(AccountViewModel acc)
        {
            if (!this.accService.CheckIfAccountExists(acc.Username))
            {
                var newAcc = new Account()
                {
                    Username = acc.Username,
                    Password = acc.Password,
                    ConfirmPassword = acc.ConfirmPassword,
                    ClassNumber = acc.ClassNumber,
                    Email = acc.Email,
                    FirstName = acc.FirstName,
                    LastName = acc.LastName,
                    Grade = acc.Grade,
                    StudentNumber = acc.StudentNumber,
                };

                await this.accRepo.AddAsync(newAcc);
                await this.accRepo.SaveChangesAsync();
                this.HttpContext.Session.SetString("username", newAcc.Username);
                return this.RedirectToAction("Index", "Home");
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "Потребителското име вече е заето");

                return this.View();
            }
        }
    }
}

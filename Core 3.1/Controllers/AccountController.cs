using System;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLE_System.Models;
using System.Diagnostics;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;
using Npgsql;
using Core_3._1.Models;
using System.Security.Claims;
using System.Collections.Generic;

namespace Core_3._1.Controllers
{
    public class AccountController : Controller
    {

        static List<String> guidsList = new List<String>() { "55bb6329-af78-4f04-8606-a42b95d205f4", "fa14be36-d457-44d7-9f83-422c2486719d", "c65c5bdb-6031-4873-a6bd-6e228f18b61e", "7141f5e4-aaa8-4312-b9b6-d6ec501f448e",
            "23cc0803-2847-4790-b71b-b87d5a9bc998"};
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = AesOperation.EncryptString(model.Email),
                    Alias = guidsList[0],
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    guidsList.RemoveAt(0);
                    //var mailuNewEmail = Guid.NewGuid().ToString();
                    //var neededUser = await _userManager.GetUserAsync(HttpContext.User);
                    var userAlias = user.Alias;
                    var newEmail = userAlias + "@aliases.website";
                    var mailPassword = "admin";
                    //var path = Directory.GetCurrentDirectory() + "\\shell\\mailuScript.sh";
                    var path = "/backend/shell/mailuScript.sh";

                    //var logFileName = Directory.GetCurrentDirectory() + "\\log.txt";

                    var logFileName = Directory.GetCurrentDirectory() + "/log.txt";
                    using (StreamWriter logFile = new StreamWriter(logFileName))
                    {
                        logFile.WriteLine(path);
                        logFile.WriteLine(userAlias);
                        logFile.WriteLine(model.Password);
                    }

                    //mailu registration
                    /*Process process = new Process();

                    var command = "sh";
                    var args = "cd /mailu && docker-compose exec admin flask mailu user " + userAlias + " aliases.website" + " admin";

                    var processInfo = new ProcessStartInfo();
                    processInfo.UseShellExecute = false;
                    processInfo.FileName = command;   // 'sh' for bash   
                    processInfo.Arguments = args;    // The Script name */

                    /*process = Process.Start(processInfo);   // Start that process.
                    process.WaitForExit();*/

                    /* Process process = new Process
                     {
                         StartInfo = new ProcessStartInfo
                         {
                             FileName = "zsh",
                             RedirectStandardInput = true,
                             RedirectStandardOutput = true,
                             RedirectStandardError = true,
                             UseShellExecute = false
                         }
                     };
                     process.Start();
                     var args = "cd /mailu && docker-compose exec admin flask mailu user " + userId + " aliases.online " + model.Password;
                     await process.StandardInput.WriteLineAsync(args);*/


                    //Job.StartJob(newEmail, model.Password, model.Email);

                    //test case
                    //Job.StartJob(newEmail, mailPassword, user.Email);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {

            //var branch = new Branch
            //{
            //    branchName = "Regie",
            //    address = "Naval"

            //};
            //branchContext.Branch.Add(branch);
            //branchContext.SaveChanges();

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    //Job.StartJob("833285df-e60d-45ab-8272-b94bb21ba430@aliases.online", "*8HXye85sMe$!P4", "laykdimon278@gmail.com");
                    
                    //test case
                    
                    /*var currentUser = await _userManager.FindByEmailAsync(user.Email);
                    var emailSent = currentUser.Id + "@aliases.website";
                    var emailReceived = user.Email;
                    var mailPassword = "admin";

                    Job.StartJob(emailSent, mailPassword, emailReceived);*/
                    //Job.StartJob("laykdimon27890@gmail.com", "DmDmBBGmA7GmGo", "laykdimon278@gmail.com");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded) {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Login");
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", _userManager.Users);
        }
    }
}

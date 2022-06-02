using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Riode.WebUI.AppCode.Extensions;
using Riode.WebUI.Models.Entities.Membership;
using Riode.WebUI.Models.FormModel;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<RiodeUser> signInManager;
        private readonly UserManager<RiodeUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IActionContextAccessor ctx;

        public AccountController(SignInManager<RiodeUser> signInManager, 
            UserManager<RiodeUser> userManager, 
            IConfiguration configuration,
            IActionContextAccessor ctx)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.ctx = ctx;
        }
        public IActionResult Profile()
        {
            return View();
        }
        [Route("/logout.html")]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Wishlist()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("/signin.html")]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/signin.html")]
        public async Task<IActionResult> SignIn(LoginFormModel user)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    RiodeUser foundUser = null;
                    if (user.UserName.IsMail())
                    {
                        foundUser = await userManager.FindByEmailAsync(user.UserName);
                    }
                    else
                    {
                        foundUser = await userManager.FindByNameAsync(user.UserName);
                    }

                    if (foundUser == null)
                    {
                        ViewBag.Message = "Username or password is not correct!";
                        goto end;
                    }

                    var signInResult = await signInManager.PasswordSignInAsync(foundUser, user.Password, true, true);

                    if (!signInResult.Succeeded)
                    {
                        ViewBag.Message = "Username or password is not correct!";
                        goto end;
                    }

                    var callBackUrl = Request.Query["ReturnUrl"];
                    if (!string.IsNullOrEmpty(callBackUrl))
                    {
                        return Redirect(callBackUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (System.Exception ex)
            {

                throw;
            }
        end:
            return View(user);
        }
        [AllowAnonymous]
        [Route("/register.html")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/register.html")]
        public async Task<IActionResult> Register(RegisterFormModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new RiodeUser();

                user.Email = model.Email;
                user.UserName = model.Email;
                user.Name = model.Name;
                user.Surname = model.Surname;
                //user.EmailConfirmed = true;

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    string link = $"{ctx.GetAppLink()}/registration-confirm.html?email={user.Email}&token={token}";

                    var emailSuccess = configuration.SendEmail(
                                            user.Email,
                                            "Riode registration confirmation",
                                            $"Please, <a href=\"{link}\"> click here </a> to confirm your registration.");
                    if (emailSuccess)
                    {
                        ViewBag.Message = "Congratulations! Your registration is complited.";
                    }
                    else
                    {
                        ViewBag.Message = "Unsuccessful. Please, try again!";
                    }

                    return RedirectToAction(nameof(SignIn));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        [Route("/registration-confirm.html")]
        public async Task<IActionResult> RegisterConfirm(string email, string token)
        {
            var foundUser = await userManager.FindByEmailAsync(email);
            if(foundUser == null)
            {
                ViewBag.Message = "Invalid Token!";
                goto end;
            }
            token = token.Replace(" ", "+");
            var result = await userManager.ConfirmEmailAsync(foundUser, token);
            if (!result.Succeeded)
            {
                ViewBag.Message = "Invalid Token!";
                goto end;
            }
            ViewBag.Message = "Your account has confirmed.";
            end:
            return RedirectToAction(nameof(SignIn));

        }
        [AllowAnonymous]
        [Route("/accessdenied.html")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

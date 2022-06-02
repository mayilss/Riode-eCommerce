using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.DataContexts;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Riode.WebUI.Models.Entities.Membership;
using Riode.WebUI.AppCode.Extensions;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly RiodeDbContext db;

        public UsersController(RiodeDbContext db)
        {
            this.db = db;
        }
        [Authorize(Policy = "admin.user.index")]
        public async Task<IActionResult> Index()
        {
            var data = await db.Users.ToListAsync();
            return View(data);
        }
        [Authorize(Policy = "admin.user.details")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                return NotFound();
            }
            ViewBag.Roles = await (from r in db.Roles
                                   join ur in db.UserRoles on
                                   new { RoleId = r.Id, UserId = user.Id } equals new { ur.RoleId, ur.UserId } into lJoin
                                   from lj in lJoin.DefaultIfEmpty()
                                   select Tuple.Create(r.Id, r.Name, lj != null)).ToListAsync();

            ViewBag.Principals = (from p in Program.principals
                                  join uc in db.UserClaims on
                                  new { ClaimValue = "1", ClaimType = "p", UserId = user.Id } equals
                                  new { uc.ClaimValue, uc.ClaimType, uc.UserId } into lJoin
                                  from lj in lJoin.DefaultIfEmpty()
                                  select Tuple.Create(p, lj != null)).ToList();

            return View(user);
        }
        [HttpPost]
        [Route("/user-set-role")]
        [Authorize(Policy = "admin.user.setrole")]
        public async Task<IActionResult> SetRole(int userId, int roleId, bool selected)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(user == null)
            {
                return Json(new
                {
                    error = true,
                    message = "Invalid request!"
                });
            }

            if (userId == User.GetCurrentUserId())
            {
                return Json(new
                {
                    error = true,
                    message = "Users can not add themselves to a role!"
                });
            }

            var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return Json(new
                {
                    error = true,
                    message = "Invalid request!"
                });
            }

            if (selected)
            {
                if(await db.UserRoles.AnyAsync(ur=>ur.UserId == userId && ur.RoleId == roleId))
                {
                    return Json(new
                    {
                        error = true,
                        message = $"'{user.Name} {user.Surname}' is already in the '{role.Name}' role!"
                    });
                }
                else
                {
                    db.UserRoles.Add(new RiodeUserRole
                    {
                        UserId = userId,
                        RoleId = roleId
                    });
                    await db.SaveChangesAsync();
                    return Json(new
                    {
                        error = false,
                        message = $"'{user.Name} {user.Surname}' added to the '{role.Name}' role."
                    });
                }
            }
            else
            {
                var userRole = await db.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
                if (userRole == null)
                {
                    return Json(new
                    {
                        error = true,
                        message = $"'{user.Name} {user.Surname}' is not in the '{role.Name}' role!"
                    });
                }
                else
                {
                    db.UserRoles.Remove(userRole);
                    await db.SaveChangesAsync();
                    return Json(new
                    {
                        error = false,
                        message = $"'{user.Name} {user.Surname}' removed from the '{role.Name}' role."
                    });
                }
            }
        }
        [HttpPost]
        [Route("/user-set-principal")]
        [Authorize(Policy = "admin.user.setprincipal")]
        public async Task<IActionResult> SetPrincipal(int userId, string principalName, bool selected)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Json(new
                {
                    error = true,
                    message = "Invalid request!"
                });
            }

            if (userId == User.GetCurrentUserId())
            {
                return Json(new
                {
                    error = true,
                    message = "Users can not add themselves to a claim!"
                });
            }

            var hasPrincipal = Program.principals.Contains(principalName);
            if (!hasPrincipal)
            {
                return Json(new
                {
                    error = true,
                    message = "Invalid request!"
                });
            }

            if (selected)
            {
                if (await db.UserClaims.AnyAsync(uc => uc.UserId == userId && uc.ClaimType.Equals(principalName) && uc.ClaimValue.Equals("1")))
                {
                    return Json(new
                    {
                        error = true,
                        message = $"'{user.Name} {user.Surname}' is already has the '{principalName}' claim!"
                    });
                }
                else
                {
                    db.UserClaims.Add(new RiodeUserClaim
                    {
                        UserId = userId,
                        ClaimType = principalName,
                        ClaimValue = "1"
                    });
                    await db.SaveChangesAsync();
                    return Json(new
                    {
                        error = false,
                        message = $"'{user.Name} {user.Surname}' added to the '{principalName}' claim."
                    });
                }
            }
            else
            {
                var userClaim = await db.UserClaims.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ClaimType.Equals(principalName) && uc.ClaimValue.Equals("1"));
                if (userClaim == null)
                {
                    return Json(new
                    {
                        error = true,
                        message = $"'{user.Name} {user.Surname}' does not have the '{principalName}' claim!"
                    });
                }
                else
                {
                    db.UserClaims.Remove(userClaim);
                    await db.SaveChangesAsync();
                    return Json(new
                    {
                        error = false,
                        message = $"'{user.Name} {user.Surname}' removed from the '{principalName}' claim."
                    });
                }
            }
        }

    }
}

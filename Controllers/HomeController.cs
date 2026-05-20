using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using TrustPlus.Models;

namespace TrustPlus.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbManager _db;

        public HomeController(DbManager db)
        {
            _db = db;
            var conn = _db.Database.GetDbConnection().ConnectionString;
            Console.WriteLine("EF Core Connection String: " + conn);
        }

        public IActionResult Index()
        {
            return View();
        }


        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Index(LoginModel model)
        //    {
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    using (SqlConnection con = new SqlConnection(_connStr))
        //    {
        //        string query = "SELECT RoleId, Email, Password, role_mst.Role as Role FROM User_mst  left join role_mst on role_mst.RoleId=User_mst.Role WHERE Email=@Email AND User_mst.Role=@Role";

        //        SqlCommand cmd = new SqlCommand(query, con);
        //        cmd.Parameters.AddWithValue("@Email", model.Email);
        //        cmd.Parameters.AddWithValue("@Role", model.RoleId);

        //        con.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            string dbPassword = reader["Password"].ToString();


        //            if (model.Password == dbPassword)
        //            {
        //                var claims = new List<Claim>
        //            {
        //                new Claim(ClaimTypes.Name, reader["Email"].ToString()),
        //                new Claim(ClaimTypes.Role, reader["Role"].ToString()),

        //            };

        //                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        //                var principal = new ClaimsPrincipal(identity);

        //                await HttpContext.SignInAsync("MyCookieAuth", principal);

        //                return RedirectToAction("Dashboard", "Admin");
        //            }
        //        }
        //    }

        //    ModelState.AddModelError("", "Invalid email or password");
        //    return View(model);
        //}
        
        [HttpPost]
        public async Task<JsonResult> Login(LoginModel model)
        {

            var user = _db.User_Master.FirstOrDefault(x =>
                x.EmailAddress.ToLower() == model.EmailAddress.ToLower());
                //&&
                //x.Password == model.Password);

            string roleName = "Unknown";
            string sessionUserId = "";
            string sessionUserName = "";
            Client_Master clientMaster = null;


            if (user == null )
            {
                clientMaster = await _db.Client_Master.FirstOrDefaultAsync(x =>
            x.CUserId.ToLower() == model.EmailAddress.ToLower()); //&&
            //x.CPassword == model.Password);

                if (clientMaster != null && BCrypt.Net.BCrypt.Verify(model.Password, clientMaster.CPassword))
                {
                    roleName = "Client"; // Client table se aaya hai toh Role default Client hoga

                    // Data ready for Session (Client_Master ke columns ke mutabik)
                    sessionUserId = clientMaster.ClientId.ToString();
                    sessionUserName = clientMaster.ClientName;
                }
            }
           ;

            if (user != null || clientMaster != null )
            {
                //  Role mapping (3 roles)
                if (roleName == "Unknown" && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    roleName = user.Role == 1 ? "Admin"
                               : user.Role == 2 ? "Client"
                               : user.Role == 3 ? "Executor"
                               : user.Role == 4 ? "Executor"
                               : "Unknown";

                    sessionUserId = user.UserId.ToString();
                    sessionUserName = user.UserName;


                    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.EmailAddress),
            new Claim(ClaimTypes.Role, roleName)
        };


                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetString("UserId", sessionUserId);
                    HttpContext.Session.SetString("UserName", sessionUserName);
                    HttpContext.Session.SetString("Role", roleName);

                    return Json(new { success = true, role = roleName });
                }






                var claimss = new List<Claim>
        {
            new Claim(ClaimTypes.Name, clientMaster.CUserId),
            new Claim(ClaimTypes.Role, roleName)
        };


                var identityy = new ClaimsIdentity(claimss, CookieAuthenticationDefaults.AuthenticationScheme);
                var principall = new ClaimsPrincipal(identityy);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principall);

                HttpContext.Session.SetString("UserId", sessionUserId);
                HttpContext.Session.SetString("UserName", sessionUserName);
                HttpContext.Session.SetString("Role", roleName);

                return Json(new { success = true, role = roleName });
            }

            return Json(new { success = false });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

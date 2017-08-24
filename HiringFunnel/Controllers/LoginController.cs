using HiringFunnel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HiringFunnel.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            if(Session["User"]!=null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Email,Password")] LoginViewModel login)

        {
           
            if (ModelState.IsValid)
            {
                SessionLogin log = DataProvider.Dp.Login(login);
                if (log != null)
                {
                    Session["User"] = log;
                    if (log.Role == UserLevel.Interviewer)
                    {
                        return RedirectToAction("Interviewer", "Process");
                    }
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Error = "pogresan Mail ili sifra";
                return View();
            }

            return View();
        }


        public ActionResult LogOut()
        {
            if (Session["User"] != null)
            {
                Session["User"] = null;

            }
            return RedirectToAction("Index", "Login");
        }
    }
}
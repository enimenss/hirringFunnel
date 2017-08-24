using HiringFunnel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HiringFunnel.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();

            TempData["Techno"] = teh;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,FirstName,LastName,Seniority,Role,Email,Password")] RegisterUser NewUser, FormCollection param)
        {
            if (ModelState.IsValid)
            {
                if (!DataProvider.Dp.CheckMail(NewUser.Email))
                {
                    ViewBag.Error = "Postoji korisnik sa tim mailom";
                    List<TechnologyView> tehs = DataProvider.Dp.GetAllTechnologies();
                    TempData["Techno"] = tehs;
                    return View();
                }
                string[] tech = param["techno"].Split(',');
                foreach (string t in tech)
                {
                    int TechId = Int32.Parse(t);
                    NewUser.Technologies.Add(new TechnologyView() { Id = TechId });
                }
                DataProvider.Dp.Register(NewUser);


                return RedirectToAction("UserManagement", "Home");
            }

            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            return View();

        }
    }
    }

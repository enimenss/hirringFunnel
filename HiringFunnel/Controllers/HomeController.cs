using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using HiringFunnel.DTO;
using System.IO;

namespace HiringFunnel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            return View();
        }

        public ActionResult LoadContactData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            var FirstName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var LastName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Seniority = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var Technologies = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var v = DataProvider.Dp.GetAllContacts();

            if (!string.IsNullOrEmpty(FirstName))
            {
                v = v.Where(a => a.FirstName.Contains(FirstName)).ToList();
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                v = v.Where(a => a.LastName.Contains(LastName)).ToList();
            }

            if (!string.IsNullOrEmpty(Seniority))
            {
                v = v.Where(a => a.Seniority.ToString().Contains(Seniority)).ToList();
            }

            if (!string.IsNullOrEmpty(Technologies))
            {
                foreach (ContactView e in v.ToList())
                {
                    List<TechnologyView> l = new List<TechnologyView>();
                    l = e.Technologies.Where(s => s.Name.Contains(Technologies)).ToList();
                    if (l.Count <= 0)
                    {
                        v.Remove(e);
                    }
                };
            }

            v = v.OrderBy(x => x.Id).ToList();
            recordsTotal = v.Count();
            var data = (from n in v select new {
                Id = n.Id,
                FirstName = n.FirstName,
                LastName = n.LastName,
                Email = n.Email,
                Phone = n.Phone,
                Seniority = n.Seniority.ToString(),
                Technologies = (from tech in n.Technologies select " " + tech.Name)

            }).Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadUserData()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            var FirstName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var LastName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Seniority = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var Technologies = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var v = DataProvider.Dp.GetAllUsers();

            if (!string.IsNullOrEmpty(FirstName))
            {
                v = v.Where(a => a.FirstName.Contains(FirstName)).ToList();
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                v = v.Where(a => a.LastName.Contains(LastName)).ToList();
            }

            if (!string.IsNullOrEmpty(Seniority))
            {
                v = v.Where(a => a.Seniority.ToString().Contains(Seniority)).ToList();
            }

            if (!string.IsNullOrEmpty(Technologies))
            {
                foreach (RegisterUser e in v.ToList())
                {
                    List<TechnologyView> l = new List<TechnologyView>();
                    l = e.Technologies.Where(s => s.Name.Contains(Technologies)).ToList();
                    if (l.Count <= 0)
                    {
                        v.Remove(e);
                    }
                };
            }

            v = v.OrderBy(x => x.Id).ToList();
            recordsTotal = v.Count();
            var data = (from n in v
                        select new
                        {
                            Id = n.Id,
                            FirstName = n.FirstName,
                            LastName = n.LastName,
                            Seniority = n.Seniority.ToString(),
                            Email = n.Email,
                            Role=n.Role.ToString(),
                            
                            Technologies = (from tech in n.Technologies select " " + tech.Name)

                        }).Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UserManagement()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            return View(new RegisterUser());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserManagement([Bind(Include = "Id,FirstName,LastName,Seniority,Role,Email,Password")] RegisterUser NewUser, FormCollection param)
        {
            if (ModelState.IsValid)
            {
                if (param["UserId"] != null)
                {
                    int id = Int32.Parse(param["UserId"]);
                    DataProvider.Dp.DeleteUser(id);
                    return RedirectToAction("UserManagement", "Home");
                }
                
                if ( NewUser.Id==0 && !DataProvider.Dp.CheckMail(NewUser.Email))
                {
                    ViewBag.Error = "Postoji korisnik sa tim mailom";
                    
                    return RedirectToAction("UserManagement", "Home");
                }
                if (param["techno"] != null)
                {
                    string[] tech = param["techno"].Split(',');
                    foreach (string t in tech)
                    {
                        int TechId = Int32.Parse(t);
                        NewUser.Technologies.Add(new TechnologyView() { Id = TechId });
                    }
                }
                DataProvider.Dp.Register(NewUser);


               
            }

            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            ViewBag.Error = true;
            return View(NewUser);

        }


        [HttpPost]
        public JsonResult DeleteUser(int Id)
        {
            
                DataProvider.Dp.DeleteUser(Id);
            
                return Json(new { data = true });
            
        }


        public ActionResult ContactManagement()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            return View();

        }

        [HttpPost]
        public JsonResult DeleteContact(int Id)
        {
            
                DataProvider.Dp.DeleteContact(Id);
            
            return Json(new { data = true });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactManagement([Bind(Include = "FirstName,LastName,Phone,Seniority,LinkedIn,Location,DateOfBirth,Email,CV")] ContactView NewContact, FormCollection param)
        {
            if (ModelState.IsValid)
            {

                if (param["techno"] != null)
                {
                    string[] tech = param["techno"].Split(',');
                    foreach (string t in tech)
                    {
                        int TechId = Int32.Parse(t);
                        NewContact.Technologies.Add(new TechnologyView() { Id = TechId });
                    }
                }
                if (Request.Files["CV"] != null && Request.Files["CV"].ContentLength > 0)
                    using (BinaryReader br = new BinaryReader(Request.Files["CV"].InputStream))
                    {
                        NewContact.CVContent = br.ReadBytes(Request.Files["CV"].ContentLength);
                        NewContact.CVContentType = Request.Files["CV"].ContentType;
                    }
                DataProvider.Dp.AddNewContact(NewContact);

            }

            

            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            ViewBag.Error = true;
            return View(NewContact);

        }

        [HttpPost]
        public JsonResult GetAllNotContactProcess(int ContactId)
        {
            List<ProcessView> lista = DataProvider.Dp.GetAllNotContactProcess(ContactId);

            var data = (from p in lista select new { Id = p.Id, Name = p.Name }).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult LoadContactHistory()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            var Identity = Request.Form.GetValues("Id").FirstOrDefault();
            int Id = Convert.ToInt32(Identity); //provera 


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var v = DataProvider.Dp.LoadContactHistory(Id);


            v = v.OrderByDescending(x => x.StartDate).ToList();
            recordsTotal = v.Count();
            var data = (from d in v
                        select new
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Technologies = d.Technologies,
                            Seniority = d.Seniority.ToString(),
                            StartDate = d.StartDate.Year.ToString(),
                            pInsContact = d.pInsContact,
                            isEnd = (d.EndDate != null)?true:false /*DataProvider.Dp.IsEndProcessForContact(d.pInsContact)*/
                        }).Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

        }

    }
}
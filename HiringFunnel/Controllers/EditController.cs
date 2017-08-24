using HiringFunnel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HiringFunnel.Models;
using System.IO;

namespace HiringFunnel.Controllers
{
    public class EditController : Controller
    {
        // GET: Edit
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        public ActionResult Contact(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            ContactEditView cont = DataProvider.Dp.GetContactEditView( id);
            return View(cont);
        }


        public ActionResult Info (int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            ContactEditView cont = DataProvider.Dp.GetContactEditView(id);
            return View(cont);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info([Bind(Include = "Id,FirstName,LastName,Phone,Seniority,LinkedIn,Location,DateOfBirth,Email,CV")] ContactView NewContact, FormCollection param)
        {

            string[] tech = param["techno"].Split(',');
            foreach (string t in tech)
            {
                int TechId = Int32.Parse(t);
                NewContact.Technologies.Add(new TechnologyView() { Id = TechId });
            }

            if (Request.Files["CV"] != null && Request.Files["CV"].ContentLength>0)
                using (BinaryReader br = new BinaryReader(Request.Files["CV"].InputStream))
                {
                    NewContact.CVContent = br.ReadBytes(Request.Files["CV"].ContentLength);
                    NewContact.CVContentType = Request.Files["CV"].ContentType;
                }
            DataProvider.Dp.UpdateContact(NewContact);
            return RedirectToAction("Info", "Edit", new { id = NewContact.Id });
        }

        [HttpPost]
        public ActionResult AddContactComment(FormCollection param)
        {
            SessionLogin Author = (SessionLogin)Session["User"];
            int visHide = 1;
            if (param["visHide"]==null)
            {
                visHide = 0;
            }
            string Msg= param["Message"].ToString();
            int Recv = Int32.Parse(param["RecvId"]);
            Annotation Coment = new Annotation()
            {
                AuthorId = Author.Id,
                ContactId = Recv,
                InstanceOfProcess = null,
                Hidden = visHide == 1 ? true : false,
                Message = Msg,
                Time = DateTime.Now,
                Type = AnnotationType.Contact_Static

            };
            DataProvider.Dp.AddNewContactComent(Coment);
            return RedirectToAction("Info", "Edit", new { id = Recv});
        }

        [HttpPost]
        public ActionResult UpdateComment(FormCollection param)
        {
            int RecvId= Int32.Parse(param["Id"]);
            string Msg = param["modalMsg"].ToString();
            int Id = Int32.Parse(param["commentId"]);
            Annotation Coment = new Annotation()
            {
                Id=Id,
                Message = Msg,

            };
            DataProvider.Dp.UpdateContactComent(Coment);
            return RedirectToAction("Info", "Edit", new { id = RecvId });
        }

        [HttpPost]
        public bool DeleteComment(int Id)
        {
            return DataProvider.Dp.DeleteComment(Id);
        }

        [HttpPost]
        public ActionResult LoadContactComment()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var Identity= Request.Form.GetValues("Id").FirstOrDefault();
            var CommentHidden = Request.Form.GetValues("comment").FirstOrDefault();

            int Id = Convert.ToInt32(Identity); //provera kasnije
            int comHid = Convert.ToInt32(CommentHidden);

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var comments = DataProvider.Dp.GetAllContactComment(Id,comHid);



            comments = comments.OrderByDescending(x => DateTime.Parse(x.Time)).ToList();
            recordsTotal = comments.Count();
            var data = comments.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet] // da li moze interviewer?
        public FileResult OpenContactCV(int Id)
        {

            Contact cvContact = DataProvider.Dp.GetContact(Id);

            if (cvContact.CVContent == null || cvContact.CVContentType==null)
            {
                return null;
            }

            return File(cvContact.CVContent, cvContact.CVContentType);
            
        }
    }
}
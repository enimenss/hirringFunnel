using HiringFunnel.DTO;
using HiringFunnel.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HiringFunnel.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
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

        [HttpPost]
        public ActionResult Index([Bind(Include = "Name,Seniority,Technologies")] ProcessView proces)
        {
            proces.StartDate = DateTime.Now;
            DataProvider.Dp.CreateProcess(proces);
            return RedirectToAction("Index");
        }


        public ActionResult Interviewer()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");

            }
            SessionLogin inter = (SessionLogin)Session["User"];
            if (inter.Role != UserLevel.Interviewer)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoadProcessData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            var Name = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var Seniority = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Technologies = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var v = DataProvider.Dp.GetAllProcess();

            if (!string.IsNullOrEmpty(Name))
            {
                v = v.Where(a => a.Name.Contains(Name)).ToList();
            }


            if (!string.IsNullOrEmpty(Seniority))
            {
                v = v.Where(a => a.Seniority.ToString().Contains(Seniority)).ToList();
            }

            if (!string.IsNullOrEmpty(Technologies))
            {
                v = v.Where(a => a.Technologies.ToString().Contains(Technologies)).ToList();
            }

            v = v.OrderByDescending(x => x.StartDate).ToList();
            recordsTotal = v.Count();
            var data = (from d in v
                        select new
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Technologies = d.Technologies,
                            Seniority = d.Seniority.ToString(),
                            isEnd = d.EndDate != null ? true:false,
                            StartDate = d.StartDate.ToString()
                        }).Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult EditProcess(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ProcessView p = DataProvider.Dp.GetProcess(id);
            ViewData["disablecontrols"] = false;
            if (p.EndDate != null)
            {
                ViewData["disablecontrols"] = true;
            }
            List<TechnologyView> teh = DataProvider.Dp.GetAllTechnologies();
            TempData["Techno"] = teh;
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProcess([Bind(Include = "Id,Name,Technologies,Seniority,StartDate")] ProcessView proc)
        {
            DataProvider.Dp.EditProcess(proc);
            return RedirectToAction("EditProcess", "Process", new { id = proc.Id });
        }

        public ActionResult LoadContactProcessData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            var FirstName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var LastName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Seniority = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var Technologies = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var Identity = Request.Form.GetValues("Id").FirstOrDefault();
            int Id = Convert.ToInt32(Identity); //provera kasnije

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var v = DataProvider.Dp.GetAllNotProcessContacts(Id);

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
                foreach (ContactNotProcessView e in v.ToList())
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
                            Technologies = (from tech in n.Technologies select " " + tech.Name)

                        }).Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadContactInProcessData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            var Identity = Request.Form.GetValues("Id").FirstOrDefault();
            int Id = Convert.ToInt32(Identity); //provera 

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var v = DataProvider.Dp.GetAllProcessNotContactedContacts(Id);


            v = v.OrderBy(x => x.Id).ToList();
            recordsTotal = v.Count();
            var data = (from n in v
                        select new
                        {
                            Id = n.Id,
                            ProcInstId = n.ProcInstId,
                            FirstName = n.FirstName,
                            LastName = n.LastName,
                            Seniority = n.Seniority.ToString(),
                            Technologies = (from tech in n.Technologies select " " + tech.Name),

                        }).Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult Dodaj(int ProcessId, int ContactId)
        {
            ProcessInstance inst = new ProcessInstance()
            {
                CreationDate = DateTime.Now,
                ProcessId = ProcessId,
                ContactId = ContactId
            };
            var data = DataProvider.Dp.CreateInstanse(inst);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Contacted(int ProcessId, int ContactId)
        {
            var data = DataProvider.Dp.Contacted(ProcessId, ContactId);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult EndProcess(int Id)
        {
            DataProvider.Dp.EndProcess(Id);
            var data = true;
            return Json(data);
        }

        public ActionResult EditContactInProcess(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ProcessInstance inst = DataProvider.Dp.GetInstance(id);

            Contact cont = DataProvider.Dp.GetContact(inst.ContactId);
            List <InterviewerView> interviewers= DataProvider.Dp.GetAllInterviewers();
            Process p = inst.InstanceOf;
            TempData["process"] = p;
            TempData["cont"] = cont;
            TempData["interviewers"] = interviewers;
            ViewData["disablecontrols"] = false;
            if (inst.InstanceOf.EndDate!=null)
            {
                ViewData["disablecontrols"] = true;
            }
            return View(inst);
        }

        [HttpPost]
        public ActionResult EditContactInProcess(ProcessInstance pIns,FormCollection param)
        {
            pIns.Interviewers = new Collection<Interviewer>();

            if (pIns.OfferSent)
            {
                pIns.OfferDate = DateTime.Now;
            }
            if (pIns.Accepted)
            {
                pIns.AcceptanceDate = DateTime.Now;
            }
            if (pIns.Rejected)
            {
                pIns.RejectionDate = DateTime.Now;
            }
            if (pIns.Quit)
            {
                pIns.QuitDate = DateTime.Now;
            }

            SessionLogin author = (SessionLogin)Session["User"];


            if (!string.IsNullOrEmpty(param["Contact_Comment"]))
                {
                string Message = param["Contact_Comment"].ToString();

                Annotation Coment = new Annotation()
                {
                    AuthorId = author.Id,
                    PInsId = pIns.Id,
                    Hidden = (param["Contact_Comment_Hide"]!=null)?true:false,
                    Message = Message,
                    Time = DateTime.Now,
                    Type = AnnotationType.Contact_Comment

                };
                DataProvider.Dp.AddNewContactComent(Coment);

            }


            if (!string.IsNullOrEmpty(param["Interview_Feedback"]))
            {
                string Message = param["Interview_Feedback"].ToString();

                Annotation Coment = new Annotation()
                {
                    AuthorId = author.Id,
                    PInsId = pIns.Id,
                    Hidden =false,
                    Message = Message,
                    Time = DateTime.Now,
                    Type = AnnotationType.Interview_Feedback

                };
                DataProvider.Dp.AddNewContactComent(Coment);

            }

            if (!string.IsNullOrEmpty(param["Test_Feedback"]))
            {
                string Message = param["Test_Feedback"].ToString();

                Annotation Coment = new Annotation()
                {
                    AuthorId = author.Id,
                    PInsId = pIns.Id,
                    Hidden = false,
                    Message = Message,
                    Time = DateTime.Now,
                    Type = AnnotationType.Test_Feedback

                };
                DataProvider.Dp.AddNewContactComent(Coment);

            }

            if (!string.IsNullOrEmpty(param["TestDefense_Feedback"]))
            {
                string Message = param["TestDefense_Feedback"].ToString();

                Annotation Coment = new Annotation()
                {
                    AuthorId = author.Id,
                    PInsId = pIns.Id,
                    Hidden = false,
                    Message = Message,
                    Time = DateTime.Now,
                    Type = AnnotationType.TestDefense_Feedback

                };
                DataProvider.Dp.AddNewContactComent(Coment);

            }

            if (!string.IsNullOrEmpty(param["Offer_Comment"]))
            {
                string Message = param["Offer_Comment"].ToString();

                Annotation Coment = new Annotation()
                {
                    AuthorId = author.Id,
                    PInsId = pIns.Id,
                    Hidden = (param["Offer_Comment_Hide"]!=null)?true:false,
                    Message = Message,
                    Time = DateTime.Now,
                    Type = AnnotationType.Offer_Comment

                };
                DataProvider.Dp.AddNewContactComent(Coment);

            }

            if (!string.IsNullOrEmpty(param["Acceptance_State"]))
            {
                string Message = param["Acceptance_State"].ToString();

                Annotation Coment = new Annotation()
                {
                    AuthorId = author.Id,
                    PInsId = pIns.Id,
                    Hidden = false,
                    Message = Message,
                    Time = DateTime.Now,
                    Type = AnnotationType.Acceptance_State

                };
                DataProvider.Dp.AddNewContactComent(Coment);

            }

            if (!string.IsNullOrEmpty(param["Rejection_State"]))
            {
                string Message = param["Rejection_State"].ToString();

                Annotation Coment = new Annotation()
                {
                    AuthorId = author.Id,
                    PInsId = pIns.Id,
                    Hidden = false,
                    Message = Message,
                    Time = DateTime.Now,
                    Type = AnnotationType.Rejection_State

                };
                DataProvider.Dp.AddNewContactComent(Coment);

            }

            if (!string.IsNullOrEmpty(param["Quit_State"]))
            {
                string Message = param["Quit_State"].ToString();

                Annotation Coment = new Annotation()
                {
                    AuthorId = author.Id,
                    PInsId = pIns.Id,
                    Hidden = false,
                    Message = Message,
                    Time = DateTime.Now,
                    Type = AnnotationType.Quit_State

                };
                DataProvider.Dp.AddNewContactComent(Coment);

            }

            if (param["IntervjuInerviewers"] != null)
            {
                string[] inter = param["IntervjuInerviewers"].Split(',');
                foreach (string id in inter)
                {
                    int UserId = Int32.Parse(id);
                   pIns.Interviewers.Add(new Interviewer { InterviewerId = UserId, PInsId = pIns.Id, Type = AnnotationType.Interview_Feedback });
                }
            }

            if (param["TestInterviewers"] != null)
            {
                string[] inter = param["TestInterviewers"].Split(',');
                foreach (string id in inter)
                {
                    int UserId = Int32.Parse(id);
                    pIns.Interviewers.Add(new Interviewer { InterviewerId = UserId, PInsId = pIns.Id, Type = AnnotationType.Test_Feedback });
                }
            }

            if (param["OdbranaInterviewers"] != null)
            {
                string[] inter = param["OdbranaInterviewers"].Split(',');
                foreach (string id in inter)
                {
                    int UserId = Int32.Parse(id);
                    pIns.Interviewers.Add(new Interviewer { InterviewerId = UserId, PInsId = pIns.Id, Type = AnnotationType.TestDefense_Feedback});
                }
            }

            pIns.Annotations = DataProvider.Dp.GetAllAnnotationsInInstance(pIns.Id);
            bool pom = true;
            while (pom)
            {
                if (DataProvider.Dp.IsRejectInstances(pIns))
                {
                    break;
                }
                if (DataProvider.Dp.IsQuitInstances(pIns))
                {
                    break;
                }
                if (DataProvider.Dp.IsAcceptInstance(pIns))
                {
                    break;
                }
                if (DataProvider.Dp.IsOfferInstances(pIns))
                {
                    break;
                }
                if (DataProvider.Dp.IsTestDefenseInstances(pIns))
                {
                    break;
                }
                if (DataProvider.Dp.IsTestInstances(pIns))
                {
                    break;
                }
                if (DataProvider.Dp.IsInterviewInstances(pIns))
                {
                    break;
                }
                pIns.CurrentPhase = Phase.Contact_Phase;

                pom = false;
            }
            //poziv funkcija za odlucivanje faze

            DataProvider.Dp.UpdateProcessInstance(pIns);

            return RedirectToAction("EditContactInProcess", "Process", new { id = pIns.Id });
        }




        [HttpPost]
        public JsonResult GetInstancesData(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> contactInstances = DataProvider.Dp.GetContactInstances(processId);
            ICollection<ProcessInstanceInfo> interviewInstances = DataProvider.Dp.GetInterviewInstances(processId);
            ICollection<ProcessInstanceInfo> testInstances = DataProvider.Dp.GetTestInstances(processId);
            ICollection<ProcessInstanceInfo> testDefenseInstances = DataProvider.Dp.GetTestDefenseInstances(processId);
            ICollection<ProcessInstanceInfo> offerInstances = DataProvider.Dp.GetOfferInstances(processId);
            ICollection<ProcessInstanceInfo> acceptInstances = DataProvider.Dp.GetAcceptInstances(processId);
            ICollection<ProcessInstanceInfo> rejectInstances = DataProvider.Dp.GetRejectInstances(processId);
            ICollection<ProcessInstanceInfo> quitInstances = DataProvider.Dp.GetQuitInstances(processId);

            var data = new
            {
                Contacts = contactInstances.OrderByDescending(x => x.Date),
                Interviews = interviewInstances,
                Tests = testInstances,
                TestDefenses = testDefenseInstances,
                Offers = offerInstances,
                Accepts = acceptInstances,
                Rejects = rejectInstances,
                Quits = quitInstances
            };

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInterviewerInstancesData()
        {
            SessionLogin inter = (SessionLogin)Session["User"];
            ICollection<ProcessInstanceInfo> contactInstances = DataProvider.Dp.GetInterviewerContactInstances(inter.Id);
            ICollection<ProcessInstanceInfo> interviewInstances = DataProvider.Dp.GetInterviewerInterviewInstances(inter.Id);
            ICollection<ProcessInstanceInfo> testInstances = DataProvider.Dp.GetInterviewerTestInstances(inter.Id);
            ICollection<ProcessInstanceInfo> testDefenseInstances = DataProvider.Dp.GetInterviewerTestDefenseInstances(inter.Id);
            ICollection<ProcessInstanceInfo> offerInstances = DataProvider.Dp.GetInterviewerOfferInstances(inter.Id);
            ICollection<ProcessInstanceInfo> acceptInstances = DataProvider.Dp.GetInterviewerAcceptInstances(inter.Id);
            ICollection<ProcessInstanceInfo> rejectInstances = DataProvider.Dp.GetInterviewerRejectInstances(inter.Id);
            ICollection<ProcessInstanceInfo> quitInstances = DataProvider.Dp.GetInterviewerQuitInstances(inter.Id);

            var data = new
            {
                Contacts = contactInstances,
                Interviews = interviewInstances,
                Tests = testInstances,
                TestDefenses = testDefenseInstances,
                Offers = offerInstances,
                Accepts = acceptInstances,
                Rejects = rejectInstances,
                Quits = quitInstances
            };

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangePhase(int Instance,string Phase)
        {

            ProcessInstance inst = DataProvider.Dp.GetInstance(Instance);
            switch (Phase)
            {
                //sta ce jos da se popuni odluciti
                case "Kontakt":
                    {
                        inst.InterviewHeld = false;
                        inst.InterviewNoticed = false;
                        inst.InterviewScheduled = false;
                        inst.InterviewDate = null;
                        inst.TestDate = null;
                        inst.TestDefenseDate = null;
                        inst.OfferDate = null;
                        inst.AcceptanceDate = null;
                        inst.RejectionDate = null;
                        inst.QuitDate = null;
                        inst.OfferSent = false;
                        inst.Quit = false;
                        inst.Rejected = false;
                        inst.TestDefenseHeld = false;
                        inst.TestDefenseNoticed = false;
                        inst.TestDefenseScheduled =false;
                        inst.TestHeld = false;
                        inst.TestNoticed = false;
                        inst.TestScheduled = false;
                        inst.Accepted = false;
                    
                        inst.CurrentPhase = HiringFunnel.Phase.Contact_Phase;
                        break;
                    }
                case "Intervju":
                    {
                      

                        inst.InterviewHeld = false;
                        inst.InterviewNoticed = true;
                        inst.InterviewScheduled = true;
                        inst.InterviewDate = DateTime.Now;
                        inst.TestDate = null;
                        inst.TestDefenseDate = null;
                        inst.OfferDate = null;
                        inst.AcceptanceDate = null;
                        inst.RejectionDate = null;
                        inst.QuitDate = null;
                        inst.OfferSent = false;
                        inst.Quit = false;
                        inst.Rejected = false;
                        inst.TestDefenseHeld = false;
                        inst.TestDefenseNoticed = false;
                        inst.TestDefenseScheduled = false;
                        inst.TestHeld = false;
                        inst.TestNoticed = false;
                        inst.TestScheduled = false;
                        inst.Accepted = false;

                        if (inst.Interviewers.Where(x => x.Type == AnnotationType.Interview_Feedback).Count() == 0)
                        {
                            InterviewerView i = (DataProvider.Dp.GetAllInterviewers()).FirstOrDefault();
                            Interviewer inter = new Interviewer { InterviewerId = i.Id, PInsId = inst.Id, Type = AnnotationType.Interview_Feedback };
                            DataProvider.Dp.AddInterviewerOnPhase(inter);
                        }

                      
                        inst.CurrentPhase = HiringFunnel.Phase.Interview_Phase;
                        break;
                    }
                case "Test":
                    {
                      

                         inst.InterviewHeld = true;
                        inst.InterviewNoticed = true;
                        inst.InterviewScheduled = true;
                        inst.OfferSent = false;
                        inst.Quit = false;
                        inst.Rejected = false;
                        inst.TestDate = DateTime.Now;
                        inst.TestDefenseDate = null;
                        inst.OfferDate = null;
                        inst.AcceptanceDate = null;
                        inst.RejectionDate = null;
                        inst.QuitDate = null;
                        inst.TestDefenseHeld = false;
                        inst.TestDefenseNoticed = false;
                        inst.TestDefenseScheduled = false;
                        inst.TestHeld = false;
                        inst.TestNoticed = true;
                        inst.TestScheduled = true;
                        inst.Accepted = false;
                        if (inst.Interviewers.Where(x => x.Type ==AnnotationType.Test_Feedback).Count() == 0)
                        {
                            InterviewerView i = (DataProvider.Dp.GetAllInterviewers()).FirstOrDefault();
                            Interviewer inter = new Interviewer { InterviewerId = i.Id, PInsId = inst.Id, Type = AnnotationType.Test_Feedback };
                            DataProvider.Dp.AddInterviewerOnPhase(inter);
                        }

                        if (inst.Annotations.Where(x => x.Type == AnnotationType.Interview_Feedback).Count() == 0)
                        {
                            SessionLogin author = (SessionLogin)Session["User"];
                            Annotation Coment = new Annotation()
                            {
                                AuthorId = author.Id,
                                PInsId = inst.Id,
                                Message = "Prebacen",
                                Time = DateTime.Now,
                                Type = AnnotationType.Interview_Feedback

                            };
                            DataProvider.Dp.AddNewContactComent(Coment);

                        }

                        inst.CurrentPhase = HiringFunnel.Phase.Test_Phase;
                        break;
                    }
                case "Odbrana":
                    {

                        inst.InterviewHeld =true;
                        inst.InterviewNoticed = true;
                        inst.InterviewScheduled =true;
                        inst.InterviewDate = null;
                        inst.OfferSent = false;
                        inst.Quit = false;
                        inst.Rejected = false;
                        inst.TestDefenseDate = DateTime.Now;
                        inst.OfferDate = null;
                        inst.AcceptanceDate = null;
                        inst.RejectionDate = null;
                        inst.QuitDate = null;
                        inst.TestDefenseHeld =false;
                        inst.TestDefenseNoticed = true;
                        inst.TestDefenseScheduled = true;
                        inst.TestHeld =true;
                        inst.TestNoticed = true;
                        inst.TestScheduled = true;
                        inst.Accepted =false;
                        if (inst.Interviewers.Where(x => x.Type == AnnotationType.TestDefense_Feedback).Count() == 0)
                        {
                            InterviewerView i = (DataProvider.Dp.GetAllInterviewers()).FirstOrDefault();
                            Interviewer inter=new Interviewer { InterviewerId = i.Id,PInsId=inst.Id, Type = AnnotationType.TestDefense_Feedback };
                            DataProvider.Dp.AddInterviewerOnPhase(inter);
                        }

                        if (inst.Annotations.Where(x => x.Type == AnnotationType.Test_Feedback).Count() == 0)
                        {
                            SessionLogin author = (SessionLogin)Session["User"];
                            Annotation Coment = new Annotation()
                            {
                                AuthorId = author.Id,
                                PInsId=inst.Id,
                                Message = "Prebacen",
                                Time = DateTime.Now,
                                Type = AnnotationType.Test_Feedback

                            };
                            DataProvider.Dp.AddNewContactComent(Coment);

                        }
                       inst.CurrentPhase = HiringFunnel.Phase.TestDefense_Phase;
                        break;
                    }
                case "Ponuda":
                    {

                        inst.InterviewHeld = true;
                        inst.InterviewNoticed = true;
                        inst.InterviewScheduled = true;
                        inst.OfferSent = false;
                        inst.OfferDate = DateTime.Now;
                        inst.AcceptanceDate = null;
                        inst.RejectionDate = null;
                        inst.QuitDate = null;
                        inst.Quit = false;
                        inst.Rejected = false;
                        inst.TestDefenseHeld = true;
                        inst.TestDefenseNoticed = true;
                        inst.TestDefenseScheduled = true;
                        inst.TestHeld = true;
                        inst.TestNoticed = true;
                        inst.TestScheduled = true;
                        inst.Accepted = false;
                       

                        if (inst.Annotations.Where(x => x.Type == AnnotationType.TestDefense_Feedback).Count() == 0)
                        {
                            SessionLogin author = (SessionLogin)Session["User"];
                            Annotation Coment = new Annotation()
                            {
                                AuthorId = author.Id,
                                PInsId = inst.Id,
                                Message = "Prebacen",
                                Time = DateTime.Now,
                                Type = AnnotationType.TestDefense_Feedback

                            };
                            DataProvider.Dp.AddNewContactComent(Coment);

                        }
                        inst.CurrentPhase = HiringFunnel.Phase.Offer_Phase;
                        break;
                    }
                case "Primljen":
                    {

                        inst.InterviewHeld = true;
                        inst.InterviewNoticed = true;
                        inst.InterviewScheduled = true;
                        inst.OfferSent = true;
                        inst.Quit = false;
                        inst.AcceptanceDate =DateTime.Now;
                        inst.RejectionDate = null;
                        inst.QuitDate = null;
                        inst.Rejected = false;
                        inst.TestDefenseHeld = true;
                        inst.TestDefenseNoticed = true;
                        inst.TestDefenseScheduled = true;
                        inst.TestHeld = true;
                        inst.TestNoticed = true;
                        inst.TestScheduled = true;
                        inst.Accepted = true;


                        if (inst.Annotations.Where(x => x.Type == AnnotationType.Offer_Comment && x.Hidden).Count() == 0)
                        {
                            SessionLogin author = (SessionLogin)Session["User"];
                            Annotation Coment = new Annotation()
                            {
                                AuthorId = author.Id,
                                PInsId = inst.Id,
                                Message = "Prebacen",
                                Time = DateTime.Now,
                                Hidden = true,
                                Type = AnnotationType.Offer_Comment
                                

                            };
                            DataProvider.Dp.AddNewContactComent(Coment);

                        }
                        inst.CurrentPhase = HiringFunnel.Phase.Acceptance_Phase;
                        break;
                    }
                case "Odustao":
                    {
                        inst.InterviewHeld = true;
                        inst.InterviewNoticed = true;
                        inst.InterviewScheduled = true;
                        inst.OfferSent = true;
                        inst.Quit = true;
                        inst.AcceptanceDate =null;
                        inst.RejectionDate = null;
                        inst.QuitDate = DateTime.Now;
                        inst.Rejected = false;
                        inst.TestDefenseHeld = true;
                        inst.TestDefenseNoticed = true;
                        inst.TestDefenseScheduled = true;
                        inst.TestHeld = true;
                        inst.TestNoticed = true;
                        inst.TestScheduled = true;
                        inst.Accepted = false;


                        if (inst.Annotations.Where(x => x.Type == AnnotationType.Quit_State).Count() == 0)
                        {
                            SessionLogin author = (SessionLogin)Session["User"];
                            Annotation Coment = new Annotation()
                            {
                                AuthorId = author.Id,
                                PInsId = inst.Id,
                                Message = "Prebacen",
                                Time = DateTime.Now,
                                Type = AnnotationType.Quit_State

                            };
                            DataProvider.Dp.AddNewContactComent(Coment);

                        }
                        inst.CurrentPhase = HiringFunnel.Phase.Quit_Phase;
                        break;
                    }
                case "Odbijen":
                    {
                        inst.InterviewHeld = true;
                        inst.InterviewNoticed = true;
                        inst.InterviewScheduled = true;
                        inst.OfferSent = true;
                        inst.Quit = false;
                        inst.Rejected = true;
                        inst.AcceptanceDate = null;
                        inst.RejectionDate = DateTime.Now;
                        inst.QuitDate = null;
                        inst.TestDefenseHeld = true;
                        inst.TestDefenseNoticed = true;
                        inst.TestDefenseScheduled = true;
                        inst.TestHeld = true;
                        inst.TestNoticed = true;
                        inst.TestScheduled = true;
                        inst.Accepted = false;


                        if (inst.Annotations.Where(x => x.Type == AnnotationType.Rejection_State).Count() == 0)
                        {
                            SessionLogin author = (SessionLogin)Session["User"];
                            Annotation Coment = new Annotation()
                            {
                                AuthorId = author.Id,
                                PInsId = inst.Id,
                                Message = "Prebacen",
                                Time = DateTime.Now,
                                Type = AnnotationType.Rejection_State

                            };
                            DataProvider.Dp.AddNewContactComent(Coment);

                        }
                        inst.CurrentPhase = HiringFunnel.Phase.Rejection_Phase;
                        break;
                    }

                default: { break; }
            }
            DataProvider.Dp.UpdateInstance(inst);


            return Json(new { data = true }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult KontaktIPonudaKomentar(int InstanceId ,string Message ,bool isHidden,string Phase)
        {
            AnnotationType myStatus;
            Enum.TryParse(Phase, out myStatus);
            SessionLogin author = (SessionLogin)Session["User"];
            Annotation Coment = new Annotation()
            {
                AuthorId = author.Id,
                PInsId = InstanceId,
                Hidden = isHidden,
                Message = Message,
                Time = DateTime.Now,
                Type = myStatus

            };
            DataProvider.Dp.AddNewContactComent(Coment);

            return Json(new { data = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInstanseComment(int InstanceId, string Phase)
        {
            AnnotationType myStatus;
            Enum.TryParse(Phase, out myStatus);
            List<CommentView> list = DataProvider.Dp.GetInstanceComment(InstanceId, myStatus);

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddToDo(int InstanceId,string Message)
        {
            ToDoItem toDo = DataProvider.Dp.AddToDo(InstanceId, Message);
            var data = new
            {
                Id = toDo.Id,
                Message = toDo.Message
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetToDo(int InstanceId)
        {
            List<ToDoItem> list=DataProvider.Dp.GetToDo(InstanceId);
            var data = from l in list
                       select new
                       {
                           Id = l.Id,
                           Message = l.Message,
                           Completed = l.Completed
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public JsonResult DisableToDo(int Id)
        {
            DataProvider.Dp.DisableToDo(Id);
            return Json(new { data = true }, JsonRequestBehavior.AllowGet);

        }
    }
}
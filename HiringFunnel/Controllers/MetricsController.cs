using HiringFunnel.DTO;
using HiringFunnel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HiringFunnel.Controllers
{
    public class MetricsController : Controller
    {
        // GET: Metrics
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
        public ActionResult Index(FormCollection Datum)
        {
            DateTime d = DateTime.Parse(Datum["Datum"].ToString());
            return View();

        }


        [HttpPost]
        public JsonResult VratiNajduziProces()
        {
            List<ProcessView> lista = DataProvider.Dp.GetAllProcess();

            var now = DateTime.Now;
            ProcessView najduzi = lista.FirstOrDefault();
            if (najduzi.EndDate == null)
                najduzi.EndDate = now;
            foreach (var p in lista)
            {
                if (p.EndDate == null)
                    p.EndDate = now;
                if ((p.EndDate - p.StartDate) > (najduzi.EndDate - najduzi.StartDate))
                    najduzi = p;
            }

            var data = new
            {
                Name = najduzi.Name,
                StartDate = najduzi.StartDate.ToString(),
                EndDate = najduzi.EndDate == now ? "Nije jos zavrsen" : najduzi.EndDate.ToString()
            };

            return Json(data);
        }

        [HttpPost]
        public JsonResult VratiNajduziOdSelektovanihProcesa(SeniorityLevel Seniority, string Technology)
        {
            List<Process> lista = DataProvider.Dp.GetAllProcess(Seniority, Technology);

            var now = DateTime.Now;
            Process najduzi = lista.FirstOrDefault();
            if (najduzi.EndDate == null)
                najduzi.EndDate = now;
            foreach (var p in lista)
            {
                if (p.EndDate == null)
                    p.EndDate = now;
                if ((p.EndDate - p.StartDate) > (najduzi.EndDate - najduzi.StartDate))
                    najduzi = p;
            }

            var data = new
            {
                Name = najduzi.Name,
                StartDate = najduzi.StartDate.ToString(),
                EndDate = najduzi.EndDate == now ? "Nije jos zavrsen" : najduzi.EndDate.ToString()
            };

            return Json(data);
        }

        [HttpPost]
        public JsonResult VratiNajkraciProces()
        {
            List<ProcessView> lista = DataProvider.Dp.GetAllProcess();

            lista = lista.Where(x => x.EndDate != null).ToList();

            ProcessView najkraci = lista.FirstOrDefault();

            foreach (var p in lista)
            {
                if ((p.EndDate - p.StartDate) <= (najkraci.EndDate - najkraci.StartDate))
                    najkraci = p;
            }

            var data = new
            {
                Name = najkraci.Name,
                StartDate = najkraci.StartDate.ToString(),
                EndDate = najkraci.EndDate.ToString()
            };

            return Json(data);
        }

        [HttpPost]
        public JsonResult VratiNajkraciOdSelektovanihProcesa(SeniorityLevel Seniority, string Technology)
        {
            List<Process> lista = DataProvider.Dp.GetAllProcess(Seniority, Technology);

            lista = lista.Where(x => x.EndDate != null).ToList();

            Process najkraci = lista.FirstOrDefault();

            foreach (var p in lista)
            {
                if ((p.EndDate - p.StartDate) <= (najkraci.EndDate - najkraci.StartDate))
                    najkraci = p;
            }

            var data = new
            {
                Name = najkraci.Name,
                StartDate = najkraci.StartDate.ToString(),
                EndDate = najkraci.EndDate.ToString()
            };

            return Json(data);
        }
        [HttpPost]
        public JsonResult ProsecnoTrajanjeProcesa(SeniorityLevel Seniority, string Technology)
        {

            List<Process> lista = DataProvider.Dp.GetAllProcess(Seniority, Technology);
            int count = lista.Count();
            var date = DateTime.Now;
            long Sum = 0;
            foreach (var p in lista)
            {
                if (p.EndDate == null)
                {
                    p.EndDate = date;
                }

                Sum = Sum + (p.EndDate.Value.Ticks - p.StartDate.Ticks);
            }

            Sum = Sum / (long)count;
            DateTime Avg = new DateTime(Sum);
            var data = new { data = "Dani:" + (Avg.Day - 1).ToString() + " Meseci:" + (Avg.Month - 1).ToString() + " Godine:" + (Avg.Year - 1).ToString() };



            return Json(data);

        }

        [HttpPost]
        public JsonResult brZapIProc(SeniorityLevel Seniority, string Technology, DateTime Date)
        {
            List<Process> procesi = DataProvider.Dp.GetAllProcess(Seniority, Technology, Date);

            int count = procesi.Count();
            int brZap = 0;
            foreach (var proc in procesi)
            {
                foreach (var inst in proc.Instances)
                {
                    if (inst.CurrentPhase == Phase.Acceptance_Phase)
                    {
                        brZap++;
                    }
                }
            }
            var data = new { Zap = brZap, Proc = count };
            return Json(data);
        }


        [HttpPost]
        public JsonResult LoadProcessData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            var Seniority = Request.Form.GetValues("Seniority").FirstOrDefault();

            var Technology=Request.Form.GetValues("Technology").FirstOrDefault();

            var DateT = Request.Form.GetValues("Date").FirstOrDefault();
            DateTime Date = new DateTime();
            if (string.IsNullOrEmpty(DateT))
            {
                Date = DateTime.Now;
            }
            else
            {
                Date= DateTime.Parse(DateT);
            }

            SeniorityLevel Sen = (SeniorityLevel)Enum.Parse(typeof(SeniorityLevel),Seniority);
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var v = DataProvider.Dp.GetAllProcess(Sen,Technology,Date);

            
            var data = (from d in v
                        select new
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Technologies = d.Technologies,
                            Seniority = d.Seniority.ToString(),
                            Duration = d.StartDate.ToShortDateString() + "-" + ((d.EndDate != null) ? d.EndDate.Value.ToShortDateString() :"Nije zavrsen"),
                        }).Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //public JsonResult NajduzaFaza()
        //{

        //}
    }
}
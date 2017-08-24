using HiringFunnel.DAL;
using HiringFunnel.DTO;
using HiringFunnel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HiringFunnel
{
    public class DataProvider
    {

        private static DataProvider dp;
        private HFContext db = new HFContext();

        public static DataProvider Dp
        {
            get
            {
                if (dp == null)
                    dp = new DataProvider();
                return dp;
            }
        }

        public ContactEditView GetContactEditView(int? id)
        {
            Contact cont = db.Contacts.Find(id);
            ContactEditView contView = new ContactEditView()
            {
                DateOfBirth = cont.DateOfBirth,
                Email = cont.Email,
                FirstName = cont.FirstName,
                Id = cont.Id,
                LastName = cont.LastName,
                LinkedIn = cont.LinkedIn,
                Location = cont.Location,
                Phone = cont.Phone,
                Seniority = cont.Seniority,
                Technologies = new Collection<TechnologyView>()
            };
            foreach(var t in cont.Technologies)
            {
                contView.Technologies.Add(new TechnologyView(t));
            }
            return contView;
        }

       public void CreateProcess(ProcessView proces)
        {
            Process p = new Process()
            {
                Name = proces.Name,
                Seniority = proces.Seniority,
                Technologies = proces.Technologies,
                StartDate = proces.StartDate
            };
            db.Processes.Add(p);
            db.SaveChanges();
          
        }

        public SessionLogin Login(LoginViewModel login)
        {
            SessionLogin log = null;
            User user = (from u in db.Users where u.Email == login.Email && u.PasswordHash == login.Password select u).SingleOrDefault();
            if (user != null)
            {
                log = new SessionLogin() { FirstName = user.FirstName, Id = user.Id, Role = user.Role };
            }

            return log;

        }

        public List<Process> GetAllProcess(SeniorityLevel seniority, string technology)
        {
            List<Process> lista = new List<Process>();
            using (var data = new HFContext())
            {
                lista = (from p in data.Processes where p.Seniority == seniority && p.Technologies == technology select p).ToList();
            }
            return lista;
        }

        public List<CommentView> GetAllContactComment(int id,int comHid)
        {
            List<CommentView> lista = new List<CommentView>();
            using (var date=new HFContext())
            {
                lista=(from c in date.Annotations
                 where c.Type == AnnotationType.Contact_Static && c.ContactId == id && c.Hidden == (comHid == 1 ? false : true)
                 select new CommentView()
                 {
                     Id = c.Id,
                     Message = c.Message,
                     Time = c.Time.ToString(),
                     Type = c.Type,
                     AuthorName = c.Author.FirstName
                 }).ToList();
            }
            return lista;
        }

        public ProcessView GetProcess(int? id)
        {
            ProcessView proc = (ProcessView)(from p in db.Processes where p.Id==id
                                select new ProcessView()
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    Seniority = p.Seniority,
                                    Technologies = p.Technologies,
                                    StartDate = p.StartDate,
                                    EndDate = p.EndDate
                                }
                                 ).SingleOrDefault();
            return proc;
        }

        public void EditProcess(ProcessView proc)
        {
            Process p = db.Processes.Find(proc.Id);
            p.Name = proc.Name;
            p.Seniority = proc.Seniority;
            p.StartDate = proc.StartDate;
            p.Technologies = proc.Technologies;
            db.SaveChanges();
        }

        public void AddNewContactComent(Annotation coment)
        {
            db.Annotations.Add(coment);
            db.SaveChanges();
        }

        public List<ContactView> GetAllContacts()
        {
           // return (from l in db.Contacts select new ContactView(l)).ToList<ContactView>();
            return db.Contacts.Include("Technologies").Where(x => x.Deleted == false).ToList<Contact>().Select(l => new ContactView(l)).ToList();
        }

        public List<ContactNotProcessView> GetAllNotProcessContacts(int id)
        {
            List<ContactNotProcessView> cont = new List<ContactNotProcessView>();

            using (var data = new HFContext())
            {

                cont = (from c in data.Contacts
                        where (c.Deleted==false) && (c.Instances.Count == 0 || c.Instances.Where(x=>x.ProcessId==id).FirstOrDefault()==null)
                        select new ContactNotProcessView()
                        {
                            Id = c.Id,
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            Seniority = c.Seniority,
                            Technologies = (from t in c.Technologies select new TechnologyView() { Id = t.Id, Name = t.Name }).ToList<TechnologyView>()

                        }).ToList();
            }
            return cont;
            
        }

        public List<Process> GetAllProcess(SeniorityLevel seniority, string technology, DateTime date)
        {
            List<Process> lista = new List<Process>();
            using (var data = new HFContext())
            {
                lista = data.Processes.Include("Instances").Where(x => x.Seniority == seniority && x.Technologies == technology &&
                  (x.StartDate.Year <= date.Year || (x.StartDate.Year == date.Year && x.StartDate.Month <= date.Month)) &&
                  ((x.EndDate==null && date<DateTime.Now) || (x.EndDate.Value.Year >= date.Year || (x.EndDate.Value.Year == date.Year && x.EndDate.Value.Month >= date.Month)) )).ToList();
            }
            return lista;
        }

        public void UpdateContactComent(Annotation coment)
        {
            Annotation com = db.Annotations.Find(coment.Id);
            com.Message = coment.Message;
            db.SaveChanges();
        }

        public bool DeleteComment(int id)
        {
            Annotation com = db.Annotations.Find(id);
            db.Annotations.Remove(com);
            db.SaveChanges();
            return true;
        }

        public List<RegisterUser> GetAllUsers()
        {
            List<RegisterUser> lista = new List<RegisterUser>();
            using ( var dbd =new HFContext())
            {
                lista= db.Users.Include("Technologies").Where(x => x.Deleted == false).ToList<User>().Select(l => new RegisterUser(l)).ToList();
            }

            return lista;
        }

        public List<ProcessView> GetAllProcess()
        {
            return (from p in db.Processes
                    select new ProcessView()
                    {
                        Id=p.Id,
                        Name = p.Name,
                        Seniority = p.Seniority,
                        Technologies = p.Technologies,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate
                    }).ToList();
        }

        public bool CreateInstanse(ProcessInstance inst)
        {
            using (var data = new HFContext())
            {
                data.ProcessInstances.Add(inst);
                data.SaveChanges();
            }
            return true;
        }

        public List<ContactNotProcessView> GetAllProcessNotContactedContacts(int id)
        {
            List<ContactNotProcessView> cont = new List<ContactNotProcessView>();

            using (var data = new HFContext())
            {

                cont = (from c in data.Contacts
                        where c.Deleted==false && (c.Instances.Count != 0 && c.Instances.Where(x => x.ProcessId == id && x.ContactDate==null).FirstOrDefault() != null)
                        select new ContactNotProcessView()
                        {
                            Id = c.Id,
                            ProcInstId= c.Instances.Where(x => x.ProcessId == id && x.ContactDate == null).FirstOrDefault().Id,
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            Seniority = c.Seniority,
                            Technologies = (from t in c.Technologies select new TechnologyView() { Id = t.Id, Name = t.Name }).ToList<TechnologyView>()

                        }).ToList();
            }
            return cont;
        }

        public bool Contacted(int processId, int contactId)
        {
            using (var data = new HFContext()) {
                ProcessInstance ins = (ProcessInstance)(from i in data.ProcessInstances where i.ContactId == contactId && i.ProcessId == processId select i).SingleOrDefault();
        
               ins.ContactDate = DateTime.Now;
                ins.CurrentPhase = Phase.Contact_Phase;
                data.SaveChanges();
                    }
            return true;
        }

        public List<InterviewerView> GetAllInterviewers()
        {
            List<InterviewerView> list = new List<InterviewerView>();
            using (var data = new HFContext())
            {
                list = (from i in data.Users where i.Deleted==false && i.Role == UserLevel.Interviewer select new InterviewerView { Id = i.Id, FirstName = i.FirstName }).ToList();
            }
            return list;
        }

        public void EndProcess(int id)
        {
            using (var data = new HFContext())
            {
                Process p = data.Processes.Find(id);
                p.EndDate = DateTime.Now;
                data.SaveChanges();
            }
        }

        public Contact GetContact(int contactId)
        {
            Contact c = null;
            using (var data = new HFContext())
            {
                c = data.Contacts.Find(contactId);
            }
            return c;

        }

        public List<TechnologyView> GetAllTechnologies()
        {
            List<TechnologyView> lista = (from t in db.Technologies select new TechnologyView(){Id=t.Id,Name=t.Name} ).ToList();
            return lista;
        }

        public  List<InstProcessEditView> GetAlProcessInstance(int processId)
        {
            List<InstProcessEditView> list = new List<InstProcessEditView>();
            using(var data =new HFContext())
            {
                list = (from i in data.ProcessInstances where i.ProcessId == processId select new InstProcessEditView() {
                    Id = i.Id,
                    ContactDate = i.ContactDate,
                    FirstName = i.ContactInProcess.FirstName,
                    Seniority = i.ContactInProcess.Seniority.ToString()
                }
                ).ToList();
            }
            return list;
        }

        public List<ProcessView> LoadContactHistory(int id)
        {
            List<ProcessView> list = new List<ProcessView>();
            using (var data = new HFContext())
            {
                list = (from p in data.Processes
                        where p.Instances.Count != 0 && p.Instances.Where(x => x.ContactId == id).FirstOrDefault() != null
                        select new ProcessView
                        {
                            Id = p.Id,
                            StartDate = p.StartDate,
                            EndDate=p.EndDate,
                            Name = p.Name,
                            Seniority = p.Seniority,
                            Technologies = p.Technologies,
                            pInsContact = p.Instances.Where(x => x.ContactId == id).FirstOrDefault().Id
                        }).ToList();
                   
            }
            return list;
        }

        public bool IsEndProcessForContact(int pInsContact)
        {
            ProcessInstance p = new ProcessInstance();
            using (var data = new HFContext())
            {
                p = data.ProcessInstances.Include("Annotations").Where(x => x.Id == pInsContact).SingleOrDefault();
            }
            if (IsQuitInstances(p) || IsRejectInstances(p) || IsAcceptInstance(p))
            {
                return true;

            }
            return false;
        }

        public void Register(RegisterUser newUser)
        {
            if (newUser.Id == 0)
            {
                User user = new User
                {
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    PasswordHash = newUser.Password,
                    Role = newUser.Role,
                    Seniority = newUser.Seniority,
                    Technologies = new Collection<Technology>()
                };
                foreach (TechnologyView tech in newUser.Technologies)
                {

                    Technology t = db.Technologies.Find(tech.Id);

                    user.Technologies.Add(t);

                }
                db.Users.Add(user);
            }
            else
            {
                User user = db.Users.Find(newUser.Id);
                user.Email = newUser.Email;
                user.FirstName = newUser.FirstName;
                user.LastName = newUser.LastName;
                if (newUser.Password != null)
                    user.PasswordHash = newUser.Password;
                user.Role = newUser.Role;
                user.Seniority = newUser.Seniority;
                user.Technologies = new Collection<Technology>();
                     foreach (TechnologyView tech in newUser.Technologies)
                {

                    Technology t = db.Technologies.Find(tech.Id);

                    user.Technologies.Add(t);

                }

            }
            db.SaveChanges();
        }

        public ICollection<Annotation> GetAllAnnotationsInInstance(int id)
        {
            using (var data = new HFContext())
            {
                return (from a in data.Annotations where a.PInsId==id select a).ToList();
            }
        }

        public void UpdateProcessInstance(ProcessInstance pIns)
        {
            ProcessInstance p = null;
            using (var data = new HFContext())
            {
                p = data.ProcessInstances.Find(pIns.Id);
                p.InterviewDate = pIns.InterviewDate;
                p.InterviewHeld = pIns.InterviewHeld;
                p.InterviewNoticed = pIns.InterviewNoticed;
                p.InterviewScheduled = pIns.InterviewScheduled;
                p.OfferSent = pIns.OfferSent;
                p.Quit = pIns.Quit;
                p.Rejected = pIns.Rejected;
                p.TestDate = pIns.TestDate;
                p.TestDefenseDate = pIns.TestDefenseDate;
                p.TestDefenseHeld = pIns.TestDefenseHeld;
                p.TestDefenseNoticed = pIns.TestDefenseNoticed;
                p.TestDefenseScheduled = pIns.TestDefenseScheduled;
                p.TestHeld = pIns.TestHeld;
                p.TestNoticed = pIns.TestNoticed;
                p.TestScheduled = pIns.TestScheduled;
                p.Accepted = pIns.Accepted;
                p.CurrentPhase = pIns.CurrentPhase;
                foreach(var i in p.Interviewers.ToList())
                {
                    data.Interviewers.Remove(i);
                }
                p.Interviewers = new Collection<Interviewer>();
                foreach(var inter in pIns.Interviewers)
                {
                    p.Interviewers.Add(inter);
                }
                data.SaveChanges();
            }
        }

        public ICollection<ProcessInstanceInfo> GetInterviewerOfferInstances(int InterviewerId)
        {
            ICollection<ProcessInstanceInfo> offers = null;

            using (HFContext hfdb = new HFContext())
            {
               
                    offers = (from pIns in hfdb.ProcessInstances
                              where (pIns.InstanceOf.EndDate == null) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Offer_Phase && pIns.Interviewers.Where(x => x.InterviewerId == InterviewerId).FirstOrDefault() != null

                              select new ProcessInstanceInfo
                              {
                                  Id = pIns.Id,
                                  ProcessName = pIns.InstanceOf.Name,
                                  isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                  ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                  ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                  ProcessTechnologies = pIns.InstanceOf.Technologies,
                                  ContactSeniority = pIns.ContactInProcess.Seniority.ToString()
                                  

                              }).ToList();

                
            }

            return offers;
        }

        public  ICollection<ProcessInstanceInfo> GetInterviewerAcceptInstances(int InterviewerId)
        {
            ICollection<ProcessInstanceInfo> accepts = null;

            using (HFContext hfdb = new HFContext())
            {
               
                    accepts = (from pIns in hfdb.ProcessInstances
                               where (pIns.InstanceOf.EndDate == null) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Acceptance_Phase && pIns.Interviewers.Where(x => x.InterviewerId == InterviewerId).FirstOrDefault() != null

                               select new ProcessInstanceInfo
                               {
                                   Id = pIns.Id,
                                   ProcessName = pIns.InstanceOf.Name,
                                   isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                   ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                   ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                   ProcessTechnologies = pIns.InstanceOf.Technologies,
                                   ContactSeniority = pIns.ContactInProcess.Seniority.ToString()
                                   

                               }).ToList();
                
            }

            return accepts;
        }

        public ICollection<ProcessInstanceInfo> GetInterviewerQuitInstances(int InterviewerId)
        {
            ICollection<ProcessInstanceInfo> quits = null;

            using (HFContext hfdb = new HFContext())
            {

                quits = (from pIns in hfdb.ProcessInstances
                         where (pIns.InstanceOf.EndDate == null) && !pIns.ContactInProcess.Deleted
                                    && pIns.CurrentPhase == Phase.Quit_Phase && pIns.Interviewers.Where(x => x.InterviewerId == InterviewerId).FirstOrDefault() != null
                         
                             select new ProcessInstanceInfo
                             {
                                 Id = pIns.Id,
                                 ProcessName = pIns.InstanceOf.Name,
                                 isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                 ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                 ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                 ProcessTechnologies = pIns.InstanceOf.Technologies,
                                 ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                

                             }).ToList();

                }      

            return quits;
        }

        public ICollection<ProcessInstanceInfo> GetInterviewerRejectInstances(int InterviewerId)
        {
            ICollection<ProcessInstanceInfo> rejects = null;

            using (HFContext hfdb = new HFContext())
            {
                
                    rejects = (from pIns in hfdb.ProcessInstances
                               where (pIns.InstanceOf.EndDate == null) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Rejection_Phase && pIns.Interviewers.Where(x => x.InterviewerId == InterviewerId).FirstOrDefault() != null

                               select new ProcessInstanceInfo
                               {
                                   Id = pIns.Id,
                                   ProcessName = pIns.InstanceOf.Name,
                                   isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                   ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                   ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                   ProcessTechnologies = pIns.InstanceOf.Technologies,
                                   ContactSeniority = pIns.ContactInProcess.Seniority.ToString()
                                  

                               }).ToList();

                
            }

            return rejects;
        }

        public ICollection<ProcessInstanceInfo> GetInterviewerTestDefenseInstances(int InterviewerId)
        {
            ICollection<ProcessInstanceInfo> testDefenses = null;

            using (HFContext hfdb = new HFContext())
            {
             
                    testDefenses = (from pIns in hfdb.ProcessInstances
                                    where (pIns.InstanceOf.EndDate == null) /*&& !pIns.ContactInProcess.Deleted*/
                                              && pIns.CurrentPhase == Phase.TestDefense_Phase && pIns.Interviewers.Where(x => x.InterviewerId == InterviewerId).FirstOrDefault() != null

                                    select new ProcessInstanceInfo
                                    {
                                        Id = pIns.Id,
                                        ProcessName = pIns.InstanceOf.Name,
                                        isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                        ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                        ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                        ProcessTechnologies = pIns.InstanceOf.Technologies,
                                        ContactSeniority = pIns.ContactInProcess.Seniority.ToString()
                                        
                                    }).ToList();

                
            }

            return testDefenses;
        }

        public ICollection<ProcessInstanceInfo> GetInterviewerTestInstances(int InterviewerId)
        {
            ICollection<ProcessInstanceInfo> tests = null;

            using (HFContext hfdb = new HFContext())
            {
                
                    tests = (from pIns in hfdb.ProcessInstances
                             where (pIns.InstanceOf.EndDate == null) && !pIns.ContactInProcess.Deleted
                                       && pIns.CurrentPhase == Phase.Test_Phase && pIns.Interviewers.Where(x => x.InterviewerId == InterviewerId).FirstOrDefault() != null

                             select new ProcessInstanceInfo
                             {
                                 Id = pIns.Id,
                                 ProcessName = pIns.InstanceOf.Name,
                                 isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                 ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                 ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                 ProcessTechnologies = pIns.InstanceOf.Technologies,
                                 ContactSeniority = pIns.ContactInProcess.Seniority.ToString()
                             }).ToList();

                
            }

            return tests;
        }

        public ICollection<ProcessInstanceInfo> GetInterviewerInterviewInstances(int InterviewerId)
        {
            ICollection<ProcessInstanceInfo> interviews = null;

            using (HFContext hfdb = new HFContext())
            {
               
                    interviews = (from pIns in hfdb.ProcessInstances
                                  where (pIns.InstanceOf.EndDate == null) && !pIns.ContactInProcess.Deleted
                                            && pIns.CurrentPhase == Phase.Interview_Phase && pIns.Interviewers.Where(x => x.InterviewerId == InterviewerId).FirstOrDefault() != null

                                  select new ProcessInstanceInfo
                                  {
                                      Id = pIns.Id,
                                      ProcessName = pIns.InstanceOf.Name,
                                      isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                      ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                      ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                      ProcessTechnologies = pIns.InstanceOf.Technologies,
                                      ContactSeniority = pIns.ContactInProcess.Seniority.ToString()
               
                                  }).ToList();
                
            }

            return interviews;
        }

        public ICollection<ProcessInstanceInfo> GetInterviewerContactInstances(int InterviewerId)
        {
            ICollection<ProcessInstanceInfo> contacts = null;

            using (HFContext hfdb = new HFContext())
            {
               
                    contacts = (from pIns in hfdb.ProcessInstances
                                where (pIns.InstanceOf.EndDate == null) && !pIns.ContactInProcess.Deleted
                                          && pIns.CurrentPhase == Phase.Contact_Phase && pIns.Interviewers.Where(x => x.InterviewerId == InterviewerId).FirstOrDefault() != null
                                select new ProcessInstanceInfo
                                {
                                    Id = pIns.Id,
                                    ProcessName = pIns.InstanceOf.Name,
                                    ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                    ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                    ProcessTechnologies = pIns.InstanceOf.Technologies,
                                    ContactSeniority = pIns.ContactInProcess.Seniority.ToString()
                                   
                                }).ToList();

                }
            

            return contacts;
        }

        public void UpdateContact(ContactView newContact) {

            Contact c = db.Contacts.Find(newContact.Id);

            c.Email = newContact.Email;
            c.FirstName = newContact.FirstName;
            c.LastName = newContact.LastName;
            c.Phone = newContact.Phone;
            c.Seniority = newContact.Seniority;
            c.Location = newContact.Location;
            c.DateOfBirth = newContact.DateOfBirth;
            c.LinkedIn = newContact.LinkedIn;
            if (newContact.CVContent != null)
            {
                c.CVContent = newContact.CVContent;
                c.CVContentType = newContact.CVContentType;
            }
            c.Technologies = new Collection<Technology>();
           
            foreach (TechnologyView tech in newContact.Technologies)
            {

                Technology t = db.Technologies.Find(tech.Id);

                 c.Technologies.Add(t);

            }
            db.SaveChanges();
        }

        public ProcessInstance GetInstance(int instance)
        {
            ProcessInstance p = null;
            using(var data=new HFContext())
            {
                p = data.ProcessInstances.Include("Interviewers").Include("Annotations").Include("InstanceOf").Where(x=>x.Id==instance).SingleOrDefault();
            }
            return p;
        }

        public void UpdateInstance(ProcessInstance inst)
        {
            using (var data = new HFContext())
            {
                data.Entry(inst).State = EntityState.Modified;
                data.SaveChanges();
            }
         
        }

        public List<CommentView> GetInstanceComment(int instanceId, AnnotationType myStatus)
        {
            List<CommentView> lista = new List<CommentView>();
            using(var data=new HFContext())
            {
                lista = (from c in data.Annotations
                         where c.PInsId == instanceId && c.Type == myStatus
                         select new CommentView()
                         {
                             Id = c.Id,
                             AuthorName = c.Author.FirstName,
                             Message = c.Message,
                             Time = c.Time.ToString(),
                             Type=c.Type,
                             Hidden=c.Hidden

                         }).ToList();
            }
            return lista;
        }

        public void AddInterviewerOnPhase(Interviewer inter)
        {
            using (var data = new HFContext())
            {
                data.Interviewers.Add(inter);
                data.SaveChanges();
            }
        }

        public void AddNewContact(ContactView newContact)
        {
            Contact contact = new Contact
            {
                Email = newContact.Email,
                CVContent=newContact.CVContent,
                CVContentType=newContact.CVContentType,
                FirstName = newContact.FirstName,
                LastName = newContact.LastName,
                Phone = newContact.Phone,
                Seniority = newContact.Seniority,
                Location = newContact.Location,
                DateOfBirth = newContact.DateOfBirth,
                LinkedIn = newContact.LinkedIn,
                Technologies = new Collection<Technology>()
            };
            foreach (TechnologyView tech in newContact.Technologies)
            {

                Technology t = db.Technologies.Find(tech.Id);

                contact.Technologies.Add(t);

            }
            db.Contacts.Add(contact);
            db.SaveChanges();
        }

        public bool CheckMail(string Emaill)
        {
            int pom = (from s in db.Users where s.Email == Emaill select s).Count();
            if (pom > 0)
                return false;
            return true;
        }

        public void DeleteUser(int id)
        {
            User u = db.Users.Find(id);
            u.Deleted = true;
            db.SaveChanges();

        }

        public void DeleteContact(int id)
        {
            Contact c = db.Contacts.Find(id);
            c.Deleted = true;
            db.SaveChanges();
        }


        public ICollection<ProcessInstanceInfo> GetContactInstances(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> contacts = null;

            using (HFContext hfdb = new HFContext())
            {
                if (processId != 0)
                {
                    contacts = (from pIns in hfdb.ProcessInstances
                                where (pIns.ProcessId == processId) && !pIns.ContactInProcess.Deleted
                                          && pIns.CurrentPhase == Phase.Contact_Phase
                                //&& pIns.Contacted && pIns.ContactDate != null
                                select new ProcessInstanceInfo
                                {
                                    Id = pIns.Id,
                                    ProcessName = pIns.InstanceOf.Name,
                                    //isEnd=pIns.InstanceOf.EndDate!=null?true:false,
                                    ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                    ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                    ProcessTechnologies = pIns.InstanceOf.Technologies,
                                    ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                    Date = pIns.ContactDate,
                                    //ContactTechnologies = pIns.ContactInProcess.Technologies.Aggregate((curr, next) => new Technology { Name = curr.Name + "  " + next.Name }).Name;
                                    ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                    // ContactTechnologies = pIns.ContactInProcess.Technologies.Select(x=>" "+x.Name).Aggregate((curr, next) => curr + " " + next)
                                }).ToList();
                }
                else
                {
                    contacts = (from pIns in hfdb.ProcessInstances
                                where (pIns.InstanceOf.EndDate==null) && !pIns.ContactInProcess.Deleted
                                          && pIns.CurrentPhase == Phase.Contact_Phase
                                //&& pIns.Contacted && pIns.ContactDate != null
                                select new ProcessInstanceInfo
                                {
                                    Id = pIns.Id,
                                    ProcessName = pIns.InstanceOf.Name,
                                    //isEnd=pIns.InstanceOf.EndDate!=null?true:false,
                                    ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                    ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                    ProcessTechnologies = pIns.InstanceOf.Technologies,
                                    ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                    Date = pIns.ContactDate,
                                    ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                    // ContactTechnologies = pIns.ContactInProcess.Technologies.Aggregate((curr, next) => new Technology { Name = curr.Name + "  " + next.Name }).Name
                                    //ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                    // ContactTechnologies = pIns.ContactInProcess.Technologies.Select(x=>" "+x.Name).Aggregate((curr, next) => curr + " " + next)
                                }).ToList();

                }
            }

            return contacts;
        }

       


        public ICollection<ProcessInstanceInfo> GetInterviewInstances(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> interviews = null;

            using (HFContext hfdb = new HFContext())
            {
                if (processId != 0)
                {
                    interviews = (from pIns in hfdb.ProcessInstances
                                  where (pIns.ProcessId == processId) && !pIns.ContactInProcess.Deleted
                                            && pIns.CurrentPhase == Phase.Interview_Phase
                                  //&& pIns.InterviewScheduled && pIns.InterviewNoticed
                                  //&& pIns.InterviewDate != null
                                  //&& pIns.Interviewers.SingleOrDefault(intw =>
                                  //    intw.Type == Data.Phase.Interview_Phase) != null
                                  select new ProcessInstanceInfo
                                  {
                                      Id = pIns.Id,
                                      ProcessName = pIns.InstanceOf.Name,
                                      isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                      ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                      ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                      ProcessTechnologies = pIns.InstanceOf.Technologies,
                                      ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                      ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                      // ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)
                                  }).ToList();
                }
                else
                {
                    interviews = (from pIns in hfdb.ProcessInstances
                                  where (pIns.InstanceOf.EndDate==null) && !pIns.ContactInProcess.Deleted
                                            && pIns.CurrentPhase == Phase.Interview_Phase
                                  //&& pIns.InterviewScheduled && pIns.InterviewNoticed
                                  //&& pIns.InterviewDate != null
                                  //&& pIns.Interviewers.SingleOrDefault(intw =>
                                  //    intw.Type == Data.Phase.Interview_Phase) != null
                                  select new ProcessInstanceInfo
                                  {
                                      Id = pIns.Id,
                                      ProcessName = pIns.InstanceOf.Name,
                                      isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                      ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                      ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                      ProcessTechnologies = pIns.InstanceOf.Technologies,
                                      ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                      ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                      // ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)
                                  }).ToList();
                }
            }

            return interviews;
        }

       

        public ICollection<ProcessInstanceInfo> GetTestInstances(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> tests = null;

            using (HFContext hfdb = new HFContext())
            {
                if (processId != 0)
                {
                    tests = (from pIns in hfdb.ProcessInstances
                             where (pIns.ProcessId == processId) && !pIns.ContactInProcess.Deleted
                                       && pIns.CurrentPhase == Phase.Test_Phase
                             //&& pIns.InterviewHeld && pIns.Annotations.SingleOrDefault(anno =>
                             //                             anno.Type == Data.Phase.Interview_Phase) != null
                             //&& pIns.TestScheduled && pIns.TestNoticed && pIns.TestDate != null
                             //&& pIns.Interviewers.SingleOrDefault(intw =>
                             //    intw.Type == Data.Phase.Test_Phase) != null
                             select new ProcessInstanceInfo
                             {
                                 Id = pIns.Id,
                                 ProcessName = pIns.InstanceOf.Name,
                                 isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                 ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                 ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                 ProcessTechnologies = pIns.InstanceOf.Technologies,
                                 ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                 ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                 // ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)
                             }).ToList();
                }
                else
                {
                    tests = (from pIns in hfdb.ProcessInstances
                             where (pIns.InstanceOf.EndDate==null) && !pIns.ContactInProcess.Deleted
                                       && pIns.CurrentPhase == Phase.Test_Phase
                             //&& pIns.InterviewHeld && pIns.Annotations.SingleOrDefault(anno =>
                             //                             anno.Type == Data.Phase.Interview_Phase) != null
                             //&& pIns.TestScheduled && pIns.TestNoticed && pIns.TestDate != null
                             //&& pIns.Interviewers.SingleOrDefault(intw =>
                             //    intw.Type == Data.Phase.Test_Phase) != null
                             select new ProcessInstanceInfo
                             {
                                 Id = pIns.Id,
                                 ProcessName = pIns.InstanceOf.Name,
                                 isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                 ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                 ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                 ProcessTechnologies = pIns.InstanceOf.Technologies,
                                 ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                 ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                 // ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)
                             }).ToList();

                }
            }

            return tests;
        }
      

        public ICollection<ProcessInstanceInfo> GetTestDefenseInstances(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> testDefenses = null;

            using (HFContext hfdb = new HFContext())
            {
                if (processId != 0)
                {
                    testDefenses = (from pIns in hfdb.ProcessInstances
                                    where (pIns.ProcessId == processId) /*&& !pIns.ContactInProcess.Deleted*/
                                              && pIns.CurrentPhase == Phase.TestDefense_Phase
                                    //&& pIns.TestHeld && pIns.Annotations.SingleOrDefault(anno =>
                                    //                        anno.Type == Data.Phase.Test_Phase) != null
                                    //&& pIns.TestDefenseScheduled && pIns.TestDefenseNoticed
                                    //&& pIns.TestDefenseDate != null
                                    //&& pIns.Interviewers.SingleOrDefault(intw =>
                                    //    intw.Type == Data.Phase.TestDefense_Phase) != null
                                    select new ProcessInstanceInfo
                                    {
                                        Id = pIns.Id,
                                        ProcessName = pIns.InstanceOf.Name,
                                        isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                        ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                        ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                        ProcessTechnologies = pIns.InstanceOf.Technologies,
                                        ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                        ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                        //  ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)
                                    }).ToList();
                }
                else
                {
                    testDefenses = (from pIns in hfdb.ProcessInstances
                                    where (pIns.InstanceOf.EndDate==null) /*&& !pIns.ContactInProcess.Deleted*/
                                              && pIns.CurrentPhase == Phase.TestDefense_Phase
                                    //&& pIns.TestHeld && pIns.Annotations.SingleOrDefault(anno =>
                                    //                        anno.Type == Data.Phase.Test_Phase) != null
                                    //&& pIns.TestDefenseScheduled && pIns.TestDefenseNoticed
                                    //&& pIns.TestDefenseDate != null
                                    //&& pIns.Interviewers.SingleOrDefault(intw =>
                                    //    intw.Type == Data.Phase.TestDefense_Phase) != null
                                    select new ProcessInstanceInfo
                                    {
                                        Id = pIns.Id,
                                        ProcessName = pIns.InstanceOf.Name,
                                        isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                        ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                        ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                        ProcessTechnologies = pIns.InstanceOf.Technologies,
                                        ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                        ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                        //  ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)
                                    }).ToList();

                }
            }

            return testDefenses;
        }

       

        public ICollection<ProcessInstanceInfo> GetOfferInstances(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> offers = null;

            using (HFContext hfdb = new HFContext())
            {
                if (processId != 0)
                {
                    offers = (from pIns in hfdb.ProcessInstances
                              where (pIns.ProcessId == processId) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Offer_Phase
                              //&& pIns.TestDefenseHeld && pIns.Annotations.SingleOrDefault(anno =>
                              //                               anno.Type == Data.Phase.TestDefense_Phase) != null
                              select new ProcessInstanceInfo
                              {
                                  Id = pIns.Id,
                                  ProcessName = pIns.InstanceOf.Name,
                                  isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                  ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                  ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                  ProcessTechnologies = pIns.InstanceOf.Technologies,
                                  ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                  ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                  // ContactTechnologies  = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)

                              }).ToList();
                }
                else
                {
                    offers = (from pIns in hfdb.ProcessInstances
                              where (pIns.InstanceOf.EndDate==null) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Offer_Phase
                              //&& pIns.TestDefenseHeld && pIns.Annotations.SingleOrDefault(anno =>
                              //                               anno.Type == Data.Phase.TestDefense_Phase) != null
                              select new ProcessInstanceInfo
                              {
                                  Id = pIns.Id,
                                  ProcessName = pIns.InstanceOf.Name,
                                  isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                  ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                  ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                  ProcessTechnologies = pIns.InstanceOf.Technologies,
                                  ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                  ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                  // ContactTechnologies  = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)

                              }).ToList();

                }
            }

            return offers;
        }

       

        public ICollection<ProcessInstanceInfo> GetAcceptInstances(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> accepts = null;

            using (HFContext hfdb = new HFContext())
            {
                if (processId != 0)
                {
                    accepts = (from pIns in hfdb.ProcessInstances
                               where (pIns.ProcessId == processId) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Acceptance_Phase
                               //&& pIns.OfferSent && pIns.OfferDate != null
                               //&& pIns.Accepted && pIns.AcceptanceDate != null // da li treba?
                               //&& pIns.Annotations.SingleOrDefault(anno =>
                               //    anno.Hidden && anno.Type == Data.Phase.Offer_Phase) != null
                               select new ProcessInstanceInfo
                               {
                                   Id = pIns.Id,
                                   ProcessName = pIns.InstanceOf.Name,
                                   isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                   ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                   ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                   ProcessTechnologies = pIns.InstanceOf.Technologies,
                                   ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                   ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                   //ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)

                               }).ToList();
                }
                else
                {
                    accepts = (from pIns in hfdb.ProcessInstances
                               where (pIns.InstanceOf.EndDate==null) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Acceptance_Phase
                               //&& pIns.OfferSent && pIns.OfferDate != null
                               //&& pIns.Accepted && pIns.AcceptanceDate != null // da li treba?
                               //&& pIns.Annotations.SingleOrDefault(anno =>
                               //    anno.Hidden && anno.Type == Data.Phase.Offer_Phase) != null
                               select new ProcessInstanceInfo
                               {
                                   Id = pIns.Id,
                                   ProcessName = pIns.InstanceOf.Name,
                                   isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                   ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                   ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                   ProcessTechnologies = pIns.InstanceOf.Technologies,
                                   ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                   ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                   //ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)

                               }).ToList();
                }
            }

            return accepts;
        }

        public ToDoItem AddToDo(int instanceId, string message)
        {
            ToDoItem toDo = new ToDoItem();
            using (HFContext hfdb = new HFContext())
            {
                toDo = new ToDoItem { Completed = false, PInsId = instanceId, Message = message, Time = DateTime.Now };
                hfdb.ToDoItems.Add(toDo);
                hfdb.SaveChanges();
                toDo.Id = toDo.Id;
            }
            return toDo;
       
        }

        public List<ToDoItem> GetToDo(int instanceId)
        {
            List<ToDoItem> list = new List<ToDoItem>();
            using (HFContext hfdb = new HFContext())
            {
                list = (from t in hfdb.ToDoItems where t.PInsId == instanceId select t).ToList();
            }
            return list;
        }

        public void DisableToDo(int id)
        {
            using (HFContext hfdb = new HFContext())
            {
                ToDoItem item = hfdb.ToDoItems.Find(id);
                item.Completed = true;
                hfdb.SaveChanges();
            }
        }

        public ICollection<ProcessInstanceInfo> GetRejectInstances(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> rejects = null;

            using (HFContext hfdb = new HFContext())
            {
                if (processId != 0)
                {
                    rejects = (from pIns in hfdb.ProcessInstances
                               where (pIns.ProcessId == processId) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Rejection_Phase
                               //&& pIns.Rejected && pIns.RejectionDate != null
                               //&& pIns.Annotations.SingleOrDefault(anno =>
                               //    anno.Type == Data.Phase.Rejection_Phase) != null
                               select new ProcessInstanceInfo
                               {
                                   Id = pIns.Id,
                                   ProcessName = pIns.InstanceOf.Name,
                                   isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                   ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                   ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                   ProcessTechnologies = pIns.InstanceOf.Technologies,
                                   ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                   ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                   // ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)

                               }).ToList();
                }
                else
                {
                    rejects = (from pIns in hfdb.ProcessInstances
                               where (pIns.InstanceOf.EndDate==null) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Rejection_Phase
                               //&& pIns.Rejected && pIns.RejectionDate != null
                               //&& pIns.Annotations.SingleOrDefault(anno =>
                               //    anno.Type == Data.Phase.Rejection_Phase) != null
                               select new ProcessInstanceInfo
                               {
                                   Id = pIns.Id,
                                   ProcessName = pIns.InstanceOf.Name,
                                   isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                   ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                   ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                   ProcessTechnologies = pIns.InstanceOf.Technologies,
                                   ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                   ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                   // ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)

                               }).ToList();

                }
            }

            return rejects;
        }

      

        public ICollection<ProcessInstanceInfo> GetQuitInstances(int processId = 0)
        {
            ICollection<ProcessInstanceInfo> quits = null;

            using (HFContext hfdb = new HFContext())
            {
                if (processId != 0)
                {
                    quits = (from pIns in hfdb.ProcessInstances
                             where (pIns.ProcessId == processId) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Quit_Phase
                             //&& pIns.Quit && pIns.QuitDate != null
                             //&& pIns.Annotations.SingleOrDefault(anno =>
                             //    anno.Type == Data.Phase.Quit_Phase) != null
                             select new ProcessInstanceInfo
                             {
                                 Id = pIns.Id,
                                 ProcessName = pIns.InstanceOf.Name,
                                 isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                 ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                 ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                 ProcessTechnologies = pIns.InstanceOf.Technologies,
                                 ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                 ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                 //  ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)

                             }).ToList();
                }
                else
                {
                    quits = (from pIns in hfdb.ProcessInstances
                             where (pIns.InstanceOf.EndDate==null) && !pIns.ContactInProcess.Deleted
                                        && pIns.CurrentPhase == Phase.Quit_Phase
                             //&& pIns.Quit && pIns.QuitDate != null
                             //&& pIns.Annotations.SingleOrDefault(anno =>
                             //    anno.Type == Data.Phase.Quit_Phase) != null
                             select new ProcessInstanceInfo
                             {
                                 Id = pIns.Id,
                                 ProcessName = pIns.InstanceOf.Name,
                                 isEnd = pIns.InstanceOf.EndDate != null ? true : false,
                                 ContactName = pIns.ContactInProcess.FirstName + " " + pIns.ContactInProcess.LastName,
                                 ProcessSeniority = pIns.InstanceOf.Seniority.ToString(),
                                 ProcessTechnologies = pIns.InstanceOf.Technologies,
                                 ContactSeniority = pIns.ContactInProcess.Seniority.ToString(),
                                 ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name)
                                 //  ContactTechnologies = (from t in pIns.ContactInProcess.Technologies select " " + t.Name).ToList().Aggregate((curr, next) => curr + " " + next)

                             }).ToList();

                }
            }

            return quits;
        }

       




        public List<ProcessView> GetAllNotContactProcess (int ContactId)
        {
            List<ProcessView> lista = new List<ProcessView>();

            using(var data=new HFContext())
            {
                lista = (from p in data.Processes where p.Instances.Where(x => x.ContactId == ContactId).Count() == 0 select new ProcessView { Id = p.Id, Name = p.Name }).ToList();
            }

            return lista;
        }



        public bool IsCntactInstances(ProcessInstance pIns)
        {
            if(pIns.ContactDate != null)
            {
                pIns.CurrentPhase = Phase.Contact_Phase;
                return true;
            }
            return false;
        }

        public bool IsInterviewInstances(ProcessInstance pIns)
        {
            if (pIns.InterviewScheduled && pIns.InterviewNoticed
                           && pIns.InterviewDate != null
                           && pIns.Interviewers.FirstOrDefault(intw =>
                          intw.Type == AnnotationType.Interview_Feedback) != null)
            {
                pIns.CurrentPhase = Phase.Interview_Phase;
                return true;
            }
            return false;
        }

        public bool IsTestInstances(ProcessInstance pIns)
        {
            if (pIns.InterviewHeld && pIns.Annotations.FirstOrDefault(anno =>
                          anno.Type == AnnotationType.Interview_Feedback) != null
                         && pIns.TestScheduled && pIns.TestNoticed && pIns.TestDate != null
                         && pIns.Interviewers.FirstOrDefault(intw =>
                         intw.Type == AnnotationType.Test_Feedback) != null)
            {
                pIns.CurrentPhase = Phase.Test_Phase;
                return true;
            }
            return false;
        }

        public bool IsTestDefenseInstances(ProcessInstance pIns)
        {
            if (pIns.TestHeld && pIns.Annotations.FirstOrDefault(anno =>
                                 anno.Type == AnnotationType.Test_Feedback) != null
                                && pIns.TestDefenseScheduled && pIns.TestDefenseNoticed
                               && pIns.TestDefenseDate != null
                                && pIns.Interviewers.FirstOrDefault(intw =>
                                intw.Type == AnnotationType.TestDefense_Feedback) != null)
            {
                pIns.CurrentPhase = Phase.TestDefense_Phase;
                return true;
            }
            return false;
        }

        public bool IsOfferInstances(ProcessInstance pIns)
        {
            if (pIns.TestDefenseHeld && pIns.Annotations.FirstOrDefault(anno =>
                 anno.Type == AnnotationType.TestDefense_Feedback) != null)
            {
                pIns.CurrentPhase = Phase.Offer_Phase;
                return true;
            }
            return false;
        }

        public bool IsAcceptInstance(ProcessInstance pIns)
        {
            if (pIns.OfferSent && pIns.Accepted && 
                    pIns.Annotations.FirstOrDefault(anno =>
                      anno.Hidden && anno.Type == AnnotationType.Offer_Comment) != null)
            {
                pIns.CurrentPhase = Phase.Acceptance_Phase;
                return true;

            }
            return false;
        }

        public bool IsRejectInstances(ProcessInstance pIns)
        {
            if (pIns.Rejected /*&& pIns.RejectionDate != null*/
                 && pIns.Annotations.FirstOrDefault(anno =>
                   anno.Type == AnnotationType.Rejection_State) != null)
            {
                pIns.CurrentPhase = Phase.Rejection_Phase;
                return true;
            }
            return false;

        }

        public bool IsQuitInstances(ProcessInstance pIns)
        {
            if (pIns.Quit && /*pIns.QuitDate != null*/
                  pIns.Annotations.FirstOrDefault(anno => anno.Type == AnnotationType.Quit_State) != null)
            {
                pIns.CurrentPhase = Phase.Quit_Phase;
                return true;
            }
            return false;

        }






    }
}
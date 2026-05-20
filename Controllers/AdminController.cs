using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TrustPlus.Models;

namespace TrustPlus.Controllers
{
    //[Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly DbManager _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(DbManager db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Dashboard()
        {
            var data = _db.Job_Master.ToList();

           // foreach (var item in data)
          //  {
              //  var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);

                //var completed = _db.Verification_Status.Count(x =>
                //    x.ClientId == item. &&
                //    x.Status == "Completed");

               // item.Progress = total > 0 ? (completed * 100 / total) : 0;
           // }

           var pending = _db.Job_Master
    .Count(x => x.Status == "Pending");

            var inProgress = _db.Job_Master
                .Count(x => x.Status == "In Progress");

            var complete = _db.Job_Master
               .Count(x => x.Status == "Completed");

           ViewBag.Pending = pending;
         ViewBag.InProgress = inProgress;
           ViewBag.Completed = complete;
            ViewBag.d=data.Count();
             data = _db.Job_Master
               .OrderByDescending(x => x.JobId)
               .ToList();

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NewRequest()
        {
           var data = _db.Verification_Master.ToList();
            ViewBag.Data = data;
            return View();
        }
        public IActionResult RequestMgmt()
        {
            //var Data = (from jm in _db.Job_Master
            //            join jd in _db.Job_Dtl on jm.JobId equals jd.JobId
            //            join cm in _db.Client_Master on jm.ClientId equals cm.ClientId // Fixed: linked to jd instead of jm
            //            select new
            //            {
            //                JobMaster = jm,
            //                JobDetail = jd,
            //                ClientMaster = cm
            //            })
            //.ToList();


            //var Data = _db.Job_Master
            //      .Where(jm => _db.Job_Dtl.Any(jd => jd.JobId == jm.JobId && jd.AssignedTo == null))

            //      .ToList();

            var Data = (from jm in _db.Job_Master
                        join cm in _db.Client_Master
                            on jm.ClientId equals cm.ClientId
                        select new Job_Master
                        {
                            JobId = jm.JobId,
                            ClientId = jm.ClientId,
                            EmployeeName = jm.EmployeeName,
                            EmailAddress = jm.EmailAddress,
                            PhoneNumber = jm.PhoneNumber,
                            PostionApplied = jm.PostionApplied,
                            Department = jm.Department,
                            JobDate = jm.JobDate,
                            Status = jm.Status,
                            Priority = jm.Priority,
                            ClientName = cm.ClientName   // extra property
                        }).ToList();




            //           var data = _db.Client_Dtl
            //  
            //   //
            //   .Where(c => c.Emp_Id == null)
            //     .ToList();
            //
            //
            //
            //
            //
            //            foreach (var item in data)
            //            {
            //                var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);
            //
            //                var completed = _db.Verification_Status.Count(x =>
            //                    x.ClientId == item.Id &&
            //                    x.Status == "Completed");
            //
            //                item.Progress = total > 0 ? (completed * 100 / total) : 0;
            //            }
            //
            //            var pending = _db.Client_Dtl
            //                    .Where(c => c.Emp_Id == null)
            //    .Count(x => x.Status == "Pending");
            //
            //            var inProgress = _db.Client_Dtl
            //                    .Where(c => c.Emp_Id == null)
            //                .Count(x => x.Status == "In Progress");
            //
            //            var complete = _db.Client_Dtl.Where(c => c.Emp_Id == null)
            //                .Count(x => x.Status == "Completed");
            //
            //            ViewBag.Pending = pending;
            //            ViewBag.InProgress = inProgress;
            //            ViewBag.Completed = complete;
            //            //var data = _db.Client_Dtl
            //            //    .OrderByDescending(x => x.Id)
            //    .ToList();

            return View(Data);
        }

        public IActionResult EmployeeMst()
        {
           // var employee = _db.EmployeeMst.ToList();
            return View();
        }
        [HttpGet]
        public IActionResult GetData()
        {
            var employee = _db.User_Master
                
                .ToList();
            foreach (var emp in employee)
            {
                emp.Password = "********";
            }
            return Json(employee);
        }
        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies=_db.Client_Master.Select(c=>new
            { c.ClientId, c.ClientName }).ToList();

            return Json(companies);
        }
        [HttpPost]
        public IActionResult AddEmployee([FromForm]User_Master model)
        {
          
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                    throw new ArgumentException("Password is required");

                //  Encrypt password before saving
                model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                _db.User_Master.Add(model);
                _db.SaveChanges();
                return Json(new { success = true, message = "Employee saved successfully!" });
            }
            return Json(new { success = false, message = "Validation failed." });
        }
        [HttpGet]
        public IActionResult GetEmployeeById(int id)
        {
            var employee = _db.User_Master.FirstOrDefault(e => e.UserId == id);
            if (employee != null)
            {
                return Json(employee);
            }
            return Json(new { success = false, message = "Employee  not found" });

        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            var emp = _db.User_Master.FirstOrDefault(e => e.UserId == id);
            if (emp != null)
            {
                _db.User_Master.Remove(emp);
                _db.SaveChanges();
                return Json(new { success = true, message = "Employee deleted successfully!" });
            }
            return Json(new { success = false, message = "Employee not found." });
        }

        [HttpPost]
        public IActionResult UpdateEmployee([FromForm] User_Master model)
        {
            if (ModelState.IsValid)
            {
                var employee = _db.User_Master.FirstOrDefault(e => e.UserId == model.UserId);
                if (employee != null)
                {
                    //update filds
                    employee.UserName = model.UserName;
                    employee.EmailAddress = model.EmailAddress;
                    employee.MobileNumber = model.MobileNumber;
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                   
                        employee.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    }
                    employee.LoginId = model.LoginId;
                    employee.UpdatedAt = DateTime.Now;

                    _db.SaveChanges();

                    return Json(new { success = true, message = "Employee Updated Successfully" });
                }
              
            }
            return Json(new { success = false, message = "Employee Not Updated" });
        }

        public IActionResult AssignedJob()
        {

            //      var data = _db.Job_Master
            //.Select(jm => new JobWithDetailsViewModel
            //{
            //    JobId = jm.JobId,
            //    ClientId = jm.ClientId,
            //    CaseId = jm.CaseId,
            //    EmployeeName = jm.EmployeeName,
            //    EmailAddress = jm.EmailAddress,
            //    PhoneNumber = jm.PhoneNumber,
            //    PostionApplied = jm.PostionApplied,
            //    Department = jm.Department,
            //    Priority = jm.Priority,
            //    Status = jm.Status,
            //    JobDate = jm.JobDate,
            //    //FinalReport = jm.FinalReport,
            //    //FinalSummary = jm.FinalSummary,
            //    CreatedAt = jm.CreatedAt,
            //    UpdatedAt = jm.UpdatedAt,

            //    JobDetails = (from jd in _db.Job_Dtl
            //                  join cm in _db.Client_Master
            //                      on jd.AssignedTo equals cm.ClientId into cmGroup
            //                  from cm in cmGroup.DefaultIfEmpty()
            //                  where jd.JobId == jm.JobId
            //                  select new JobDtlViewModel
            //                  {
            //                      JobDtlId = jd.JobDtlId,
            //                      JobId = jd.JobId,
            //                      Status = jd.Status,

            //                      AssignedTo = jd.AssignedTo,
            //                      AssignedToName = cm != null ? cm.ClientName : null
            //                  }).ToList()
            //}).ToList();

            //var data = (from jm in _db.Job_Master
            //            join jd in _db.Job_Dtl on jm.JobId equals jd.JobId into jdGroup
            //            from jd in jdGroup.DefaultIfEmpty()   // left join

            //            join u in _db.User_Master on jd.AssignedTo equals u.UserId into uGroup
            //            from u in uGroup.DefaultIfEmpty()     // left join

            //            select new JobWithDetailsViewModel
            //            {
            //                JobId = jm.JobId,
            //                ClientId = jm.ClientId,
            //                CaseId = jm.CaseId,
            //                EmployeeName = jm.EmployeeName,
            //                EmailAddress = jm.EmailAddress,
            //                PhoneNumber = jm.PhoneNumber,
            //                PostionApplied = jm.PostionApplied,
            //                Department = jm.Department,
            //                Priority = jm.Priority,
            //                Status = jm.Status,
            //                JobDate = jm.JobDate,
            //                CreatedAt = jm.CreatedAt,
            //                UpdatedAt = jm.UpdatedAt,
            //                AssignedUserName = u != null ? u.UserName : null,

            //                // If you want JobDetails populated, you can map them here
            //                //  JobDetails = jd != null
            //                //      ? new List<JobDtlViewModel>
            //                //        {
            //                //new JobDtlViewModel
            //                //{
            //                //    JobDtlId = jd.JobDtlId,
            //                //    AssignedTo = jd.AssignedTo,
            //                //    // add other fields from Job_Dtl if needed
            //                //}
            //                //        }
            //                //      : new List<JobDtlViewModel>()

            //            })
            // .Take(1000)
            // .ToList();
            var data = (from jm in _db.Job_Master
                        join jd in _db.Job_Dtl on jm.JobId equals jd.JobId into jdGroup
                        from jd in jdGroup.DefaultIfEmpty()   // left join

                        join u in _db.User_Master on jd.AssignedTo equals u.UserId into uGroup
                        from u in uGroup.DefaultIfEmpty()     // left join

                        join cm in _db.Client_Master on jm.ClientId equals cm.ClientId into cmGroup
                        from cm in cmGroup.DefaultIfEmpty()   // left join

                            // Group by the JobMaster details
                        group new { jd, u } by new
                        {
                            jm.JobId,
                            jm.ClientId,
                            jm.CaseId,
                            jm.EmployeeName,
                            jm.EmailAddress,
                            jm.PhoneNumber,
                            jm.PostionApplied,
                            jm.Department,
                            jm.Priority,
                            jm.Status,
                            jm.JobDate,
                            jm.CreatedAt,
                            jm.UpdatedAt,
                            ClientName = cm.ClientName
                        } into grouped

                        select new
                        {
                            Key = grouped.Key,
                            // Get the raw list of user names and details first (SQL friendly)
                            UserNames = grouped.Where(g => g.u != null).Select(g => g.u.UserName).Distinct(),
                            Details = grouped.Where(g => g.jd != null)
                                             .Select(g => new JobDtlViewModel
                                             {
                                                 JobDtlId = g.jd.JobDtlId,
                                                 AssignedTo = g.jd.AssignedTo
                                             }).ToList()
                        })
                        .Take(1000)
                        .AsEnumerable() 
                        .Select(x => new JobWithDetailsViewModel
                        {
                            JobId = x.Key.JobId,
                            ClientId = x.Key.ClientId,
                            CaseId = x.Key.CaseId,
                            EmployeeName = x.Key.EmployeeName,
                            EmailAddress = x.Key.EmailAddress,
                            PhoneNumber = x.Key.PhoneNumber,
                            PostionApplied = x.Key.PostionApplied,
                            Department = x.Key.Department,
                            Priority = x.Key.Priority,
                            Status = x.Key.Status,
                            JobDate = x.Key.JobDate,
                            CreatedAt = x.Key.CreatedAt,
                            UpdatedAt = x.Key.UpdatedAt,

                            ClientName = x.Key.ClientName,

                            // string.Join runs perfectly fine here in-memory
                            AssignedUserName = string.Join(", ", x.UserNames),
                            JobDetails = x.Details
                        }).Where(x=>x.Status!="Completed")
                        .ToList();

            ViewBag.Pending = data.Count(x => x.Status == "Pending");
            ViewBag.InProgress = data.Count(x => x.Status == "In Progress");
            ViewBag.Completed = data.Count(x => x.Status == "Completed");

            return View(data);

        }






        public IActionResult CompletedJob()
        {

            //      var data = _db.Job_Master
            //.Select(jm => new JobWithDetailsViewModel
            //{
            //    JobId = jm.JobId,
            //    ClientId = jm.ClientId,
            //    CaseId = jm.CaseId,
            //    EmployeeName = jm.EmployeeName,
            //    EmailAddress = jm.EmailAddress,
            //    PhoneNumber = jm.PhoneNumber,
            //    PostionApplied = jm.PostionApplied,
            //    Department = jm.Department,
            //    Priority = jm.Priority,
            //    Status = jm.Status,
            //    JobDate = jm.JobDate,
            //    //FinalReport = jm.FinalReport,
            //    //FinalSummary = jm.FinalSummary,
            //    CreatedAt = jm.CreatedAt,
            //    UpdatedAt = jm.UpdatedAt,

            //    JobDetails = (from jd in _db.Job_Dtl
            //                  join cm in _db.Client_Master
            //                      on jd.AssignedTo equals cm.ClientId into cmGroup
            //                  from cm in cmGroup.DefaultIfEmpty()
            //                  where jd.JobId == jm.JobId
            //                  select new JobDtlViewModel
            //                  {
            //                      JobDtlId = jd.JobDtlId,
            //                      JobId = jd.JobId,
            //                      Status = jd.Status,

            //                      AssignedTo = jd.AssignedTo,
            //                      AssignedToName = cm != null ? cm.ClientName : null
            //                  }).ToList()
            //}).ToList();

            //var data = (from jm in _db.Job_Master
            //            join jd in _db.Job_Dtl on jm.JobId equals jd.JobId into jdGroup
            //            from jd in jdGroup.DefaultIfEmpty()   // left join

            //            join u in _db.User_Master on jd.AssignedTo equals u.UserId into uGroup
            //            from u in uGroup.DefaultIfEmpty()     // left join

            //            select new JobWithDetailsViewModel
            //            {
            //                JobId = jm.JobId,
            //                ClientId = jm.ClientId,
            //                CaseId = jm.CaseId,
            //                EmployeeName = jm.EmployeeName,
            //                EmailAddress = jm.EmailAddress,
            //                PhoneNumber = jm.PhoneNumber,
            //                PostionApplied = jm.PostionApplied,
            //                Department = jm.Department,
            //                Priority = jm.Priority,
            //                Status = jm.Status,
            //                JobDate = jm.JobDate,
            //                CreatedAt = jm.CreatedAt,
            //                UpdatedAt = jm.UpdatedAt,
            //                AssignedUserName = u != null ? u.UserName : null,

            //                // If you want JobDetails populated, you can map them here
            //                //  JobDetails = jd != null
            //                //      ? new List<JobDtlViewModel>
            //                //        {
            //                //new JobDtlViewModel
            //                //{
            //                //    JobDtlId = jd.JobDtlId,
            //                //    AssignedTo = jd.AssignedTo,
            //                //    // add other fields from Job_Dtl if needed
            //                //}
            //                //        }
            //                //      : new List<JobDtlViewModel>()

            //            })
            // .Take(1000)
            // .ToList();
            var data = (from jm in _db.Job_Master
                        join jd in _db.Job_Dtl on jm.JobId equals jd.JobId into jdGroup
                        from jd in jdGroup.DefaultIfEmpty()   // left join

                        join u in _db.User_Master on jd.AssignedTo equals u.UserId into uGroup
                        from u in uGroup.DefaultIfEmpty()     // left join

                        join cm in _db.Client_Master on jm.ClientId equals cm.ClientId into cmGroup
                        from cm in cmGroup.DefaultIfEmpty()   // left join

                            // Group by the JobMaster details
                        group new { jd, u } by new
                        {
                            jm.JobId,
                            jm.ClientId,
                            jm.CaseId,
                            jm.EmployeeName,
                            jm.EmailAddress,
                            jm.PhoneNumber,
                            jm.PostionApplied,
                            jm.Department,
                            jm.Priority,
                            jm.Status,
                            jm.JobDate,
                            jm.CreatedAt,
                            jm.UpdatedAt,
                            ClientName = cm.ClientName
                        } into grouped

                        select new
                        {
                            Key = grouped.Key,
                            // Get the raw list of user names and details first (SQL friendly)
                            UserNames = grouped.Where(g => g.u != null).Select(g => g.u.UserName).Distinct(),
                            Details = grouped.Where(g => g.jd != null)
                                             .Select(g => new JobDtlViewModel
                                             {
                                                 JobDtlId = g.jd.JobDtlId,
                                                 AssignedTo = g.jd.AssignedTo
                                             }).ToList()
                        })
                        .Take(1000)
                        .AsEnumerable() // <--- CRITICAL: Switches execution to memory so string.Join works
                        .Select(x => new JobWithDetailsViewModel
                        {
                            JobId = x.Key.JobId,
                            ClientId = x.Key.ClientId,
                            CaseId = x.Key.CaseId,
                            EmployeeName = x.Key.EmployeeName,
                            EmailAddress = x.Key.EmailAddress,
                            PhoneNumber = x.Key.PhoneNumber,
                            PostionApplied = x.Key.PostionApplied,
                            Department = x.Key.Department,
                            Priority = x.Key.Priority,
                            Status = x.Key.Status,
                            JobDate = x.Key.JobDate,
                            CreatedAt = x.Key.CreatedAt,
                            UpdatedAt = x.Key.UpdatedAt,
                            ClientName = x.Key.ClientName,
                            // string.Join runs perfectly fine here in-memory
                            AssignedUserName = string.Join(", ", x.UserNames),
                            JobDetails = x.Details
                        }).Where(x => x.Status == "Completed")
                        .ToList();

            ViewBag.Pending = data.Count(x => x.Status == "Pending");
            ViewBag.InProgress = data.Count(x => x.Status == "In Progress");
            ViewBag.Completed = data.Count(x => x.Status == "Completed");

            return View(data);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> CreateRequest(VerificationVM model)
        {
            if (ModelState.IsValid)
            {
                string fileName =null;

               
                if (model.DocumentPath != null && model.DocumentPath.Length > 0)
                {
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    
                    fileName = Guid.NewGuid().ToString() + "_" + model.DocumentPath.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DocumentPath.CopyToAsync(stream);
                    }
                }

                var newRequest = new ClientDtl
                {
                    Name = model.Name,
            
                    Email = model.Email,
                    Mobile = model.Mobile,
                    DOB = model.DOB,
                    Position = model.Position,
                    Department = model.Department,
                    Priority = model.Priority,
                    Client_Id=model.Client_Id,
                    Emp_Id=model.Emp_Id,
                    DocumentPath = fileName,
                    VerifyType = model.VerifyTypes != null
                       ? string.Join(",", model.VerifyTypes)
                       : "",

                    CreatedAt = DateTime.Now,
                    Status = "Pending"
                };

                _db.Client_Dtl.Add(newRequest);
                await _db.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }
            return View(model);
        }

        [HttpPost]
                public async Task<IActionResult> NewRequest(Job_Master model)
        {
            if (ModelState.IsValid)
            {
                // 1. Handle Final Report Upload
                //if (model.FinalReportFile != null)
                //{
                //    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "finalreports");
                //    if (!Directory.Exists(uploadsFolder))
                //    {
                //        Directory.CreateDirectory(uploadsFolder);
                //    }

                //    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.FinalReportFile.FileName);
                //    var filePath = Path.Combine(uploadsFolder, fileName);

                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await model.FinalReportFile.CopyToAsync(stream);
                //    }

                //    model.FinalReport = "/uploads/finalreports/" + fileName;

                //}
                // 2. Handle Documents Upload (if any)
         

                // 3. Save Job_Master record
                _db.Job_Master.Add(model);
                await _db.SaveChangesAsync();

                model.CaseId = model.JobId;

                _db.Job_Master.Update(model);
                await _db.SaveChangesAsync();

                if (model.DocumentFiles != null && model.DocumentFiles.Count > 0)
                {
                    foreach (var file in model.DocumentFiles)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "documents");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {







                            await file.CopyToAsync(stream);
                        }

                        var doc = new JobDocumentDtl
                        {
                            JobId = model.JobId,
                            DocumentName = fileName
                        };

                        _db.JobDocument_Dtl.Add(doc);
                    }

                    await _db.SaveChangesAsync();
                }
                //  Save Verification Types into Job_Dtl
                var selectedVerifications = Request.Form["VerifyTypes"];
                if (selectedVerifications.Count > 0)
                {
                    foreach (var vId in selectedVerifications)
                    {
                        var vName = _db.Verification_Master
                                       .Where(v => v.VerificationId == Convert.ToInt32(vId))
                                       .Select(v => v.VerficationName)
                                       .FirstOrDefault();

                        var jobDtl = new Job_Dtl
                        {
                            JobId = model.JobId,
                            VerificationId = Convert.ToInt32(vId),
                            VarificationName = vName,

                           
                            CompanyDocument = null,              
                            Status = "Pending",                  
                            IssueType = null,                    
                            ExcuterRemark = null,                
                            EvidanceDocument = null,             
                                     
                            AssignedTo = null,                   
                       
                         
                        };

                        _db.Job_Dtl.Add(jobDtl);
                    }

                    await _db.SaveChangesAsync();
                }


                TempData["SuccessMessage"] = "Verification request submitted successfully!";
                return RedirectToAction("RequestMgmt"); // or wherever you want to go
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult GetClients()
        {
            var clients = _db.Client_Master
                            .Select(c => new {
                                clientId = c.ClientId,
                                clientName = c.ClientName
                            })
                            .ToList();
           

            return Json(clients);
        }
          [HttpPost]
       public IActionResult SaveEmp(int id,int JobId)
       {


           if (id == 0 || JobId == 0)
           {
               return Json(new { success = false, message = "Invalid Employee or Client Id!" });
           }

            // Get all verification records for this client
            // Get all verification records for this client
            var records = _db.Job_Dtl.Where(c => c.JobId == JobId).ToList();

            if (records.Any())
            {
                foreach (var record in records)
                {
                    record.AssignedTo = id;         // Employee assign kiya
                    record.Status = "In Progress";    // Status update kiya (String truncation error se bachne ke liye 'Progress' rakha hai)

                    // EF ko batayein ki ye row modify hui hai
                    _db.Entry(record).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }

                // Loop ke BAHR ek baar Job_Master ka status bhi update kar dete hain
                var jobMaster = _db.Job_Master.FirstOrDefault(m => m.JobId == JobId);
                if (jobMaster != null)
                {
                    jobMaster.Status = "In Progress";
                    _db.Entry(jobMaster).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }

                _db.SaveChanges(); // Isse sirf update queries chalengi, koi naya record insert nahi hoga
                return Json(new { success = true, message = "Employee assigned successfully for client." });
            }
            return Json(new { success = false, message = "Employee Not Saved!" });
       }

        //[HttpPost]
        //public IActionResult SaveEmpInClient(int? id, int clientId, int companyId)
        //{


        //    if (id == 0 || clientId == 0)
        //    {
        //        return Json(new { success = false, message = "Invalid Employee or Client Id!" });
        //    }

        //    // Get all verification records for this client
        //    var records = _db.Verification_Status
        //                     .Where(v => v.ClientId == clientId)
        //                     .ToList();

        //    if (records.Any())
        //    {
        //        foreach (var record in records)
        //        {
        //            record.Emp_Id = id;          // assign employee to each record

        //        }

        //        _db.SaveChanges();
        //        return Json(new { success = true, message = "Employee assigned  for client " });
        //    }
        //    return Json(new { success = false, message = "Employee Not Saved!" });
        //}
        public async Task<IActionResult> Details(int id)
        {
            if (id != 0)
            {
                if (id == 0)
                    return BadRequest();

                // 1. Job_Master से job निकालना
                var job = await _db.Job_Master
                                   .Include(j => j.Documents)   // load related documents
                                   .FirstOrDefaultAsync(j => j.JobId == id);

                if (job == null)
                    return NotFound();

                // 2. Client_Master से client का नाम निकालना (foreign key ClientId से)
                var clientName = await _db.Client_Master
                                          .Where(c => c.ClientId == job.ClientId)
                                          .Select(c => c.ClientName)
                                          .FirstOrDefaultAsync();

                // 3. ViewModel बनाना
                var vm = new ClientDetailsViewModel
                {
                    Job = job,
                    ClientName = clientName,
                    Documents = job.Documents.ToList()
                };




                var data = _db.Job_Dtl
                 .Where(x => x.JobId == id)
                 .ToList();

                int total = data.Count();
                int completed = data.Count(x => x.Status == "Completed");
                int progress = total > 0 ? (completed * 100) / total : 0;

                ViewBag.Total = total;
                ViewBag.Completed = completed;
                ViewBag.Progress = progress;

                ViewBag.VarData = data;

                return View(vm);

                //// 1. Existing Verification Status Data
                //ViewBag.VarData = (from s in _db.Verification_Status
                //                   join n in _db.Mst_VerificationChecks
                //                   on s.VarId equals n.Id into temp
                //                   from n in temp.DefaultIfEmpty()
                //                   where s.ClientId == id
                //                   select new
                //                   {
                //                       s.Id,
                //                       s.ClientId,
                //                       s.VarId,
                //                       s.Status,
                //                       s.CreatedAt,
                //                       s.DocumentPath,
                //                       s.ExecutorNotes,
                //                       Name = n != null ? n.Name : ""
                //                   }).ToList();


                //return View(_db.Client_Dtl.FirstOrDefault(x => x.Id == id));

                //  return View(data);
            }
            else
            {
                return RedirectToAction("Dashboard");
            }

        }
        [HttpPost]
        public IActionResult EditClient(Client_Master model)
        {
            var employee = _db.Client_Master.FirstOrDefault(e => e.ClientId == model.ClientId);
            if (employee != null)
            {
                //update filds
                employee.Address = model.Address;
                employee.MobileNumber = model.MobileNumber;
                employee.WebAddress = model.WebAddress;
                if (!string.IsNullOrEmpty(model.CPassword))
                {

                    employee.CPassword = BCrypt.Net.BCrypt.HashPassword(model.CPassword);
                }
                employee.ContactPerson = model.ContactPerson;
                employee.ClientName = model.ClientName;
               
               

                _db.SaveChanges();

                return Json(new { success = true, message = "Client Updated Successfully!!" });
            }
            return Json(new { success = false, message = "Client Data Not Updated !!" });

        }
        public IActionResult GetClient(int id)
        {
            var data = _db.Client_Master.Where(x => x.ClientId == id).FirstOrDefault();
            if (data != null)
            {
                return Json(data);
            }
            return Json(new { success = false, message = "Data Not Found!" });
           
        }
        [HttpPost]
        public IActionResult DeleteClient(int id)
        {
            var data = _db.Client_Master.Where(x => x.ClientId == id).FirstOrDefault();
            if (data != null)
            {
                _db.Client_Master.Remove(data);
                _db.SaveChanges();
                return Json(new { success = true, message = "Data deleted succesfully!!" });
            }
            return Json(new { success =false, message = "Data Not deleted !!" });
        }
        public IActionResult Registration()
        {
            return View();
        }
        public IActionResult ClientMst()
        {
            var data=_db.Client_Master.ToList();
            if(data!= null)
            {
                return View(data);
            }
            return View();
         
        }

        [HttpPost]
        public IActionResult Registration(Client_Master model)
        {
            if (model != null)
            {
                if (string.IsNullOrWhiteSpace(model.CPassword))
                    throw new ArgumentException("Password is required");

                //  Encrypt password before saving
                model.CPassword = BCrypt.Net.BCrypt.HashPassword(model.CPassword);
                var data = _db.Client_Master.Add(model);
                _db.SaveChanges();
                TempData["success"] = "Client Saved Successfully!!";
                return RedirectToAction("ClientMst", "Admin");
            }

            TempData["error"] = "Client Not Saved !!";
            return RedirectToAction("Registration", "Admin");
        }
    }
}

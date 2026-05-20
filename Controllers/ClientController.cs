using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using TrustPlus.Models;

namespace TrustPlus.Controllers
{
    [Authorize(Roles = "Client")]

    public class ClientController : Controller
    {
        private readonly DbManager _db;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public ClientController(DbManager db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Dashboard()
        {
            int loggedInClientId =Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var Data = (from jm in _db.Job_Master
                        join cm in _db.Client_Master on jm.ClientId equals cm.ClientId
                        where jm.ClientId == loggedInClientId
                        select new Job_Master
                        {
                            JobId = jm.JobId,
                            ClientId = jm.ClientId,
                            EmployeeName = jm.EmployeeName,
                            EmailAddress = jm.EmailAddress,
                            PhoneNumber = jm.PhoneNumber,
                            PostionApplied = jm.PostionApplied,
                            Department = jm.Department,
                            Priority = jm.Priority,
                            Status = jm.Status,
                            JobDate = jm.JobDate,
                            CreatedAt = jm.CreatedAt,
                            UpdatedAt = jm.UpdatedAt,
                            ClientName = cm.ClientName   // NotMapped property
                        }).ToList();


            //var Data = _db.Job_Master.Where(x => x.ClientId == loggedInClientId).
            //    ToList();


            //        var data = _db.Client_Dtl.ToList();

            //        foreach (var item in data)
            //        {
            //            var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);

            //            var completed = _db.Verification_Status.Count(x =>
            //                x.ClientId == item.Id &&
            //                x.Status == "Completed");

            //            item.Progress = total > 0 ? (completed * 100 / total) : 0;
            //        }

            var pending = Data.Count(x => x.Status == "Pending");

                    var inProgress = Data.Count(x => x.Status == "In Progress");

                   var complete =Data.Count(x => x.Status == "Completed");

            ViewBag.Pending = pending;
            ViewBag.InProgress = inProgress;
            ViewBag.Completed = complete;
                  //var data = _db.Client_Dtl
                  //    .OrderByDescending(x => x.Id)
                  //    .ToList();

            return View(Data);
        }
        public IActionResult MyCases()
        {
            var data = _db.Client_Dtl.ToList();

            foreach (var item in data)
            {
                var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);

                var completed = _db.Verification_Status.Count(x =>
                    x.ClientId == item.Id &&
                    x.Status == "Completed");

                item.Progress = total > 0 ? (completed * 100 / total) : 0;
            }

            var pending = _db.Client_Dtl
    .Count(x => x.Status == "Pending");

            var inProgress = _db.Client_Dtl
                .Count(x => x.Status == "In Progress");

            var complete = _db.Client_Dtl
                .Count(x => x.Status == "Completed");

            ViewBag.Pending = pending;
            ViewBag.InProgress = inProgress;
            ViewBag.Completed = complete;
            //var data = _db.Client_Dtl
            //    .OrderByDescending(x => x.Id)
            //    .ToList();

            return View(data);

        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NewRequest()
        {
            string loggedInClientId = HttpContext.Session.GetString("UserId");

            var data = _db.Verification_Master.ToList();
            ViewBag.Data = data;
            ViewBag.LoggedInClientId = loggedInClientId;
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> NewRequest(VerificationVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string fileName = null;

        //        if (model.DocumentPath  != null && model.DocumentPath .Length > 0)
        //        {
        //            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        //            if (!Directory.Exists(uploadDir))
        //                Directory.CreateDirectory(uploadDir);

        //            fileName = Guid.NewGuid().ToString() + "_" + model.DocumentPath .FileName;

        //            string filePath = Path.Combine(uploadDir, fileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await model.DocumentPath .CopyToAsync(stream);
        //            }
        //        }

        //        var newRequest = new ClientDtl
        //        {
        //            Name = model.Name,
        //            Email = model.Email,
        //            Mobile = model.Mobile,
        //            DOB = model.DOB,
        //            Position = model.Position,
        //            Department = model.Department,
        //            Priority = model.Priority,

        //            DocumentPath  = fileName,

        //            //VerifyType = model.VerifyTypes != null
        //            //    ? string.Join(",", model.VerifyTypes)
        //            //    : "",

        //            CreatedAt = DateTime.Now,
        //            Status = "Pending"
        //        };

        //        // Save Client
        //        _db.Client_Dtl.Add(newRequest);
        //        await _db.SaveChangesAsync();

        //        // =====================================
        //        // INSERT INTO tbl_verification
        //        // =====================================

        //        if (model.VerifyTypes != null)
        //        {
        //            foreach (var item in model.VerifyTypes)
        //            {
        //                Verification verification = new Verification()
        //                {
        //                    ClientId = newRequest.Id,

        //                    VarId = Convert.ToInt32(item),

        //                    Status = "Pending",

        //                    CreatedAt = DateTime.Now
        //                };
        //                _db.Verification_Status.Add(verification);
        //            }

        //            await _db.SaveChangesAsync();
        //        }

        //        TempData["SuccessMessage"] = "Verification request submitted successfully.";

        //        return RedirectToAction("Dashboard");
        //    }

        //    return View(model);
        //}

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
                return RedirectToAction("Dashboard"); // or wherever you want to go
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
    
  
    
    }
}

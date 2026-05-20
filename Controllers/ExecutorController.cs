using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustPlus.Models;

namespace TrustPlus.Controllers
{
    [Authorize(Roles = "Executor")]
    public class ExecutorController : Controller
    {
        private readonly DbManager _db;

        public ExecutorController(DbManager db)
        {
            _db = db;
        }

        public IActionResult Dashboard()
        {
            //        var data = _db.Job_Master.ToList();

            //        foreach (var item in data)
            //        {
            //            var total = _db.Job_Master
            //             .Where(x => x.JobId == item.JobId)
            //             .ToList();

            //            var completed = _db.Job_Dtl.Count(x =>
            //                x.JobId == item.JobId &&
            //                x.Status == "Completed");

            //            //item.Progress = total > 0 ? (completed * 100 / total) : 0;
            //        }

            //        var pending = _db.Job_Master
            //.Count(x => x.Status == "Pending");

            //        var inProgress = _db.Job_Master
            //            .Count(x => x.Status == "In Progress");

            //        var complete = _db.Job_Master
            //            .Count(x => x.Status == "Completed");

            //        ViewBag.Pending = pending;
            //        ViewBag.InProgress = inProgress;
            //        ViewBag.Completed = complete;
            //var data = _db.Client_Dtl
            //    .OrderByDescending(x => x.Id)
            //    .ToList();
            // 1. Session se logged-in user ki details nikalen (Aapki choice ke hisab se login name ya id use karein)
            string loggedInUserName = HttpContext.Session.GetString("UserName");

            // 2. Query ko optimize aur filter kiya gaya hai
            var data = _db.Job_Master
                // Unhi Jobs ko select karein jismein logged-in user assigned ho (JobId repeat nahi hogi)
                .Where(jm => _db.Job_Dtl.Any(jd => jd.JobId == jm.JobId && _db.User_Master.Any(u => u.UserId == jd.AssignedTo && u.UserName == loggedInUserName)))
                .Select(jm => new JobWithDetailsViewModel
                {
                    JobId = jm.JobId,
                    ClientId = jm.ClientId,
                    CaseId = jm.CaseId,
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

                    // Direct user ka naam mil jayega (Kyunki ek login user ke liye single record hi load hoga)
                    AssignedUserName = loggedInUserName,
                    ClientName = _db.Client_Master
                        .Where(cm => cm.ClientId == jm.ClientId)
                        .Select(cm => cm.ClientName)
                        .FirstOrDefault(),

                    // JobDetails mein sirf wahi rows aayengi jo is logged-in user ki hain
                    JobDetails = (from jd in _db.Job_Dtl
                                  join u in _db.User_Master on jd.AssignedTo equals u.UserId
                                  where jd.JobId == jm.JobId && u.UserName == loggedInUserName
                                  select new JobDtlViewModel
                                  {
                                      JobDtlId = jd.JobDtlId,
                                      JobId = jd.JobId,
                                      Status = jd.Status,
                                      AssignedTo = jd.AssignedTo,
                                      AssignedToName = u.UserName
                                  }).ToList()
                })
                .ToList();

            // 3. ViewBag counters aapki filter ki hui list par bilkul sahi chalenge
            ViewBag.Pending = data.Count(x => x.Status == "Pending");
            ViewBag.InProgress = data.Count(x => x.Status == "In Progress");
            ViewBag.Completed = data.Count(x => x.Status == "Completed");

            // 4. Data ko View par bhej diya
            return View(data);

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
        //public async Task<IActionResult> Details(int id, bool success = false)
        //{
        //    if (id == 0)
        //        return RedirectToAction("Dashboard");

        //    // Job_Master record
        //    var job = await _db.Job_Master
        //        .Where(j => j.JobId == id)
        //        .FirstOrDefaultAsync();

        //    if (job == null)
        //        return NotFound();

        //    // Job_Dtl + Client_Master join
        //    var jobDetails = (from jd in _db.Job_Dtl
        //                      join cm in _db.Client_Master
        //                          on jd.AssignedTo equals cm.ClientId into cmGroup
        //                      from cm in cmGroup.DefaultIfEmpty()
        //                      where jd.JobId == job.JobId
        //                      select new JobDtlViewModel
        //                      {
        //                          JobDtlId = jd.JobDtlId,
        //                          JobId = jd.JobId,
        //                          Status = jd.Status,
        //                         // AssignedDate = jd.AssignedDate,
        //                          AssignedTo = jd.AssignedTo,
        //                          AssignedToName = cm != null ? cm.ClientName : null
        //                      }).ToList();

        //    // Verification_Status records
        //    var verifications = _db.Verification_Status
        //        .Where(v => v.ClientId == job.ClientId)
        //        .Select(v => new VerificationViewModel
        //        {
        //            Id = v.Id,
        //            VarId = v.VarId,
        //            ClientId = v.ClientId,
        //            Status = v.Status,
        //            Flag = v.Flag,
        //            DocumentPath = v.DocumentPath,
        //            ExecutorNotes = v.ExecutorNotes
        //        }).ToList();

        //    // Progress calculation
        //    var total = verifications.Count;
        //    var completed = verifications.Count(v => v.Status == "Completed");
        //    var progress = total > 0 ? (completed * 100 / total) : 0;

        //    // Build ViewModel
        //    var model = new DetailsViewModel
        //    {
        //        JobId = job.JobId,
        //        ClientId = job.ClientId,
        //        CaseId = job.CaseId,
        //        EmployeeName = job.EmployeeName,
        //        EmailAddress = job.EmailAddress,
        //        PhoneNumber = job.PhoneNumber,
        //        PostionApplied = job.PostionApplied,
        //        Department = job.Department,
        //        Priority = job.Priority,
        //        Status = job.Status,
        //        JobDate = job.JobDate,
        //        //FinalReport = job.FinalReport,
        //        //FinalSummary = job.FinalSummary,
        //        CreatedAt = job.CreatedAt,
        //        UpdatedAt = job.UpdatedAt,
        //        Progress = progress,
        //        JobDetails = jobDetails,
        //        Verifications = verifications
        //    };

        //    ViewBag.Data = new
        //    {
        //        job.EmployeeName,
        //        job.JobId,
        //        job.PostionApplied,
        //        job.Department
        //    };

        //    return View(model);
        //}
        public IActionResult Details(int id)
        {

            // 1. Session se logged-in user nikalen
            string loggedInUserName = HttpContext.Session.GetString("UserName");

            // 2. Main Job Master Data extract karein
            var jobMaster = _db.Job_Master.FirstOrDefault(x => x.JobId == id);
            if (jobMaster == null)
            {
                return NotFound();
            }

            // 3. ViewBag.Data banana jo aapke view ki heading me binded hai
            ViewBag.Data = new {
                Id = jobMaster.JobId,
                Name = jobMaster.EmployeeName,
                Position = jobMaster.PostionApplied,
                Department = jobMaster.Department
            };

            // 4. Job Details list nikalna strictly is login user ke liye
            var verificationList = (from jd in _db.Job_Dtl
                                    join u in _db.User_Master on jd.AssignedTo equals u.UserId
                                    where jd.JobId == id && u.UserName == loggedInUserName
                                    select new VerificationItemViewModel // Aapke list inner model ki properties
                                    {
                                        Id = jd.JobDtlId,       // Mapping JobDtlId
                                        VarId = jd.VerificationId,
                                        Name = jd.VarificationName,
                                        Status = jd.Status ?? "Pending",
                                        Flag = jd.IssueType,     // Database column IssueType maps to Flag
                                        DocumentPath = jd.EvidanceDocument, // EvidanceDocument maps to DocumentPath
                                        ExecutorNotes = jd.ExcuterRemark    // ExcuterRemark maps to ExecutorNotes
                                    }).ToList();

            // 5. Progress Calculate karna (Kitne checks Completed hain)
            int totalChecks = verificationList.Count;
            int completedChecks = verificationList.Count(x => x.Status == "Completed");
            int progressPercentage = totalChecks > 0 ? (completedChecks * 100) / totalChecks : 0;

            // 6. Final ViewModel ko bind karna jo view accept karega (@model TrustPlus.Models.VerificationSubmitVM)
            var viewModel = new VerificationSubmitVM
            {
                JobId = jobMaster.JobId,
                  ClientId = jobMaster.ClientId,
                Progress = progressPercentage,
                FinalSummary=jobMaster.FinalSummary,
                  FinalReport = jobMaster.FinalReport, // Final report file name from db
                Verifications = verificationList,
          
            };
            var clientDoc = _db.JobDocument_Dtl.FirstOrDefault(x => x.JobId == id);

            if (clientDoc != null)
            {
                ViewBag.DocumentPath = clientDoc.DocumentName;
                ViewBag.DocumentName = "Client Attachment";
            }

          

           


            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> SubmitVerification(VerificationSubmitVM model, string submitType)
        {
            // Check if state is invalid
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Details", new { id = model.JobId });
            //}

            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

          //handle file upload
            string finalReportPath = "";
            if (model.FinalReportFile != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.FinalReportFile.FileName);
                string fullPath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.FinalReportFile.CopyToAsync(stream);
                }
                finalReportPath = fileName;
            }

        //update job master
            var jobMaster = await _db.Job_Master.FirstOrDefaultAsync(x => x.JobId == model.JobId);
            if (jobMaster != null)
            {
                // Null values are only updated if a file was actually uploaded
                if (!string.IsNullOrEmpty(finalReportPath))
                {
                    jobMaster.FinalReport = finalReportPath;

                }
                jobMaster.FinalSummary = model.FinalSummary;

                // Handle case resolution status
                if (submitType == "Final")
                {
                    jobMaster.Status = "Completed";
                }
                else
                {
                    jobMaster.Status = "In Progress";
                }

                jobMaster.UpdatedAt = DateTime.Now;
            }

            
            //  update job dtl
         
            if (model.Verifications != null)
            {
                foreach (var item in model.Verifications)
                {
                    // Bind using JobDtlId primary key matching data
                    var jobDetail = await _db.Job_Dtl.FirstOrDefaultAsync(x => x.JobDtlId == item.Id);

                    if (jobDetail != null)
                    {
                        // Assign current/old file string context path as fallback
                        string currentDocPath = jobDetail.EvidanceDocument;

                        // Process single item file input upload stream
                        if (item.EvidenceFile != null)
                        {
                            string childFileName = Guid.NewGuid().ToString() + Path.GetExtension(item.EvidenceFile.FileName);
                            string childFullPath = Path.Combine(uploadFolder, childFileName);

                            using (var stream = new FileStream(childFullPath, FileMode.Create))
                            {
                                await item.EvidenceFile.CopyToAsync(stream);
                            }
                            currentDocPath = childFileName; // set newly populated unique filename
                        }

                        // Update row column updates filling up old NULL scopes
                        jobDetail.Status = item.Status;
                        jobDetail.IssueType = item.Flag;
                        jobDetail.ExcuterRemark = item.ExecutorNotes;
                        jobDetail.EvidanceDocument = currentDocPath;
                        jobDetail.UpdatedAt = DateTime.Now;
                    }
                }
            }

            // Commit changes context memory pipeline straight down to SQL Database Server
            await _db.SaveChangesAsync();

            TempData["success"] = "Verification Data Inserted Successfully!";
            return RedirectToAction("Details", new { id = model.JobId });
        }
    }
}
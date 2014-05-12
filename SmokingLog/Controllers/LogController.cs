using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmokingLog.Models;
using PagedList;
using SmokingLog.ViewModels;
using Microsoft.AspNet.Identity;
using SmokingLog.Utilities;

namespace SmokingLog.Controllers
{
    public class LogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult DisplayUserGravatar()
        {
            ViewBag.GravatarURL = "http://www.gravatar.com/avatar/" + db.Users.Find(User.Identity.GetUserId()).EmailMD5 + "?size=50";
            return PartialView("_DisplayGravatar");
        }

        // GET: /Log/
        public ActionResult Index(int? page)
        {
            string UserID = User.Identity.GetUserId();
            var logs = db.Logs.Where(l => l.ApplicationUserID == UserID).OrderByDescending(l => l.logDate);

            int pageNum = (page ?? 1);
            int pageSize = 7;

            populateNextInputDate();
            populateLogStats();

            return View(logs.ToPagedList(pageNum, pageSize));
        }

        public void populateNextInputDate()
        {
            string UserID = User.Identity.GetUserId();

            int logCount = db.Logs.Where(l => l.ApplicationUserID == UserID).Count();
            if (logCount > 0)
            {
                DateTime nextDate = db.Logs.Where(l => l.ApplicationUserID == UserID).Max(l => l.logDate).AddDays(1).Date;
                DateTime thisDate = System.DateTime.Now.Date;

                ViewBag.NextDate = (DateTime.Compare(nextDate, thisDate) < 0 ? nextDate : thisDate).ToString("yyyy-MM-dd");
            }
            else
            {
                ViewBag.NextDate = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
            }
        }

        public void populateLogStats()
        {
            string UserID = User.Identity.GetUserId();
            LogStats ls = new LogStats();
            
            int totalDays = db.Logs.Where(l => l.ApplicationUserID == UserID).Count();
            if (totalDays > 0)
            {
                float totalSmoked = db.Logs.Where(l => l.ApplicationUserID == UserID).Sum(l => l.numberOfCigarettes);

                DateTime mostRecentDate = db.Logs.Where(l => l.ApplicationUserID == UserID).Max(l => l.logDate);
                DateTime weekAgo = mostRecentDate.AddDays(-7).Date;
                float totalSmokedThisWeek = db.Logs.Where(l => l.ApplicationUserID == UserID && l.logDate > weekAgo).Sum(l => l.numberOfCigarettes);

                float averagePerDay = totalSmoked / totalDays;

                ls.TotalAverage = Math.Round(averagePerDay, 2);
                ls.TotalPacksPerWeek = Math.Round((averagePerDay * 7) / 25, 2);
                ls.LastWeekAverage = Math.Round(totalSmokedThisWeek / 7, 2);
                ls.LastWeekPacks = Math.Round(totalSmokedThisWeek / 25, 2);
            }
            else
            {
                ls.TotalAverage = 0.0;
                ls.TotalPacksPerWeek = 0.0;
                ls.LastWeekAverage = 0.0;
                ls.LastWeekPacks = 0.0;
            }

            ViewBag.LogStats = ls;
        }

        // POST: /Log/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string LogDate, string CigarettesToday)
        {
            string UserID = User.Identity.GetUserId();
            if (LogDate != null && CigarettesToday != null)
            {
                try
                {
                    int numCigarettes = Convert.ToInt32(CigarettesToday);
                    DateTime LogDateDT = Convert.ToDateTime(LogDate);

                    Log log = db.Logs.Where(l => l.ApplicationUserID == UserID && l.logDate.Equals(LogDateDT)).SingleOrDefault();

                    if (log == null)
                    {
                        log = new Log();
                        log.logDate = LogDateDT;
                        log.numberOfCigarettes = numCigarettes;
                        log.ApplicationUserID = User.Identity.GetUserId();
                        db.Logs.Add(log);
                    }
                    else
                    {
                        log.numberOfCigarettes = numCigarettes;
                        db.Entry(log).State = EntityState.Modified;
                    }
                    
                    db.SaveChanges();
                }
                catch (System.FormatException) { }
                catch (System.InvalidCastException) { }
                catch (System.OverflowException) { }
            }
            return RedirectToAction("Index");
        }

        // POST: /Log/EditAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAjax(string LogDate, string NumberOfCigarettes)
        {
            // invalid date -  01/01/0001 00:00:00
            // invalid number - 0
            string UserID = User.Identity.GetUserId();
            if (LogDate != null && NumberOfCigarettes != null)
            {
                try
                {
                    int NumCigarettes = Convert.ToInt32(NumberOfCigarettes);
                    DateTime LogDateDT = Convert.ToDateTime(LogDate);

                    Log log = db.Logs.Where(l => l.ApplicationUserID == UserID && l.logDate.Equals(LogDateDT)).SingleOrDefault();

                    if (log == null)
                    {
                        log = new Log();
                        log.logDate = LogDateDT;
                        log.numberOfCigarettes = NumCigarettes;
                        log.ApplicationUserID = User.Identity.GetUserId();
                        db.Logs.Add(log);
                    }
                    else
                    {
                        log.numberOfCigarettes = NumCigarettes;
                        db.Entry(log).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    object SavedParameters = new { logDate = LogDateDT.ToString("yyyy-MM-dd"), NumberOfCigarettes = NumCigarettes };
                    return Json(JsonResponseFactory.SuccessResponse(SavedParameters));
                }
                catch (System.FormatException e) { return Json(JsonResponseFactory.ErrorResponse("Update Failed - " + e.Data)); }
                catch (System.InvalidCastException e) { return Json(JsonResponseFactory.ErrorResponse("Update Failed - " + e.Data)); }
                catch (System.OverflowException e) { return Json(JsonResponseFactory.ErrorResponse("Update Failed - " + e.Data)); }
            }
            else
            {
                return Json(JsonResponseFactory.ErrorResponse("Update Failed - LogDate: " + LogDate + "; CigarettesToday:  " + NumberOfCigarettes));
            }
        }

        // GET: /Log/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                return HttpNotFound();
            }
            return View(log);
        }

        // POST: /Log/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Log log = db.Logs.Find(id);
            db.Logs.Remove(log);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LoanAppPOC.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace LoanAppPOC.Controllers
{
    public class CustomTableLayoutsController : Controller
    {
        private ClientInfoEntities2 db = new ClientInfoEntities2();

        // GET: CustomTableLayouts/Index
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.AspNetUsersId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // GET: CustomTableLayouts/SecondTable
        [Authorize]
        public ActionResult SecondTable()
        {
            ViewBag.AspNetUsersId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: Template code to stringify AG Grid and pass custom grid layout to SQL Server
        public JsonResult PostCustomGridLayout([Bind(Include = "CustomTableLayoutId,JsonLayoutInstructions,AspNetUsersId,GridName")]CustomTableLayout customTableLayout)
        {
            string currentGrid = customTableLayout.GridName;

            string currentUserId = User.Identity.GetUserId();
            customTableLayout.AspNetUser = db.AspNetUsers.FirstOrDefault(x => x.Id == currentUserId);

            //Check if there are any rows that already contain the current user's ID AND grid name. If so...
            foreach (var customlayout in db.CustomTableLayouts.Where(x => x.AspNetUsersId == currentUserId).Where(x => x.GridName == currentGrid))
            {
                //Remove value with same grid name
                db.CustomTableLayouts.Remove(customlayout);
            }

            if (ModelState.IsValid)
            {
                //Add back the correct, current layout    
                db.CustomTableLayouts.Add(customTableLayout);
                db.SaveChanges();
            }

            return new JsonResult() { Data = JsonConvert.SerializeObject(customTableLayout.CustomTableLayoutId), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        //GET: Template code to serialize AG Grid's JSON object onGridReady
        public JsonResult GetTableLayoutJson(string currentGrid)
        {
            //get current user
            string currentUserId = User.Identity.GetUserId();

            //"Select[JsonLayoutInstructions], [CustomTableLayoutId], [AspNetUsersId], [GridName] from [CustomTableLayout] where [AspNetUsersId] = \'" + currentUserId + "\' AND [GridName] = \'" + currentGrid + "\'"
            DbSqlQuery<CustomTableLayout> customLayout = db.CustomTableLayouts.SqlQuery("Select[JsonLayoutInstructions], [CustomTableLayoutId], [AspNetUsersId], [GridName] from [CustomTableLayout] where [AspNetUsersId] = \'" + currentUserId + "\' AND [GridName] = \'" + currentGrid + "\'");

            var output = JsonConvert.SerializeObject(customLayout, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
                );

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSecondTableGrid() {
            string currentGrid = "weeklyCashFlow";
            return GetTableLayoutJson(currentGrid);
        }

        public JsonResult GetIndexGrid()
        {
            string currentGrid = "clientDetails";
            return GetTableLayoutJson(currentGrid);
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

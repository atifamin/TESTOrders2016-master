using OrderForm2016.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderForm2016.Helpers;

namespace OrderForm2016.Controllers
{
    public class ErrorHandlerController : Controller
    {

        public ActionResult ShowError(int bFormID = 0)
        {
            if (TempData["ex"] != null)
            {
                Exception exc = (Exception)TempData["ex"];
                Error error = new Error();
                error.FriendlyMessage = exc.Message;
                if (exc.InnerException != null)
                    error.ErrorMessage = exc.InnerException.Message;
                error.ErrorLocation = exc.StackTrace;
                error.base_form_id = bFormID;
                error.base_form_id = 0;
                error.ErrDate = DateTime.Now;
                new ModelToSQL<Error>().WriteInsertSQL("Error", error, "error_id", CommonProcs.OFStr);
                return View("Error", error);
            }
            else
            {
                return View("Error");
            }

        }
    }
}
using System;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Results;
using NiitBlog.Models;
using NiitBlog.Validator;

namespace NiitBlog.Controllers
{
    public class FeedBackController : Controller
    {

         private IRepositoryContainer _repository;

         public FeedBackController()
        {
            this._repository = new RepositoryContainer();
        }

        [HttpPost]
        public ActionResult Index()
        {
            try
            {
                Complaint complaint = new Complaint { title = HttpUtility.HtmlEncode(Request["title"]), text = HttpUtility.HtmlEncode(Request["text"]), email = Request["email"] };
                ComplaintValidation eomplaintValidation = new ComplaintValidation();
                ValidationResult validationResult = eomplaintValidation.Validate(complaint);
                string Msg = "";
                if (!validationResult.IsValid)
                {
                    foreach (var failure in validationResult.Errors)
                    {
                        Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                    }
                    return Json(new { Result = "Error", Message = Msg });
                }
                _repository._CompaintRepositories.AddCompaint(complaint);
                return Json(new { Result = "OK"});
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Frogger_Blogger.Models;
using Microsoft.AspNet.Identity;

namespace Frogger_Blogger.Controllers
{
    [RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Author).Include(c => c.BlogPost);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }
        //[HttpPost]
        //public ActionResult Comment(CommentViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var dataComment = new Comment();
        //        dataComment.BlogPostId = viewModel.Id;
        //        dataComment.Author = viewModel.Author;
        //        dataComment.Body = viewModel.Body;
        //        dataComment.Email = viewModel.Email;

        //        //Create Comment and save
        //        commentRepository.Create(comment);
        //        commentRepository.SaveChanges();
        //        return new EmptyResult();

        //    }
        //    var modelErrors = this.BuildModelErrors();

        //    HttpContext.Response.StatusCode = 400;

        //    return Json(modelErrors);


        //}
        //private List<ModelError> BuildModelErrors()
        //{
        //    var modelErrors = new List<ModelError>();
        //    var erroneousFields = this.ModelState.Where(ms => ms.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors });
        //    foreach (var erroneousField in erroneousFields)
        //    {
        //        var fieldKey = erroneousField.Key;
        //        var fieldErrors = erroneousField.Errors.Select(error => new ModelError(fieldKey, error.ErrorMessage));
        //        modelErrors.AddRange(fieldErrors);
        //    }
        //    return modelErrors;
        //}
        //private class ModelError
        //{
        //    public ModelError(string key, string errorMessage)
        //    {
        //        Key = key;
        //        ErrorMessage = errorMessage;
        //    }
        //    public string Key { get; set; }
        //    public string ErrorMessage { get; set; }
        //}




        // GET: Comments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
        //    ViewBag.BlogPostId = new SelectList(db.BlogPosts, "Id", "Title");
        //    return View();
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogPostId")] Comment comment, [ModelBinder(typeof(AllowHtmlBinder))] string commentBody, string slug)
        {
            if (ModelState.IsValid)
            {
                
                comment.Body = commentBody;
                comment.AuthorId = User.Identity.GetUserId();
                comment.Created = DateTimeOffset.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                //return RedirectToAction();
                return RedirectToAction("Details", "BlogPosts", new { slug = slug });
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.Author);
            ViewBag.BlogPostId = new SelectList(db.BlogPosts, "Id", "Title", comment.BlogPostId);
            return View(comment);
        }
        //public class AllowHtmlBinder : IModelBinder
        //{
        //    public object BindModel(ControllerContext controllerContext,ModelBindingContext bindingContext)
        //    {
        //        var request = controllerContext.HttpContext.Request;
        //        var comment = bindingContext.ModelName;
        //        return request.Unvalidated[comment];
        //    }
        //}



        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.Author);
            ViewBag.BlogPostId = new SelectList(db.BlogPosts, "Id", "Title", comment.BlogPostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BlogPostId,AuthorId,Body,Created,Updated,UpdateReason")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.Author);
            ViewBag.BlogPostId = new SelectList(db.BlogPosts, "Id", "Title", comment.BlogPostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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

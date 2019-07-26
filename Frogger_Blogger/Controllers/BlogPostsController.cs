using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Frogger_Blogger.Models.EmailModel;
using Frogger_Blogger.Models;
using Frogger_Blogger.Utilities;
using PagedList;
using PagedList.Mvc;
using System.Collections;
using System.Collections.Generic;

namespace Frogger_Blogger.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);
            int pageSize = 4;
            int pageNumber = page ?? 1;

            //var publishedPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize);



            return View( blogList.ToPagedList(pageNumber, pageSize));
        }
        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();

                result = result.Where(p => p.Title.Contains(searchStr) 
                || p.Body.Contains(searchStr)
                || p.Comments.Any(c => c.Body.Contains(searchStr)
                || c.Author.FirstName.Contains(searchStr)
                || c.Author.LastName.Contains(searchStr)
                || c.Author.DisplayName.Contains(searchStr)
                || c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();

            }
            return result.OrderByDescending(p => p.Created);
        }



        [Authorize(Roles ="Admin")]
        public ActionResult AdminIndex()
        {
            var allBlogPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(b => b.Created).ToList();
            return View("Index", allBlogPosts);
        }
        
       

        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            return View();
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


        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Abstract,Body,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image )
        {
            if (ModelState.IsValid)
            {
                //var slugMaker = new string_utilities();
                //var slug = slugMaker.MakeSlug(blogPost.Title);
                var slug = string_utilities.SlugMaker(blogPost.Title);

                if (string.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("title", "Invalid title");
                    return View(blogPost);


                }
                if (db.BlogPosts.Any(p => p.Slug == slug))
                {
                    ModelState.AddModelError("title", "the title must be unique");
                    return View(blogPost);
                }
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var filename = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), filename));
                    blogPost.MediaUrl = "/Uploads/" + filename;
                }


                blogPost.Slug = slug;
                blogPost.Created = DateTimeOffset.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);

        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Abstract,Slug,Body,MediaUrl,Published,Created,Updated")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                //var slugMaker = new string_utilities();
                //var slug = slugMaker.MakeSlug(blogPost.Title);
                var newslug = string_utilities.SlugMaker(blogPost.Title);

                if (newslug != blogPost.Slug)
                { 
                    if (string.IsNullOrWhiteSpace(newslug))
                    {
                        ModelState.AddModelError("title", "Invalid title");
                        return View(blogPost);


                    }
                    if (db.BlogPosts.Any(p => p.Slug == newslug))
                    {
                        ModelState.AddModelError("title", "the title must be unique");
                        return View(blogPost);
                    }

                    blogPost.Slug = newslug;
                }
            
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var filename = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), filename));
                    blogPost.MediaUrl = "/Uploads/" + filename;
                }

            blogPost.Updated = DateTimeOffset.Now;
            db.BlogPosts.Add(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Frogger_Blogger.Models;
using Microsoft.AspNet.Identity;

namespace Frogger_Blogger.Controllers
{
    public class CommentRepliesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CommentReplies
        public async Task<ActionResult> Index()
        {
            var commentReplies = db.CommentReplies.Include(c => c.Author).Include(c => c.BlogPost);
            return View(await commentReplies.ToListAsync());
        }

        // GET: CommentReplies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentReply commentReply = await db.CommentReplies.FindAsync(id);
            if (commentReply == null)
            {
                return HttpNotFound();
            }
            return View(commentReply);
        }

        // GET: CommentReplies/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
        //    ViewBag.BlogPostId = new SelectList(db.BlogPosts, "Id", "Title");
        //    return View();
        //}

        // POST: CommentReplies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogPostId")] CommentReply commentReply, [ModelBinder(typeof(AllowHtmlBinder))] string commentReplyBody, string slug)
        {
            if (ModelState.IsValid)
            {

                commentReply.Body = commentReplyBody;
                commentReply.AuthorId = User.Identity.GetUserId();
                commentReply.Created = DateTimeOffset.Now;
                db.CommentReplies.Add(commentReply);
                db.SaveChanges();
                //return RedirectToAction();
                return RedirectToAction("Details", "BlogPosts", new { slug = slug });
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", commentReply.Author);
            ViewBag.BlogPostId = new SelectList(db.BlogPosts, "Id", "Title", commentReply.BlogPostId);
            return View(commentReply);
        }

        // GET: CommentReplies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentReply commentReply = await db.CommentReplies.FindAsync(id);
            if (commentReply == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", commentReply.AuthorId);
            ViewBag.BlogPostId = new SelectList(db.BlogPosts, "Id", "Title", commentReply.BlogPostId);
            return View(commentReply);
        }

        // POST: CommentReplies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AuthorId,Body,Created,Updated,BlogPostId")] CommentReply commentReply)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commentReply).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", commentReply.AuthorId);
            ViewBag.BlogPostId = new SelectList(db.BlogPosts, "Id", "Title", commentReply.BlogPostId);
            return View(commentReply);
        }

        // GET: CommentReplies/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentReply commentReply = await db.CommentReplies.FindAsync(id);
            if (commentReply == null)
            {
                return HttpNotFound();
            }
            return View(commentReply);
        }

        // POST: CommentReplies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CommentReply commentReply = await db.CommentReplies.FindAsync(id);
            db.CommentReplies.Remove(commentReply);
            await db.SaveChangesAsync();
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

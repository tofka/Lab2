using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab2.Models.Repositories;
using Lab2.Models.Entities;
using Lab2.ViewModels;
using Lab2.Models.SessionManager;

namespace Lab2.Controllers {
    public class ForumsController : Controller {
        //
        // GET: /Forums/

        public ActionResult Index() {
            List<ForumThread> threads = Repository.Instance.GetSortedThreads();
          

            return View(threads);
        }

        public ActionResult Thread(Guid id) {
            ThreadViewModel vm = new ThreadViewModel();
            vm.ForumThread = Repository.Instance.GetThreadById(id);
            vm.Posts = Repository.Instance.GetPostsByThreadId(vm.ForumThread.ID);

            return View(vm);
        }

        public ActionResult CreatePost() {
            if (SessionManager.CurrentUser == null)
                return RedirectToAction("Index", "Forums");
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(Post Post) {
            if (SessionManager.CurrentUser == null)
                return RedirectToAction("Index", "Forums");
            if (ModelState.IsValid) {
                
                ForumThread forumThread = new ForumThread();
                Repository.Instance.Save<Post>(Post);
                forumThread.ID = Guid.NewGuid();
                Post.ThreadID = forumThread.ID;
                forumThread.Title = Post.Title;                

                if (SessionManager.CurrentUser != null) {
                    Post.CreatedByID = SessionManager.CurrentUser.ID; 
                }
                Repository.Instance.Save<ForumThread>(forumThread);
              
                return RedirectToAction("Thread", "Forums", new { id = forumThread.ID });
            }           

            return View();
        }
    }
}
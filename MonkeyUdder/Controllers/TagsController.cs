using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonkeyUdder.Data;
using MonkeyUdder.Models;
using MonkeyUdder.Models.ViewModels;

namespace MonkeyUdder.Controllers
{
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext context;

        private readonly IHostingEnvironment hostingEnvironment;
        private static readonly HashSet<String> AllowedExtensions = new HashSet<String> { ".jpg", ".jpeg", ".png" };

        public TagsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            return View(await context.Tags.ToListAsync());
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View(new TagCreateModel());
        }

        // POST: Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var tag = new Tag
                {
                    Fullname = model.Fullname,
                    Shortname = model.Shortname,
                    Description = model.Description
                };

                var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.Image.ContentDisposition).FileName.Trim('"'));
                var fileExt = Path.GetExtension(fileName);
                if (!TagsController.AllowedExtensions.Contains(fileExt))
                {
                    this.ModelState.AddModelError(nameof(model.Image), "This file type is prohibited");
                }

                var imageId = Guid.NewGuid();
                var imagePath = Path.Combine(this.hostingEnvironment.WebRootPath, "images/tags", imageId.ToString("N") + fileExt);

                tag.PreviewPath = $"/images/tags/{imageId:N}{fileExt}";

                using (var fileStream = new FileStream(imagePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                this.context.Add(tag);
                await this.context.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(Int32? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await context.Tags.SingleOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            var model = new TagEditModel
            {
                Fullname = tag.Fullname,
                Shortname = tag.Shortname,
                Description = tag.Description
            };

            return View(model);
        }

        // POST: Tags/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? id, TagEditModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await context.Tags.SingleOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                tag.Fullname = model.Fullname;
                tag.Shortname = model.Shortname;
                tag.Description = model.Description;

                if (model.Image != null)
                {
                    var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.Image.ContentDisposition).FileName.Trim('"'));
                    var fileExt = Path.GetExtension(fileName);
                    if (!TagsController.AllowedExtensions.Contains(fileExt))
                    {
                        this.ModelState.AddModelError(nameof(model.Image), "This file type is prohibited");
                    }

                    System.IO.File.Delete(this.hostingEnvironment.WebRootPath + tag.PreviewPath);

                    var imageId = Guid.NewGuid();
                    var imagePath = Path.Combine(this.hostingEnvironment.WebRootPath, "images/tags", imageId.ToString("N") + fileExt);

                    tag.PreviewPath = $"/images/tags/{imageId:N}{fileExt}";

                    using (var fileStream = new FileStream(imagePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }
                }

                await this.context.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(Int32? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await context.Tags.FindAsync(id);

            System.IO.File.Delete(this.hostingEnvironment.WebRootPath + tag.PreviewPath);

            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
            return context.Tags.Any(e => e.Id == id);
        }
    }
}

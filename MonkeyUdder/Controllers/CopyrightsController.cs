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
    public class CopyrightsController : Controller
    {
        private readonly ApplicationDbContext context;

        private readonly IHostingEnvironment hostingEnvironment;
        private static readonly HashSet<String> AllowedExtensions = new HashSet<String> { ".jpg", ".jpeg", ".png" };

        public CopyrightsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Copyrights
        public async Task<IActionResult> Index()
        {
            return View(await context.Copyrights.ToListAsync());
        }

        // GET: Copyrights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var copyright = await context.Copyrights
                .FirstOrDefaultAsync(m => m.Id == id);
            if (copyright == null)
            {
                return NotFound();
            }

            return View(copyright);
        }

        // GET: Copyrights/Create
        public IActionResult Create()
        {
            return View(new CopyrightCreateModel());
        }

        // POST: Copyrights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CopyrightCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var copyright = new Copyright
                {
                    Name = model.Name
                };

                var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.Image.ContentDisposition).FileName.Trim('"'));
                var fileExt = Path.GetExtension(fileName);
                if (!CopyrightsController.AllowedExtensions.Contains(fileExt))
                {
                    this.ModelState.AddModelError(nameof(model.Image), "This file type is prohibited");
                }

                var imageId = Guid.NewGuid();
                var imagePath = Path.Combine(this.hostingEnvironment.WebRootPath, "images/copyrights", imageId.ToString("N") + fileExt);

                copyright.ImagePath = $"/images/copyrights/{imageId:N}{fileExt}";

                using (var fileStream = new FileStream(imagePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                this.context.Add(copyright);
                await this.context.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Copyrights/Edit/5
        public async Task<IActionResult> Edit(Int32? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var copyright = await context.Copyrights.SingleOrDefaultAsync(m => m.Id == id);
            if (copyright == null)
            {
                return NotFound();
            }

            var model = new CopyrightEditModel
            {
                Name = copyright.Name
            };

            return View(model);
        }

        // POST: Copyrights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? id, CopyrightEditModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var copyright = await context.Copyrights.SingleOrDefaultAsync(m => m.Id == id);
            if (copyright == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                copyright.Name = model.Name;

                if (model.Image != null)
                {
                    var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.Image.ContentDisposition).FileName.Trim('"'));
                    var fileExt = Path.GetExtension(fileName);
                    if (!CopyrightsController.AllowedExtensions.Contains(fileExt))
                    {
                        this.ModelState.AddModelError(nameof(model.Image), "This file type is prohibited");
                    }

                    System.IO.File.Delete(this.hostingEnvironment.WebRootPath + copyright.ImagePath);

                    var imageId = Guid.NewGuid();
                    var imagePath = Path.Combine(this.hostingEnvironment.WebRootPath, "images/copyrights", imageId.ToString("N") + fileExt);

                    copyright.ImagePath = $"/images/copyrights/{imageId:N}{fileExt}";

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

        // GET: Copyrights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var copyright = await context.Copyrights
                .FirstOrDefaultAsync(m => m.Id == id);
            if (copyright == null)
            {
                return NotFound();
            }

            return View(copyright);
        }

        // POST: Copyrights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var copyright = await context.Copyrights.FindAsync(id);

            System.IO.File.Delete(this.hostingEnvironment.WebRootPath + copyright.ImagePath);

            context.Copyrights.Remove(copyright);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CopyrightExists(int id)
        {
            return context.Copyrights.Any(e => e.Id == id);
        }
    }
}

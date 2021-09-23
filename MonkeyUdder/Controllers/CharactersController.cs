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
    public class CharactersController : Controller
    {
        private readonly ApplicationDbContext context;

        private readonly IHostingEnvironment hostingEnvironment;
        private static readonly HashSet<String> AllowedExtensions = new HashSet<String> { ".jpg", ".jpeg", ".png"};

        public CharactersController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Characters
        public async Task<IActionResult> Index()
        {
            return View(await context.Characters.ToListAsync());
        }

        // GET: Characters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await context.Characters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        // GET: Characters/Create
        public IActionResult Create()
        {
            return this.View(new CharacterCreateModel());
        }

        // POST: Characters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CharacterCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var character = new Character
                {
                    Name = model.Name
                };

                var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.Image.ContentDisposition).FileName.Trim('"'));
                var fileExt = Path.GetExtension(fileName);
                if (!CharactersController.AllowedExtensions.Contains(fileExt))
                {
                    this.ModelState.AddModelError(nameof(model.Image), "This file type is prohibited");
                }

                var imageId = Guid.NewGuid();
                var imagePath = Path.Combine(this.hostingEnvironment.WebRootPath, "images/characters", imageId.ToString("N") + fileExt);

                character.ImagePath = $"/images/characters/{imageId:N}{fileExt}";

                using (var fileStream = new FileStream(imagePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                this.context.Add(character);
                await this.context.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Characters/Edit/5
        public async Task<IActionResult> Edit(Int32? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await context.Characters.SingleOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            var model = new CharacterEditModel
            {
                Name = character.Name
            };

            return View(model);
        }

        // POST: Characters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Int32? id, CharacterEditModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await context.Characters.SingleOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                character.Name = model.Name;

                if (model.Image != null)
                {
                    var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.Image.ContentDisposition).FileName.Trim('"'));
                    var fileExt = Path.GetExtension(fileName);
                    if (!CharactersController.AllowedExtensions.Contains(fileExt))
                    {
                        this.ModelState.AddModelError(nameof(model.Image), "This file type is prohibited");
                    }

                    System.IO.File.Delete(this.hostingEnvironment.WebRootPath + character.ImagePath);

                    var imageId = Guid.NewGuid();
                    var imagePath = Path.Combine(this.hostingEnvironment.WebRootPath, "images/characters", imageId.ToString("N") + fileExt);

                    character.ImagePath = $"/images/characters/{imageId:N}{fileExt}";

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

        // GET: Characters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await context.Characters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        // POST: Characters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var character = await context.Characters.FindAsync(id);

            System.IO.File.Delete(this.hostingEnvironment.WebRootPath + character.ImagePath);

            context.Characters.Remove(character);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

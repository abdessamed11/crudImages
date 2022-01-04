using CURDOperationWithImageUploadCore5_Demo.Data;
using CURDOperationWithImageUploadCore5_Demo.Models;
using CURDOperationWithImageUploadCore5_Demo.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CURDOperationWithImageUploadCore5_Demo.Controllers
{
    public class EtudiantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EtudiantsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
        }

        // GET: EtudiantsController
        public async Task<IActionResult> Index()
        {
            var etudiant = await _context.etudiants.ToListAsync();
            return View(etudiant);
        }

        // GET: EtudiantsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EtudiantsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EtudiantsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EtudiantViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                Etudiants etudiant = new Etudiants
                {
                    Name = model.Name,
                    ProfilePicture = uniqueFileName
                };
                _context.Add(etudiant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);

        }

        public void Del_file(int id,EtudiantViewModel model)
        {
            var filename = _context.etudiants.Find(model.Id).ProfilePicture;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder);
            string filePath = Path.Combine(uploadsFolder, filename);
            System.IO.File.Delete(filePath);

             
        }

        private string ProcessUploadedFile(EtudiantViewModel model)
        {
            string uniqueFileName = null;

            if (model.SpeakerPicture != null)
            {
               string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SpeakerPicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SpeakerPicture.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        

        // GET: EtudiantsController/Edit/5
        public ActionResult Edit(int? id)
        {
            
            if (id == null) return NotFound();

            var etudiant = _context.etudiants.Find(id);
            var picture = (_context.etudiants.Find(id).ProfilePicture.Split('_'))[1];
            var etudiantViewModel = new EtudiantViewModel
            {
                Id = etudiant.Id,
                Name=etudiant.Name,
                ExistingImage = etudiant.ProfilePicture
            };
            if (etudiant == null) return NotFound();

            return View(etudiantViewModel);
        }

        // POST: EtudiantsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EtudiantViewModel model)
        {
            if (ModelState.IsValid)
            {
                var etudiant =await _context.etudiants.FindAsync(model.Id);
                etudiant.Id = model.Id;
                etudiant.Name = model.Name;
                Del_file(id, model);

                if (model.SpeakerPicture != null)
                {
                    var oldfile = _context.etudiants.Find(model.Id).ProfilePicture;
                    var newfile = model.SpeakerPicture;
                    string uploads = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
                    
                    etudiant.ProfilePicture = ProcessUploadedFile(model);
                }
                
                _context.Update(etudiant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: EtudiantsController/Delete/5
        public ActionResult Delete(int id,EtudiantViewModel model)
        {
            var etudiant = _context.etudiants.FirstOrDefault(a=>a.Id == id);
            var etud = new EtudiantViewModel()
            {
                Id = etudiant.Id,
                Name = etudiant.Name,
                ExistingImage = etudiant.ProfilePicture
            };
            if (etudiant == null)
            {
                return NotFound();
            }
            return View(etud);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Etudiant = await _context.etudiants.FindAsync(id);
            string path = Directory.GetCurrentDirectory();
            var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), FileLocation.DeleteFileFromFolder, Etudiant.ProfilePicture);
            _context.etudiants.Remove(Etudiant);
            if (System.IO.File.Exists(CurrentImage))
            {
                System.IO.File.Delete(CurrentImage);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunrise.Contexts;
using Sunrise.Models;

namespace Sunrise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }



        /// <summary>
        /// Get all the categories
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult GetCategories(string sortType, string sortOrder, int pageSize = 20, int pageNumber = 1)
        {
            IQueryable<Category> cats = _context.Categories.AsQueryable();

            if (sortType == "Name" && sortOrder == "asc")
                cats = cats.OrderBy(c => c.Name);
            else if (sortType == "Name" && sortOrder == "desc")
                cats = cats.OrderByDescending(c => c.Name);
            else if (sortType == "Description" && sortOrder == "asc")
                cats = cats.OrderBy(c => c.Description);
            else if (sortType == "Description" && sortOrder == "desc")
                cats = cats.OrderByDescending(c => c.Description);



            if (pageSize > 50) pageSize = 50;
            if (pageSize < 1) pageSize = 1;
            if (pageNumber < 1) pageNumber = 1;


            return Ok(cats.Include(c => c.Products).ToList());
        }


        /// <summary>
        /// get the category name , description and products id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult GetCategory(int id)
        {
            if (id == 0) return BadRequest();//400
            Category cat = _context.Categories.Include(c => c.Products).FirstOrDefault(c=>c.Id == id);

            if (cat == null) return NotFound();//404

            return Ok(cat);//200
        }


        
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult PostCategory([FromForm]Category cat)
        {
            if (cat == null) return BadRequest();//400

            if(_context.Categories.Any(c=>c.Name == cat.Name))
            {
                ModelState.AddModelError("DuplicateName", "Name registered before ,Try Another Name");
                return BadRequest(ModelState);
            }
            if (cat.Name == cat.Description)
            {
                ModelState.AddModelError("Nameanddescription", "Name and Description must be different");
                return BadRequest(ModelState);
            }


            if (cat.Image == null)
            {
                cat.ImagePath = "\\images\\noimg.jpeg";

            }
            else
            {
                string ImgExtention = Path.GetExtension(cat.Image.FileName);
                Guid imgGuid = Guid.NewGuid();
                string imgName = imgGuid + ImgExtention;
                cat.ImagePath = "\\images\\categories\\" + imgName;

                string imgFullPath = _webHostEnvironment.WebRootPath + cat.ImagePath;

                FileStream imgStrem = new FileStream(imgFullPath, FileMode.Create);
                cat.Image.CopyTo(imgStrem);
                imgStrem.Dispose();

            }




            cat.CreatedAt = DateTime.Now;
            _context.Categories.Add(cat);
            _context.SaveChanges();

            return CreatedAtAction("GetCategory", new { id = cat.Id }, cat);

        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut]
        //edit
        public ActionResult PutCategory([FromForm]Category cat)
        {
            if (cat == null) return BadRequest();//400

            if (_context.Categories.Any(c => c.Name == cat.Name && c.Id != cat.Id))
            {
                ModelState.AddModelError("DuplicateName", "Name registered before ,Try Another Name");
                return BadRequest(ModelState);//400
            }

            if (cat.Name == cat.Description)
            {
                ModelState.AddModelError("NameandDescription", "Name and Description must be different");
                return BadRequest(ModelState);//400
            }

            if (cat.Image != null)
            {
                if (cat.ImagePath != "\\images\\noimg.jpeg")
                {
                    string oldImageFullPath = _webHostEnvironment.WebRootPath + cat.ImagePath;
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }

                }

                string ImgExtention = Path.GetExtension(cat.Image.FileName);
                Guid imgGuid = Guid.NewGuid();
                string imgName = imgGuid + ImgExtention;
                cat.ImagePath = "\\images\\categories\\" + imgName;

                string imgFullPath = _webHostEnvironment.WebRootPath + cat.ImagePath;
                FileStream imgStream = new FileStream(imgFullPath, FileMode.Create);

                cat.Image.CopyTo(imgStream);
                imgStream.Dispose();


            }


            cat.LastUpdatedAt = DateTime.Now;
            _context.Categories.Update(cat);
            _context.SaveChanges();

            return NoContent();//204

        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(int id)
        {
            if (id == null) return BadRequest();//400

            Category cat = _context.Categories.Find(id);

            if (cat == null)
            {
                return NotFound();//404
            }

            if (cat.ImagePath != "\\images\\noimg.jpeg")
            {
                string imgFullPath = _webHostEnvironment.WebRootPath + cat.ImagePath;

                if (System.IO.File.Exists(imgFullPath))
                {
                    System.IO.File.Delete(imgFullPath);
                }
            }


            _context.Categories.Remove(cat);
            _context.SaveChanges();

            return NoContent();//204

        }



    }
}

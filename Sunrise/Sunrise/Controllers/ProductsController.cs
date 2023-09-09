using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunrise.Contexts;
using Sunrise.Models;

namespace Sunrise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment= webHostEnvironment;
        }

        //XML comments

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>List of products</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult GetProducts(int catId,string? search , string sortType, string sortOrder, int pageSize = 20 ,int pageNumber = 1)
        {

            IQueryable<Product> prods = _context.Products.AsQueryable();

            if(catId !=0 )
            {
                prods = prods.Where(p=>p.CategoryId == catId);

            }

            if(string.IsNullOrEmpty(search) == false)
            {
                prods = prods.Where(p=>p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (sortType == "Name" && sortOrder == "asc")
                prods = prods.OrderBy(p => p.Name);
            else if (sortType == "Name" && sortOrder == "desc")
                prods = prods.OrderByDescending(p => p.Name);
            else if (sortType == "Description" && sortOrder == "asc")
                prods = prods.OrderBy(p => p.Description);
            else if (sortType == "Description" && sortOrder == "desc")
                prods = prods.OrderByDescending(p => p.Description);



            if (pageSize > 50) pageSize = 50;
            if(pageSize < 1) pageSize = 1;
            if(pageNumber < 1) pageNumber = 1;

            return Ok(prods.Include(p=>p.Category).ToList());
        }




        /// <summary>
        /// gets the products name , description , price and category id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a JASON object for the product</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult GetProduct(int id)
        {
            if (id == 0) return BadRequest();//400
            Product prod = _context.Products.Include(p => p.Category).FirstOrDefault(p=>p.Id == id);

            if (prod == null) return NotFound();//404

            return Ok(prod);//200
        }



        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        //add
        public ActionResult PostProduct([FromForm]Product prod)
        {
            if (prod == null) return BadRequest();//400

            if (_context.Products.Any(p => p.Name == prod.Name))
            {
                ModelState.AddModelError("DuplicateName", "Name registered before ,Try Another Name");
                return BadRequest(ModelState);//400
            }

            if (prod.Name == prod.Description)
            {
                ModelState.AddModelError("Nameanddescription", "Name and Description must be different");
                return BadRequest(ModelState);//400
            }
            if (prod.ProductDate > DateTime.Now)
            {
                ModelState.AddModelError("FutureProductionDate", "Production Date not valid");
                return BadRequest(ModelState);
            }

            if (prod.Image == null)
            {
                prod.ImagePath = "\\images\\noimg.jpeg";

            }
            else
            {
                string ImgExtention = Path.GetExtension(prod.Image.FileName);
                Guid imgGuid = Guid.NewGuid();
                string imgName = imgGuid + ImgExtention;
                prod.ImagePath = "\\images\\products\\" + imgName;

                string imgFullPath = _webHostEnvironment.WebRootPath + prod.ImagePath;

                FileStream imgStrem = new FileStream(imgFullPath, FileMode.Create);
                prod.Image.CopyTo(imgStrem);
                imgStrem.Dispose();

            }



            prod.CreatedAt = DateTime.Now;
            _context.Products.Add(prod);
            _context.SaveChanges();
            //anounymus object
            return CreatedAtAction("GetProduct", new { id = prod.Id }, prod); //201 & RequestedUrl 
            //CreatedAtAction( string actionName, object routeValues , object value)
            //return Ok(prod);

        }



       
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut]
        //edit
        public ActionResult PutProduct([FromForm]Product prod)
        {
            if (prod == null) return BadRequest();//400

            if (_context.Products.Any(p => p.Name == prod.Name && p.Id != prod.Id))
            {
                ModelState.AddModelError("DuplicateName", "Name registered before ,Try Another Name");
                return BadRequest(ModelState);//400
            }

            if (prod.Name == prod.Description)
            {
                ModelState.AddModelError("NameandDescription", "Name and Description must be different");
                return BadRequest(ModelState);//400
            }
            if (prod.ProductDate > DateTime.Now)
            {
                ModelState.AddModelError("FutureProductionDate", "Production Date can not be a future date");
                return BadRequest(ModelState);//400
            }

            if (prod.Image != null)
            {
                if(prod.ImagePath != "\\images\\noimg.jpeg")
                {
                    string oldImageFullPath = _webHostEnvironment.WebRootPath + prod.ImagePath;
                    if(System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }

                }

                string ImgExtention = Path.GetExtension(prod.Image.FileName);
                Guid imgGuid = Guid.NewGuid();
                string imgName = imgGuid + ImgExtention;
                prod.ImagePath = "\\images\\products\\" + imgName;

                string imgFullPath = _webHostEnvironment.WebRootPath + prod.ImagePath;
                FileStream imgStream = new FileStream(imgFullPath, FileMode.Create);

                prod.Image.CopyTo(imgStream);
                imgStream.Dispose();


            }


            prod.LastUpdatedAt = DateTime.Now;
            _context.Products.Update(prod);
            _context.SaveChanges();

            return NoContent();//204

        }



        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            if (id == null) return BadRequest();//400

            Product prod = _context.Products.Find(id);

             if (prod == null)
             {
                 return NotFound();//404
             }

             if(prod.ImagePath != "\\images\\noimg.jpeg")
            {
                string imgFullPath = _webHostEnvironment.WebRootPath + prod.ImagePath; 

                if(System.IO.File.Exists(imgFullPath))
                {
                    System.IO.File.Delete(imgFullPath);
                }
            }

            
            _context.Products.Remove(prod);
            _context.SaveChanges();

            return NoContent();//204

        }



    }
}

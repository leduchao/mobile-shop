using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileWeb.Areas.Admin.Services.ProductService;
using MobileWeb.Models;
using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;

namespace MobileWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/products")]
    public class ProductsController : Controller
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        //[Route("danh-sach-san-pham")]
        public async Task<IActionResult> Index(string keyword, int page)
        {
            var productList = _service.GetAllAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                productList = productList.Where(p => p.Name!.ToLower().Contains(keyword.ToLower()));
            }

            productList = productList.OrderByDescending(p => p.CreateAt);

            if (page < 1) page = 1;
            int pageSize = 15;

            var paginatedList = await PaginatedList<Product>.CreateAsync(productList, page, pageSize);

            return View(paginatedList);
        }

        [HttpGet]
        [Route("product-detail")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _service.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            var specifications = await _service.GetSpecificationsAsync(product.Id);
            var productImages = await _service.GetAllProductImagesAsync(product);

            if (specifications == null)
                return NotFound();

            product.Specifications ??= new Specifications();

            var productDTO = new ProductDTO
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category,
                Color = product.Color,
                RAM = specifications!.RAM,
                ROM = specifications.ROM,
                NumberOfBehindCamera = specifications.NumberOfBehindCamera,
                Model = specifications.Model,
                OperatingSystemVersion = specifications.OperatingSystem,
                Specifications = product.Specifications,
                ImageList = product.Images,
            };

            ViewBag.ProductId = product.Id;
            ViewBag.ProductImg = product.ImgUrl;
            ViewBag.ProductImages = productImages;

            return View(productDTO);
        }

        // GET: Admin/Products/Create
        [Route("create-product")]
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(
                await _service.GetAllCategoriesAsync(), "Id", "Name");

            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [Route("create-product")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDTO request)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(request);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(
                await _service.GetAllCategoriesAsync(), "Id", "Name");

            return View(request);
        }

        // GET: Admin/Products/Edit/5
        [Route("edit-product")]
        public async Task<IActionResult> Edit(int id)
        {

            var product = await _service.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var specifications = await _service.GetSpecificationsAsync(product.Id);

            if (specifications == null)
            {
                return NotFound();
            }

            var productDTO = new ProductDTO
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                Color = product.Color,
                RAM = specifications.RAM,
                ROM = specifications.ROM,
                NumberOfBehindCamera = specifications.NumberOfBehindCamera,
                Model = specifications.Model,
                OperatingSystemVersion = specifications.OperatingSystem
            };

            var productImages = await _service.GetAllProductImagesAsync(product);

            ViewBag.ProductId = product.Id;
            ViewBag.CategoryId = new SelectList(
                await _service.GetAllCategoriesAsync(), "Id", "Name");
            ViewBag.ProductImages = productImages;
            ViewBag.Avatar = product.ImgUrl;

            return View(productDTO);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [Route("edit-product")]
        public async Task<IActionResult> Edit(int id, ProductDTO request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(id, request);
                    return RedirectToAction(nameof(Details), new {id});
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

            }

            ViewData["CategoryId"] = new SelectList(
                await _service.GetAllCategoriesAsync(), "Id", "Name");

            return View(request);
        }

        // GET: Admin/Products/Delete/5
        [HttpGet]
        [Route("delete-product")]
        public async Task<IActionResult> Delete(int id, int page)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { page });
        }

        [HttpPost]
        [Route("delete-product-image")]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            var result = await _service.DeleteImageAsync(id);

            if (result)
                return Ok();

            return NotFound();
        }

        //[HttpPost]
        //[Route("load-product-image")]
        //public async Task<IActionResult> LoadProductImages(int id)
        //{
        //    var product = await _service.GetByIdAsync(id);

        //    if (product is not null)
        //    {
        //        var productImages = await _service.GetAllProductImagesAsync(product);

        //        return Json(new { success = 1, images = productImages });
        //    }

        //    return Json(new { success = 0, message = "There is no product!" });
        //}
    }
}

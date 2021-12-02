using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Park_Michael_HW5.DAL;
using Park_Michael_HW5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace Park_Michael_HW5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products
                .Include(c => c.Suppliers)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            //course was not found in the database
            if (product == null)
            {
                return View("Error", new String[] { "That course was not found in the database." });
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,Description,Price")] Product product, int[] SelectedSuppliers)
        {
            if (ModelState.IsValid == false)
            {

                ViewBag.AllSuppliers = GetAllSuppliers();
                //go back to the Create view to try again
                return View(product);
            }


            _context.Add(product);
            await _context.SaveChangesAsync();

            foreach (int supplierID in SelectedSuppliers)
            {
                //find the supplier associated with that id
                Supplier dbSupplier = _context.Suppliers.Find(supplierID);

                //add the supplier to the course's list of suppliers and save changes
                product.Suppliers.Add(dbSupplier);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));

        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //if the user didn't specify a course id, we can't show them 
            //the data, so show an error instead
            if (id == null)
            {
                return View("Error", new string[] { "Please specify a course to edit!" });
            }

            //find the course in the database
            //be sure to change the data type to course instead of 'var'
            Product product = await _context.Products.Include(c => c.Suppliers)
                                           .FirstOrDefaultAsync(c => c.ProductID == id);

            //if the course does not exist in the database, then show the user
            //an error message
            if (product == null)
            {
                return View("Error", new string[] { "This course was not found!" });
            }

            //populate the viewbag with existing departments
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ProductID,ProductName,Description,Price,ProductType")] Product product, int[] SelectedSuppliers)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid == false) //there is something wrong
            {
                ViewBag.AllSuppliers = GetAllSuppliers(product);
                return View(product);
            }
            
                try
                {
                    Product dbProduct = _context.Products
                        .Include(c => c.Suppliers)
                        .FirstOrDefault(c => c.ProductID == product.ProductID);

                    List<Supplier> SuppliersToRemove = new List<Supplier>();

                    foreach (Supplier supplier in dbProduct.Suppliers)
                    {
                        //see if the new list contains the department id from the old list
                        if (SelectedSuppliers.Contains(supplier.SupplierID) == false)//this department is not on the new list
                        {
                            SuppliersToRemove.Add(supplier);
                        }
                    }

                    foreach (Supplier supplier in SuppliersToRemove)
                    {
                        //remove this course department from the course's list of departments
                        dbProduct.Suppliers.Remove(supplier);
                        _context.SaveChanges();
                    }


                    foreach (int supplierID in SelectedSuppliers)
                    {
                        if (dbProduct.Suppliers.Any(d => d.SupplierID == supplierID) == false)//this department is NOT already associated with this course
                        {
                            //Find the associated department in the database
                            Supplier dbSupplier = _context.Suppliers.Find(supplierID);

                            //Add the department to the course's list of departments
                            dbProduct.Suppliers.Add(dbSupplier);
                            _context.SaveChanges();
                        }
                    }

                    //update the course's scalar properties
                    dbProduct.Price = product.Price;
                    dbProduct.ProductName = product.ProductName;
                    dbProduct.Description = product.Description;

                    //save the changes
                    _context.Products.Update(dbProduct);
                    _context.SaveChanges();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        private MultiSelectList GetAllSuppliers()
        {
            //Create a new list of departments and get the list of the departments
            //from the database
            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            //Multi-select lists do not require a selection, so you don't need 
            //to add a dummy record like you do for select lists

            //use the MultiSelectList constructor method to get a new MultiSelectList
            MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName");

            //return the MultiSelectList
            return mslAllSuppliers;
        }

        private MultiSelectList GetAllSuppliers(Product product)
        {
            //Create a new list of departments and get the list of the departments
            //from the database
            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            //loop through the list of course departments to find a list of department ids
            //create a list to store the department ids
            List<Int32> selectedSupplierIDs = new List<Int32>();

            //Loop through the list to find the DepartmentIDs
            foreach (Supplier associatedSupplier in product.Suppliers)
            {
                selectedSupplierIDs.Add(associatedSupplier.SupplierID);
            }

            //use the MultiSelectList constructor method to get a new MultiSelectList
            MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName", selectedSupplierIDs);

            //return the MultiSelectList
            return mslAllSuppliers;
        }
    }
}

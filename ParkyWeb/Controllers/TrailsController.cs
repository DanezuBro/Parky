using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;

        public TrailsController(INationalParkRepository npRepo, ITrailRepository trailRepo)
        {
            _npRepo = npRepo;
            _trailRepo = trailRepo;
        }

        public IActionResult Index()
        {
            return View(new TrailsVM() { });
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));
            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = i.Name, Value=i.Id.ToString()}),
                Trail = new Trail()
        };

            if (id == null)
            {
              return View(objVM);
            }
            objVM.Trail = await _trailRepo.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
            if(objVM.Trail == null)
            {
                return NotFound();
            }
            return View(objVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if(ModelState.IsValid)
            {
                
                if(obj.Trail.Id==0)
                {
                    await _trailRepo.CreateAsync(SD.TrailAPIPath,obj.Trail, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _trailRepo.UpdateAsync(SD.TrailAPIPath+obj.Trail.Id, obj.Trail, HttpContext.Session.GetString("JWToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));
                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                    Trail = obj.Trail
                };
                return View(objVM);
            }
        }


        #region APICalls

        public async Task<IActionResult> GetAllTrails()
        {
         return Json(new { data  = await _trailRepo.GetAllAsync(SD.TrailAPIPath, HttpContext.Session.GetString("JWToken")) });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trailRepo.DeleteAsync(SD.TrailAPIPath, id, HttpContext.Session.GetString("JWToken"));
            if(status)
            {
                return Json(new { success = true, message="Delete successful"});
            }
            return Json(new { success = false, message = "Delete not successful" });
        }
        #endregion


    }
}

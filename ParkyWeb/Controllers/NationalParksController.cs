using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;

        public NationalParksController(INationalParkRepository npRepo)
        {
            _npRepo = npRepo;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }


        #region APICalls

        public async Task<IActionResult> GetAllNationalParks() 
        {
            return Json(new { data  = await _npRepo.GetAllAsync(SD.APIBaseUrl + SD.NationalParkAPIPath)});
        }

        #endregion


    }
}

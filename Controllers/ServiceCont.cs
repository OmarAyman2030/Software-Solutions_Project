using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Software_Solutions.DTO;
using Software_Solutions.Interface;

namespace Software_Solutions.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ServiceCont : ControllerBase
    {
        private readonly IService _service;

        public ServiceCont(IService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var services = await _service.GetAllServices();
            return Ok(services);

        }

        [HttpGet("id")]
         public async Task<ActionResult> GetByID(int id)
         {
            var service = await _service.GetServiceById(id);
            if (service == null) return NotFound();

            return Ok(service);

         }

        [Authorize (Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateService(ServiceDto dto)
        {
            var NewService = await _service.CreateService(dto);
            return CreatedAtAction(nameof (GetByID), new {id = NewService.Id} , NewService);

        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult> UpdateService(int id , ServiceDto dto)
        {
            var updated = await _service.UpdateService(id,dto);

            if(!updated) return NotFound();

            return NoContent();

        }

        [Authorize (Roles = "Admin")]
        [HttpDelete]
        public async Task<ActionResult> Delete (int id)
        {
            var deleted = await _service.DeleteService(id);
            if(!deleted) return NotFound();

            return NoContent();
        }






    }

}

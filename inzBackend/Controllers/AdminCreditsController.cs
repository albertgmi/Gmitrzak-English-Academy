using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.CreditModels;
using inzBackend.Services.CreditServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/admin/credits")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CreditController : ControllerBase
    {
        private readonly ICreditService _creditService;
        public CreditController(ICreditService creditService)
        {
            _creditService = creditService;
        }

        [HttpGet("summary")]
        public ActionResult<List<UserCreditSummaryDto>> GetSummary()
        {
            var result = _creditService.getAllUsersCreditSummary();
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public ActionResult<StudentCreditDetailDto> GetStudentDetail(int studentId)
        {
            var result = _creditService.getStudentCreditDetail(studentId);
            return Ok(result);
        }

        [HttpPatch("purchase/{purchaseId}/status")]
        public ActionResult UpdateStatus(int purchaseId, [FromBody] UpdatePurchaseStatusRequest request)
        {
            _creditService.updatePurchaseStatus(purchaseId, request.Status);
            return Ok();
        }
    }
}

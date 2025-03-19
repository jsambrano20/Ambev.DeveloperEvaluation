using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetPaginatedSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.PatchProductSale;
using Ambev.DeveloperEvaluation.Application.Sales.PatchSale;
using Ambev.DeveloperEvaluation.Application.Sales.RemoveProductSale;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetPaginatedSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.PatchProductSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.PatchSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveProductSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{

    /// <summary>
    /// Controller to manage sale operations, including retrieving, creating, updating, and deleting sales.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the SaleController.
        /// </summary>
        /// <param name="mediator">The mediator instance used for sending commands and handling requests.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        public SaleController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a paginated list of sales for the authenticated user.
        /// </summary>
        /// <param name="request">Contains the query parameters for pagination.</param>
        /// <param name="cancellationToken">Token to cancel the request if needed.</param>
        /// <returns>Returns a paginated list of sales.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<PaginatedList<GetPaginatedSaleResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSale([FromQuery] GetPaginatedSaleRequest request, CancellationToken cancellationToken)
        {
            var validator = new PaginatedRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<GetPaginatedSalesCommand>(request);

            var result = await _mediator.Send(command, cancellationToken);

            var response = _mapper.Map<PaginatedList<GetPaginatedSaleResponse>>(result);

            return Ok(
                new ApiResponseWithData<PaginatedList<GetPaginatedSaleResponse>>()
                {
                    Success = true,
                    Message = "Sales retrieved successfully",
                    Data = response
                });
        }

        /// <summary>
        /// Retrieves a sale by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the sale.</param>
        /// <param name="cancellationToken">Token to cancel the request if needed.</param>
        /// <returns>Returns the details of the requested sale.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetSaleRequest { Id = id };
            var validator = new GetSaleRequestValidator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<GetSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = _mapper.Map<GetSaleResponse>(response)
            });
        }

        /// <summary>
        /// Creates a new sale based on the provided data.
        /// </summary>
        /// <param name="request">Contains the sale data to be created.</param>
        /// <param name="cancellationToken">Token to cancel the request if needed.</param>
        /// <returns>Returns the created sale details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
        {
            var validationResult = request.Validate();

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<CreateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<CreateSaleResponse>(response)
            });
        }

        
        /// <summary>
        /// Updates the status of a sale.
        /// </summary>
        /// <param name="id">The unique identifier of the sale.</param>
        /// <param name="cancellationToken">Token to cancel the request if needed.</param>
        /// <returns>Returns the result of the status update operation.</returns>
        /// <remarks>
        /// <listheader>The update sequence:</listheader> 
        /// <list type="bullet">"cancel" -> "active"</list>
        /// <list type="bullet">"active" -> "payed"</list>
        /// <list type="bullet">"payed" -> "delivery"</list>
        /// <list type="bullet">"delivery" -> "finished"</list>
        /// </remarks>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new PatchSaleRequest { Id = id };
            var validatorResult = request.Validate();
            if (!validatorResult.IsValid)
                return BadRequest(validatorResult.Errors);

            var command = _mapper.Map<PatchSaleCommand>(request);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result) return BadRequest(new ApiResponse { Message = "Unable to update sale", Success = false });

            return Ok(new ApiResponse { Success = true, Message = $"Product Sale {id} updated successfully" });
        }

        /// <summary>
        /// Updates the product quantity within a sale.
        /// </summary>
        /// <param name="id">The sale id to update.</param>
        /// <param name="request">The data to be updated.</param>
        /// <param name="cancellationToken">Token to cancel the request if needed.</param>
        /// <returns>Returns the result of the product update operation.</returns>
        [HttpPatch("{id}/Product")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchProductSale([FromRoute] Guid id, [FromBody] PatchProductSaleRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;
            var validationResult = request.Validate();

            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var command = _mapper.Map<PatchProductSaleCommand>(request);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result) return BadRequest(new ApiResponse() { Success = false, Message = "Unable to update sale" });

            return Ok(new ApiResponse() { Success = true, Message = $"Product Sale {id} successfully updated" });
        }

        /// <summary>
        /// Cancels an existing sale, provided its status is "Active".
        /// </summary>
        /// <param name="id">The unique identifier of the sale to be canceled.</param>
        /// <param name="cancellationToken">Token to cancel the request if needed.</param>
        /// <returns>Returns the result of the cancellation operation.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new DeleteSaleRequest { Id = id };
            var validator = new DeleteSaleRequestValidator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<DeleteSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            if (!response) return BadRequest("Unable to cancel sale");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Sale canceled successfully",
            });
        }

        /// <summary>
        /// Cancels specific products from a sale.
        /// </summary>
        /// <param name="id">The sale id to remove products from.</param>
        /// <param name="request">The product data to be removed.</param>
        /// <param name="cancellationToken">Token to cancel the request if needed.</param>
        /// <returns>Returns the result of the product removal operation.</returns>
        [HttpDelete("{id}/Product")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveProductSale([FromRoute] Guid id, [FromBody] RemoveProductSaleRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;

            var validationResult = request.Validate();

            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var command = _mapper.Map<RemoveProductSaleCommand>(request);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result) return BadRequest(new ApiResponse() { Success = false, Message = "Unable to cancel product item from sale" });

            return Ok(new ApiResponse() { Success = true, Message = $"Product Sale {id} canceled successfully" });
        }
    }

}
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetPaginatedProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetPaginatedProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products
{

    /// <summary>
    /// Controller responsible for handling product-related operations such as creation, retrieval, pagination, and deletion.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance used for sending requests.</param>
        /// <param name="mapper">The AutoMapper instance used for mapping objects.</param>
        public ProductController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new product based on the provided data.
        /// </summary>
        /// <param name="request">The request data to create a new product.</param>
        /// <param name="cancellationToken">The cancellation token used for operation cancellation.</param>
        /// <returns>A response indicating whether the product was successfully created.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateProductResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateProductRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<CreateProductCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new ApiResponseWithData<CreateProductResponse>
            {
                Success = true,
                Message = "Product created successfully",
                Data = _mapper.Map<CreateProductResponse>(response)
            });
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="cancellationToken">The cancellation token used for operation cancellation.</param>
        /// <returns>The product data if found, otherwise a 404 Not Found response.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetProductRequest { Id = id };
            var validator = new GetProductRequestValidator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<GetProductCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            if (response == null)
                return NotFound(new ApiResponse { Message = "Product not found", Success = false });

            return Ok(new ApiResponseWithData<GetProductResponse>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = _mapper.Map<GetProductResponse>(response)
            });
        }

        /// <summary>
        /// Retrieves a paginated list of all products.
        /// </summary>
        /// <param name="request">The request data for pagination of products.</param>
        /// <param name="cancellationToken">The cancellation token used for operation cancellation.</param>
        /// <returns>A paginated list of products.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<PaginatedList<GetPaginatedProductResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct([FromQuery] GetPaginatedProductRequest request, CancellationToken cancellationToken)
        {
            var validator = new PaginatedRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<GetPaginatedProductCommand>(request);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<PaginatedList<GetPaginatedProductResponse>>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = _mapper.Map<PaginatedList<GetPaginatedProductResponse>>(response)
            });
        }

        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to be deleted.</param>
        /// <param name="cancellationToken">The cancellation token used for operation cancellation.</param>
        /// <returns>A response indicating whether the product was successfully deleted or not found.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new DeleteProductRequest { Id = id };
            var validator = new DeleteProductRequestValidator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            var command = _mapper.Map<DeleteProductCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound(new ApiResponse { Message = "Product not found", Success = false });

            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"Product {id} deleted successfully"
            });
        }
    }
}
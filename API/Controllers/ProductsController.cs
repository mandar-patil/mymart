using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Specifications;
using AutoMapper;
using API.Dtos;
using API.Errors;
using API.Helpers;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productsBrandRepo;
        private readonly IGenericRepository<ProductType> _productsTypeRepo;

        private readonly IMapper _mapper;
        public ProductsController(IGenericRepository<Product> productsRepo,
        IGenericRepository<ProductBrand> productsBrandRepo,
        IGenericRepository<ProductType> productsTypeRepo,
        IMapper mapper)
        {
            this._productsRepo = productsRepo;
            this._productsBrandRepo = productsBrandRepo;
            this._productsTypeRepo = productsTypeRepo;
            this._mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<Pagination<IReadOnlyList<Product>>>> GetProducts(
            [FromQuery]ProductSpecParams productParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItem = await _productsRepo.CountAsync(countSpec);
            var products = await _productsRepo.ListAsync(spec);
            var data = _mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex,productParams.PageSize,totalItem,data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _productsRepo.GetEntityWithSpec(spec);

            if(product == null)
                return NotFound(new ApiResponse(404));

            return _mapper.Map<Product,ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productsBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<ProductType>>> GetProductTypes()
        {
            return Ok(await _productsTypeRepo.ListAllAsync());
        }
    }
}
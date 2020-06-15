using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.Dtos;
using AutoMapper;
using API.Errors;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productsRepo, IGenericRepository<ProductBrand> productBrandRepo, IGenericRepository<ProductType> productTypeRepo, IMapper mapper)
        {
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
            _productBrandRepo = productBrandRepo;
            _productsRepo = productsRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(string sort)
        {
            /*
                var products = await _productsRepo.ListAllAsync();
                return Ok(products);
            */
            // so we need to include the producttype and brands now this is how the specification + generic works their charm
            var spec = new ProductsWithTypesAndBrandsSpecification(sort);

            var products = await _productsRepo.ListAsync(spec);
            /*it would be efficient to do this in specification but we just keep it simple for now*/

            //return Ok(products); so we need to flatten this out too same as below getproduct by id
            //return products.Select(product => new ProductToReturnDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Description = product.Description,
            //    PictureUrl = product.PictureUrl,
            //    Price = product.Price,
            //    ProductBrand = product.ProductBrand.Name,
            //    ProductType = product.ProductType.Name
            //}).ToList();

            //since we now have an automapper we need to change now the return
            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));

        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // this is for swagger for it to document 
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)] // specified the type of so that in swagger it knows the return type
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            // since we change the return below the original public async Task<ActionResult<Product>> GetProduct(int id) will change to producttoreturndeto

            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productsRepo.GetEntityWithSpec(spec);
            //return await _productsRepo.GetEntityWithSpec(spec);
            // so we want to flatten out the return from the commented code above before we return it to our client so we use the Dtos
            //return new ProductToReturnDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Description = product.Description,
            //    PictureUrl = product.PictureUrl,
            //    Price = product.Price,
            //    ProductBrand = product.ProductBrand.Name,
            //    ProductType = product.ProductType.Name
            //};
            //So we now use the automapper nuget package for us not to always code the ProductTOreturnDto since we want it to automap by itself
            if (product == null) return NotFound(new ApiResponse(404));

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var brands = await _productBrandRepo.ListAllAsync();
            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            var types = await _productTypeRepo.ListAllAsync();
            return Ok(types);
        }
    }
}
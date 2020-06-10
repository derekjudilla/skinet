using System.Linq;
using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        // public static method are those we can use where we don't needing to generate an instance of this class 
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;
            
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria); //ex p => p/ProductTypeId == id this line of code will replace inside Where("here replace")
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            
            #region Equivalent of query see inside
                /*
                    the query above is basically the replacement of include in this query below

                     return await _context.Products
                        .Include(p => p.ProductType)
                        .Include(p => p.ProductBrand)
                        .ToListAsync();

                */
            #endregion

            return query;
        }
    }
}
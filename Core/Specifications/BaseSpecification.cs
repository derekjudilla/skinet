using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification()
        {
        }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria {get; }

        public List<Expression<Func<T, object>>> Includes {get; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        //protected means is we can use this method here in the base class and all other derived class under basespecifications
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        #region Purpose for this class and also Ispecification class

        /*
            Is to replace the "INCLUDE" found in the code block below which is present in ProductRepository
                public async Task<IReadOnlyList<Product>> GetProductsAsync()
                {
                    // return await _context.Products.ToListAsync();
                    //eagerloading
                    return await _context.Products
                        .Include(p => p.ProductType)
                        .Include(p => p.ProductBrand)
                        .ToListAsync();

                }
            refer to SpecificationEvaluator.cs to further see whats the use
        
        */

        #endregion
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }
    }
}
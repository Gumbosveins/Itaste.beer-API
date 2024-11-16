using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ItbApi
{
    public static class IQueryableExtensions
    {
        public static IQueryable<User> IncludeAll(this IQueryable<User> query)
        {
            return query
                .Include(u => u.Room)
                    .ThenInclude(r => r.Room2ReviewTypes)
                    .ThenInclude(a => a.ReviewType)
                .Include(u => u.Room)
                    .ThenInclude(r => r.BeverageReviews)
                .Include(u => u.Room)
                    .ThenInclude(r => r.Room2Beverages)
                    .ThenInclude(a => a.Beverage)
                    .ThenInclude(a => a.Brewery)
                .Include(u => u.Room)
                    .ThenInclude(r => r.Room2Beverages)
                    .ThenInclude(a => a.Beverage)
                    .ThenInclude(a => a.BeverageGroup)
                .Include(u => u.Room)
                    .ThenInclude(r => r.Room2Beverages)
                    .ThenInclude(a => a.Beverage)
                    .ThenInclude(a => a.BeverageType)
                .Include(u => u.BeverageReviews)
                    .ThenInclude(a => a.ReviewParts)
                    .AsSplitQuery();
        }

        public static IQueryable<Room> IncludeAll(this IQueryable<Room> query)
        {
            return query
                .Include(r => r.Users)
                    .ThenInclude(u => u.BeverageReviews)
                    .ThenInclude(a => a.ReviewParts)
                .Include(r => r.Room2ReviewTypes)
                    .ThenInclude(a => a.ReviewType)
                .Include(r => r.BeverageReviews)
                .Include(r => r.Room2Beverages)
                    .ThenInclude(a => a.Beverage)
                    .ThenInclude(a => a.Brewery)
                .Include(r => r.Room2Beverages)
                    .ThenInclude(a => a.Beverage)
                    .ThenInclude(a => a.BeverageGroup)
                .Include(r => r.Room2Beverages)
                    .ThenInclude(a => a.Beverage)
                    .ThenInclude(a => a.BeverageType)
                    .AsSplitQuery();
        }

        public static IQueryable<Room> IncludeBeerAll(this IQueryable<Room> query)
        {
            return query
                .Include(r => r.Room2Beverages)
                    .ThenInclude(a => a.Beverage)
                    .ThenInclude(a => a.Brewery)
                    .AsSplitQuery();
            ;

        }

        public static IQueryable<Brewery> IncludeAll(this IQueryable<Brewery> query)
        {
            return query
                .Include(b => b.Beverages);
        }

        public static IQueryable<BeverageGroup> IncludeAll(this IQueryable<BeverageGroup> query)
        {
            return query
                .Include(bg => bg.Beverages);
        }

        public static IQueryable<BeverageType> IncludeAll(this IQueryable<BeverageType> query)
        {
            return query
                .Include(bt => bt.Beverages);
        }

        public static IQueryable<Beverage> IncludeAll(this IQueryable<Beverage> query)
        {
            return query
                .Include(b => b.Brewery)
                .Include(b => b.BeverageGroup)
                .Include(b => b.BeverageType)
                .Include(b => b.Room2Beverages)
                .AsSplitQuery();
            
        }

        public static IQueryable<ReviewType> IncludeAll(this IQueryable<ReviewType> query)
        {
            return query
                .Include(rt => rt.Room2ReviewTypes)
                .Include(rt => rt.ReviewParts)
                .AsSplitQuery();
        }

        public static IQueryable<Room2ReviewType> IncludeAll(this IQueryable<Room2ReviewType> query)
        {
            return query
                .Include(r2rt => r2rt.Room)
                .Include(r2rt => r2rt.ReviewType)
                .AsSplitQuery();

        }

        public static IQueryable<BeverageReview> IncludeAll(this IQueryable<BeverageReview> query)
        {
            return query
                .Include(br => br.User)
                .Include(br => br.Room)
                .Include(br => br.ReviewParts)
                .AsSplitQuery();

        }

        public static IQueryable<ReviewPart> IncludeAll(this IQueryable<ReviewPart> query)
        {
            return query
                .Include(rp => rp.BeverageReview)
                .Include(rp => rp.ReviewType)
                .AsSplitQuery();
        }

        public static IQueryable<Room2Beverage> IncludeAll(this IQueryable<Room2Beverage> query)
        {
            return query
                .Include(r2b => r2b.Room)
                .Include(r2b => r2b.Beverage)
                .AsSplitQuery();
        }
    }
}

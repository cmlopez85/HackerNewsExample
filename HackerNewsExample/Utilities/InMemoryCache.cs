using System;
using System.Runtime.Caching;

namespace HackerNewsExample.Utilities
{
    /// <summary>  
    ///  A simple class for caching items in memory. This sort of  wraps/extends the functionality
    ///  of System.Runtime.Caching.MemoryCache.  This code is not mine - it is from an answer 
    ///  on Stack Overflow.  I figured there was no real reason to re-invent the wheel here.
    /// </summary>  
    public class InMemoryCache
    {
        /// <summary>  
        ///  This method caches items in memory.  It will get/set the item using the cacheKey parameter
        ///  that is passed in, which acts as the name/identifier.  The second parameter is a callback function
        ///  which will be called to retrieve the item in the event that it has not been previously cached.
        ///  Once retrieved using the callback, the item will then be cached.
        /// </summary>  
        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            // Try to retrieve item from cache
            T item = MemoryCache.Default.Get(cacheKey) as T;
            // If the item is null and was not found in cache
            if (item == null)
            {
                // Execute the passed in callback function to retrieve the item and assign the returned value to
                // the item variable
                item = getItemCallback();
                // Now cache the item which was retrieved by the callback function
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(10));
            }
            // Return item
            return item;
        }
    }
}
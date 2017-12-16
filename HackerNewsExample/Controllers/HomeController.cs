using HackerNewsExample.Models;
using HackerNewsExample.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HackerNewsExample.Controllers
{
    /// <summary>  
    ///  This the controller class for Home.  It will display the best 200 stories from Hacker News
    ///  as well as the user by whom the story was submitted.
    /// </summary>  
    public class HomeController : Controller
    {
        protected const string baseUrl = "https://hacker-news.firebaseio.com/";
        protected InMemoryCache cacheProvider = new InMemoryCache();

        /// <summary>  
        ///  This is the Index method which will return the View object which will be displayed
        ///  when the user navigates to index/root.
        /// </summary>  
        public async Task<ActionResult> Index()
        {
            // Retrieve a list of the 200 best stories.  They will either be found in in-memory cache
            // or re-retrieved from the Hacker News server.
            List<Story> bestStories = await cacheProvider.GetOrSet("bestStories", GetBestStories);
            return View(bestStories);
        }

        /// <summary>  
        ///  This method will retrieve a list of strings containing the IDs for the 200 "best" stories
        ///  from Hacker News.
        /// </summary>  
        protected async Task<List<string>> GetBestStoryIds()
        {
            // Instantiate list of strings which the story IDs will be added to
            List<string> bestStoryIds = new List<string>();
            using (var client = new HttpClient())
            {
                // Set the base URL
                client.BaseAddress = new Uri(baseUrl);
                // Clear default request headers
                client.DefaultRequestHeaders.Clear();
                // Set the the media type to json, as that's what we're exepecting back
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Wait for the HttpClient to read the list of IDs
                HttpResponseMessage res = await client.GetAsync("v0/beststories.json?print=pretty");
                // If the HTTP response indicates that the request was successful
                if (res.IsSuccessStatusCode)
                {
                    // Store the json result in a variable
                    var json = res.Content.ReadAsStringAsync().Result;
                    // Deseriable the json to a list of strings
                    bestStoryIds = JsonConvert.DeserializeObject<List<string>>(json);
                }
                // Return the list of IDs
                return bestStoryIds;
            }
        }

        /// <summary>  
        ///  This method will retrieve a list of Story objects containing the details of the 200 best stories
        ///  from Hacker News.
        /// </summary>  
        protected async Task<List<Story>> GetBestStories()
        {
            // Instantiate list of Story objects which the best stories will be added to
            List<Story> bestStories = new List<Story>();
            // Retrieve the IDs of the best 200 stories so that we know where to find them.  They will either
            // be found in in-memory cache or re-retrieved from the server.
            List<string> bestStoryIds = await cacheProvider.GetOrSet("bestStoryIds", GetBestStoryIds);
            using (var client = new HttpClient())
            {
                // Set the base URL
                client.BaseAddress = new Uri(baseUrl);
                // Clear default request headers
                client.DefaultRequestHeaders.Clear();
                // Set the the media type to json, as that's what we're exepecting back
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Loop through the list of best story IDs
                foreach (var id in bestStoryIds)
                {
                    // Wait for the HttpClient to read the details for the story with this ID, which is indicated before
                    // .json in the URL.
                    HttpResponseMessage res = await client.GetAsync(String.Format("v0/item/{0}.json?print=pretty", id));
                    // If the HTTP response indicates that the request was successful
                    if (res.IsSuccessStatusCode)
                    {
                        // Store the json result in a variable
                        var json = res.Content.ReadAsStringAsync().Result;
                        // Deseriable the json to a Story object
                        var story = JsonConvert.DeserializeObject<Story>(json);
                        // Add the Story to the list which we instantiated at the top of the method body.
                        bestStories.Add(story);
                    }
                }
            }
            // Return the final list of Story objects
            return bestStories;
        }
    }
}
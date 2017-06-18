using nyt.core.users.client;
using System;
using System.Linq;

namespace nyt.core.tasks.client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TaskServiceAPI(new Uri("http://taskservice.dev"));
            Console.WriteLine($"API Result: {String.Join(",", client.ApiTasksGet().Select(o => o.Title))}");
        }
    }
}
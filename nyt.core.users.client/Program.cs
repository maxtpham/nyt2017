using System;
using System.Collections.Generic;
using System.Text;

namespace nyt.core.users.client
{
    static class Program
    {
        [STAThread]
        public static void Main()
        {
            var userServiceAPI = new UserServiceAPI(new Uri("http://localhost:8001"));
            var x = userServiceAPI.ApiValuesGet();
        }
    }
}

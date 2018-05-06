using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.Redis;

namespace DNWS
{
  class StatPlugin : IPlugin
  {
    protected static RedisManagerPool redisManager = null;
    public StatPlugin()
    {
        redisManager =  new RedisManagerPool("redis:6379");
    }

    public void PreProcessing(HTTPRequest request)
    {
       using (var client = redisManager.GetClient()) 
       {
           client.IncrementValue(request.Url);
       }
    }
    public virtual HTTPResponse GetResponse(HTTPRequest request)
    {
      HTTPResponse response = null;
      StringBuilder sb = new StringBuilder();
      sb.Append("<html><body><h1>Stat:</h1>");
      using (var client = redisManager.GetClient()) {
        List<String> entryKeys = client.GetAllKeys();
        foreach (String entryKey in entryKeys)
        {
          sb.Append(entryKey + ": " + client.GetValue(entryKey) + "<br />");
        }
      }
      sb.Append("</body></html>");
      response = new HTTPResponse(200);
      response.Body = Encoding.UTF8.GetBytes(sb.ToString());
      return response;
    }

    public HTTPResponse PostProcessing(HTTPResponse response)
    {
      throw new NotImplementedException();
    }
  }
}
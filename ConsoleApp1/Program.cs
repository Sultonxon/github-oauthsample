// See https://aka.ms/new-console-template for more information

using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

string json = "{" +
              "\"error\": true," +
              "\"errordescription\": \"The code passed is incorrect or expired.\"," +
              "\"erroruri\": \"https://docs.github.com/apps/managing-oauth-apps/troubleshooting-oauth-app-access-token-request-errors/#bad-verification-code\"" +
              "}";
Console.WriteLine(json);
var obj = JObject.Parse(json);


Console.WriteLine(obj.Property("error").Value.ToString().ToLower() == true.ToString().ToLower());
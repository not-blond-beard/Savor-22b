using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphQL.SystemTextJson;


public class Query : MonoBehaviour
{
    public static async Task Main(string[] args)
    {
        var schema = Schema.For(@"
        type Query {
            hello: String
        }
        ");
    
    var json = await schema.ExecuteAsync(_ =>
    {
    _.Query = "{ hello }";
    _.Root = new { Hello = "Hello World!"};
    });
    
    Console.WriteLine(json);
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

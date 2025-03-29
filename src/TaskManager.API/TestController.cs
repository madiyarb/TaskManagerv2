using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;
using TaskManager.API.Attributes;
using TaskManager.Application;
using TaskManager.Application.Enums;
using TaskManager.Application.Events;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.DomainModels;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Interfaces;

namespace TaskManager.API;

[ApiController]
[Route("Test")]
public class TestController : ControllerBase
{

    /// <summary>
    /// test
    /// </summary>
    /// <returns></returns>
    [HttpGet("test")]
    [HeaderAttribute("Idempotence-Key", typeof(string), true)]
    // [Idempotency(2)]
    public async Task<IActionResult> GetTest()
    {
        return Ok();
    }
    
    /// <summary>
    /// test
    /// </summary>
    /// <returns></returns>
    [HttpGet("test2")]
    [Idempotency(2)]
    public async Task<IActionResult> GetTest2(
        [FromServices] ILogger<TestController> logger)
    {
        Test test = new Test()
        {
            Test1 = "",
            Test2 = ""
        };
        Test2 test2 = new Test2()
        {
            Test3 = 0,
            Test4 = 0
        };
        logger.LogInformation("Test test {@Test}", test);
        logger.LogInformation("Return test {@Test2}", test2);
        return Ok(test2);
    }

    [HttpGet]
    [HeaderAttribute("salam", typeof(string), false)]
    public async Task<IActionResult> GetTest3(
        [FromServices] ITaskRepository repository)
    {
        await repository.Create(new TaskDomainModel("Name", "87786944750", Statuses.Created), CancellationToken.None);
        return Ok();
    }
    
    [HttpGet("dapper/{id}")]
    public async Task<IActionResult> GetTest4(
        [FromRoute] int id,
        [FromServices] ITaskQueryHandler repository,
        [FromServices] IDiagnosticContext context)
    {
        context.Set("TaskId", id);
        var task = await repository.GetByIdAsync(id);
        return Ok(task);
    }

    [HttpGet("exceptiontest2")]
    public async Task<IActionResult> GetAspectTest(
        [FromServices] ITaskRepository repository,
        [FromServices] ILogger<TestController> logger)
    {
         logger.LogInformation("Test test, start test method");
        await repository.Create(new TaskDomainModel("MadiyarBBBBB", "87786944750", Statuses.Created), CancellationToken.None);
        return Ok();
    }
    
    [HttpGet("hybridcache")]
    public async Task<IActionResult> GetHybridCacheTest(
        [FromServices] ICacheProcessor cache,
        [FromServices] ILogger<TestController> logger)
    {
        logger.LogInformation("Test test, start test method");
        Test test = new Test()
        {
            Test1 = "Madiyar",
            Test2 = "Madiyar"
        };
         await cache.SetAsync(CacheTypeEnums.Hybrid, "AAA:BBBB", test, TimeSpan.FromMinutes(5));
        var result1 = await cache.GetAsync<Test>(CacheTypeEnums.Redis,"AAA:BBBB");
        var result2 =await cache.GetOrSetAsync(CacheTypeEnums.Redis,"AAA:CCCC", test, TimeSpan.FromMinutes(5));
        var result3 = await cache.GetOrSetAsync(CacheTypeEnums.Hybrid,"CCCC", test, TimeSpan.FromMinutes(5));
        var result4 = await cache.GetStringAsync(CacheTypeEnums.Redis,"VVVV");
        return Ok();
    }
    
    [HttpGet("kafka/{name}/{key}")]
    public async Task<IActionResult> kafkaTest(
        [FromRoute] string name,
        [FromRoute] int key,
        [FromServices] IProduceService produceService,
        [FromServices] ILogger<TestController> logger)
    {
        TaskDomainModel task = new TaskDomainModel(name, "87786944750", Statuses.Created);
        var myEvent = new TaskCreated
        {
            Task = task
        };
        var message = JsonSerializer.Serialize(myEvent);
        await produceService.ProduceAsync("Tasks", key.ToString(), myEvent);
        return Ok();
    }
}

public class Test()
{
    public string Test1 { get; set; }
    public string Test2 { get; set; }
}

public class Test2()
{
    public int Test3 { get; set; }
    public int Test4 { get; set; }
}

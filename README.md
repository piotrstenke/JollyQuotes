<img align="left" src="img/logo-64.png" alt="JollyQuotes logo"/>

<div align="right">
	<a href="https://www.nuget.org/packages/JollyQuotes">
		<img src="https://img.shields.io/nuget/v/JollyQuotes?color=seagreen&style=flat-square" alt="Version"/>
	</a>
	<a href="https://www.nuget.org/packages/JollyQuotes">
		<img src="https://img.shields.io/nuget/dt/JollyQuotes?color=blue&style=flat-square" alt="Downloads"/>
	</a> <br />
	<a href="https://github.com/piotrstenke/JollyQuotes/actions">
		<img src="https://img.shields.io/github/workflow/status/piotrstenke/JollyQuotes/.NET?style=flat-square" alt="Build"/>
	</a>
	<a href="https://github.com//piotrstenke/JollyQuotes/blob/master/LICENSE.md">
		<img src="https://img.shields.io/github/license/piotrstenke/JollyQuotes?color=orange&style=flat-square" alt="License"/>
	</a>
</div>

# JollyQuotes

**JollyQuotes provides a simple, class-oriented interface for quick and painless access to multiple quote generation APIs scattered all across the Internet.**

## Table of Contents

1. [Supported APIs](#supported-apis)
2. [Structure](#structure)
3. [Custom APIs](#custom-apis)

### Supported APIs

Currently, **JollyQuotes** provides access to the following APIs:

 - [[nuget]](https://www.nuget.org/packages/JollyQuotes.KanyeRest) [kanye.rest](https://kanye.rest/)
 - [[nuget]](https://www.nuget.org/packages/JollyQuotes.TronaldDump) [Tronald Dump](https://www.tronalddump.io/)

The following APIs are not yet supported, but will be in the future:

 - [quotable](https://github.com/lukePeavey/quotable)

If you have any suggestions regarding additional API support, feel free to ask or create your own push request!

### Structure

Every **JollyQuotes** API package consists of 4 main classes:

- *[api-name]Service* - offers access to all actions defined by the target web API.
- *[api-name]QuoteGenerator* - generates quotes using actions available in the *[api-name]Service* class.
- *[api-name]Quote* - a model that contains data about the quote, such as its author, source, date.
- *[api-name]Resources* - provides links to important resources of the target web API, such as the api page, documentation, GitHub.

Some packages provide more functionality, such as converters or additional models.

### Custom APIs

To wire up a custom or unsupported API with **JollyQuotes**, you need to do the following:

 1. Download [this](https://www.nuget.org/packages/JollyQuotes) NuGet package.

 2. Create a quote model (preferably a record) named *[api-name]Quote* and implement the [*JollyQuotes.IQuote*](src/JollyQuotes/_intf/IQuote.cs) interface. 

	**Note**: There are two built-in implementations of this interface: [*JollyQuotes.Quote*](src/JollyQuotes/Quote.cs) and [*JollyQuotes.QuoteWithId*](src/JollyQuotes/QuoteWithId.cs), so you can feel free to either inherit from them or use them directly instead of creating a new model.

	Example:

	```csharp
	public record TestQuote : IQuote
	{
		public int Id { get; init; }
		public string Value { get; init; }
		public string Author { get; init; }
		public string Source { get; init; }
		public DateTime? Date { get; init; }
		public string[] Tags { get; init; }

		int IQuote.GetId()
		{
			return Id;
		}
	}
	```

 3. Create a static class named *[api-name]Resources* and containing important links and resources about the API you are implementing, such as api page or documentation.

	Example:
	
	```csharp
	public static class TestResources
	{
		public const string ApiPage = "http://api.test.com";
		public const string GitHub = "https://github.com//abc/Test"
	}
	```

 4. Create a class named *[api-name]Service* that defines all actions that can be performed using the API being implemented.

	**Note**: It is recommended to implement the [*JollyQuotes.IQuoteService*](src/JollyQuotes/_intf/IQuoteService.cs) interface or inherit the [*JollyQuotes.QuoteService*](src/JollyQuotes/QuoteService.cs) class for better clarity and testability.

	**Note**: It is recommended for all publicly-visible methods of the service to return [*System.Threading.Tasks.Task*](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-6.0) or [*System.Threading.Tasks.Task\<T\>*](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1?view=net-6.0) instead of direct types.

	**Note**: It is recommended to also create an interface to allow mocking.

	Example:
	
	```csharp

	public interface ITestService : IQuoteService
	{
		Task<TestQuote> GetRandomQuote();
		Task<TestQuote> GetRandomQuote(string tag);
	}
	
	public class TestService : ITestService
	{
		public IResourceResolver Resolver { get; }

		public TestService(IResourceResolver resolver)
		{
			Resolver = resolver;
		}

		public Task<TestQuote> GetRandomQuote()
		{
			return Resolver.ResolveAsync<TestQuote>($"{TestResources.ApiPage}/randomQuote");
		}

		public Task<TestQuote> GetRandomQuote(string tag)
		{
			return Resolver.ResolveAsync<TestQuote>($"{TestResources.ApiPage}/randomQuote/{tag}");
		}
	}
	```

 5. Create a class named *[api-name]QuoteGenerator* that generates quotes on demand, preferably by using a *[api-name]Service*. There are multiple ways to achieve that, but by far the easiest and the least painful is to inherit one of the following pre-built abstract classes:

	- [*JollyQuotes.RandomQuoteGenerator*](src/JollyQuotes/RandomQuoteGenerator.cs) - basic implementation of the [*JollyQuotes.IRandomQuoteGenerator*](src/JollyQuotes/_intf/IRandomQuoteGenerator.cs) interface; does not provide any additional features.

	- [*JollyQuotes.QuoteResolver*](src/JollyQuotes/QuoteResolver.cs) - provides a [*JollyQuotes.IResourceResolver*](src/JollyQuotes/_intf/IResourceResolver.cs) and implements the [*System.IDisposable*](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-6.0) interface.

	- [*JollyQuotes.QuoteClient*](src/JollyQuotes/QuoteClient.cs) - good choice for web-based APIs; provides a [*System.Net.Http.HttpClient*](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-6.0) and a [*JollyQuotes.HttpResolver*](src/JollyQuotes/HttpResolver.cs); implements the [*System.IDisposable*](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-6.0) interface.

	- [*JollyQuotes.EnumerableQuoteGenerator*](src/JollyQuotes/EnumerableQuoteGenerator.cs)  - basic implementation of the [*JollyQuotes.IRandomQuoteGenerator*](src/JollyQuotes/_intf/IRandomQuoteGenerator.cs) interface; implements the [*System.Collections.Generic.IEnumerable\<T\>*](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=net-6.0) interface.

	- [*JollyQuotes.EnumerableQuoteResolver*](src/JollyQuotes/EnumerableQuoteResolver.cs) - provides a [*JollyQuotes.IResourceResolver*](src/JollyQuotes/_intf/IResourceResolver.cs) and implements the [*System.Collections.Generic.IEnumerable\<T\>*](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=net-6.0) and [*System.IDisposable*](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-6.0) interfaces.

	- [*JollyQuotes.EnumerableQuoteClient*](src/JollyQuotes/EnumerableQuoteClient.cs) - good choice for web-based APIs; provides a [*System.Net.Http.HttpClient*](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-6.0) and a [*JollyQuotes.HttpResolver*](src/JollyQuotes/HttpResolver.cs); implements the [*System.Collections.Generic.IEnumerable\<T\>*](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=net-6.0) and [*System.IDisposable*](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-6.0) interfaces. 

	Example:

	```csharp
	public class TestQuoteGenerator : QuoteClient<TestQuote>
	{
		public ITestService Service { get; }

		public TestQuoteGenerator(ITestService service) : base(service.Resolver)
		{
			Service = service;
		}

		public override TestQuote GetRandomQuote()
		{
			return Service.GetRandomQuote().Result;
		}

		public override TestQuote GetRandomQuote(string tag)
		{
			return Service.GetRandomQuote(tag).Result;
		}
	}
	```

	**Note**: Generator classes listed above do not support caching of the generated quotes, meaning calls to their methods will in majority cases result in a call to one of [*System.Net.Http.HttpClient*](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-6.0)'s methods. To avoid that issue, each of said classes provides a second implementation of itself with support for caching, in form of an inner class named *WithCache*.

	Be aware that the *WithCache* version has slightly different method signatures, so make sure whether you want support caching **before** you actually write the generator.

	```csharp
	public class TestQuoteGenerator : QuoteClient<TestQuote>.WithCache
	{
		public ITestService Service { get; }

		public TestQuoteGenerator(ITestService service) : base(service.Resolver)
		{
			Service = service;
		}

		protected override TestQuote DownloadRandomQuote()
		{
			return Service.GetRandomQuote().Result;
		}

		protected override TestQuote DownloadRandomQuote(string tag)
		{
			return Service.GetRandomQuote(tag).Result;
		}
	}
	```
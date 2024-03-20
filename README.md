# ZooFoodManagement technical assignment

This is my proposed solution to the technical assignment. 
I've tried to make it as simple and segmented, organized as close to **Clean architecture**. Future improvements are of course possible. 

 - The Default project is the web.Api project `Zoo.FoodManagement`. It is based on net8 framework. Only basic middleware is registered, like swagger, logging, and application-specific services. The provided files are organized into folder structure `External` and then appropriately for different file types. That configuration is loaded into DI. 
 - The main work is done in the `Zoo.Services` project, a class library.  Abstractions on which the main project has dependencies are implemented there. Based on the requirements calculation takes place in the `ZooPriceService`.
 - The models are organized in `Zoo.Models` project. The actual models' implementation is based on the file's content, further hierarchy for redundancy and optimization is implemented. 
 - All the services have unit tests in the `Zoo.Services.Test` project. I tried to cover all scenarios. I used Moq  to mock dependencies and Fixture for most of the test data, except for the xml and csv. I partially tested the output of the csv reader (external lib.) and custom xml serialization.
> **Note:** 
I've used CsvHelper as an external library for parsing .csv files, Serilog as a logging library, Bogus as a test generation data service, and FluentAssertions for better test assertions.  This is a backend-only solution, so interaction with the application is through the Swagger documentation endpoint.

 - The remaining project is a `Zoo.Common`, a helper and configuration project. There are models which are used or are intended to be used in many parts of the solution.

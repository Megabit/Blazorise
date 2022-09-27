---
title: Best practices for building maintainable Blazor apps
description: Best practices for building and maintaining Blazor applications
permalink: /blog/blazor-best-practises-part-one
canonical: /blog/blazor-best-practises-part-one
image-url: /img/blog/2022-09-22/Best_practices_for_building_maintainable_Blazor_apps.png
image-text: image text
author-name: James Amattey
author-image: james
posted-on: September 28th, 2022
read-time: 6 min
---

In this blog, we will explore some best practices for building maintainable Blazor apps. We will discuss how to structure your app, how to manage state, and how to handle errors.
By following these best practices, you can build an app that is easier to maintain and more robust.

---

## App Structure
The app or project structure is the arrangement of files, folders and projects in your solution. Sometimes these arrangements can be done to depict the architecture of the application but they are not always the same thing.

A typical application consists of a client-side, a server side, a database and an API.

App structure refers to the projects and folders in your solution. Whilst application structure and architecture are different, the lines between are often blurred as applications are structured in a way that depicts the application architecture. 

There are a few types of application architectures that your app structure should depict

---

### Layered Architecture

In a layered architecture, different components of the application are arranged in layers. 

Most layered architectures consist of four standard layers: presentation, business, persistence, and database

[Layered Architecture!](/img/blog/2022-09-28/1.png)

As shown by the figure above, at the buttom is the database layer, which talks directly to the database by sending and retrieving information from the database. 

The persistence logic layer sits on top of the database layer and performs interactions with the database such as CRUD. As a result, it is sometimes called the data access layer or the repository layer because it uses the repository pattern to abstract the implementation of the business logic. 

The business layer is responsible for executing specific business rules associated with the request related to accomplishing functional requirements.

In some cases, the business layer and persistence layer are combined into a single business layer, particularly when the persistence logic is embedded within the business layer components.

The presentation layer is the user interface that displays information in the browser. This could be a [blazor wasm project](blog/what-is-blazor-wasm) where users can send and view information and web pages from the server. 

One of the powerful features of the layered architecture pattern is the separation of concerns among components. Components within a specific layer deal only with logic that pertains to that layer.

Each layer in marked as a closed layer. This means that a request originating from the presentation layer must first go through the business layer and then to the persistence layer before finally hitting the database layer. 

This type of component classification makes it easy to build effective roles and responsibility models into your architecture, and also makes it easy to develop, test, govern, and maintain applications using this architecture pattern due to well-defined component interfaces and limited component scope.

Although the layered architecture pattern does not specify the number and types of layers that must exist in the pattern, it is very common to see 3 or 4 layers in a typical application.

However, there are other variations of the layered architecture such as the N-tier architecture and the 4+1 Krueger Architecture which takes this a step further.


Layered architecture is perfect for smaller projects.However when the project gets bigger, maintenance can become a headache. Whilst each layer depicts a seperation of concerns, each layer is tightly coupled to the layers around it.

As a result, a change in the presentation layer will result in other layers. This can lead to spaghetti code as code is often nonreusable with a lot of class inheritance and encapsulation.

---

### Onion Architecture

Unlike the rectangular nature of the layered, the onion architecture is shaped in an onion mimic, with several layers underneath. The onion architecture is used to implement Domain Driven Design or Command Query Responsibility Seperation.

The premise of domain-driven design is that the structure and language of software code (and other artifacts) should match the structure and language of the business domain. CQRS is a pattern that separates reads from writes. 

Onion architecture is a variation of microservices implementation that takes advantage of domain-driven design principles to provide a clear separation of concerns between services.

This separation enables each service to be independently developed, deployed, and scaled without affecting other services in the system.

CQRS is a pattern that can be used in conjunction with microservices or Domain-Driven Design to provide a higher level of abstraction between the data model and the business logic.

By separating these two concerns, it becomes easier to scale each component independently and make changes to the system without impacting other parts of the system.

Other variations of Onion Architecture are the Hexagonal Architecture and Ports and Adapters.

Onion Architecture is based on the inversion of control principle. Onion Architecture is comprised of multiple concentric layers interfacing each other towards the core that represents the domain.

The architecture does not depend on the data layer as in classic multi-tier architectures, but on the actual domain models.

[Onion Architecture!](/img/blog/2022-09-28/2.png)


Onion architecture allows each layer to scale independently as they are not tightly coupled to each other. The repository nature of the architecture means that there is an encapsulation of the logic required to access data sources.

The layers are connected through interfaces and can be quickly tested because the application core does not depend on anything. 


This leads to  better maintainability and decoupling the infrastructure or technology used to access databases from the domain model layer.

### Microservice Architecture

A more complex implementation of CQRS and Domain-Driven Design can be used in microservices architecture to make sure that different services can scale independently.


## State Management In Blazor Applications

In Blazor, state can be managed either on the server or the client. Blazor uses a Razor templating system to generate UI components on the server, which are then downloaded and rendered on the client.

These components handle user events and can be nested to build complex user interfaces.

This means that each component can have its own state, which can be managed independently from the rest of the application.

One of the unique features of Blazor is that it uses an event-driven programming model, which means that UI updates are automatically propagated to all clients connected to the application.

Server-side state management is useful for scenarios where data needs to be persisted across page refreshes, or when data needs to be shared between users.

Client-side state management is useful for scenarios where data can be cached locally and does not need to be persisted across page refreshes.

While Blazor does not have built-in support for state management, there are a few different ways that you can manage state in your Blazor applications.

We'll explore how to manage state in a Blazor application with component state, service injection, and global state.


### Component State

Blazor uses a unique approach to managing state in the UI; instead of using traditional browser techniques like cookies, Blazor uses something called 'component state'.

Component state is a way of keeping track of changes to the UI without having to send information back and forth to the server.

This means that your web app can be more responsive and have a better user experience.

### Service Injection

Service injection is a powerful feature in blazor that allows developers to inject services into their components making services available throughout the component's lifecycle. Services are typically registered with the dependency injector (DI) container at startup, and then injected into components when they are created.

Services can be injected into components either via the @inject directive or by using the service provider. 

This gives developers the ability to reuse services across their application and also makes it easy to unit test components.


## Error Handling

Error handling is important. Error handling is the process of responding to and recovering from error conditions in your program. No one wants their website or application to crash and burn, so having a good error-handling strategy in place is key. 

One way is to simply let the Exception bubble up to the top and handle it there. Another way is to use try/catch blocks within your code.

Personally, I prefer the try/catch method because it allows me to be more specific with my error handling.

For example, I can catch a particular type of Exception and then handle it accordingly.

This is especially useful if you want to gracefully recover from an error instead of just displaying a generic error message to the user.

We will highlight both methods of Error Handling in Blazor so that you can decide which one is right for your own applications.

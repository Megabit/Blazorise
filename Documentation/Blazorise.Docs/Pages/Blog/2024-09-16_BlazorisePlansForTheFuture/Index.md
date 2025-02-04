---
title: Blazorise 2.0: Plans and Vision for the Future
description: In this post, we outline the roadmap for Blazorise 1.7 and 2.0, detailing the new features, API changes, and other developments that will shape the future of the project.
permalink: /blog/blazorise-2-plans-and-vision-for-the-future
canonical: /blog/blazorise-2-plans-and-vision-for-the-future
image-url: /img/blog/2024-09-16/blazorise-future.png
image-text: Blazorise 2.0: Plans and Vision for the Future
author-name: Mladen Macanović
author-image: mladen
posted-on: September 14th, 2024
read-time: 4 min
---

# Blazorise 2.0: Plans and Vision for the Future

As Blazorise continues to grow and evolve, our team is excited to share our plans for the upcoming releases, including the much-anticipated Blazorise 2.0. Blazorise has consistently aimed to simplify and enhance UI development in Blazor by offering an extensive range of components and features. With each iteration, we strive to bring significant improvements to both developer experience and application performance.

In this post, I’ll outline the roadmap for Blazorise 1.7 and 2.0, detailing the new features, API changes, and other developments that will shape the future of the project.

## Blazorise 1.7: A Bridge to the Future

The next step in our journey begins with the release of Blazorise 1.7, which we plan to roll out in October 2024. This update is not just a simple bug fix or a patch release. While it won’t be as groundbreaking as 2.0, it still brings several new components and introduces minor but important API changes that pave the way for future updates.

Here’s a sneak peek into what to expect in Blazorise 1.7:

### New Components

1. **PdfViewer**: One of the most requested features has been the ability to easily integrate and display PDF files within Blazor applications. The PdfViewer component will provide seamless support for viewing PDFs, directly within your Blazor applications without the need for third-party libraries. This will empower developers to easily handle document display tasks in applications like dashboards, reporting systems, and more.
2. **Skeleton Component**: Skeleton screens are widely used as a temporary placeholder while content loads, providing a better user experience during loading times. With the new Skeleton component, developers can create sleek, animated placeholders to indicate that content is being fetched. This enhancement will be valuable for creating modern, responsive applications that prioritize user experience.

### API Cleanups and Deprecation

One of the key focuses of Blazorise 1.7 is to begin the process of cleaning up the API. While the 1.7 update will not make drastic API changes, we will be marking some APIs as obsolete. This is a necessary step toward preparing for the major updates that will come with Blazorise 2.0.

While these changes might sound intimidating, rest assured that detailed migration guides will be provided to help you smoothly transition from the old APIs to the new ones. Our goal is to ensure the best possible developer experience while keeping the framework lean and efficient.

## Blazorise 2.0: A Major Leap Forward

After the 1.7 release, the team will immediately shift focus toward the development of Blazorise 2.0. This major release will mark a significant milestone in the evolution of Blazorise, bringing a host of improvements, feature enhancements, and optimizations aimed at improving the quality of life for developers.

Blazorise 2.0 will focus on two primary areas: cleaning up major APIs and aligning with modern .NET versions. Let’s take a closer look at these aspects.

### Major API Cleanups

As a UI framework, Blazorise's components and APIs play a crucial role in how developers build their applications. Over time, as new features are added and the framework evolves, some APIs become outdated or redundant. This can lead to confusion or unnecessary complexity when building apps.

Blazorise 2.0 will be an opportunity to reimagine and clean up our existing APIs to ensure they are consistent, intuitive, and easy to use. We aim to address various pain points, starting with issue [5596](https://github.com/Megabit/Blazorise/issues/5596), and will systematically go through the entire API surface to improve it.

All the APIs marked as obsolete in 1.7 will be removed in 2.0, giving us a clean slate to work with. This cleanup will help streamline the development process for both new users and long-time developers, offering a simpler and more consistent approach to building UIs with Blazorise.

### Dropping Support for Older .NET Versions

Another significant change with Blazorise 2.0 will be the decision to drop support for older .NET versions, specifically:

- **.NET 6**
- **.NET 7**

This decision wasn’t taken lightly, but it’s necessary for the continued growth and modernization of Blazorise. Supporting older .NET versions can often hinder the ability to take advantage of new features and performance improvements that come with more recent releases. By focusing on modern .NET versions, we can introduce better optimizations, reduce technical debt, and stay aligned with the latest advancements in the .NET ecosystem.

Going forward, Blazorise will support:

- **.NET 8**: As a Long-Term Support (LTS) version, .NET 8 will be the primary version that Blazorise 2.0 is built around. Its LTS status ensures stability and extended support, which makes it the logical choice for the majority of our users.
- **.NET 9**: Once .NET 9 is released, Blazorise will also provide full support for it. We are committed to staying at the forefront of .NET development and ensuring our framework is compatible with the latest versions of the platform.

### Why This Change Matters

The decision to drop support for older .NET versions may raise concerns for some developers, but we believe it’s a necessary step to keep Blazorise moving forward. By shedding the constraints of outdated frameworks, we can focus on delivering features and improvements that genuinely enhance the framework.

For the vast majority of our users, .NET 8’s LTS status ensures long-term stability and compatibility, which should make the transition smooth. We will also provide clear documentation and migration guides for those still using older versions, to ensure they can update their applications without hassle.

## Transparent Communication and Support

As always, we value open communication with our community. All changes and improvements will be clearly documented in our release notes, with migration steps thoroughly outlined to ensure that no developer is left behind. The Blazorise community has always been at the heart of what we do, and we remain committed to providing the resources you need to make the most of these upcoming changes.

## Conclusion

The release of Blazorise 1.7 in October 2024 will mark the beginning of a transformative phase for the framework, with the major changes coming in Blazorise 2.0. The introduction of new components, the cleanup of existing APIs, and the shift toward modern .NET versions all represent significant improvements to the developer experience.

We believe that Blazorise 2.0 will empower developers to build even more dynamic, responsive, and performant web applications. We’re excited for what the future holds, and we look forward to continuing this journey with our incredible community.

Stay tuned for more updates, and as always, thank you for your continued support!
## Open Development

All work on Blazorise happens directly on GitHub. All external contributors must send pull requests which will go through the review process.

## Setup Development Environment

To get the development and test environment set up on your local machine, you need to do the following:

1. Ensure recent build of Chrome browser installed: https://www.google.com/chrome/.
2. Install selenium-standalone: https://www.npmjs.com/package/selenium-standalone#install--run

You are now ready to build and test Blazorise:

## How to Write a Community Blog?

To write a blog post, you must create a new subfolder under the `Pages/Blog` in the `Blazorise.Docs` project. The subfolder must contain a blog date and name in the format `YYYY-MM-DD_BlogName`, e.g., `2022-06-08_BeginnersGuideToCreateBlazoriseApp`. Note that the blog name must be unique.

Next create the `Index.md` file under the same folder. The following metadata should always be present at the beginning of the file. Once pasted, adjust them for your blog.

```
---
title: How to create a Blazorise WASM application: A Beginner's Guide
description: Learn How to create a Blazorise WASM application: A Beginner's Guide.
permalink: /blog/how-to-create-a-blazorise-application-beginners-guide
canonical: /blog/how-to-create-a-blazorise-application-beginners-guide
image-url: img/blog/2022-06-08/How_to_create_a_Blazorise_application_A_Beginners_Guide.png
image-title: Blazorise WASM application: A Beginner's Guide
author-name: Mladen Macanović
author-image: mladen
posted-on: June 8th, 2022
read-time: 5 min
---
```

Most of the settings should be self-explanatory. The only one that might need some extra work is the `author-image`. The image should be placed in the `wwwroot/img/avatars` under the `Blazorise.Docs.Server` project and in the `*.png` file format. An image should be at least **512x512 px** in size and should be [optimized for minimum size](https://imagecompressor.com/).

After you have written or added a blog content, you can try running the documentation.

> :info: Before you start writing, please look at the **Branch Organization** to learn how we organize our work and publish the new versions.

## Running the Documentation

1. Start a command prompt and navigate to the `\Documentation\Blazorise.Docs.Server` folder under Blazorise root.
2. Run command: `dotnet watch run`.
4. Wait for build to finish and your browser will automatically be opened

## Testing

1. Start a command prompt and run: selenium-standalone start. Note: this service needs to be running before you start Visual Studio, or test runs may fail.
2. Open the Blazorise solution at the root folder (Blazorise.sln).
3. Select Build > Build Solution on main menu. All of the projects should build successfully.
4. Run all of the unit tests in the solution use Test > Run All Tests on main menu. They should all pass at this point.

## Branch Organization

We use `master` branch for all development and for all new features and bug fixes that are going into the ongoing milestone.

Once we finish the work for the current milestone, we will create a new **support** branch named `rel-X.Y`, where all the bug fixes will go. Also, we regularly publish our web from the latest `rel-X.Y` branch, so if you plan to write a blog, this is the recommended way and place to create it.

Branch naming must follow this guidelines:

- `dev-` prefix for new features based on `master` branch
- `rel-` prefix for fixed on existing release branch

## Pull requests

When submitting a pull request:

1. Fork the repository.
2. Create a branch from `master` or `rel-X.Y` and give it a meaningful name (e.g. `rel-{version-num}-my-awesome-new-feature`) and describe the feature or fix.  
4. Open a pull request on GitHub.

> :warning: **Don't make any changes on master branch**: You must always create a feature branch by following our guidelines or we will close the PR until the changes are properly organized.
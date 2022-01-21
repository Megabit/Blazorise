## Open Development

All work on Blazorise happens directly on GitHub. All external contributors must send pull requests which will go through the review process.

## Setup Development Environment

To get the development and test environment set up on your local machine, you need to do the following:

1. Ensure recent build of Chrome browser installed: https://www.google.com/chrome/.
2. Install selenium-standalone: https://www.npmjs.com/package/selenium-standalone#install--run

You are now ready to build and test Blazorise:

## Running documentation

1. Start a command prompt and navigate to the `\Documentation\Blazorise.Docs.Server` folder under Blazorise root.
2. Run command: `dotnet watch run`.
4. Wait for build to finish and your browser will automatically be opened

## Testing

1. Start a command prompt and run: selenium-standalone start. Note: this service needs to be running before you start Visual Studio, or test runs may fail.
2. Open the Blazorise solution at the root folder (Blazorise.sln).
3. Select Build > Build Solution on main menu. All of the projects should build sucessfully.
4. Run all of the unit tests in the solution use Test > Run All Tests on main menu. They should all pass at this point.

## Branch Organization

We use `master` branch for all development and for all new features and bug fixes that are going into the ongoing milestone.

Once we finish with the work for the current milestone we will create a new **support** branch named `rel-X.Y` where all the bug fixes will go.

Branch naming must follow this guidelines:

- `dev-` prefix for new features based on `master` branch
- `rel-` prefix for fixed on existing release branch

## Pull requests

When submitting a pull request:

1. Fork the repository.
2. Create a branch from `master` or `rel-X.Y` and give it a meaningful name (e.g. `rel-{version-num}-my-awesome-new-feature`) and describe the feature or fix.  
4. Open a pull request on GitHub.

> :warning: **Don't make any changes on master branch**: You must always create a feature branch by following our guidelines or we will close the PR until the changes are properly organized.

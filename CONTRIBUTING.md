## Open Development

All work on Blazorise happens directly on GitHub. All external contributors must send pull requests which will go through the review process.

## Setup Development Environment

To get the development and test environment set up on your local machine, you need to do the following:

1. Install Jekyll for documentation (https://jekyllrb.com/docs/installation/). If installing on Windows, follow these specific instructions: https://jekyllrb.com/docs/installation/windows/.
2. Install Node.JS (required for Selenium): https://nodejs.org/en/download/. Use the latest LTS, like node-v12.16.1.
3. Install Java SE (required for Selenium): https://www.java.com/en/download/. Use v8 or later.
4. Ensure recent build of Chrome browser installed: https://www.google.com/chrome/.
5. Install selenium-standalone: https://www.npmjs.com/package/selenium-standalone#install--run

You are now ready to build and test Blazorise:

### Running documentation

1. Start a command prompt and navigate to the docs folder under Blazorise root.
2. Run command: `bundle exec jekyll serve`.
4. Wait for build to finish and then open your browser and navigate to http://localhost:4000/

> Note: In case `bundle exec jekyll serve` fails it's probably because you don't have all the gems installed. Try running `bundle install` command.

### Testing

1. Start a command prompt and run: selenium-standalone start. Note: this service needs to be running before you start Visual Studio, or test runs may fail.
2. Open the Blazorise solution at the root folder (Blazorise.sln).
3. Select Build > Build Solution on main menu. All of the projects should build sucessfully.
4. Run all of the unit tests in the solution use Test > Run All Tests on main menu. They should all pass at this point.

## Branch Organization

### Development

We use `master` branch for all development and for all new features and bug fixes that are going into the ongoing milestone.

Once we finish with the work for the current milestone we will create a new **support** branch named `rel-X.Y` where all the bug fixes will go.

## Pull requests

When submitting a pull request:

1. Fork the repository.
2. Create a branch from `master` or `rel-X.Y` and give it a meaningful name (e.g. `rel-{version-num}-my-awesome-new-feature`) and describe the feature or fix.
3. Open a pull request on GitHub.

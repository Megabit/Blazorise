---
title: "Contributing"
permalink: /docs/contributing/
excerpt: "How to contribute."
---

We love pull requests from everyone. By participating in this project, you
agree to abide by the thoughtbot [code of conduct].

[code of conduct]: https://thoughtbot.com/open-source-code-of-conduct

## Branch Organization
We use separate branches for development and for all new features. After all the features are done in the seperate branch it will be merged into `master`.

Development branches follows the naming based on the milestone version number. For example if the milestone is `0.5.2` the main develop branch will be named `dev052` and all feature branches will be based on `dev052`. Feature branches follows the naming `dev052-feature-name`.

## Pull requests
When submitting a pull request:

1. Fork the repository.
2. Create a branch from `dev{version-num}` and give it a meaningful name (e.g. `dev{version-num}-my-awesome-new-feature`) and describe the feature or fix.
3. Open a pull request on GitHub into the appropriate `dev{version-num}` branch.

Note: We do not accept pull requests directly into `master`.
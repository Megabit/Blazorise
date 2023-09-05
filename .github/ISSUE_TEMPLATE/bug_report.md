name: "\U0001F41E Bug report advanced"
description: Create a report to help us improve
title: "[Bug]: "
labels: ["Type: Bug \U0001F41E", "Triage"]
body:
  - type: markdown
    attributes:
      value: |
        **Before submitting a bug report**

        The issue list is reserved for bug reports and feature requests. If you have a usage question, you can:

        - Read the [documentation](https://blazorise.com/docs)
        - Start a discussion on [Blazorise](https://github.com/Megabit/Blazorise/discussions)
        - Ask questions on [Discord](https://discord.io/blazorise)

        Also, try to search for your issue/feature - another user may have already requested something similar; Thanks!

  - type: input
    id: version
    attributes:
      label: Blazorise Version
    validations:
      required: true
  - type: dropdown
    id: provider
    attributes:
      label: What Blazorise provider are you running on?
      multiple: false
      options:
        - AntDesign
        - Bootstrap4
        - Bootstrap5
        - Bulma
        - Material
        - Tailwind
  - type: textarea
    id: reproduction-link
    attributes:
      label: Link to minimal reproduction, or a simple code snippet
      description: |
        The easiest way to provide a reproduction is to provide a link to a public GitHub repository or a tool like [Telerik Repl](https://blazorrepl.telerik.com/).

        The reproduction should be **minimal**. This means it should contain only the bare minimum amount of code needed
        to run the bug, including all the class definitions, models, services, etc.
      placeholder: Reproduction Link
    validations:
      required: true
  - type: textarea
    id: steps-to-reproduce
    attributes:
      label: Steps to reproduce
      description: |
        What must we do after opening your repro to make the bug happen? Clear and concise reproduction instructions are important for us to be able to triage your issue in a timely manner. Note that you can use [Markdown](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#quoting-code) to format lists and code.
      placeholder: Steps to reproduce
    validations:
      required: true
  - type: textarea
    id: expected
    attributes:
      label: What is expected?
    validations:
      required: true
  - type: textarea
    id: actually-happening
    attributes:
      label: What is actually happening?
    validations:
      required: true
  - type: dropdown
    id: browsers
    attributes:
      label: What browsers are you seeing the problem on?
      multiple: true
      options:
        - Chrome
        - Microsoft Edge
        - Safari
        - Firefox
        - Vivaldi
        - Brave
        - Other
  - type: textarea
    id: additional-comments
    attributes:
      label: Any additional comments?
      description: E.g., some background/context of how you ran into this bug.

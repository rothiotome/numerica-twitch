# Contributing to Numerica

First off, thanks for taking the time to contribute! ❤️

All types of contributions are encouraged and valued. See the [Table of Contents](#table-of-contents) for different ways to help and details about how this project handles them. Please make sure to read the relevant section before making your contribution. It will make it a lot easier for us maintainers and smooth out the experience for all involved. The community looks forward to your contributions. 🎉

> And if you like the project, but just don't have time to contribute, that's fine. There are other easy ways to support the project and show your appreciation, which we would also be very happy about:
> - Star the project
> - Tweet about it
> - Refer this project in your project's readme
> - Mention the project at local meetups and tell your friends/colleagues
> - Mention the game to your friends/colleagues or favoutire streamers

## Table of Contents

- [I Have a Question](#i-have-a-question)
- [I Want To Contribute](#i-want-to-contribute)
- [Reporting Bugs](#reporting-bugs)
- [Suggesting Enhancements](#suggesting-enhancements)
- [Your First Code Contribution](#your-first-code-contribution)
- [Improving The Documentation](#improving-the-documentation)
- [Styleguides](#styleguides)
- [Commit Messages](#commit-messages)
- [Join The Project Team](#join-the-project-team)

## I Have a Question

> If you want to ask a question, we assume that you have read the available [Documentation](https://github.com/rothiotome/numerica-twitch/Readme.md).

Before you ask a question, it is best to search for existing [Issues](https://github.com/rothiotome/numerica-twitch/issues) that might help you. In case you have found a suitable issue and still need clarification, you can write your question in this issue. It is also advisable to search the internet for answers first. Also you can ask in the [discord server] (https://discord.gg/29tREmCJk3)

If you then still feel the need to ask a question and need clarification, we recommend the following:

- Open an [Issue](https://github.com/rothiotome/numerica-twitch/issues/new).
- Provide as much context as you can about what you're running into.
- Provide project and platform versions, depending on what seems relevant.

We will then take care of the issue as soon as possible.

## I Want To Contribute

> ### Legal Notice 
> When contributing to this project, you must agree that you have authored 100% of the content, that you have the necessary rights to the content and that the content you contribute may be provided under the project license.

### Reporting Bugs

#### Before Submitting a Bug Report

A good bug report shouldn't leave others needing to chase you up for more information. Therefore, we ask you to investigate carefully, collect information and describe the issue in detail in your report. Please complete the following steps in advance to help us fix any potential bug as fast as possible.

- Make sure that you are using the latest version.
- Determine if your bug is really a bug and not an error on your side e.g. using incompatible environment components/versions (Make sure that you have read the [documentation](https://github.com/rothiotome/numerica-twitch/Readme.md). If you are looking for support, you might want to check [this section](#i-have-a-question)).
- To see if other users have experienced (and potentially already solved) the same issue you are having, check if there is not already a bug report existing for your bug or error in the [bug tracker](https://github.com/rothiotome/numerica-twitchissues?q=label%3Abug).
- Also make sure to search the internet (including Stack Overflow) to see if users outside of the GitHub community have discussed the issue.
- Collect information about the bug:
- Stack trace (Traceback) ( if possible )
- OS, Platform and Version (Windows, Linux)
- Version of the interpreter, compiler, SDK, runtime environment, package manager, unity version, depending on what seems relevant.
- Possibly your input and the output
- Can you reliably reproduce the issue? And can you also reproduce it with older versions?

#### How Do I Submit a Good Bug Report?

> You must never report security related issues, vulnerabilities or bugs including sensitive information to the issue tracker, or elsewhere in public. Instead sensitive bugs must be DM to Rothio or some team member via [discord] (https://discord.gg/29tREmCJk3)

We use GitHub issues to track bugs and errors. If you run into an issue with the project:

- Open an [Issue](https://github.com/rothiotome/numerica-twitch/issues/new). (Since we can't be sure at this point whether it is a bug or not, we ask you not to talk about a bug yet and not to label the issue.)
- Fill all the information needed in the template which is:
    - Explain the behavior you would expect and the actual behavior.
    - Please provide as much context as possible and describe the *reproduction steps* that someone else can follow to recreate the issue on their own. This usually includes your code. For good bug reports you should isolate the problem and create a reduced test case.
    - Provide the information you collected in the previous section.

Once it's filed:

- The project team will label the issue accordingly. The first tag will be `needs-triage` which means will be checked
- A team member will try to reproduce the issue with your provided steps. If there are no reproduction steps or no obvious way to reproduce the issue, the team will ask you for those steps and mark the issue as `needs-repro`. Bugs with the `needs-repro` tag will not be addressed until they are reproduced.
- If the team is able to reproduce the issue, it will be marked `needs-fix`, as well as possibly other tags (such as `critical`), and the issue will be left to be [implemented by someone](#your-first-code-contribution).

### Suggesting Enhancements

This section guides you through submitting an enhancement suggestion for Numerica, **including completely new features and minor improvements to existing functionality**. Following these guidelines will help maintainers and the community to understand your suggestion and find related suggestions.

#### Before Submitting an Enhancement

- Make sure that you are using the latest version.
- Read the [documentation](https://github.com/rothiotome/numerica-twitch/Readme.md) carefully and find out if the functionality is already covered, maybe by an individual configuration.
- Perform a [search](https://github.com/rothiotome/numerica-twitch/issues) to see if the enhancement has already been suggested. If it has, add a comment to the existing issue instead of opening a new one.
- Find out whether your idea fits with the scope and aims of the project. It's up to you to make a strong case to convince the project's developers of the merits of this feature ( mainly Rothio ). Keep in mind that we want features that will be useful to the majority of our users and not just a small subset.

#### How Do I Submit a Good Enhancement Suggestion?

Enhancement suggestions are tracked as [GitHub issues](https://github.com/rothiotome/numerica-twitch/issues).

- Use a **clear and descriptive title** for the issue to identify the suggestion.
- Provide a **step-by-step description of the suggested enhancement** in as many details as possible.
- **Describe the current behavior** and **explain which behavior you expected to see instead** and why. At this point you can also tell which alternatives do not work for you.
- You may want to **include screenshots and animated GIFs** which help you demonstrate the steps or point out the part which the suggestion is related to. You can use [this tool](https://www.cockos.com/licecap/) to record GIFs on macOS and Windows, and [this tool](https://github.com/colinkeenan/silentcast) or [this tool](https://github.com/GNOME/byzanz) on Linux. <!-- this should only be included if the project has a GUI -->
- **Explain why this enhancement would be useful** to most Numerica users. You may also want to point out the other projects that solved it better and which could serve as inspiration.
- Try to fill all the fields in the template 

### Your First Code Contribution
So the steps to add a contribution in this repo are:

1. Fork the repository and clone it locallys
2. Create a branch for your edits.
3. If needed, test your changes! Run your changes against any existing tests if they exist and create new ones when needed.
4. Create a PR ( pull request from you branch )
5. Fill the template with all the information including screenshots and all the stuff needed ( will take a few minutes )
6. Contribute in the style of the project to the best of your abilities.

Since is a open source project and probably is your first contribution you can check this [page](https://opensource.guide/how-to-contribute/)with all the information you need.

### Improving The Documentation
Try as much as possible to improve the documentation of your code and code in general. Remember that you can NOT assume that the reviewer or colleague understands what the original problem was or why a certain solution was chosen and not another. It is also important not to assume that the code is self-evident/self-documented.

## Styleguides
### Commit Messages
A properly formed git commit subject line should always be able to complete the following sentence

If applied, this commit will <your subject line here>

So, the rules for a great git commit message style:

- Try to be concise in your messages
- Try to explain what each commit does
- Use the imperative mode in the subject line
- If necessary you can refer to an issue using the [#123] <your message here> notation.

## Join The Project Team
The Numerica team will be chosen by RothioTome on merit and based on its criteria.

## Attribution
This guide is based on the **contributing-gen**. [Make your own](https://github.com/bttger/contributing-gen)!
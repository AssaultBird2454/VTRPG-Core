# Virtual TRPG (Core)
Software for playing TRPG(Tabletop Role Playing Games) over the internet, with some automation and tasks simplified.

## About the Project
This project is aimed to help improve and automate GMing and Running TRPG sessions by making dice rolls and calculating damage just by calling an action, with full support for custom actions or allowing the GM to manually deal damage or roll dice.

This project it's self does not support any TRPG systems however there will be some systems that are supported by official plugins, with the ability for users to create custom plugins with their systems or pre-existing systems with their modifications if they are good with programming

### Support this project
This project is free and open source, however development costs and would be greatly appreciated if you could donate to help sustain the ongoing development. The funds raised for this will be used to pay for Licences for development tools, IDE's, Test Servers on GCP and Telemetry Servers like Sentry.

Your help would be greatly appreciated.

### Join this project
This project is not a one man task, and if you would love to help with the development of this project then fork the repository, make your changes in a branch named ``Feature/[Name your feature]`` the branch should have been created from the ``Dev`` branch as that is where all the developed code goes before being released

## Project Work Flow
Features/Fixes/Improvements are made in a branch on a fork of the base repo, the branch on the fork should be created from the ``Dev`` branch and named with the branch naming convention below.

|Type       |Name Convention      |
|-----------|---------------------|
|Feature    |Feature/[Short Name] |
|Fix        |Fix/#[Bug ID]        |
|Improvement|Improve/[Short Name] |

To update your ``Dev`` branch with the latest code in the base repos ``Dev`` branch, create a PR into your fork's ``Dev`` branch and Merge it using a ``Rebase Merge``

When you’re ready to submit your Fix/Improvement/Feature create a PR into the base repo's ``Dev`` branch. Name the PR by the naming convention below

|Type       |Name Convention        |
|-----------|-----------------------|
|Feature    |Feature - [Name]       |
|Fix        |Bug Fix - #[Bug ID]    |
|Improvement|Improve - [Name]       |

**Note:** When submitting anything, you need to update its relevant documentation to reflect the changes made... And don’t forget to add your name to the ``contributors.js`` if you haven’t already so as to get credit on the website for your contributions

### Error & Crash Collection
This software can and will catch errors and crashes through Sentry. We don’t collect any personally identifiable information (POI) nor do we use it if any gets missed by the automatic scrubbing. We will not supply any of the information that we collect through telemetry to any third party. We do however supply minimal access to third party plugin developers who make plugins for this software, and it is limited to only exceptions that occur within their plugin's name space.

And our plugin API will allow support for 3rd party plugin developers to use our sentry telemetry implementation for free. This service is currently in planning and more details about how 3rd partys may use the Sentry Telemetry will come at a later date.

## Project State
This project is in active development and planning, none of the base features are currently in a working state.

Here is a list of all the projects inside the solution and their state

|Project Name|Namespace       |Description                                                        |State    |
|------------|----------------|-------------------------------------------------------------------|---------|
|Core        |VTRPG.Core      |The core code for the VTRPG project                                |Planning |
|Save Editor |VTRPG.SaveEditor|The save editor application                                        |Planning |
|Server      |VTRPG.Server    |The dedicated server software                                      |Planning |
|Web Client  |VTRPG.WebClient |The web client that all players use to connect to the virtual table|Planning |

## 1st Party System Plugins
**Note:** The developers of this software did not develop the TRPG systems

|Name                      |Link                   |State         |Latest Version        |
|--------------------------|-----------------------|--------------|----------------------|
|Pokemon Tabletop United   |https://virtual-ptu.com|Planning      |-                     |
|Pokemon Mystery Dungeon V3|-                      |Planning      |-                     |

## Popular 3rd Party System Plugins
**Note:** The developers of this software did not develop the TRPG systems

|Name                      |Link                   |State         |Latest Version        |
|--------------------------|-----------------------|--------------|----------------------|
|-                         |-                      |-             |-                     |

## Useful Links
https://trello.com/b/UN2UHTKY -> Trello board for the projects roadmap<br>
(Coming Soon) -> Discord server to join and chat to the developers or other people (Note, The server is under the old name)<br>
(Coming Soon) -> Sentry Telemetry Panel for developers to get crash information to assist in fixing bugs<br>
http://www.virtual-trpg.com (Not Available) -> VTRPG project website for information, viewing docs, guides and downloading resources...<br>
http://Listings.virtual-trpg.com (Not Available) -> VTRPG campaign listings site, GM's post your campaigns here if you’re looking for players. This service will allow for cloud saving your saves to keep a backup as well as a way to automate the player application process (Optional)<br>
http://Forums.virtual-trpg.com (Not Available) -> VTRPG forums, Discuss and talk to developers outside of discord here, post your resources, tips and guides here<br>

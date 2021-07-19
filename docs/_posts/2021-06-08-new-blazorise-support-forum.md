---
title: "New Blazorise Support forum"
permalink: /news/new-blazorise-support-forum/
classes: wide
categories:
  - Announcement
tags:
  - blazorise
  - announcement
  - support
  - forum
---

{% include figure image_path="/assets/images/news/support.png" alt="Support" %}

Today we're happy to announce the release of our very new Blazorise Support forum 🎉! It marks a new milestone for our commercial Blazorise users, and I really hope the new web will be self-explanatory. It has that familiar _GitHub feel_, so I think everyone will very quickly get used to it.

The new support web can be found by visiting [https://support.blazorise.com](https://support.blazorise.com/) . 

## How it's done?

And now a little more on the technical details. The entire web was finished in just two weeks. It is done fully with the latest Blazorise `v0.9.4` and is `99.99%` CSS free. That last `0.01%` is just for setting the page font, and some other small bits so I'm OK with it 🙃. For the backend, we chose SQL Server Express with Entity Framework Core 5, and for the email provider, we're using SendGrid.

To make it work without (almost) any CSS we had to implement many new Blazorise utilities like _flex_, _border_, _shadow_, _overflow_, etc. All of them will be part of the new Blazorise `v0.9.4` once it is released.

---

Other than the native Blazorise components, we also had to create some new custom components and tools for the new web. One of the new components worth mentioning is the markdown editor, which is running on [EasyMDE](https://github.com/Ionaru/easy-markdown-editor). The second tool is actually a service for showing messages and confirmation modal dialogs.

All of these new components and services will soon be part of the Blazorise open-source ecosystem.

## Where is the source?

The entire source code of the new web is available for free as part of the Blazorise [commercial plan](https://commercial.blazorise.com/). Everyone who purchases a license(Professional or Enterprise) will get the read access to our private repository and some other Blazorise themes.

## Closing details

For the end, I just want to say big thanks to all our Blazorise users. Without you, none of this would have been possible. I really hope we'll continue the new journey together.

Thank you everyone! Until next time, stay 💪.
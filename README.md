![VeraWAF project logo](http://www.verawaf.com/Images/Logo_185x77.png "VeraWAF project logo")

# Vera Web App Framework (VeraWAF)

Homepage: [VeraWAF project homepage](http://www.verawaf.com/) 

## Kickstarter open source framework for Microsoft(R) Azure(TM) Web Applications.
Developer-centric open source framework with Web content management, search indexing, user managent, and much more. Have your Azure web app ready in minutes and save months or even years of work. Smart caching saves you up to 90% of your storage transaction costs.

>"Vera is great for programmers who wants to quickly start building Azure Web Applications without having to develop fundamental functionality like user and content management from scratch. Vera saves you months or more of research & development."* - Developer

### Reduce development time
* Ready to be used fundamental functionality like access control, social sign-in, indexed search, Web Content Management, forums, newsletters, RSS feeds, and much more.
* Utilize existing templates or create your own.
* Shorten development time with Vera fundamentals.

### Web Content Managment
* Access control for all Web Content Management roles and users.
* Simple user interfaces to manage pages, users, roles, and the Web Application.
* Adaptable with dynamic menu items, styling, page templates, and user control templates.

### Quick & easy setup
* No SQL database needed.
* Well documented the setup & development process.
* Get quickly up and running with little configuration, and little or no coding.

### Social features and forums
* Easy to create and manage forums.
* Users get social points by interacting with eachother in the forums.
* Forum voting system, internal messaging, user profile pages, and post favoriting leverages user interaction.

### Extendable
* Open source code that is simple to extend with your own requirements and needs.
* Develop in C# with ASP.NET web forms and/or ASP.NET MVC 5.
* Extend Microsoft Azure worker role with your own background tasks.

### Secure
* Anti request-hammering, comment-hammering, and sign-in hammering protection.
* Easy to setup Web Application encryption with HTTPS certificates.
* E-mail address validation though user interaction during the user registration workflow.
* Single sign-in with accounts from all the major social network sites like Facebook, Twitter, LinkedIn etc, in addition to ASP.NET forms authentication.

## VeraWAF Feature List Overview

### Microsoft(R) Azure(TM) integration
* Fully integrated with Microsoft(R) Azure(TM).
* Uses the Azure Table Storage as a database.
* Uses the Azure Blob Storage for files.
* Simple to integrate with the Microsoft Azure Content Delivery Network (CDN).
* Send clouds commands between the cloud nodes using a custom message bus.
* Page to monitor all the cloud node settings and get recommendation for optimization.
* Centralized logging pages.
* Real-time performance monitoring.

### Web content management
* Template based solutions with lots of pretty templates ready to be used.
* Role-based security.
* Simple to create new content pages or edit existing ones.
* Virtual paths enables you to easily create the site structure you want.
* Page to view all content pages.
* Page to create or edit a content page.
* Page to view all user comments.
* Page to send newsletter to all subscribers.
* Page to upload files to the Azure Blob.
* Page to view all your Azure Blob Files.
* Page to edit a Azure Blob File.
* Search page with hit highlighting.
* All cloud nodes immediately reflect any changes to content across the cloud.
* All templates uses HTML5 with CSS v3 and RDFa v1.1
* Automatic RSS 2.0 syndication for all content pages
* Newsletter functionality.
* Rich-text HTML editor makes adding HTML to page content fields simple.
* Simple to change UI styling for an entire site
* All pre-made pages like search page, admin pages, and editor pages can be overridden with your own templates and/or logic.
* Site map page.
* Manage multiple web site within a single Microsoft Azure Table Storage.

### Performance
* Pages are cached locally on each node and not retrieved from the database; and can therefore be loaded extremely fast.
* Automatic CSS style sheet compression/minimizing & merging.
* Automatic JavaScript compression/minimizing & merging.
* Automatic HTML page compression.
* Images are loaded dynamically; e.g. images are only loaded when they are visible inside the browser viewport.

### Search engine
* All content pages are indexed automatically.
* Search page.
* Search control with real-time lookup.
* Real-time search with built-in RESTful API.
* Rich query language through the Query Parser; write complex rules or let the users make complex queries.
* Hit highlighting.

### Storage
* Uses the Azure Table Storage, so no SQL server or database setup is required.
* Uses the Azure Blb Storage for images.
* Temporary files are cached locally on each node for better performance.

### ASP.NET providers
* Membership provider persists data in the Azure Table Storage.
  * Social sign-in with your Twitter, Facebook, Google, accounts etc., or with local users.
  * Register account page with e-mail confirmation and Question/Answer.
  * Sign In page with automatic locking after too many failed attempts.
  * Sign Out page.
  * Change password page with e-mail confirmation.
  * Reset password page where new password is sent to e-mail account.
  * Locked account functionality where user can unlock account using link sent to users e-mail account.
  * User e-mail validation by using link sent after account validation, account is not activated before e-mail is validated.
  * Admin page for user management.
  * Admin page that shows some site statistics about the users.
* Role provider persists data in the Azure Table Storage.
  * Admin page for role management.
* User Profile provider persists data in the Azure Table Storage.
  * User profile page where user can update his/her settings and upload a profile image that is stored as a Azure Blob.
* Session-State provider persists data in the Azure Table Storage.
  * Maintain session-state between server instances in the Azure cloud.
* Sitemap provider.
  * Automatic sitemap.xml file generation.

### Background tasks
* Framework for building multi-threaded worker role; each worker runs in its own thread.
  * Easy to extend with your own background tasks.
* Worker Role that cleans up expired session-state data from the Azure Table Storage.
* Worker roles are monitored and automatically restarted if they go down.
* Worker roles implement a back-off pattern to potentially save processing time costs.
* Queue based E-mail worker role.
  * All e-mail sent from the application is handled by a worker role that picks e-mails from the Azure Queue.
  * Sends e-mail asynchronously.
  * Logic prevents flooding the e-mail server when bulk e-mail lists are processed (like when sending newsletters).
  * Simple API makes it easy to send e-mails from your code.
  * Will wait for a set time and retry for a set amount of times if sending an e-mail fails.
  * Simple to setup with your own SMTP e-mail server.
  
### REST APIs
* Ready to be used REST APIs.
* Optional OAuth authentication scheme with consumer keys & secrets.
* Easy to extend with your own REST API functionality.
* Supports XML, JSON, and JSONP interchange formats.
* Optimized settings made by experts for the greatest possible throughput.

### Diagnostics & logging to the Azure Table and Blob Storage
* Centralized application and system logging with Web UI.
* View real-time performance for all cloud nodes with Web UI.
  * Processor percentage.
  * Available internal memory.
  * Offline node notifications.
* Windows events with Web UI.
* Directories.
* Infrastructure logs.
* Performance counters.
* Trace logs.

### Forums
* Easy to create forums with pre-made templates.
* Social features like user voting and post favouring.
* Administrate all forum comments from one central location UI.
* Advanced BBCode formatting.

### Social features
* Users can vote up & down in forums, these actions affect the user's *social score*.
* Favouring forum posts affect the user's *social score*.
* User public profile pages
  * See user social score
  * See user comments.
  * See user posts and favourites.
  * Inter-messaging between users without revealing any e-mail addresses.

### Developer-centric
* Simple to learn
  * Work with familiar technologies like .NET, C#, ASP.NET, HTML5 with CSS v3, JavaScript(JQuery), and RDFa v1.1.
  * Well documented.
  * Video tutorials directly from the mouth of the developers themselves.
  * Fully featured demo site "Silicon Burgers" is automatically created from the downloaded open source code.
* Simple to style and make your own templates and controls.
* Few dependencies; Microsoft Visual Studio 2013 and the Microsoft Azure SDK.
* Open source license.
* Extremely fast
  * Pre-optimized and pre-tuned solution by experts using well-known best practices.
  * Automatic compression of style sheets, JavaScript, and the served HTML page to minimize size.
* Lots of pre-made functionality like controls and templates
* Simple to extend with your own page template, controls, background tasks, database functionality, entities etc.
* Works with both ASP.NET web forms and ASP.NET MVC 5.
* All code is written in the familiar C# v4.0 programming language for the Microsoft .NET v4.5.1 platform.
* Simple to make work with TLS/SSL to enable HTTPS web sites.
* Highly and easily configurable; simple to enable or disable features.
* Get code using GIT or download the whole solution in a ZIP package.
* Tons of pre-made logic ready to use.

### Smart caching
* Cloud nodes make very few database lookups and caches most content in-memory.
* Cloud nodes sends messages directly to each other using a custom message bus to update caches and other notifications.
* Smart caching saves you up to 90% of your storage transaction costs.

### Access control
* ASP.NET memberships for users and roles.
* Manage users and roles using Web UI.

### Simple licensing
* Simple Apache v2.0 license with only one extra paragraph
* Unlimited and free open source
* Free to use in all your environments; pay only to remove title "VeraWAF" watermark.
* No registration. No credit card required. No hidden fees. Cancel anytime.

### Security
* Access control using ASP.NET user memberships and roles
* Anti sign-in hammering that automatically bans IP addresses for a limited preset amount of time.
* Anti request hammering that automatically bans IP addresses for a limited preset amount of time.
* Anti post hammering that automatically bans IP addresses for a limited preset amount of time.
* Anti e-mail hammering that automatically bans IP addresses for a limited preset amount of time.
* Centralized logging makes it easy to see security issues across all the cloud nodes in one UI.
* Simple to ban users from the UI.

...and much more.

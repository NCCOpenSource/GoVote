# Go Vote
Voting Web app built in C# to run in Azure, using Azure B2C to authenticate users.  Built by Newcastle City Council to replace an aging physical voting system used inside the council chambers.

You can see an example of GoVote being used in a virtual council meeting for Newcastle city council on the 24th of June 2020 [here](https://youtu.be/-UxsgA4PDWU?t=1000 "here")

A demo video is also available [here](https://github.com/NCCOpenSource/GoVote/blob/master/DemoVideo.md "here")

GoVote allows you to authenticate a user to allow them to vote on polls pushed to their device (works on web for any device). Admins can trigger Polls on demand from and admin screen for a given subject text. Vote results can be displayed on a results screen mapped to a room layout in real-time. Votes can be analysed after the session in Power BI.

Users need to be authenticated and checked in to be able to vote.

Product currently in Beta and basic format.

Contributions welcome.

Project licensed under the terms of the MIT license.

Please share and use if you find it helpful. This is our first open source project so be kind world. 😊 .

# Getting Started
The GoVote is a simple application to allow users (members) to vote during a session (originally used during a council meetings).

It is written in C# using the .netcore framework and is secured using MS Azure B2C.

The system can be broadly summarised as having 4 main core components.

**Voting area** -  where the members log into and cast their votes

**Admin panel** - where an user can start/stop votes and administer where members sit in the chamber

**Registration screen** - where members will clock in and out of the meetings. members need to be clocked in to allow them to vote.

**Display ** - the display used to show the live results on the big screen

SignalR is used in the voting app to activate/close votes on a Members device and to display the live results of the poll on the main screen.

There is also a Power BI report template which allows post analysis of the data.

# Prerequisites
The Code was built to be deployed in to azure (Please see high level Architecture).  You would need the following resources in order to deploy the code and use the application

-	Somewhere to host the application. We used an Azure web app and tend to keep this scaled down and only scale up during a meeting to ensure stability and responsiveness.
-	Identity platform to authenticate your users. We used an Azure B2C tenant.
-	Backend Database and / or storage location store votes and activity logs.

# Installing
A step by step series of examples that tell you how to get a development env running

1.	Create database and tables 
2.	Populate your  app setting with the entries in the appSettings.json file  in solution.
3.	Create IDP (using B2C users who are admins will require job roles as 'MemberAdmin', to use the application the role of ‘member’ is required.

# High level Architecture

[![Go Vote High Level Architecture](https://raw.githubusercontent.com/NCCOpenSource/GoVote/master/READMEImages/HighLevelArch-GoVote.png "Go Vote High Level Architecture")](https://raw.githubusercontent.com/NCCOpenSource/GoVote/master/READMEImages/HighLevelArch-GoVote.png "Go Vote High Level Architecture")

# Built With
- 	Net.core using C#
- 	NuGet Packages required:
	- Microsoft.AspNet.SignalR
	- Microsoft..ASPNetyCore.signalRClient
	- Microsoft..AspNetCore.SignalRClientCore
	- Microsoft,.Graph
	- Microsoft.Identity.Client
	- Microsoft.AspNetCore.Authentication.AzureB2C.UI
- 	Power BI Report template used for post vote analysis.


# SignalR- what is it all that about then?

Microsoft say:

`Today's modern apps are expected to deliver up-to-date information without hitting a refresh button. Add real-time functionality to your dashboards, maps, games and more.`

`What is real-time functionality? It's the ability to have your server-side code push content to connected clients as it happens, in real-time.`

The technology is [documented](http://https://docs.microsoft.com/en-gb/aspnet/core/signalr/introduction?view=aspnetcore-2.2 "documented") comprehensibly (ish) on the internet.

At a high level, in GoVote, it works like this:

To start a vote-

Admin clicks start vote from the admin panel. This button has a JS method action called `beginPoll()`
beginPoll fires 2 SignalR events to the signalR hub.

1.	OpenPoll- which enables the poll on the users device
2.	ClearDisplay- which refreshes the big screen display

These events are fired and are received by the SignalR hub (/Hubs/VoteHub.cs).

The hub then interpolates these calls and fires an asynchronous event to all the clients connected to that hub.

OpenPoll fires `await Clients.All.SendAsync("OpenPoll", pollid);`

In the JavaScript on the members device there is a method waiting for this call.

```javascript
connection.on("OpenPoll", function (pollid) {
    $("#seatNo").text("Seat: " + clientID);
    $("#voteNumber").text(pollid);
    $("#currentVote").hide();
    $("#voteButtons").show();
});
```
The user is then shown the poll number and the vote buttons are displayed.

There is also a similar process called ClosePoll that hides the buttons again
```javascript
connection.on("ClosePoll", function (pollid) {
    $("#voteNumber").text("");
    $("#voteButtons").hide();
});
```

ClearDisplay works in a similar vein with the difference being that it simply refreshes the display screen using `document.refresh()`.

This works slightly differently when a member casts a vote. When they vote the page is redirected to the Vote method in the Home controller. This method works by logging the vote into the database and then displaying the vote on the display using SignalR. This SignalR implementation is contained in the Send_Vote method of the home controller using the C# library rather than the JS version.

# Managing user access for voting app in B2C
User access is controlled through the Job Title field on the B2C user.

A member needs the job title 'Member' and an admin needs the job title of 'MemberAdmin'.

To grant access:
1.	log in the azure portal (https://portal.azure.com ) as a B2C admin 
2.	Search for 'Azure AD B2C', click on Users and search for the member required.
3.	Set appropriate role in the Job info section. i.e.
Admin Page 
The Admin page is accessed form https://YOURAPPLICATIONNAME/Admin 

**In order to access this page a B2c user needs to have a jobTitle of 'MemberAdmin'.

**When a session is active you will have the below options / screen areas**

[![Admin Console Image](https://raw.githubusercontent.com/NCCOpenSource/GoVote/master/READMEImages/AdminScreen.png "Go Vote Admin Screen")](https://raw.githubusercontent.com/NCCOpenSource/GoVote/master/READMEImages/AdminScreen.png "Go Vote Admin Screen")
 
1.	Close the session. Only to be used after the meeting has closed. This will sign-out all the members so if pressed inadvertently would require them to sign-in again before they can vote.
2.	Start/Close a vote. When pressed this will send a vote to the members' devices and trigger the main display to refresh. Closing an open vote will leave the results on the main display. If for some reason this doesn't remove the vote from the user's device and they try to vote then their vote will not be counted.
3.	Blank display. This can be used to hide the previous results from the large display.
4.	Clear display. This will remotely refresh the main display window. Can be used if you think the screen hasn't refreshed during a vote or if the results have been hidden and then need to be displayed again.
5.	Members register-

N.B this list doesn't automatically refresh so use the Refresh button at the top of the page before manually checking in/out any members.

Blue members are not signed in and cannot vote.
Pink members are signed in and can vote.

# No active sessions

[![Admin Console Image](https://raw.githubusercontent.com/NCCOpenSource/GoVote/master/READMEImages/AdminNoActiveSessions.png "Go Vote Admin No Active Session Screen")](https://raw.githubusercontent.com/NCCOpenSource/GoVote/master/READMEImages/AdminNoActiveSessions.png "Go Vote Admin No Active Session Screen")

To start a session, i.e. before the beginning of a council session then click on start session.

In order to update the members who can vote you need to click on Upload seating plan.

# Remove members
To remove existing members, each member needs to be deleted or manually amended inside the azure portal. You can either delete the user entirely or if they need to keep the logon for other purposes then remove their Member job title.

Once a member is removed from Azure they will be unable to log into the system. However, they will still appear as a user on the system.

To remove them from this list you will need to refresh the members list from /Admin/UploadMemberSeatingPlan page.

# Contributing

Please read [CONTRIBUTING.md ](https://github.com/NCCOpenSource/GoVote/blob/master/CONTRIBUTING.md "CONTRIBUTING.md ")for details on our code of conduct, and the process for submitting pull requests to us.

# Authors
- 	Neil Adams - Initial work, app creation, code and documentation – Newcastle City Council
-	Colin Anderson – Phase 2 and Open Source work (code conversion) – Newcastle City Council
- 	Andy Todd – Code Review - Newcastle City Council
- 	Ste Hext – Architecture, Initial documentation conversion to Open Source – Newcastle City Council

See also the list of contributors who participated in this project.

# License

This project is licensed under the MIT License - see the [LICENSE.txt](http://https://github.com/NCCOpenSource/GoVote/blob/master/License.txt "LICENSE.txt") file for details

# Acknowledgments
Special thanks to Newcastle City Council Digital, IT and Democratic Services teams.

Newcastle City Council Digital project initially created and lead by Jenny Nelson’s Team, supported by and released by IT Newcastle.

Thanks to all who help in testing and support for GoVote. With out support of or colleagues and members the project would not have gotten this far.

Thanks you to @purpleBooth for the README template this is based on : https://gist.github.com/PurpleBooth/109311bb0361f32d87a2

# Contact details

For more information please contact <Opensourceprojects@newcastle.gov.uk>

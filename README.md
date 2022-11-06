This is to serve as a user-facing dashboard for collecting feedback, UAT sign-off, and feature voting as a Razor Class Library so that it can be added via NuGet package to Blazor Server apps.

My thinking here is that if all your work is in a private repo, your end users can't see or submit Issues. You can of course use the GitHub API for surfacing Issues in a unique way, and I've [tried this](https://github.com/adamfoneil/GitHubIssues.RCL/tree/master/GitHubApiClient). It's doable, but a bit complicated in the end -- espcially when you consider all the interactions and unique data I want to pack into this, such as [Voting](https://github.com/adamfoneil/UserVoice.RCL/blob/master/UserVoice.Database/Vote.cs) and [AcceptanceRequests](https://github.com/adamfoneil/UserVoice.RCL/blob/master/UserVoice.Database/AcceptanceRequest.cs). Or you can make a public repo specifically for Issue collection. I've done that before too. The downside of this is that end users need a GitHub account. For my situation, this is not a good solution. I wanted a user engagement portal that integrates seamlessly into my application. That way there's no additional login, and I can implement all the custom functionality I want specific for this use case.

The reason for all this is when you have a large or large-ish application that requires a lot of end-user testing and feedback, there needs to be one place where this feedback lives that everyone can see, where users can contribute and feedback is captured. There needs to be one place where outstanding test items are visible to stakeholders. So, instead of asking me "how close to done are we?" they can look at a dashboard. They should be able to tell at a glance how far along testing is, as well as how good the test scenarios are.

There's a similar need for capturing bug reports and feedback on impediments and feature requests. I'm already using [Usersnap](https://usersnap.com/) for this, which is a great tool. But it doesn't help me with the UAT side of this -- where I'd like to be able to enter test cases that select users sign off on. (I may end up adding some API integration with Usersnap in order to import feedback from there -- not sure.)

I know there's a Microsoft product called "UserVoice" or there was at one time. I named this pretty hastily. Naming can be tough as you know, and I don't love the idea of copying an existing product, so I'm already considering different names for this. But since renaming solution assets can be annoying in Visual Studio, the name "UserVoice" will likely stick for now.

# Visuals
It's quite rudimentary at the moment. There's an [ItemForm](https://github.com/adamfoneil/UserVoice.RCL/blob/master/UserVoice.RCL/Components/ItemForm.razor) component for inserting new items, along with an [ItemList](https://github.com/adamfoneil/UserVoice.RCL/blob/master/UserVoice.RCL/Components/ItemList.razor) component.

![image](https://user-images.githubusercontent.com/4549398/200182556-4546314c-208b-49bc-b371-51bda55c796a.png)

Sample code:

```csharp
<div class="container">
    <ItemList @ref="list"/>
    <ItemForm CurrentUser="@user" ItemSaved="OnItemSaved" />
</div>

@code {
    Database.User user = new();
    ItemList? list;

    protected override void OnInitialized()
    {
        user = new Database.User()
        {
            Name = "adamo",
            Email = "adamosoftware@gmail.com",
            TimeZoneId = "Eastern Standard Time"
        };
    }

    async Task OnItemSaved(Database.Item item)
    {
        await list?.Refresh();
    }
}
```

# Database Setup
Run this [sql script](https://github.com/adamfoneil/UserVoice.RCL/blob/master/UserVoice.RCL/Service/Resources/DbSchema.sql) to create the required database tables.

You'll also need to add a few `GRANTs` in your database due to the `uservoice` schema being added:
<details>
  <summary>script</summary>
  
  ```sql
  GRANT SELECT ON SCHEMA ::[uservoice] TO *your app user account*
  GRANT INSERT ON SCHEMA ::[uservoice] TO *your app user account*
  GRANT UPDATE ON SCHEMA ::[uservoice] TO *your app user account*
  GRANT DELETE ON SCHEMA ::[uservoice] TO *your app user account*
  ```
  
</details>

# Application Setup
In your application startup, configure a [ConnectionStrings](https://github.com/adamfoneil/UserVoice.RCL/blob/master/UserVoice.RCL/Service/Models/ConnectionStrings.cs) object, and add the [UserVoiceDataContext](https://github.com/adamfoneil/UserVoice.RCL/blob/master/UserVoice.RCL/Service/UserVoiceDataContext.cs).

```csharp
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddScoped<UserVoiceDataContext>();
```

# Known Issues
There are Bootstrap styling issues between the [sample app](https://github.com/adamfoneil/UserVoice.RCL/tree/master/UserVoice.Sample) where I tested locally vs the actual target project where I needed this in NuGet form. The target project uses Bootstrap 4.x, but the sample app uses 5.x. So, some elements are missing margins and padding due to the different class names (e.g. BS4 uses `ml-` for "margin left" while BS5 uses `ms-` for "margin start"). I'm not quite sure how to resolve that at the moment.

The [Spinner](https://github.com/adamfoneil/UserVoice.RCL/blob/master/UserVoice.RCL/Components/Spinner.razor) component works in the Sample app, but the image is broken when used in NuGet form.

# NuGet Package
The NuGet package is **AO.UserVoice.RCL** hosted here: https://aosoftware.blob.core.windows.net/packages/index.json

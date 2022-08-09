---
title: Guide: Create A Tabbed Login and Register Page With Blazorise Components In 5 Minutes
description: This post guides you on how to create a login page using blazorise components.
permalink: /blog/create-a-tabbed-login-form-with-blazorise-components
canonical: /blog/create-a-tabbed-login-form-with-blazorise-components
image-url: img/blog/2022-08-09/Create_A_Tabbed_Login_and_Register_Page_In_5_Minutes_In_Blazor_With_Blazorise_Components.png
image-title: Create A  Tabbed Login and Register Page With Blazorise Components In 5 Minutes
author-name: James Amattey
author-image: james
posted-on: August 9th, 2022
read-time: 5 min
---

# Guide: Create A Tabbed Login and Register Page With Blazorise Components In 5 Minutes

This focuses on just the structure and markup of the user interface and is intended to demonstrate the use of Blazorise components and does not cover advanced topics such as tokens, cryptography, and hashing.

---

## Introduction

The login page is an entry point to a web application and grants authorized users access to application resources and functionality.

According to the Open Web Application Security Project Foundation Top 10, the confirmation of the user's identity, authentication, and session management is critical to protect against authentication-related attacks, which is currently no. 7 on the OWASP Top 10.

Security starts with design, whether architecturally or via the UI and the structure and layout must be designed and properly implemented to ensure user information is exchanged quickly and securely.

## Step One - Setup

### Outline of the page

The page is structured by using [Blazorise Tabs](docs/components/tab) to have both the login and registration form on the same page. That way, both new and existing users can be onboarded to the platform.

---

## Step 2 - Create a Blazorise Application.

> To begin, scaffold a new Blazor WebAssembly project. [Click here](blog/how-to-create-a-blazorise-application-beginners-guide) for more information on how to setup [Blazor WASM](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) with [Blazorise components](docs/components).

Blazorise components use semantic HTML elements and this gives meaning to both the browser and the developer. For SEO-intensive applications, semantic HTML is a better option than non-semantic elements such as div and p

---

### Create A Sidebar Menu Item.

To make sure your login page is accessible, you need to add a `<nav link>` to your sidebar menu.

You can either create a new one from scratch by copy/pasting the snippet below into the **NavMenu.razor** file or edit any of the preexisting div-class elements. The second option might affect other pages in the project.

```html
<div class="nav-item px-4">
    <NavLink class="nav-link" href="login">
        <span class="oi oi-list-rich" aria-hidden="true"></span>Login
    </NavLink>
</div>
```

---

## Step Three - Create a login.razor page.

In the pages folder of your Blazor project, create a new razor file and name it **Login.razor**

Copy and paste the code snippet below to create the markup.

---

### Creating Tabs

The tabs element allow users to navigate to different sections of a single-page application. We embedded the login and register forms into tabs to allow users to switch easily without creating a single page for each.

The Tab Items represent the various tab menus. It is used to contain the name of the tab.

In the `<content>` element, you can embed other elements or components to complete the markup of the tab. In our case, we will use the `<Field>` component to building out our login form.

```html
<Tabs SelectedTab="@selectedTab" SelectedTabChanged="@OnSelectedTabChanged">
    <Items>
        <Tab Name="login">Login</Tab>
        <Tab Name="register">Register</Tab>
    </Items>
    <Content>
        <TabPanel Name="Login">
            Place form here
        </TabPanel>
        <TabPanel Name="Register">
            Place Form Here.
        </TabPanel>
    </Content>
</Tabs>
```

Now define the selectedTab variable in the @@code block section of the razor page.

```csharp
@code{
    string selectedTab = "login";

    private Task OnSelectedTabChanged( string name )
    {
        selectedTab = name;

        return Task.CompletedTask;
    }
}
``` 

[Click here to read more about Blazorise Tabs Component](docs/components/tab)

---

### Adding Fields

After creating the tabs component, we have to fill it with content. Since we are building a login form, the `Field` component will be embedded into the Tab Content element.

Fields are used to layout input elements to build a form.

```html
<Field>
    <FieldLabel>Username</FieldLabel>
    <TextEdit Placeholder="Enter username" />
</Field>
<Field>
    <FieldLabel>Password</FieldLabel>
    <TextEdit Placeholder="Enter password" />
</Field>
```
You can repeat this for the register tab.

After creating form fields, add a button to submit user input.

```html
<Button Color="Color.Primary">Login</Button>
```

Blazorise Field Component

> [Click here to read more about Blazorise Field Components](docs/components/field)

Blazorise Button Component

> [Click here to read more about the Button Component](docs/components/button)

### Implementing Validation

Now that we have structured both the login and register tabs, we have to ensure that users enter the correct data. The validation component allows you to verify user inputs by finding and correcting errors such as email, or the length of a password. It guides the user to input the correct form of data.

You can either validate through method handlers or through data annotations.

The code snippet below illustrates how you can validate the email field.

```html
<Validation Validator="ValidateEmail">
    <TextEdit Placeholder="Enter email">
        <Feedback>
            <ValidationNone>Please enter the email.</ValidationNone>
            <ValidationSuccess>Email is good.</ValidationSuccess>
            <ValidationError>Enter valid email!</ValidationError>
        </Feedback>
    </TextEdit>
</Validation>
```

Add variable definitions in the @@code section of the razor page

```csharp
@code{
    void ValidateEmail( ValidatorEventArgs e )
    {
        var email = Convert.ToString( e.Value );

        e.Status = string.IsNullOrEmpty( email ) ? ValidationStatus.None :
            email.Contains( "@" ) ? ValidationStatus.Success : ValidationStatus.Error;
    }
}
```

[Read more about Blazorise Validation Component](docs/components/validation)

## Conclusion

By the end, your razor page should look something like this.

```html
<Tabs @bind-SelectedTab="@SelectedTab">
    <Items>
        <Tab Name="login">Login</Tab>
        <Tab Name="register">Register</Tab>
    </Items>
    <Content>
        <TabPanel Name="login">
            Welcome Back, Please Login
            <Validation Validator="ValidatorRule.IsNotEmpty">
                <Field>
                    <FieldLabel>Username</FieldLabel>
                    <TextEdit Placeholder="Enter Username...">
                        <Feedback>
                            <ValidationNone>Please Enter A Username. </ValidationNone>
                            <ValidationSuccess>Username is good</ValidationSuccess>
                            <ValidationError>Please Enter A Valid Username</ValidationError>
                        </Feedback>
                    </TextEdit>
                </Field>
            </Validation>
            <Field>
                <FieldLabel>Password</FieldLabel>
                <TextEdit Placeholder="Enter Password.." />
            </Field>
            <Check TValue="bool" @bind-Checked="@rememberMe">Remember Me</Check>
            <Button Color="Color.Primary">Login</Button>
            <Button Color="Color.Secondary">Forgot Password</Button>
        </TabPanel>
        <TabPanel Name="register">
            New Here? Create An Account
            <Field>
                <FieldLabel>Name</FieldLabel>
                <TextEdit Placeholder="Enter Your Name" />
            </Field>
            <Validation Validator="ValidateEmail">
                <Field>
                    <FieldLabel>Email</FieldLabel>
                    <TextEdit Placeholder="Enter Your Email">
                        <Feedback>
                            <ValidationNone>Please Enter your email.</ValidationNone>
                            <ValidationSuccess>Email is valid</ValidationSuccess>
                            <VlaidationError>Enter Valid email </VlaidationError>
                        </Feedback>
                    </TextEdit>
                </Field>
            </Validation>
            <Field>
                <FieldLabel>Password</FieldLabel>
                <TextEdit Placeholder="Enter Email" />
                <FieldHelp>Password Strength: <Text TextColor="TextColor.Danger">Strong</Text></FieldHelp>
            </Field>
            <Button Color="Color.Primary">
                Create Account
            </Button>
        </TabPanel>
    </Content>
</Tabs>
```

```csharp
@code {
    bool rememberMe;
    string SelectedTab = "login";
	string Username;
	void ValidateEmail (ValidatorEventArgs e)
	{
		var email = Convert.ToString(e.Value);
		e.Status = string.IsNullOrEmpty(email) ? ValidationStatus.None:
			email.Contains("@") ? ValidationStatus.Success : ValidationStatus.Error;
	}
}
```
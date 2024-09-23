---
title: ReactiveUI, Blazorise & FluentValidation?
description: In this blog post guides you will learn how to implement ReactiveUI, Blazorise components & FluentValidation.
permalink: /blog/reactive-ui-blazorise-fluent-validation
canonical: /blog/reactive-ui-blazorise-fluent-validation
image-url: img/blog/2022-08-19/ReactiveUI_Blazorise_FluentValidation.png
image-title: ReactiveUI, Blazorise & FluentValidation?
author-name: Rich Bryant
author-image: richbryant
posted-on: August 19th, 2022
read-time: 5 min
---

# ReactiveUI, Blazorise & FluentValidation?

## Sure, why not?  
  
One of the things I enjoy most in my job is making stuff that doesn't naturally work together, work together. Let's go through the reasoning behind this.  
  
First, I'm using Blazor (WASM) with [ReactiveUI](https://github.com/reactiveui/reactiveui). Why ReactiveUI? Because it means all the functionality I need can be sat in its own testable C# class. And because I hate code in markup, it's messy and makes the debugger do funny things. I'm using Blazorise because I think it's developed into the very best of the available component libraries and also because Mladen was kind enough to implement ICommand property on the `<Button>` control for me a couple of years ago when I started fiddling with all this.

[FluentValidation](https://github.com/FluentValidation/FluentValidation) because we're already using FluentValidation, the team know it and they're comfortable with it. So when I - in my tech lead capacity - blow their minds with new stuff, I like to try to keep it familiar. So let's take a practical case. Here's a RegisterModel -
  
```cs
public class RegisterModel
{
    public string EmailAddress { get; set; }
    public string Password     { get; set; } 
}  
```  

And a validator, why not.  
  
```cs
public RegisterModelValidator()
{
    RuleFor(register => register.Email).EmailAddress().NotEmpty();
    RuleFor(register => register.Password).NotEmpty().Length(1, 24);
}  
```

And then you're going to need a library.  [BlazoriseFluentValidation](https://github.com/aladotnet/BlazoriseFluentValidation) has been around for a little while and don't let the 0.94 version on the readme page fool you, the latest on NuGet is 1.0.4. After that, two minutes of Blazorise and you've got a Registration page.

```html
@page "/account/registration"
@inherits ReactiveInjectableComponentBase<RegistrationViewModel>

<PageTitle>Account Registration</PageTitle>
<Container>
    <Heading Size="HeadingSize.Is1" Padding="Padding.Is4.FromTop">Register</Heading>
    <Paragraph Padding="Padding.Is1.FromTop">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam tempus ex non sapien porta, at efficitur massa condimentum. Nam id nibh facilisis, blandit nulla et, tempus nisl.</Paragraph>
</Container>

<Container Fluid Padding="Padding.Is4.FromTop">
    <Validations Mode="ValidationMode.Auto" Model="Model">
        <Fields>
            <Validation HandlerType="HandlerTypes.FluentValidation">
                <Field Horizontal ColumnSize="ColumnSize.Is12">
                    <FieldLabel ColumnSize="ColumnSize.Is2.OnDesktop">Email Address</FieldLabel>
                    <FieldBody ColumnSize="ColumnSize.Is6.OnDesktop">
                        <TextEdit Placeholder="Some text value..." @bind-Value="Model.EmailAddress">
                            <Feedback>
                                <ValidationError/>
                            </Feedback>
                        </TextEdit>
                    </FieldBody>
                </Field>
            </Validation>
            <Validation HandlerType="HandlerTypes.FluentValidation">
            <Field Horizontal ColumnSize="ColumnSize.Is12">
                <FieldLabel ColumnSize="ColumnSize.Is2.OnDesktop">Password</FieldLabel>
                <FieldBody ColumnSize="ColumnSize.Is6.OnDesktop">
                    <TextEdit Role="TextRole.Password" Placeholder="Some text value..." @bind-Value="Model.Password">
                        <Feedback>
                            <ValidationError/>
                        </Feedback>
                    </TextEdit>
                </FieldBody>
            </Field>
            </Validation>
        </Fields>
    </Validations>
    <Button Color="Color.Primary">Register</Button>
</Container>  
```

Which, obviously enough, gives you this  

![Register page](img/blog/2022-08-19/register-page.png)  

So far so hoopy. But there's a few wrinkles. The button doesn't do anything. And it's just a class, this isn't ReactiveUI. So let's remedy that with a ViewModel. But wait. If I change this to a viewmodel, my validator won't work anymore! I really struggled with this one and eventually I bit the bullet and added a RegistrationViewModelValidator. Which was basically cut&paste.    

```cs
public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
{
    public RegistrationViewModelValidator()
    {
        RuleFor(x => x.EmailAddress).EmailAddress().NotEmpty();
        RuleFor(x => x.Password).NotEmpty().Length(1, 24);
    }
}
```

> Authors's note: Remind me to experiment with inheriting from the Model Validator, let's not repeat ourselves

I've still got the `RegistrationModelValidator` because obviously I need to validate at the server side too.  And then it struck me, what if I injected that validator into my ViewModel to make certain that no server call would be made if the model I was sending wasn't valid? After all, I certainly didn't want to be serialising the ViewModel and sending that over the wires, it needs functionality and stuff on it.

The answer is, of course, obvious to all you Rx types. `ObservableAsPropertyHelper<T>` was what I needed.

So,

```cs
private readonly ObservableAsPropertyHelper<RegistrationModel> _registrationModel;  
```

at the top of the class. And...  

```cs
public RegistrationModel RegistrationModel => _RegistrationModel.Value;  
```

with the public properties. And in the constructor, this little gem -   

```cs
_registrationModel = this.WhenAnyValue(x => x.EmailAddress, y => y.Password)
                .Select(model => new RegistrationModel { Email = model.Item1, Password = model.Item2})
                .ToProperty(this, x => x.RegistrationModel);
```

**OAPH**s are well documented on the [ReactiveUI website](https://www.reactiveui.net/docs/handbook/observable-as-property-helper/), you don't need me going over it here. Basically, it's a calculated property which updates when ever the Email or Password properties change.

After that, I thought "well why not? Why _shouldn't_ I Rx it?"

So I added a command. This command.  
  
```cs
public ReactiveCommand<Unit, Unit> Register { get; }
```

And I added a method  
  
```cs
private async Task RegisterAccount()
{
    if (!CanRegister) return;
    try
    {
        var result = await _accountRepo.RegisterUser(RegisterModel);
        RegistrationSucceeded = true;
        _navManager.NavigateTo("/account/login");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}  
```  

Which uses an injected Refit interface to do the ugly Http stuff and also the injected `NavigationManager` to do some nice user navigation at the end. And then I turned the method into the command in the constructor.  
  
`Register = ReactiveCommand.CreateFromTask(RegisterAccount);`  
  
All very well, except I hadn't validated the outgoing model yet. I could do it the method but that lacked style, I felt. And I thought, if I make it an observable, I can do other things with that.  
  
So I put this line into the constructor, too.  
  
```cs
var isValid = this.WhenAnyValue(x => x.LoginModel)
                .Select(x => validator.Validate(x).IsValid);  
```

Now that isValid variable is an `IObservable<bool>`. Which means I can add it to my ReactiveCommand's canExecute property. Now the command literally cannot execute unless the Validator says my calculated model is valid.  
  
`Register = ReactiveCommand.CreateFromTask(RegisterAccount, isValid);`

Hmm. I need one more thing here. If you can't fire the command, I don't want to button to be enabled. But I can't bind Disabled to an Observable. Unless I bind to another **OAPH**. Aha!  
  
```cs
private readonly ObservableAsPropertyHelper<bool> _canRegister;  
public bool CanRegister => _canRegister.Value;
```

And back in the constructor, rather than create yet another observable....  
  
`_canRegister = Register.CanExecute.ToProperty(this, x => x.CanRegister);`

We're all done in the ViewModel, we've got everything we need. So what's left?  
  
Ah yes, bindings in the View.  Here's the completed markup of the View.  
  
```html
@page "/account/registration"
@inherits ReactiveInjectableComponentBase<RegistrationViewModel>

<PageTitle>Account Registration</PageTitle>
<Container>
    <Heading Size="HeadingSize.Is1" Padding="Padding.Is4.FromTop">Register</Heading>
    <Paragraph Padding="Padding.Is1.FromTop">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam tempus ex non sapien porta, at efficitur massa condimentum. Nam id nibh facilisis, blandit nulla et, tempus nisl.</Paragraph>
</Container>

<Container Fluid Padding="Padding.Is4.FromTop">
    <Validations Mode="ValidationMode.Auto" Model="ViewModel">
        <Fields>
            <Validation HandlerType="HandlerTypes.FluentValidation">
                <Field Horizontal ColumnSize="ColumnSize.Is12">
                    <FieldLabel ColumnSize="ColumnSize.Is2.OnDesktop">Email Address</FieldLabel>
                    <FieldBody ColumnSize="ColumnSize.Is6.OnDesktop">
                        <TextEdit Placeholder="Some text value..." @bind-Value=ViewModel.EmailAddress>
                            <Feedback>
                                <ValidationError/>
                            </Feedback>
                        </TextEdit>
                    </FieldBody>
                </Field>
            </Validation>
            <Validation HandlerType="HandlerTypes.FluentValidation">
            <Field Horizontal ColumnSize="ColumnSize.Is12">
                <FieldLabel ColumnSize="ColumnSize.Is2.OnDesktop">Password</FieldLabel>
                <FieldBody ColumnSize="ColumnSize.Is6.OnDesktop">
                    <TextEdit Role="TextRole.Password" Placeholder="Some text value..." @bind-Value=ViewModel.Password>
                        <Feedback>
                            <ValidationError/>
                        </Feedback>
                    </TextEdit>
                </FieldBody>
            </Field>
            </Validation>
        </Fields>
    </Validations>
    <Button Color="Color.Primary" Command="ViewModel.Register" Disabled="@(!ViewModel.CanRegister)">Register</Button>
</Container>  
```
  
So I'm binding to the ViewModel properties. I'm using Blazorise's `Command` property to bind directly to my ReactiveCommand. And I've even bound the `Disabled`property of the button to the inverse of `ViewModel.CanRegister`  
  
But there's something odd here, I've injected NavigationManager. Which isn't testable. How the hell am I going to test this?  
  
By creating a mock, of course. SO here's my MockNavigationManager.  

```cs
public sealed class MockNavigationManager
    : NavigationManager
{
    public MockNavigationManager() : base() => 
        this.Initialize("http://localhost/", "http://localhost/test");

    protected override void NavigateToCore(string uri, bool forceLoad) 
        => WasNavigateInvoked = true;

    public bool WasNavigateInvoked { get; private set; }
}  
```


And here, just to round everything off, is one of my working unit tests.

```html
[Theory, AutoNSubstituteData]
    public void RegistrationViewModel_ValidatesAndFiresRegisterCommand([Frozen]IAccountRepo repo, LoginModelValidator validator)
        => new TestScheduler().With(scheduler =>
        {
            //Arrange
            repo.RegisterUser(Arg.Any<RegistrationModel>()).Returns(true);
            var nav = new MockNavigationManager();
            
            //Act
            var viewModel = new RegistrationViewModel(repo, validator, nav)
            {
                EmailAddress = "me@mine.com",
                Password = "Password"
            };

            viewModel.Register.Execute().Subscribe();
            scheduler.AdvanceBy(5);

            //Assert
            viewModel.CanRegister.Should().BeTrue();
            repo.Received().RegisterUser(viewModel.RegistrationModel);
            nav.WasNavigateInvoked.Should().BeTrue();
        });  
```

Obviously you don't have to do it this way. But you can if you want to. 

Have fun out there!
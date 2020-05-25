# Blazor-Color-Picker
[![NuGet](https://img.shields.io/nuget/v/BlazorColorpicker.svg)](https://www.nuget.org/packages/BlazorColorpicker/)

Sometimes HTML5 colorpicker doesn't suit me for an application. I prefer to offer the user a predefined color palette

Opens a palette with the Material colors

![name-of-you-image](https://github.com/tossnet/Blazor-Color-Picker/blob/master/Blazor-Color-Picker/forGithubReadme/blazor-color-picker.png)

# Installation

Latest version in here:  [![NuGet](https://img.shields.io/nuget/v/BlazorColorpicker.svg)](https://www.nuget.org/packages/BlazorColorpicker/)


To Install 

```
Install-Package BlazorColorPicker
```
or 
```
dotnet add package BlazorColorPicker
```

For client-side and server-side Blazor - add script section to index.html or _Host.cshtml (head section) 

```html
<link href="_content/BlazorColorPicker/colorpicker.css" rel="stylesheet" />
```

## Usage



```html
@page "/"
@using BlazorColorPicker

<h1>Hello, world!</h1>

<button class="btn btn-primary" @onclick="OpenModal">Choose Color</button>

<div style="background-color:@color;width:75px;height:75px;">
</div>

<ColorPicker Title="My Blazor ColorPicker" IsOpened="isOpened" Closed="ClosedEvent" MyColor="@color">
</ColorPicker>

@code {
    bool isOpened = false;
    string color = "#F1F7E9";

    void OpenModal()
    {
        isOpened = true;
    }

    void ClosedEvent(string value)
    {
        color = value;
        isOpened = false;
    }
}
```

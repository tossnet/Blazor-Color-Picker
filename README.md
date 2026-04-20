# Blazor-Color-Picker
[![NuGet](https://img.shields.io/nuget/v/BlazorColorPicker.svg)](https://www.nuget.org/packages/BlazorColorPicker/) ![BlazorColorPicker Nuget Package](https://img.shields.io/nuget/dt/BlazorColorPicker)
[![GitHub](https://img.shields.io/github/license/tossnet/Blazor-Color-Picker?color=594ae2&logo=github&style=flat-square)](https://github.com/tossnet/Blazor-Color-Picker/blob/master/LICENSE)

Sometimes HTML5 colorpicker doesn't suit me for an application. I prefer to offer the user a predefined color palette

DEMO : https://tossnet.github.io/Blazor-Color-Picker/

Opens a palette with the Material colors

![Blazor Color Picker](https://github.com/tossnet/Blazor-Color-Picker/blob/master/BlazorColorPicker.gif)


# Installation

> [!WARNING]
> The implementation has been improved: version 4.0 uses a service declared via dependency injection

Latest version in here: [![NuGet](https://img.shields.io/nuget/v/BlazorColorPicker.svg)](https://www.nuget.org/packages/BlazorColorPicker/)

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

In program.cs, declare 

```csharp
builder.Services.AddColorPicker();
```

**ColorPicker** are rendered by the `<BlazorColorPicker.ColorPicker />`. This component needs to be added to the main layout of your application/site. You typically do this in the `MainLayout.razor` file at the end of the <main> section.

## Usage

```html
@page "/"
@using BlazorColorPicker

@inject IColorPickerService ColorPickerService

<button class="btn btn-primary" @onclick="OpenModal">
    <div style="background-color:@color" class="buttonColor"></div> Select a Color
</button>

@code {
    private string color = "#F1F7E9";

    private async Task OpenModal()
    {
        var parameters = new ColorPickerParameters
        {
        	ColorSelected = color,
        };
        color = await ColorPickerService.ShowColorPicker(parameters);
    }
}
```

## Find Closest Color

When the selected color doesn't exist in the palette, you can enable automatic matching to the closest available color using the `FindClosestIfNotFound` parameter:

```csharp
var parameters = new ColorPickerParameters
{
    ColorSelected = "#8B4513",  // SaddleBrown - not in the default palette
    FindClosestIfNotFound = true,
};
color = await ColorPickerService.ShowColorPicker(parameters);
```

The algorithm uses a **weighted Euclidean distance** in RGB color space, which accounts for human color perception (green differences are more noticeable than red or blue).

> [!NOTE]
> Color comparison is case-insensitive: `#ec407a` will match `#EC407A` in the palette.

## <a name="ReleaseNotes"></a>Release Notes

<details open="open"><summary>Version 4.2.0</summary>

>- New feature: `FindClosestIfNotFound` parameter - when the selected color is not in the palette, automatically highlights the closest matching color
>- Color comparison is now case-insensitive (`#ec407a` matches `#EC407A`)
>- Uses weighted Euclidean distance algorithm for perceptually accurate color matching
</details>

<details><summary>Version 4.1.1</summary>

>- Performance & memory optimization: default colors are now static and shared across all instances
>- Fixed memory leak in OnParametersSet() that caused unbounded list growth
>- Zero allocations when using default color palette
</details>

<details><summary>Version 4.0.2</summary>
>- Added AddColorPicker() to simplify declaration
</details>

<details><summary>Version 4.0.1</summary>
>- increase the z-index to 9999
</details>

<details><summary>Version 4.0.0</summary>
>- the implementation has been improved: version 4.0 uses a service declared via dependency injection
</details>

## ⚠️ Breaking changes ⚠️

<details><summary>Version 3.1.0</summary>
>- you can customise the size of the palette using your CSS styles
>- A red colour of the first column was not correct
>- Re-addition of .NET7 compatibility
</details>

<details><summary>Version 2.2.0</summary>
    
>- Remove the internal use of IJSRuntime
</details>

<details><summary>Version 2.1.0</summary>
    
>- no need to declare the _content/BlazorColorPicker/colorpicker.js file
</details>



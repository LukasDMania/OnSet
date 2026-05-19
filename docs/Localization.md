# UI localization

## How it works

- **Default culture:** `en` (English strings live in `Resources/Resource.resx`).
- **Dutch overrides:** `Resources/Resource.nl.resx` — only keys present here replace English; all other keys fall back to the default `.resx`.
- **Razor:** `_ViewImports.cshtml` injects `IStringLocalizer<Resource> L`. Use `@L["Key"]` or `@L["Key", arg0, ...]` for formatted strings (see footer copyright).
- **Culture persistence:** `GET /culture/set?culture=nl&returnUrl=/current/path` (`SetLanguage` page) sets the ASP.NET Core culture cookie; the footer links pass the current path as `returnUrl` so you stay on the same page after switching.

## Adding a string

1. Add a `<data name="Your_Key" xml:space="preserve"><value>English text</value></data>` entry to `Resource.resx`.
2. Optionally add the same name with a Dutch `<value>` to `Resource.nl.resx`.
3. Use `@L["Your_Key"]` in `.cshtml` (or `string.Format(L["KeyWith_{0}"].Value, x)` when not using the `L[key, args]` overload).

## Not localized yet (optional next steps)

- `[Display]` names and validation messages on view models (enable `IDataAnnotationsLocalizationProvider` / shared resource type if you want those translated).
- `Result.Fail` / FluentValidation / exception messages returned to users from handlers.
- Log messages and developer-only text.
